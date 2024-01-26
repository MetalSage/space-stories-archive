using Content.Shared.Whitelist;

namespace Content.Server.Stories.FTLKey;

/// <summary>
/// Give grid FTLDestination component with settings
/// </summary>

[RegisterComponent]
public sealed partial class FTLMarkerComponent : Component
{
    [DataField("enabled"), ViewVariables(VVAccess.ReadWrite)]
    public bool Enabled = true;

    [DataField("whitelist"), ViewVariables(VVAccess.ReadWrite)]
    public EntityWhitelist Whitelist = new();
}

