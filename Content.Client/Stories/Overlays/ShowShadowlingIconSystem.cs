using Content.Client.Antag;
using Content.Shared.SpaceStories.Shadowling;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;
public sealed class ShowShadowlingIconSystem : AntagStatusIconSystem<ShadowlingComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatusIconComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, StatusIconComponent _, ref GetStatusIconsEvent args)
    {
        var result = new List<StatusIconPrototype>();

        if (!TryComp<ShadowlingComponent>(uid, out var shadowling))
        {
            return;
        }
    }
}
