using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.SpaceStories.Shadowling;
using Content.Shared.Standing;

namespace Content.Server.SpaceStories.Shadowling;

public sealed class ShadowlingAscendanceSystem : EntitySystem
{
    [Dependency] private readonly SmokeSystem _smoke = default!;
    [Dependency] private readonly StandingStateSystem _standing = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly ShadowlingSystem _shadowling = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShadowlingComponent, ShadowlingAscendanceEvent>(OnAscendance);
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingAscendanceDoAfterEvent>(OnAscendanceDoAfter);
    }

    private void OnAscendance(EntityUid uid, ShadowlingComponent component, ref ShadowlingAscendanceEvent ev)
    {
        if (!TryComp<TransformComponent>(uid, out var transform))
            return;

        ev.Handled = true;

        var solution = new Solution();
        solution.AddReagent("ShadowlingSmokeReagent", 300);

        var smokeEnt = Spawn("Smoke", transform.Coordinates);
        _smoke.StartSmoke(smokeEnt, solution, 30, 12);
        _standing.Down(uid);

        var doAfter = new DoAfterArgs(EntityManager, uid, 30, new ShadowlingAscendanceDoAfterEvent(), uid)
        {
            BlockDuplicate = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnAscendanceDoAfter(EntityUid uid, ShadowlingComponent component, ref ShadowlingAscendanceDoAfterEvent ev)
    {
        _standing.Stand(uid);
        _shadowling.SetStage(uid, component, ShadowlingStage.Ascended);

        if (TryComp<DamageableComponent>(uid, out var damageable))
            _damageable.SetAllDamage(uid, damageable, 0);

        // TODO: Morph shadowling into ascended shadowling
    }
}
