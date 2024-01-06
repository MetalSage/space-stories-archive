using Content.Shared.DoAfter;
using Content.Shared.Input;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Player;
using Robust.Shared.Serialization;

namespace Content.Shared.Stories.Crawling;
public abstract partial class SharedCrawlingController : VirtualController
{
    public override void Initialize()
    {
        base.Initialize();

        CommandBinds.Builder
            .Bind(ContentKeyFunctions.ToggleCrawling, new CrawlInputCmdHandler(this))
            .Register<SharedCrawlingController>();
    }

    protected virtual void HandleToggleCrawlInput(EntityUid uid) { }

    private sealed class CrawlInputCmdHandler : InputCmdHandler
    {
        private SharedCrawlingController _controller;

        public CrawlInputCmdHandler(SharedCrawlingController controller)
        {
            _controller = controller;
        }

        public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
        {
            if (session?.AttachedEntity == null) return true;

            if (message.State == BoundKeyState.Down)
                _controller.HandleToggleCrawlInput(session.AttachedEntity.Value);

            return true;
        }
    }
}

[Serializable, NetSerializable]
public sealed partial class BeginCrawlingDoAfterEvent : SimpleDoAfterEvent
{
}

[Serializable, NetSerializable]
public sealed partial class EndCrawlingDoAfterEvent : SimpleDoAfterEvent
{
}
