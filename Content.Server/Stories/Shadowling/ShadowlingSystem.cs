using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared.IdentityManagement;
using Content.Shared.Mindshield.Components;
using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;
public sealed class ShadowlingSystem : SharedShadowlingSystem
{
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly StunSystem _stun = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MindShieldComponent, MapInitEvent>(MindShieldImplanted);
        SubscribeLocalEvent<ShadowlingComponent, ComponentShutdown>(Shutdown);
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

    public void ChangeStage(EntityUid uid, ShadowlingComponent component, ShadowlingStage stage)
    {
        component.Stage = stage;
        Dirty(uid, component);
    }

    private void MindShieldImplanted(EntityUid uid, MindShieldComponent comp, MapInitEvent init)
    {
        if (!TryComp<ShadowlingComponent>(uid, out var shadowling))
            return;

        if (!IsShadowlingSlave(shadowling) || shadowling.Stage == ShadowlingStage.Lower)
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

    private void Shutdown(EntityUid uid, ShadowlingComponent comp, ComponentShutdown ev)
    {
        // TODO: change metabolizm back to normal
    }
}
