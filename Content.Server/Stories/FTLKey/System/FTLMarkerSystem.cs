using Content.Server.Shuttles.Components;

namespace Content.Server.Stories.FTLKey;

public class FTLMarkerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FTLMarkerComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, FTLMarkerComponent component, ComponentInit args)
    {
        var gridUid = Transform(uid).GridUid;

        if (gridUid is null) return;

        var ftl = EnsureComp<FTLDestinationComponent>((EntityUid) gridUid);
        ftl.Enabled = component.Enabled;
        ftl.Whitelist = component.Whitelist;
    }
}

