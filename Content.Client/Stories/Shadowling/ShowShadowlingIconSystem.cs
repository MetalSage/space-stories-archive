using Content.Client.Antag;
using Content.Shared.SpaceStories.Shadowling;
using Content.Shared.StatusIcon.Components;

namespace Content.Client.Overlays;
public sealed class ShowShadowlingIconSystem : AntagStatusIconSystem<ShadowlingComponent>
{
    [Dependency] private readonly SharedShadowlingSystem _shadowling = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatusIconComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, StatusIconComponent _, ref GetStatusIconsEvent args)
    {
        if (!TryComp<ShadowlingComponent>(uid, out var shadowling))
            return;

        if (_shadowling.IsShadowlingSlave(shadowling))
            GetStatusIcon("ShadowlingThrallFaction", ref args);
        else
            GetStatusIcon("ShadowlingFaction", ref args);
    }
}
