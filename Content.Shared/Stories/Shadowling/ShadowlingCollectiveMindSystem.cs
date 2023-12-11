using Content.Shared.Popups;

namespace Content.Shared.SpaceStories.Shadowling;
public sealed class ShadowlingCollectiveMindSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingForceComponent, ShadowlingCollectiveMindEvent>(OnCollectiveEvent);
    }

    private void OnCollectiveEvent(EntityUid uid, ShadowlingForceComponent component, ref ShadowlingCollectiveMindEvent ev)
    {
        _popup.PopupClient(string.Format("У вас {0} порабощённых", component.Slaves.Count), uid, uid);

        ShadowlingForceType? nextPhase = null;

        switch (component.ForceType)
        {
            case ShadowlingForceType.ShadowlingBasic:
                if (component.Slaves.Count >= 3)
                    nextPhase = ShadowlingForceType.ShadowlingBeginning;
                break;
            case ShadowlingForceType.ShadowlingBeginning:
                if (component.Slaves.Count >= 5)
                    nextPhase = ShadowlingForceType.ShadowlingMedium;
                break;
            case ShadowlingForceType.ShadowlingMedium:
                if (component.Slaves.Count >= 9)
                    nextPhase = ShadowlingForceType.ShadowlingHigh;
                break;
            case ShadowlingForceType.ShadowlingHigh:
                if (component.Slaves.Count >= 15)
                    nextPhase = ShadowlingForceType.ShadowlingFinal;
                break;
        }

        if (nextPhase is not { } notNullNextPhase)
            return;

        _popup.PopupClient("Новые способности разблокированы", uid, uid);
        component.ForceType = notNullNextPhase;
        Dirty(uid, component);
    }
}
