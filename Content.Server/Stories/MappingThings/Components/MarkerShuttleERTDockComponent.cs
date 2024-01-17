namespace Content.Server.Stories.MappingThings;

[RegisterComponent]
public sealed partial class MarkerShuttleERTDockComponent : Component
{
    [DataField("ShuttlePath"), ViewVariables(VVAccess.ReadWrite)]
    public string ShuttlePath = "/Maps/Shuttles/cargo.yml";

    [DataField("DockingTag")]
    public string DoorTag = "ERTShuttleSummon";

}
