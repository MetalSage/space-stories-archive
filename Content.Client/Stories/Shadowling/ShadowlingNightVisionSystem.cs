using Content.Shared.SpaceStories.Shadowling;
using Robust.Client.Graphics;

namespace Content.Client.SpaceStories.Shadowling;

public sealed class ShadowlingNightVisionSystem : EntitySystem
{
    [Dependency] private readonly ILightManager _light = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingNightVisionEvent>(OnNightVision);
    }

    public void OnNightVision(EntityUid uid, ShadowlingComponent shadowling, ref ShadowlingNightVisionEvent ev)
    {
        _light.DrawShadows = !_light.DrawShadows;
    }
}
