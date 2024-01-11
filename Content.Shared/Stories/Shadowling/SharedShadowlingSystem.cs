namespace Content.Shared.Stories.Shadowling;
public abstract class SharedShadowlingSystem : EntitySystem
{
    public bool IsShadowlingSlave(EntityUid uid)
    {
        return HasComp<ShadowlingThrallComponent>(uid);
    }

    public void SetStage(EntityUid uid, ShadowlingComponent component, ShadowlingStage stage)
    {
        component.Stage = stage;
        Dirty(uid, component);
    }

    public bool IsLowerShadowling(EntityUid uid, ShadowlingComponent? shadowling = null)
    {
        if (!Resolve(uid, ref shadowling, false))
            return false;

        return shadowling.Stage == ShadowlingStage.Lower;
    }
}
