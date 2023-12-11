using Content.Server.Fluids.EntitySystems;
using Content.Shared.SpaceStories.Shadowling;
using Content.Shared.Chemistry.Components;

namespace Content.Server.SpaceStories.Shadowling;

public sealed class ShadowlingBlindnessSmokeSystem : EntitySystem
{
    [Dependency] private readonly SmokeSystem _smoke = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingBlindnessSmokeEvent>(OnShadowlingBlindnessSmokeEvent);
    }

    private void OnShadowlingBlindnessSmokeEvent(EntityUid uid, ShadowlingForceComponent component, ref ShadowlingBlindnessSmokeEvent ev)
    {
        if (!TryComp<TransformComponent>(uid, out var transform))
            return;

        var solution = new Solution();
        solution.AddReagent("ShadowlingSmokeReagent", 300);

        var foamEnt = Spawn("Foam", transform.Coordinates);
        _smoke.StartSmoke(foamEnt, solution, 30, 16);
    }
}
