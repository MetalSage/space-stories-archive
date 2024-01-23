using Content.Server.Fluids.EntitySystems;
using Content.Server.Polymorph.Systems;
using Content.Server.Stunnable;
using Content.Shared.Chemistry.Components;
using Content.Shared.DoAfter;
using Content.Shared.Stories.Shadowling;
using Content.Shared.Standing;
using Robust.Server.GameObjects;
using Robust.Shared.Physics;

namespace Content.Server.Stories.Shadowling;

public sealed class ShadowlingAscendanceSystem : EntitySystem
{
    [Dependency] private readonly SmokeSystem _smoke = default!;
    [Dependency] private readonly StandingStateSystem _standing = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;

    public readonly string ShadowlingAscendedPolymorph = "Ascended";

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

        var newNullableUid = _polymorph.PolymorphEntity(uid, ShadowlingAscendedPolymorph);

        if (newNullableUid is not { } newUid)
            return;

        _stun.TryStun(newUid, TimeSpan.FromSeconds(30), true);
        _standing.Down(newUid, dropHeldItems: false, canStandUp: false);
        _physics.SetBodyType(newUid, BodyType.Static);

        var doAfter = new DoAfterArgs(EntityManager, newUid, 30, new ShadowlingAscendanceDoAfterEvent(), newUid)
        {
            RequireCanInteract = false,
        };
        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnAscendanceDoAfter(EntityUid uid, ShadowlingComponent component, ref ShadowlingAscendanceDoAfterEvent ev)
    {
        _standing.Stand(uid);
        _physics.SetBodyType(uid, BodyType.KinematicController);
    }
}
