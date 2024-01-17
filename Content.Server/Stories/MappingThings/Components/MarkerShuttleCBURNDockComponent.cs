namespace Content.Server.Stories.MappingThings;

[RegisterComponent]
public sealed partial class MarkerShuttleCBURNDockComponent : Component
{
    [DataField("ShuttlePath"), ViewVariables(VVAccess.ReadWrite)]
    public string ShuttlePath = "/Maps/Shuttles/dart.yml";

    [DataField("DockingTag")]
    public string DoorTag = "CBURNShuttleSummon";

}
