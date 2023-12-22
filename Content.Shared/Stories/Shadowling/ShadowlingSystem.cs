using Content.Shared.Actions;

namespace Content.Shared.SpaceStories.Shadowling;
[Virtual]
public class SharedShadowlingSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ComponentStartup>(OnStartUp);
        SubscribeLocalEvent<ShadowlingComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingStageChangeEvent>(OnForceTypeChanged);
    }

    private void OnStartUp(EntityUid uid, ShadowlingComponent component, ComponentStartup args)
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
    }

    private void OnForceTypeChanged(EntityUid uid, ShadowlingComponent component, ref ShadowlingStageChangeEvent args)
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

    public static bool IsShadowlingSlave(ShadowlingComponent component)
    {
        return component.Stage switch
        {
            ShadowlingStage.Thrall or ShadowlingStage.Lower => true,
            ShadowlingStage.Start or
            ShadowlingStage.Basic or
            ShadowlingStage.Medium or
            ShadowlingStage.High or
            ShadowlingStage.Final or
            ShadowlingStage.Ascended => false,
            _ => false,
        };
    }
}
