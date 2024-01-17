using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Robust.Shared.Map;
using Robust.Server.GameObjects;
using Robust.Shared.Spawners;
using Content.Shared.Mobs;
using Content.Shared.Tag;

namespace Content.Server.Stories.MappingThings;

public sealed partial class ShuttleSummonSystem : EntitySystem
{
    [Dependency] private readonly MapLoaderSystem _loader = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly ShuttleSystem _shuttles = default!;
    [Dependency] private readonly TagSystem _tagSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MarkerShuttleSummonComponent, MapInitEvent>(MarkerShuttleSummonInit);

        SubscribeLocalEvent<MarkerShuttleERTDockComponent, MapInitEvent>(MarkerShuttleInit);
        SubscribeLocalEvent<MarkerShuttleCBURNDockComponent, MapInitEvent>(MarkerShuttleInit);
        SubscribeLocalEvent<MarkerShuttleDeadSquadDockComponent, MapInitEvent>(MarkerShuttleInit);
    }

    private void MarkerShuttleSummonInit(EntityUid uid, MarkerShuttleSummonComponent component, MapInitEvent args)
    {
        MarkedDockNearBy(uid, component.DoorTag);

        ShuttleSpawn(uid, component.ShuttlePath, component.DoorTag);
    }

    private void MarkerShuttleInit(EntityUid uid, MarkerShuttleCBURNDockComponent component, MapInitEvent args)
    {
        MarkedDockNearBy(uid, component.DoorTag);
    }

    private void MarkerShuttleInit(EntityUid uid, MarkerShuttleERTDockComponent component, MapInitEvent args)
    {
        MarkedDockNearBy(uid, component.DoorTag);
    }

    private void MarkerShuttleInit(EntityUid uid, MarkerShuttleDeadSquadDockComponent component, MapInitEvent args)
    {
        MarkedDockNearBy(uid, component.DoorTag);
    }

    private void MarkedDockNearBy(EntityUid uid, string tag)
    {
        // Find Dock to give it marker Tag
        var query = AllEntityQuery<DockingComponent>();
        while (query.MoveNext(out var dockUid, out var comp))
        {
            var dockTrans = Comp<TransformComponent>(dockUid);
            var pointTrans = Comp<TransformComponent>(uid);

            if (dockTrans is null || pointTrans is null) continue;
            if (dockTrans.GridUid != pointTrans.GridUid || dockTrans.LocalPosition != pointTrans.LocalPosition) continue;

            var tagComponent = EnsureComp<TagComponent>(dockUid);
            _tagSystem.AddTag(tagComponent, tag);
        }
    }

    public void ShuttleSpawn(EntityUid uid, string shuttlePath, string doorTag)
    {
        // Find target grid
        var xform = _entityManager.GetComponent<TransformComponent>(uid);
        if (xform is null || xform.GridUid is null) return;
        var targetGrid = (EntityUid) xform.GridUid;

        // Create shuttle
        var dummyMap = _mapManager.CreateMap();

        if (_loader.TryLoad(dummyMap, shuttlePath.ToString(), out var shuttleUids))
        {
            var shuttleUid = shuttleUids[0];
            var shuttleComp = Comp<ShuttleComponent>(shuttleUid);

            _shuttles.FTLTravel(shuttleUid, shuttleComp, targetGrid, 1f, 1f, true, doorTag);
        }
        var timer = AddComp<TimedDespawnComponent>(_mapManager.GetMapEntityId(dummyMap));
        timer.Lifetime = 5f;
    }
}


