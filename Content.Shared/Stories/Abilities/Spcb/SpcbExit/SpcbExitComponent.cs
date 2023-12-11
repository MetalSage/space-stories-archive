using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Abilities.SpcbExit;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedSpcbExitSystem))]
public sealed partial class SpcbExitComponent : Component
{
    [DataField("actionSpcbExit", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionSpcbExit = "ActionSpcbExit";

    [DataField("actionSpcbExitEntity")]
    public EntityUid? ActivateSpcbExitEntity;


    [ViewVariables(VVAccess.ReadWrite), DataField("TransMobSpawnId", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string TransMobSpawnId = "MobSpcb";
}
