using Content.Server.Actions;
using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared.Actions;
using Content.Shared.IdentityManagement;
using Content.Shared.Mindshield.Components;
using Content.Shared.SpaceStories.Mindshield;
using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;
public sealed class ShadowlingSystem : SharedShadowlingSystem
{
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly SharedShadowlingSystem _shared = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ShadowlingComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingStageChangeEvent>(OnStageChanged);
        SubscribeLocalEvent<ShadowlingComponent, MindShieldImplantedEvent>(OnMindShieldImplanted);
    }

    private void OnStartup(EntityUid uid, ShadowlingComponent component, ComponentStartup args)
    {
        if (!TryComp<ActionsComponent>(uid, out var action))
            return;

        component.Actions.TryGetValue(component.Stage, out var toGrant);
        if (toGrant == null) return;
        foreach (var id in toGrant)
        {
            EntityUid? act = null;
            if (_actions.AddAction(uid, ref act, id, uid, action))
                component.GrantedActions.Add(act.Value);
        }

        Dirty(uid, component);
    }

    private void OnShutdown(EntityUid uid, ShadowlingComponent component, ComponentShutdown args)
    {
        if (!TryComp<ActionsComponent>(uid, out var action))
            return;

        foreach (var act in component.GrantedActions)
        {
            Del(act);
        }

        component.GrantedActions.Clear();

        // TODO: change metabolizm back to normal
    }

    public List<EntityUid> GetEntitiesAroundShadowling<TFilter>(EntityUid uid, float radius, bool filterThralls = true) where TFilter : IComponent
    {
        List<EntityUid> result = new() { };

        if (!TryComp<TransformComponent>(uid, out var transform))
            return result;

        foreach (var entity in _entityLookup.GetEntitiesInRange(transform.Coordinates, radius))
        {
            if (!TryComp<TFilter>(entity, out var _))
                continue;
            if (filterThralls && TryComp<ShadowlingComponent>(entity, out var _))
                continue;

            result.Add(entity);
        }

        return result;
    }

    private void OnStageChanged(EntityUid uid, ShadowlingComponent component, ref ShadowlingStageChangeEvent args)
    {
        if (!TryComp<ActionsComponent>(uid, out var action) || args.NewActions == null)
            return;

        foreach (var act in component.GrantedActions)
        {
            Del(act);
        }

        component.GrantedActions.Clear();

        foreach (var id in args.NewActions)
        {
            EntityUid? act = null;
            if (_actions.AddAction(uid, ref act, id, uid, action))
                component.GrantedActions.Add(act.Value);
        }

        Dirty(uid, component);
    }

    private void OnMindShieldImplanted(EntityUid uid, ShadowlingComponent comp, MindShieldImplantedEvent ev)
    {
        if (!TryComp<ShadowlingComponent>(uid, out var shadowling))
            return;

        if (!_shared.IsShadowlingSlave(shadowling) || shadowling.Stage == ShadowlingStage.Lower)
        {
            RemCompDeferred<MindShieldComponent>(uid);
            return;
        }
        else
        {
            var stunTime = TimeSpan.FromSeconds(4);
            var name = Identity.Entity(uid, EntityManager);
            RemComp<ShadowlingComponent>(uid);
            _stun.TryParalyze(uid, stunTime, true);
            _popup.PopupEntity(Loc.GetString("thrall-break-control", ("name", name)), uid);
        }
    }
}
