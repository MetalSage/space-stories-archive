using Content.Server.Actions;
using Content.Shared.Actions;
using Content.Shared.Stories.Shadowling;
using Content.Shared.Weapons.Ranged.Events;

namespace Content.Server.Stories.Shadowling;
public sealed partial class ShadowlingSystem : SharedShadowlingSystem<ShadowlingThrallComponent, ShadowlingComponent>
{
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeBase();
        InitializeRadio();
        InitializeThralls();
    }

    private void InitializeBase()
    {
        SubscribeLocalEvent<ShadowlingComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ShadowlingComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<ShadowlingComponent, ShotAttemptedEvent>(OnShotAttempted);
    }

    public const string ShadowlingEnthrallAction = "ActionShadowlingEnthrall";
    public const string ShadowlingHatchAction = "ActionShadowlingHatch";

    private void OnStartup(EntityUid uid, ShadowlingComponent component, ComponentStartup args)
    {
        if (!TryComp<ActionsComponent>(uid, out var action))
            return;

        _actions.AddAction(uid, ShadowlingEnthrallAction, uid, action);
        _actions.AddAction(uid, ShadowlingHatchAction, uid, action);

        Dirty(uid, component);

        EnsureComp<ShadowlingRoleComponent>(uid);
    }

    private void OnShutdown(EntityUid uid, ShadowlingComponent component, ComponentShutdown args)
    {
        // TODO: change metabolizm back to normal
    }

    public List<EntityUid> GetEntitiesAroundShadowling<TFilter>(EntityUid uid, float radius, bool filterThralls = true) where TFilter : IComponent
    {
        List<EntityUid> result = new();

        if (!TryComp<TransformComponent>(uid, out var transform))
            return result;

        foreach (var entity in _entityLookup.GetEntitiesInRange(transform.Coordinates, radius))
        {
            if (!TryComp<TFilter>(entity, out _))
                continue;
            if (filterThralls && TryComp<ShadowlingComponent>(entity, out _))
                continue;

            result.Add(entity);
        }

        return result;
    }

    private void OnShotAttempted(EntityUid uid, ShadowlingComponent comp, ref ShotAttemptedEvent args)
    {
        _popup.PopupEntity(Loc.GetString("gun-disabled"), uid, uid);
        args.Cancel();
    }
}
