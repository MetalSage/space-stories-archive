namespace Content.Server.Stories.FTLKey;

[RegisterComponent]
public sealed partial class FTLKeyComponent : Component
{
    [DataField("access"), ViewVariables(VVAccess.ReadWrite)]
    public List<string>? FTLKeys = new();
}
