using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.SpaceStories.Shadowling;
using Content.Shared.Standing;

namespace Content.Server.SpaceStories.Shadowling;

public sealed class ShadowlingHatchSystem : EntitySystem
{
    [Dependency] private readonly SmokeSystem _smoke = default!;
    [Dependency] private readonly StandingStateSystem _standing = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedShadowlingSystem _shadowling = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShadowlingComponent, ShadowlingHatchEvent>(OnHatch);
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingHatchDoAfterEvent>(OnHatchDoAfter);
    }

    private void OnHatch(EntityUid uid, ShadowlingComponent component, ref ShadowlingHatchEvent ev)
    {
        if (!TryComp<TransformComponent>(uid, out var transform))
            return;

        ev.Handled = true;

        var solution = new Solution();
        solution.AddReagent("ShadowlingSmokeReagent", 300);

        var smokeEnt = Spawn("Smoke", transform.Coordinates);
        _smoke.StartSmoke(smokeEnt, solution, 30, 12);
        _standing.Down(uid);

        var doAfter = new DoAfterArgs(EntityManager, uid, 30, new ShadowlingHatchDoAfterEvent(), uid)
        {
            BlockDuplicate = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnHatchDoAfter(EntityUid uid, ShadowlingComponent component, ref ShadowlingHatchDoAfterEvent ev)
    {
        _standing.Stand(uid);
        _shadowling.SetStage(uid, component, ShadowlingStage.Start);

        if (TryComp<DamageableComponent>(uid, out var damageable))
            _damageable.SetAllDamage(uid, damageable, 0);
    }
}
