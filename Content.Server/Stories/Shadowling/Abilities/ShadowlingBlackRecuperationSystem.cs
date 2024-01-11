using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Rejuvenate;
using Content.Shared.Stories.Shadowling;

namespace Content.Server.Stories.Shadowling;
public sealed class ShadowlingBlackRecuperationSystem : EntitySystem
{
    [Dependency] private readonly ShadowlingSystem _shadowling = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingBlackRecuperationEvent>(OnBlackRecuperationEvent);
    }

    private void OnBlackRecuperationEvent(EntityUid uid, ShadowlingComponent component, ShadowlingBlackRecuperationEvent ev)
    {
        if (ev.Handled)
            return;

        if (!TryComp<ShadowlingComponent>(ev.Performer, out _))
            return;

        if (!TryComp<DamageableComponent>(ev.Target, out var slaveDamageable))
            return;

        if (!TryComp<MobStateComponent>(ev.Target, out var slaveState))
            return;

        // you can't heal yourself!
        if (uid == ev.Target)
            return;

        ev.Handled = true;

        if (HasComp<ShadowlingThrallComponent>(ev.Target))
        {
            var rejuvenate = new RejuvenateEvent();
            RaiseLocalEvent(ev.Target, rejuvenate);

            if (slaveState.CurrentState == MobState.Alive && !_shadowling.IsLowerShadowling(uid))
            {
                _shadowling.UpgradeThrallToLowerShadowling(ev.Target);
            }
            else
            {
                _popup.PopupEntity("Ваши раны покрываются тенью и затягиваются...", ev.Target, ev.Target);
            }
        }
    }
}
