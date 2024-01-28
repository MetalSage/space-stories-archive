using Content.Server.GameTicking;
using Content.Server.Spawners.Components;
using Content.Server.Station.Systems;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Content.Shared.Roles.Jobs;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server.Spawners.EntitySystems;

public sealed class SpawnPointSystem : EntitySystem
{
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly StationSpawningSystem _stationSpawning = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PlayerSpawningEvent>(OnSpawnPlayer);
    }

    private void OnSpawnPlayer(PlayerSpawningEvent args)
    {
        if (args.SpawnResult != null)
            return;

        // TODO: Cache all this if it ends up important.
        var points = EntityQueryEnumerator<SpawnPointComponent, TransformComponent>();
        var possiblePositions = new List<EntityCoordinates>();

        // This list is needed so that roles with this name are spawned not in the terminal, but at the central command station.
        List<string?> CCJobsList = new List<string?>() { "job-name-operator-cent-comm", "job-name-officer-cent-comm", "job-name-delegat-cent-comm", "job-name-service-cent-comm", "job-name-engineer-cent-comm", "job-name-medic-cent-comm", "job-name-head-of-staff-cent-comm" };

        while ( points.MoveNext(out var uid, out var spawnPoint, out var xform))
        {
            _protoManager.TryIndex(args.Job?.Prototype ?? string.Empty, out JobPrototype? prototype); // Space Stories for CC jobs (late join)

            if (CCJobsList.Contains(prototype?.Name)) // We determine whether the role chosen by the player is a role from the list of Central Command professions
            {
                if (_gameTicker.RunLevel == GameRunLevel.InRound &&
                spawnPoint.SpawnType == SpawnPointType.CCJob) // If the joins are later, then we are looking for spawners marked "CCJob"
                {
                    possiblePositions.Add(xform.Coordinates);
                }

                if (_gameTicker.RunLevel != GameRunLevel.InRound &&
                spawnPoint.SpawnType == SpawnPointType.Job &&
                (args.Job == null || spawnPoint.Job?.ID == args.Job.Prototype)) // If joining is a round-start, then we are looking for simple role spawners corresponding to the chosen profession
                {
                    possiblePositions.Add(xform.Coordinates);
                }

            }
            else
            {
                if (args.Station != null && _stationSystem.GetOwningStation(uid, xform) != args.Station)
                    continue;


                if (_gameTicker.RunLevel == GameRunLevel.InRound && spawnPoint.SpawnType == SpawnPointType.LateJoin)
                {
                    possiblePositions.Add(xform.Coordinates);
                }


                if (_gameTicker.RunLevel != GameRunLevel.InRound &&
                spawnPoint.SpawnType == SpawnPointType.Job &&
                (args.Job == null || spawnPoint.Job?.ID == args.Job.Prototype))
                {
                    possiblePositions.Add(xform.Coordinates);
                }
            }
        }

        if (possiblePositions.Count == 0)
        {
            // Ok we've still not returned, but we need to put them /somewhere/.
            // TODO: Refactor gameticker spawning code so we don't have to do this!
            var points2 = EntityQueryEnumerator<SpawnPointComponent, TransformComponent>();

            if (points2.MoveNext(out var uid, out var spawnPoint, out var xform))
            {
                possiblePositions.Add(xform.Coordinates);
            }
            else
            {
                Log.Error("No spawn points were available!");
                return;
            }
        }

        var spawnLoc = _random.Pick(possiblePositions);

        args.SpawnResult = _stationSpawning.SpawnPlayerMob(
            spawnLoc,
            args.Job,
            args.HumanoidCharacterProfile,
            args.Station);
    }
}
