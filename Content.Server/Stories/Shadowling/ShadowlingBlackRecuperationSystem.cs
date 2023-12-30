using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;
public sealed class ShadowlingBlackRecuperationSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedShadowlingSystem _shadowling = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingBlackRecuperationEvent>(OnBlackRecuperationEvent);
    }

    private void OnBlackRecuperationEvent(EntityUid uid, ShadowlingComponent component, ShadowlingBlackRecuperationEvent ev)
    {
        if (!TryComp<ShadowlingComponent>(ev.Performer, out var _))
            return;

        if (!TryComp<ShadowlingComponent>(ev.Target, out var shadowlingSlave))
            return;

        if (!TryComp<DamageableComponent>(ev.Target, out var slaveDamageable))
            return;

        if (!TryComp<MobStateComponent>(ev.Target, out var slaveState))
            return;

        // you can't heal yourself!
        if (shadowlingSlave.Stage != ShadowlingStage.Thrall && shadowlingSlave.Stage != ShadowlingStage.Lower)
            return;

        ev.Handled = true;

        if (slaveState.CurrentState != MobState.Dead && shadowlingSlave.Stage == ShadowlingStage.Lower)
        {
            _shadowling.SetStage(ev.Target, shadowlingSlave, ShadowlingStage.Lower);
            _popup.PopupClient("Ваше тело сливается с тенью...", ev.Target, ev.Target);
        }
        else
        {
            _damageable.SetAllDamage(ev.Target, slaveDamageable, 0);
            _mobState.ChangeMobState(ev.Target, MobState.Alive);
            _popup.PopupClient("Ваши раны покрываются тенью и затягиваются...", ev.Target, ev.Target);
        }
    }
}
