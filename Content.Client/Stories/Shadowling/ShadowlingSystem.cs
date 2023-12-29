using Content.Shared.SpaceStories.Shadowling;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.SpaceStories.Shadowling;

public sealed class ShadowlingSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly ILightManager _light = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShadowlingComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ShadowlingComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<ShadowlingComponent, PlayerDetachedEvent>(OnPlayerDetached);
    }

    private void OnStartup(EntityUid uid, ShadowlingComponent component, ref ComponentStartup ev)
    {
        Log.Debug("OnStartup; Loc ent: {0}; Ent: {1}; Stage: {2}", _player.LocalEntity, uid, component.Stage);
        if (_player.LocalEntity != uid || component.Stage == ShadowlingStage.Beginning)
            return;

        _light.DrawShadows = false;
    }

    private void OnPlayerAttached(EntityUid uid, ShadowlingComponent component, ref PlayerAttachedEvent ev)
    {
        Log.Debug("OnAttached; Loc ent: {0}; Ent: {1}; Stage: {2}", _player.LocalEntity, uid, component.Stage);
        if (_player.LocalEntity != uid || component.Stage == ShadowlingStage.Beginning)
            return;

        _light.DrawShadows = false;
    }

    private void OnPlayerDetached(EntityUid uid, ShadowlingComponent component, ref PlayerDetachedEvent ev)
    {
        Log.Debug("OnDetached; Loc ent: {0}; Ent: {1}; Stage: {2}", _player.LocalEntity, uid, component.Stage);
        if (_player.LocalEntity != uid || component.Stage == ShadowlingStage.Beginning)
            return;

        _light.DrawShadows = true;
    }

    public bool IsShadowlingSlave(ShadowlingComponent component)
    {
        return component.Stage switch
        {
            ShadowlingStage.Thrall or ShadowlingStage.Lower => true,
            ShadowlingStage.Start or
            ShadowlingStage.Basic or
            ShadowlingStage.Medium or
            ShadowlingStage.High or
            ShadowlingStage.Final or
            ShadowlingStage.Ascended => false,
            _ => false,
        };
    }
}
