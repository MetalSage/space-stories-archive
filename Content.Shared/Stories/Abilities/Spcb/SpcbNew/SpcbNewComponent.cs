using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Chemistry.Reagent;

namespace Content.Shared.Abilities.SpcbNew;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedSpcbNewSystem))]
public sealed partial class SpcbNewComponent : Component
{
    [DataField("actionSpcbNew", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionSpcbNew = "ActionSpcbNew";

    [DataField("actionSpcbNewEntity")]
    public EntityUid? ActivateSpcbNewEntity;


    [ViewVariables(VVAccess.ReadWrite), DataField("TransMobSpawnId", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string TransMobSpawnId = "MobSpcb";

    [DataField("chemical", customTypeSerializer: typeof(PrototypeIdSerializer<ReagentPrototype>))]
    public string Chemical = "Ipecac";


    [ViewVariables(VVAccess.ReadWrite), DataField("healRate")]
    public float HealRate = 10f;
}
