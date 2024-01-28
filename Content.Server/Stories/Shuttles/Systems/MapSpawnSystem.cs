using Content.Server.Shuttles.Components;
using Content.Server.Station.Events;
using Content.Shared.Shuttles.Components;
using Robust.Shared.Utility;

namespace Content.Server.Shuttles.Systems;

public sealed partial class ShuttleSystem
{
    private void InitializeMapSpawn()
    {
        SubscribeLocalEvent<MapSpawnComponent, StationPostInitEvent>(OnMapSpawnPostInit);
    }

    private void OnMapSpawnPostInit(EntityUid uid, MapSpawnComponent component, ref StationPostInitEvent args)
    {
        MapSpawns(uid, component);
    }

    private void MapSpawns(EntityUid uid, MapSpawnComponent component)
    {
        // Spawn a map for spawn grids
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

                if (_loader.TryLoad(mapId, path.ToString(), out var ent))
                {
                    TryComp<ShuttleComponent>(ent[0], out var shuttle);

                    if (group.Hide)
                    {
                        var iffComp = EnsureComp<IFFComponent>(ent[0]);
                        iffComp.Flags |= IFFFlags.HideLabel;
                        Dirty(ent[0], iffComp);
                    }

                }
                else
                {
                    Log.Error($"Error loading gridspawn for {ToPrettyString(uid)} / {path}");
                }
            }
        }
    }
}
