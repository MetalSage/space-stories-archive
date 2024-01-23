using Content.Shared.Stories.Shadowling;
using Robust.Client.Graphics;
using Robust.Client.Player;

namespace Content.Client.Stories.Shadowling;

public sealed class ShadowlingHatchSystem : EntitySystem
{
    [Dependency] private readonly ILightManager _light = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShadowlingComponent, ShadowlingHatchDoAfterEvent>(OnHatchDoAfter);
    }

    private void OnHatchDoAfter(EntityUid uid, ShadowlingComponent component, ref ShadowlingHatchDoAfterEvent ev)
    {
        if (_player.LocalEntity == uid)
        {
            _light.DrawShadows = false;
            _light.DrawHardFov = false;
        }
    }
}
