using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Shuttles.Systems;

/// <summary>
/// Similar to <see cref="GridFillComponent"/> except spawns the grid near to the station.
/// </summary>
[RegisterComponent, Access(typeof(ShuttleSystem))]
public sealed partial class MapSpawnComponent : Component
{
    /// <summary>
    /// Dictionary of groups where each group will have entries selected.
    /// String is just an identifier to make yaml easier.
    /// </summary>
    [DataField("groups", required: true)] public Dictionary<string, MapSpawnGroup> Groups = new();
}

[DataRecord]
public record struct MapSpawnGroup
{
    public List<ResPath> Paths = new();

    public int MinCount = 1;
    public int MaxCount = 1;

    /// <summary>
    /// Hide the IFF label of the grid.
    /// </summary>
    public bool Hide = false;

    /// <summary>
    /// Should we set the metadata name of a grid. Useful for admin purposes.
    /// </summary>

    public MapSpawnGroup()
    {
    }
}


