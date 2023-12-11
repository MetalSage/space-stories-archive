using Content.Shared.Spcb.Components;
using Content.Client.Antag;
using Content.Shared.StatusIcon.Components;

namespace Content.Client.Spcb;


public sealed class SpcbSystem : AntagStatusIconSystem<SpcbComponent>
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpcbComponent, GetStatusIconsEvent>(GetSpcbIcon);
    }

    private void GetSpcbIcon(EntityUid uid, SpcbComponent comp, ref GetStatusIconsEvent args)
    {
        GetStatusIcon(comp.SpcbStatusIcon, ref args);
    }
}
