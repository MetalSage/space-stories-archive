using Content.Shared.Rejuvenate;
using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;

public sealed class ShadowlingRapidReHatchSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingRapidReHatchEvent>(OnShadowlingRapidReHatchEvent);
    }

    private void OnShadowlingRapidReHatchEvent(EntityUid uid, ShadowlingForceComponent component, ref ShadowlingRapidReHatchEvent ev)
    {
        var rejuvenate = new RejuvenateEvent();
        RaiseLocalEvent(uid, rejuvenate);
    }
}
