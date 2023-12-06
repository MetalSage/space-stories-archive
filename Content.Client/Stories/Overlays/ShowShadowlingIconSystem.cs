using Content.Shared.SpaceStories.Force.Shadowling;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;
public sealed class ShowShadowlingIconSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatusIconComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, StatusIconComponent _, ref GetStatusIconsEvent @event)
    {
        var shadowlingIcons = DecideShadowlingIcon(uid);

        @event.StatusIcons.AddRange(shadowlingIcons);
    }

    private IReadOnlyList<StatusIconPrototype> DecideShadowlingIcon(EntityUid uid)
    {
        var result = new List<StatusIconPrototype>();

        if (!TryComp<ShadowlingForceComponent>(uid, out var shadowling))
        {
            return result;
        }

        switch (shadowling.ForceType)
        {
            case ShadowlingForceType.ShadowlingSlave:
            case ShadowlingForceType.ShadowlingTrell:
                if (_prototype.TryIndex<StatusIconPrototype>("ShadowlingSlave", out var shadowingSlaveIcon))
                {
                    result.Add(shadowingSlaveIcon);
                }
                break;
            case ShadowlingForceType.ShadowlingBasic:
            case ShadowlingForceType.ShadowlingBeginning:
            case ShadowlingForceType.ShadowlingMedium:
            case ShadowlingForceType.ShadowlingHigh:
            case ShadowlingForceType.ShadowlingFinal:
            case ShadowlingForceType.ShadowlingOverlord:
                if (_prototype.TryIndex<StatusIconPrototype>("Shadowling", out var shadowingIcon))
                {
                    result.Add(shadowingIcon);
                }
                break;
        }

        return result;
    }
}
