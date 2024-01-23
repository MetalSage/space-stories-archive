using Content.Shared.Stories.Shadowling;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Stories.Shadowling;

[RegisterComponent]
public sealed partial class ShadowlingThrallComponent : SharedShadowlingThrallComponent
{
    [DataField("master"), ViewVariables(VVAccess.ReadOnly)]
    public EntityUid Master;
}
