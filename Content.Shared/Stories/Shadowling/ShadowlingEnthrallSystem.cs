using Content.Shared.Body.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Mind.Components;
using Content.Shared.Mindshield.Components;
using Content.Shared.Popups;
using Robust.Shared.Serialization;

namespace Content.Shared.SpaceStories.Shadowling;
public sealed class ShadowlingEnthrallSystem : EntitySystem
{
    [Dependency] private readonly StaminaSystem _stamina = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingEnthrallEvent>(OnEnthrallEvent);
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingHypnosisEvent>(OnHypnosisEvent);
        SubscribeLocalEvent<ShadowlingForceComponent, EnthrallDoAfterEvent>(OnEnthrallDoAfterEvent);
    }

    private void OnEnthrallEvent(EntityUid uid, ShadowlingForceComponent component, ref ShadowlingEnthrallEvent ev)
    {
        if (ev.Handled)
            return;
        ev.Handled = true;

        // You cannot enthrall animals
        if (!TryComp<BodyComponent>(ev.Target, out var body) || body.Prototype == "Animal")
            return;
        // You cannot enthrall someone without mind
        if (!TryComp<MindContainerComponent>(ev.Target, out var mind) || !mind.HasMind)
            return;

        // You cannot enthrall someone or something not biological (borgs for example)
        if (!TryComp<DamageableComponent>(ev.Target, out var damage) || damage.DamageContainerID != "Biological")
            return;

        var coords = _transform.GetWorldPosition(ev.Target);
        var distance = (_transform.GetWorldPosition(uid) - coords).Length();

        if (distance > 2)
            return;

        if (TryComp<MindShieldComponent>(ev.Target, out var _))
        {
            _popup.PopupClient("Вы поглощаете чей-то разум... Некий барьер полностью отражает вашу атаку", ev.Performer, ev.Performer);
            _popup.PopupClient("Ваш разум поглощается тенями... Но некий барьер полностью изгоняет их из вашего разума", ev.Target, ev.Target);
            _stamina.TryTakeStamina(ev.Performer, 50);
            return;
        }

        _popup.PopupClient("Вы поглощаете чей-то разум... Вы ощущаете сопротивление, но вы постепенно одерживаете верх", ev.Performer, ev.Performer);
        _stamina.TryTakeStamina(ev.Target, 100);
        _popup.PopupClient("Ваш разум поглощается тенями... Вы сопротивляетесь, но тени постепенно одерживают верх", ev.Target, ev.Target);

        var doAfter = new DoAfterArgs(EntityManager, ev.Performer, 30, new EnthrallDoAfterEvent(), ev.Target)
        {
            BreakOnUserMove = true,
            BlockDuplicate = true,
            BreakOnDamage = true,
            BreakOnTargetMove = true,
        };

        _doAfterSystem.TryStartDoAfter(doAfter);
    }

    private void OnHypnosisEvent(EntityUid uid, ShadowlingForceComponent component, ref ShadowlingHypnosisEvent ev)
    {
        if (ev.Handled)
            return;
        ev.Handled = true;

        // You cannot enthrall animals
        if (!TryComp<BodyComponent>(ev.Target, out var body) || body.Prototype == "Animal")
            return;
        // You cannot enthrall someone without mind
        if (!TryComp<MindContainerComponent>(ev.Target, out var mind) || !mind.HasMind)
            return;

        // You cannot enthrall someone or something not biological (borgs for example)
        if (!TryComp<DamageableComponent>(ev.Target, out var damage) || damage.DamageContainerID != "Biological")
            return;

        var coords = _transform.GetWorldPosition(ev.Target);
        var distance = (_transform.GetWorldPosition(uid) - coords).Length();

        if (distance > 2)
            return;

        if (TryComp<MindShieldComponent>(ev.Target, out var _))
        {
            _popup.PopupClient("Некий барьер полностью отражает вашу атаку", ev.Performer, ev.Performer);
            _popup.PopupClient("Некий барьер отразил сильнейшую ментальную атаку", ev.Target, ev.Target);
            return;
        }

        _stamina.TryTakeStamina(ev.Target, 100);

        var doAfter = new DoAfterArgs(EntityManager, ev.Performer, 1, new EnthrallDoAfterEvent(), ev.Target)
        {
            BlockDuplicate = true,
        };

        _doAfterSystem.TryStartDoAfter(doAfter);
    }

    private void OnEnthrallDoAfterEvent(EntityUid uid, ShadowlingForceComponent component, ref EnthrallDoAfterEvent ev)
    {
        if (ev.Target is not { } target)
            return;

        _popup.PopupClient("Ваш разум поглощён тенями", target, target);
    }
}

/// <summary>
/// Is relayed at the end of the sericulturing doafter.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class EnthrallDoAfterEvent : SimpleDoAfterEvent
{
}
