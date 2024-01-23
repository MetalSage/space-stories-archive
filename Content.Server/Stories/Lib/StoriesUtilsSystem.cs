using Content.Server.Stories.Lib.TemporalLightOff;
using Content.Shared.Rejuvenate;
using Content.Shared.Stories.Lib;
using Content.Shared.Stories.Lib.Invisibility;
using Robust.Server.GameObjects;

namespace Content.Server.Stories.Lib;

/// <summary>
/// A system that combines common methods from systems made by Space Stories
/// And containing shortcuts for Space Wizards code
/// </summary>
public sealed partial class StoriesUtilsSystem : SharedStoriesUtilsSystem
{
    public void Rejuvenate(EntityUid uid)
    {
        if (!IsMob(uid))
        {
            Log.Error("Tried to rejuvenate not a mob");
            return;
        }

        var rejuvenate = new RejuvenateEvent();
        RaiseLocalEvent(uid, rejuvenate);
    }

    public void MakeInvisible(EntityUid uid)
    {
        EnsureComp<InvisibleComponent>(uid);
    }

    public void MakeVisible(EntityUid uid)
    {
        if (HasComp<InvisibleComponent>(uid))
        {
            RemCompDeferred<InvisibleComponent>(uid);
        }
    }
}
