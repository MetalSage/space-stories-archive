using Content.Server.Shuttles.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Stories.Shuttles.Components;

public sealed class MapSpawnSystem : EntitySystem
{
    private override void Initialize()
    {
        SubscribeLocalEvent<MapSpawnComponent, StationPostInitEvent>(OnMapSpawnPostInit);
    }

    private OnMapSpawnPostInit(EntityUid uid, MapSpawnComponentm component, ref StationPostInitEvent args)
    {

    }

    private void GridSpawns(EntityUid uid, GridSpawnComponent component)
    {
        if (!TryComp<StationDataComponent>(uid, out var data))
        {
            return;
        }

        // Spawn on a map for spawn grids
        var valid = true;
        var paths = new List<ResPath>();

        foreach (var group in component.Groups.Values)
        {
            if (group.Paths.Count == 0)
            {
                Log.Error($"Found no paths for MapSpawn");
                continue;
            }

            var mapId = _mapManager.CreateMap();
            var count = _random.Next(group.MinCount, group.MaxCount);
            paths.Clear();

            for (var i = 0; i < count; i++)
            {
                // Round-robin so we try to avoid dupes where possible.
                if (paths.Count == 0)
                {
                    paths.AddRange(group.Paths);
                    _random.Shuffle(paths);
                }

                var path = paths[^1];
                paths.RemoveAt(paths.Count - 1);

                if (_loader.TryLoad(mapId, path.ToString(), out var ent) && ent.Count == 1)
                {
                    TryComp<ShuttleComponent>(ent[0], out var shuttle);

                    if (group.Hide)
                    {
                        var iffComp = EnsureComp<IFFComponent>(ent[0]);
                        iffComp.Flags |= IFFFlags.HideLabel;
                        Dirty(ent[0], iffComp);
                    }

                    if (group.NameGrid)
                    {
                        var name = path.FilenameWithoutExtension;
                        _metadata.SetEntityName(ent[0], name);
                    }

                    foreach (var compReg in group.AddComponents.Values)
                    {
                        var compType = compReg.Component.GetType();

                        if (HasComp(ent[0], compType))
                            continue;

                        var comp = _factory.GetComponent(compType);
                        AddComp(ent[0], comp, true);
                    }
                }
                else
                {
                    valid = false;
                }

                if (!valid)
                {
                    Log.Error($"Error loading gridspawn for {ToPrettyString(uid)} / {path}");
                }
            }
        }
    }
}
