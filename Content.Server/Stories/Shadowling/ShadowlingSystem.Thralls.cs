using System.Linq;
using Content.Server.Popups;
using Content.Server.Radio.Components;
using Content.Server.Stories.Lib;
using Content.Server.Stunnable;
using Content.Shared.IdentityManagement;
using Content.Shared.Mindshield.Components;
using Content.Shared.Stories.Mindshield;

namespace Content.Server.Stories.Shadowling;
public sealed partial class ShadowlingSystem
{
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly StoriesUtilsSystem _utils = default!;

    public void InitializeThralls()
    {
        SubscribeLocalEvent<ShadowlingComponent, MindShieldImplantedEvent>(OnMindShieldImplanted);
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
        EnsureComp<ShadowlingThrallRoleComponent>(target);
    }

    public void Unthrall(EntityUid target, EntityUid shadowling)
    {
        var mastersComponent = Comp<ShadowlingComponent>(shadowling);
        mastersComponent.Slaves.Remove(target);
        Dirty(shadowling, mastersComponent);
        RemCompDeferred<ShadowlingThrallComponent>(target);
        RemCompDeferred<ShadowlingThrallRoleComponent>(target);
        var radio = Comp<ActiveRadioComponent>(target);
        radio.Channels.Remove(ShadowlingMindRadioPrototype);
    }

    private void OnMindShieldImplanted(EntityUid uid, ShadowlingComponent comp, MindShieldImplantedEvent ev)
    {
        RemCompDeferred<MindShieldComponent>(uid);
        _popup.PopupEntity(Loc.GetString("shadowling-break-mindshield"), uid);
    }
    private void OnMindShieldImplanted(EntityUid uid, ShadowlingThrallComponent comp, MindShieldImplantedEvent ev)
    {
        var stunTime = TimeSpan.FromSeconds(4);
        var name = Identity.Entity(uid, EntityManager);

        var thrallComponent = Comp<ShadowlingThrallComponent>(uid);
        Unthrall(uid, thrallComponent.Master);

        _stun.TryParalyze(uid, stunTime, true);
        _popup.PopupEntity(Loc.GetString("thrall-break-control", ("name", name)), uid);
    }

    public IEnumerable<EntityUid> GetThralls()
    {
        var mobs = _utils.GetAliveMobList();
        return mobs.Where(IsThrall);
    }
}
