using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;
public sealed class ShadowlingEnthrallSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entity = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, EnthrallDoAfterEvent>(OnEnthrallDoAfterEvent);
    }

    private void OnEnthrallDoAfterEvent(EntityUid uid, ShadowlingComponent component, ref EnthrallDoAfterEvent ev)
    {
        if (ev.Target is not { } target)
            return;

        if (!TryComp<ShadowlingComponent>(ev.User, out var shadowling))
            return;

        shadowling.Slaves.Add(target);
        _entity.AddComponent<ShadowlingComponent>(target);

        Dirty(ev.User, shadowling);
    }
}
