using System.Threading;
using Content.Server.Body.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.SpaceStories.Shadowling;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Server.SpaceStories.Shadowling;
public sealed class ShadowlingAnnihilateSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly ExplosionSystem _explosion = default!;
    [Dependency] private readonly BodySystem _body = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShadowlingComponent, ShadowlingAnnihilateEvent>(OnAnnihilateEvent);
    }

    private void OnAnnihilateEvent(EntityUid uid, ShadowlingComponent component, ShadowlingAnnihilateEvent ev)
    {
        if (!TryComp<ShadowlingComponent>(ev.Performer, out var _))
            return;

        if (!TryComp<DamageableComponent>(ev.Target, out var slaveDamageable))
            return;

        var coords = _transform.GetMapCoordinates(ev.Target);
        Timer.Spawn(_gameTiming.TickPeriod,
            () => _explosion.QueueExplosion(coords, ExplosionSystem.DefaultExplosionPrototypeId,
                4, 1, 2, maxTileBreak: 0), // it gibs, damage doesn't need to be high.
            CancellationToken.None);

        _body.GibBody(ev.Target);
    }
}
