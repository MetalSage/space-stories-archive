
using Content.Server.Popups;
using Content.Server.Radio.Components;
using Content.Server.Stunnable;
using Content.Shared.IdentityManagement;
using Content.Shared.Mindshield.Components;
using Content.Shared.Stories.Mindshield;
using Content.Shared.Stories.Shadowling;

namespace Content.Server.Stories.Shadowling;
public sealed partial class ShadowlingSystem
{
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly StunSystem _stun = default!;

    public void InitializeThralls()
    {
        SubscribeLocalEvent<ShadowlingThrallComponent, MindShieldImplantedEvent>(OnMindShieldImplanted);
    }

    /// <summary>
    /// Make someone a thrall, set up all needed components (shadowling component, shadowling mind radio)
    /// </summary>
    public void Enthrall(EntityUid target, EntityUid shadowling)
    {
        var mastersComponent = Comp<ShadowlingComponent>(shadowling);
        mastersComponent.Slaves.Add(target);
        var slave = EnsureComp<ShadowlingThrallComponent>(target);
        slave.Master = shadowling;
        Dirty(target, slave);
        Dirty(shadowling, mastersComponent);
        var radio = EnsureComp<ActiveRadioComponent>(target);
        radio.Channels.Add(ShadowlingMindRadioPrototype);
    }

    public void Unthrall(EntityUid target, EntityUid shadowling)
    {
        var mastersComponent = Comp<ShadowlingComponent>(shadowling);
        mastersComponent.Slaves.Remove(target);
        Dirty(shadowling, mastersComponent);
        RemCompDeferred<ShadowlingThrallComponent>(target);
        RemCompDeferred<ActiveRadioComponent>(target);
    }

    private void OnMindShieldImplanted(EntityUid uid, ShadowlingThrallComponent comp, MindShieldImplantedEvent ev)
    {
        if (!TryComp<ShadowlingComponent>(uid, out var shadowling))
            return;

        if (!IsShadowlingSlave(uid) || shadowling.Stage == ShadowlingStage.Lower)
        {
            RemCompDeferred<MindShieldComponent>(uid);
            _popup.PopupEntity(Loc.GetString("shadowling-break-mindshield"), uid);
            return;
        }

        var stunTime = TimeSpan.FromSeconds(4);
        var name = Identity.Entity(uid, EntityManager);
        var thrallComponent = Comp<ShadowlingThrallComponent>(uid);

        Unthrall(uid, thrallComponent.Master);

        _stun.TryParalyze(uid, stunTime, true);
        _popup.PopupEntity(Loc.GetString("thrall-break-control", ("name", name)), uid);
    }

    public void UpgradeThrallToLowerShadowling(EntityUid thrall)
    {
        var lowerShadowling = EnsureComp<ShadowlingComponent>(thrall);
        SetStage(thrall, lowerShadowling, ShadowlingStage.Lower);
        _popup.PopupEntity("Ваше тело сливается с тенью...", thrall, thrall);
    }
}
