namespace Content.Shared.SpaceStories.Shadowling;
public abstract class SharedShadowlingSystem : EntitySystem
{
    public bool IsShadowlingSlave(ShadowlingComponent component)
    {
        return component.Stage switch
        {
            ShadowlingStage.Thrall or ShadowlingStage.Lower => true,
            ShadowlingStage.Start or
            ShadowlingStage.Basic or
            ShadowlingStage.Medium or
            ShadowlingStage.High or
            ShadowlingStage.Final or
            ShadowlingStage.Ascended => false,
            _ => false,
        };
    }

    public void SetStage(EntityUid uid, ShadowlingComponent component, ShadowlingStage stage) {
        component.Stage = stage;
        Dirty(uid, component);
    }
}
