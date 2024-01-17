namespace Content.Server.Stories.MappingThings;

[RegisterComponent]
public sealed partial class MarkerShuttleSummonComponent : Component
{
    [DataField("ShuttlePath"), ViewVariables(VVAccess.ReadWrite)]
    public string ShuttlePath = "/Maps/Shuttles/cargo.yml";

    [DataField("DockingTag"), ViewVariables(VVAccess.ReadWrite)]
    public string DoorTag = "ShuttleSummonTag1";

}
