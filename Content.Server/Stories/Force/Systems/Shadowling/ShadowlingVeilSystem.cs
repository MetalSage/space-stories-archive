using Content.Server.Emp;
using Content.Shared.SpaceStories.Force.Shadowling;
using Robust.Server.GameObjects;

namespace Content.Server.SpaceStories.Force.Shadowling;

public sealed class ShadowlingVeilSystem : EntitySystem
{
    [Dependency] private readonly ShadowlingForceSystem _shadowling = default!;
    [Dependency] private readonly EmpSystem _emp = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingVeilEvent>(OnShadowlingVeilEvent);
    }

    private void OnShadowlingVeilEvent(EntityUid performer, ShadowlingForceComponent component, ref ShadowlingVeilEvent ev)
    {
        var lights = _shadowling.GetEntitiesAroundShadowling<PointLightComponent>(performer, 15);

        foreach (var entity in lights)
        {
            _emp.DoEmpEffects(entity, 50000, 60);
        }
    }
}
