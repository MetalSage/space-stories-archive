using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Body.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Stories.Shadowling;

namespace Content.Server.Stories.Shadowling;

public sealed class ShadowlingIcyVeinsSystem : EntitySystem
{
    [Dependency] private readonly ShadowlingSystem _shadowling = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly ChemistrySystem _chemistry = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingIcyVeinsEvent>(OnIcyVeinsEvent);
    }

    private void OnIcyVeinsEvent(EntityUid uid, ShadowlingComponent component, ref ShadowlingIcyVeinsEvent ev)
    {
        ev.Handled = true;
        var bodies = _shadowling.GetEntitiesAroundShadowling<BodyComponent>(uid, 15);
        var solution = new Solution();
        solution.AddReagent(component.IcyVeinsReagentId, 10);

        foreach (var entity in bodies)
        {
            if (!_solution.TryGetInjectableSolution(entity, out var entitySolution, out _))
                continue;

            _solution.AddSolution(entitySolution.Value, solution);
        }
    }
}
