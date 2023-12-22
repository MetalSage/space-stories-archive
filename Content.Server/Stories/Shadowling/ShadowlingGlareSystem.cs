using Content.Server.Flash;
using Content.Shared.Flash;
using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;

public sealed class ShadowlingGlareSystem : EntitySystem
{
    [Dependency] private readonly FlashSystem _flash = default!;
    [Dependency] private readonly ShadowlingSystem _shadowling = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingGlareEvent>(OnGlareEvent);
    }

    private void OnGlareEvent(EntityUid uid, ShadowlingComponent component, ref ShadowlingGlareEvent ev)
    {
        var entities = _shadowling.GetEntitiesAroundShadowling<FlashableComponent>(uid, 15);

        foreach (var entity in entities)
        {
            var flashable = Comp<FlashableComponent>(uid);
            _flash.Flash(entity, uid, uid, 15, 0.8f, false, flashable);
        }
    }
}
