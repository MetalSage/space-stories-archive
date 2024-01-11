using Content.Server.Flash;
using Content.Server.Stunnable;
using Content.Shared.Flash;
using Content.Shared.Stories.Shadowling;

namespace Content.Server.Stories.Shadowling;

public sealed class ShadowlingGlareSystem : EntitySystem
{
    [Dependency] private readonly FlashSystem _flash = default!;
    [Dependency] private readonly ShadowlingSystem _shadowling = default!;
    [Dependency] private readonly StunSystem _stun = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingGlareEvent>(OnGlareEvent);
    }

    private void OnGlareEvent(EntityUid uid, ShadowlingComponent component, ref ShadowlingGlareEvent ev)
    {
        ev.Handled = true;
        var entities = _shadowling.GetEntitiesAroundShadowling<FlashableComponent>(uid, 15);

        foreach (var entity in entities)
        {
            var flashable = Comp<FlashableComponent>(entity);
            _flash.Flash(entity, uid, null, 15000, 0.8f, false, flashable);
            _stun.TryStun(entity, TimeSpan.FromSeconds(1), false);
        }
    }
}
