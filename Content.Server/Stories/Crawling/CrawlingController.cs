using Content.Server.DoAfter;
using Content.Shared.DoAfter;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.Stories.Crawling;

namespace Content.Server.Stories.Crawling;
public sealed class CrawlingController : SharedCrawlingController
{
    [Dependency] private readonly StandingStateSystem _standing = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CrawlComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeedModifiersEvent);
        SubscribeLocalEvent<CrawlComponent, BeginCrawlingDoAfterEvent>(OnBeginCrawlingDoAfterEvent);
        SubscribeLocalEvent<CrawlComponent, EndCrawlingDoAfterEvent>(OnEndCrawlingDoAfterEvent);
    }

    protected override void HandleToggleCrawlInput(EntityUid uid)
    {
        if (!TryComp<CrawlComponent>(uid, out var crawl))
            return;

        if (crawl.Crawling)
        {
            EndCrawling(uid, crawl);
        }
        else
        {
            BeginCrawling(uid, crawl);
        }
    }

    private void SetCrawling(EntityUid uid, CrawlComponent crawl, bool crawling)
    {
        crawl.Crawling = crawling;
        Dirty(uid, crawl);
        _movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
    }

    public void BeginCrawling(EntityUid uid, CrawlComponent crawl)
    {
        var doAfterArgs = new DoAfterArgs(EntityManager, uid, crawl.LieDownDelay, new BeginCrawlingDoAfterEvent(), uid)
        {
            BlockDuplicate = true,
        };
        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    public void EndCrawling(EntityUid uid, CrawlComponent crawl)
    {
        var doAfterArgs = new DoAfterArgs(EntityManager, uid, crawl.GetUpDelay, new EndCrawlingDoAfterEvent(), uid)
        {
            BlockDuplicate = true,
        };
        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnBeginCrawlingDoAfterEvent(EntityUid uid, CrawlComponent crawl, ref BeginCrawlingDoAfterEvent ev)
    {
        if (ev.Cancelled)
            return;

        SetCrawling(uid, crawl, true);
        _standing.Down(uid);
    }

    private void OnEndCrawlingDoAfterEvent(EntityUid uid, CrawlComponent crawl, ref EndCrawlingDoAfterEvent ev)
    {
        if (ev.Cancelled)
            return;

        SetCrawling(uid, crawl, false);
        _standing.Stand(uid);
    }

    private void OnRefreshMovementSpeedModifiersEvent(EntityUid uid, CrawlComponent crawl, ref RefreshMovementSpeedModifiersEvent ev)
    {
        if (!crawl.Crawling)
            return;

        ev.ModifySpeed(crawl.WalkSpeedModifier, crawl.SprintSpeedModifier);
    }
}

