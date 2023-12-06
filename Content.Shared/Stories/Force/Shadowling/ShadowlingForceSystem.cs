using Content.Shared.Actions;

namespace Content.Shared.SpaceStories.Force.Shadowling;
public sealed class ShadowlingForceSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ComponentStartup>(OnStartUp);
        SubscribeLocalEvent<ShadowlingForceComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingForceTypeChangeEvent>(OnForceTypeChanged);
    }
    private void OnStartUp(EntityUid uid, ShadowlingForceComponent component, ComponentStartup args)
    {
        if (!TryComp<ActionsComponent>(uid, out var action))
            return;

        component.Actions.TryGetValue(component.ForceType, out var toGrant);
        if (toGrant == null) return;
        foreach (var id in toGrant)
        {
            EntityUid? act = null;
            if (_actions.AddAction(uid, ref act, id, uid, action))
                component.GrantedActions.Add(act.Value);
        }

        Dirty(uid, component);
    }
    private void OnShutdown(EntityUid uid, ShadowlingForceComponent component, ComponentShutdown args)
    {
        if (!TryComp<ActionsComponent>(uid, out var action))
            return;

        foreach (var act in component.GrantedActions)
        {
            Del(act);
        }

        component.GrantedActions.Clear();
    }
    private void OnForceTypeChanged(EntityUid uid, ShadowlingForceComponent component, ref ShadowlingForceTypeChangeEvent args)
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
}
