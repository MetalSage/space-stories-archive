using Content.Server.Stories.Lib;
using Content.Shared.Stories.Shadowling;

namespace Content.Server.Stories.Shadowling;

public sealed class ShadowlingRapidReHatchSystem : EntitySystem
{
    [Dependency] private readonly StoriesUtilsSystem _utils = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingRapidReHatchEvent>(OnRapidReHatchEvent);
    }

    private void OnRapidReHatchEvent(EntityUid uid, ShadowlingComponent component, ref ShadowlingRapidReHatchEvent ev)
    {
        ev.Handled = true;
        _utils.Rejuvenate(uid);
    }
}
