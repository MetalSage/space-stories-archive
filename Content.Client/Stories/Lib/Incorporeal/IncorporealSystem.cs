using Content.Shared.Stories.Lib.Incorporeal;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Stories.Lib.Incorporeal;
public sealed partial class IncorporealSystem : EntitySystem
{
    [Dependency] private readonly ILightManager _light = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IEyeManager _eye = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IncorporealComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<IncorporealComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<IncorporealComponent, PlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<IncorporealComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnStartup(EntityUid uid, IncorporealComponent component, ref ComponentStartup ev)
    {
        if (_player.LocalEntity != uid)
            return;

        _light.DrawShadows = false;
        _eye.CurrentEye.DrawFov = false;
    }

    private void OnPlayerAttached(EntityUid uid, IncorporealComponent component, ref PlayerAttachedEvent ev)
    {
        if (_player.LocalEntity != uid)
            return;

        _light.DrawShadows = false;
        _eye.CurrentEye.DrawFov = false;
    }

    private void OnPlayerDetached(EntityUid uid, IncorporealComponent component, ref PlayerDetachedEvent ev)
    {
        if (_player.LocalEntity != uid)
            return;

        if (component.TurnRenderBack)
        {
            _light.DrawShadows = true;
            _eye.CurrentEye.DrawFov = true;
        }
    }

    private void OnShutdown(EntityUid uid, IncorporealComponent component, ref ComponentShutdown ev)
    {
        if (_player.LocalEntity != uid)
            return;

        if (component.TurnRenderBack)
        {
            _light.DrawShadows = true;
            _eye.CurrentEye.DrawFov = true;
        }
    }
}
