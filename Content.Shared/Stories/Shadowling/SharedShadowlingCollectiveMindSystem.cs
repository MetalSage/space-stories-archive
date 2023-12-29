using Content.Shared.Popups;

namespace Content.Shared.SpaceStories.Shadowling;
public sealed class SharedShadowlingCollectiveMindSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingCollectiveMindEvent>(OnCollectiveEvent);
    }

    private void OnCollectiveEvent(EntityUid uid, ShadowlingComponent component, ref ShadowlingCollectiveMindEvent ev)
    {
        _popup.PopupEntity(string.Format("У вас {0} порабощённых", component.Slaves.Count), uid, uid);

        ShadowlingStage? nextPhase = null;

        switch (component.Stage)
        {
            case ShadowlingStage.Start:
                if (component.Slaves.Count >= 3)
                    nextPhase = ShadowlingStage.Basic;
                break;
            case ShadowlingStage.Basic:
                if (component.Slaves.Count >= 5)
                    nextPhase = ShadowlingStage.Medium;
                break;
            case ShadowlingStage.Medium:
                if (component.Slaves.Count >= 9)
                    nextPhase = ShadowlingStage.High;
                break;
            case ShadowlingStage.High:
                if (component.Slaves.Count >= 15)
                    nextPhase = ShadowlingStage.Final;
                break;
        }

        if (nextPhase is not { } notNullNextPhase)
            return;

        _popup.PopupEntity("Новые способности разблокированы", uid, uid);
        component.Stage = notNullNextPhase;
        Dirty(uid, component);
    }
}
