using Content.Server.Chat.Systems;
using Content.Shared.SpaceStories.Shadowling;

public sealed class ShadowlingEnthrallSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShadowlingComponent, EnthrallDoAfterEvent>(OnEnthrallDoAfterEvent);
    }

    private void OnEnthrallDoAfterEvent(EntityUid uid, ShadowlingComponent shadowling, ref EnthrallDoAfterEvent ev)
    {
        if (ev.Cancelled)
            return;
        ev.Handled = true;

        var announcementString = "Станция, говорит Центральное Командование. Сканерами дальнего действия обнаружена большая концентрация психической блюспейс-энергии. Событие вознесения тенеморфов неизбежно. Предотвратите это любой ценой!";
        _chat.DispatchGlobalAnnouncement(announcementString, colorOverride: Color.FromName("red"));
    }
}
