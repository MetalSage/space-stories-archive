using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;
public sealed class ShadowlingBlackRecuperationSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly ShadowlingForceSystem _shadowling = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingBlackRecuperationEvent>(OnBlackRecuperationEvent);
    }

    private void OnBlackRecuperationEvent(EntityUid uid, ShadowlingForceComponent component, ShadowlingBlackRecuperationEvent ev)
    {
        if (!TryComp<ShadowlingForceComponent>(ev.Performer, out var _))
            return;

        if (!TryComp<ShadowlingForceComponent>(ev.Target, out var shadowlingSlave))
            return;

        if (!TryComp<DamageableComponent>(ev.Target, out var slaveDamageable))
            return;

        // you can't heal yourself!
        if (shadowlingSlave.ForceType != ShadowlingForceType.ShadowlingSlave || shadowlingSlave.ForceType != ShadowlingForceType.ShadowlingTrell)
            return;

        _damageable.SetAllDamage(ev.Target, slaveDamageable, 0);
        _popup.PopupClient("Ваши раны покрываются тенью и затягиваются...", ev.Target, ev.Target);

        if (shadowlingSlave.ForceType != ShadowlingForceType.ShadowlingTrell)
        {
            _shadowling.ChangeForceType(ev.Target, shadowlingSlave, ShadowlingForceType.ShadowlingTrell);
            _popup.PopupClient("Ваше тело сливается с тенью...", ev.Target, ev.Target);
        }
    }
}
