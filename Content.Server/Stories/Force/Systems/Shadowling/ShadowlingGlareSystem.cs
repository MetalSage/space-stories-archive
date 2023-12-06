using Content.Server.Flash;
using Content.Shared.SpaceStories.Force.Shadowling;

namespace Content.Server.SpaceStories.Force.Shadowling;

public sealed class ShadowlingGlareSystem : EntitySystem
{
    [Dependency] private readonly FlashSystem _flash = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingGlareEvent>(OnShadowlingGlareEvent);
    }

    private void OnShadowlingGlareEvent(EntityUid performer, ShadowlingForceComponent component, ref ShadowlingGlareEvent ev)
    {
        _flash.FlashArea(performer, performer, 15, 10_000);
    }
}
