using Content.Shared.Stories.Shadowling;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Stories.Shadowling;

[RegisterComponent]
public sealed partial class ShadowlingComponent : SharedShadowlingComponent
{
    [ViewVariables(VVAccess.ReadOnly), DataField("slaves")]
    public List<EntityUid> Slaves = new();

    [ViewVariables(VVAccess.ReadOnly), DataField("inShadowWalk")]
    public bool InShadowWalk;

    [ViewVariables(VVAccess.ReadOnly), DataField("shadowWalkEndsAt", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan ShadowWalkEndsAt = TimeSpan.Zero;

    [ViewVariables(VVAccess.ReadOnly), DataField("shadowWalkEndsIn", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan ShadowWalkEndsIn = TimeSpan.FromSeconds(3);

    [ViewVariables(VVAccess.ReadOnly), DataField("grantedActions")]
    public List<string> GrantedActions = new();
}
