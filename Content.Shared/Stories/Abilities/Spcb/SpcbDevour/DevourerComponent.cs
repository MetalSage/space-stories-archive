using Content.Shared.Chemistry.Reagent;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.StatusIcon;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.SpcbDevour.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedSpcbDevourSystem))]
public sealed partial class SpcbDevourerComponent : Component
{
    [DataField("SpcbdevourAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? SpcbDevourAction = "ActionSpcbDevour";

    [DataField("SpcbdevourActionEntity")]
    public EntityUid? SpcbDevourActionEntity;

    [ViewVariables(VVAccess.ReadWrite), DataField("soundDevour")]
    public SoundSpecifier? SoundDevour = new SoundPathSpecifier("/Audio/Effects/demon_consume.ogg")
    {
        Params = AudioParams.Default.WithVolume(-3f),
    };

    [DataField("SpcbdevourTime")]
    public float SpcbDevourTime = 3f;

    [DataField("structureDevourTime")]
    public float StructureDevourTime = 10f;

    [ViewVariables(VVAccess.ReadWrite), DataField("soundStructureDevour")]
    public SoundSpecifier? SoundStructureDevour = new SoundPathSpecifier("/Audio/Machines/airlock_creaking.ogg")
    {
        Params = AudioParams.Default.WithVolume(-3f),
    };

    public Container Stomach = default!;


    [DataField("chemical", customTypeSerializer: typeof(PrototypeIdSerializer<ReagentPrototype>))]
    public string Chemical = "Omnizine";


    [ViewVariables(VVAccess.ReadWrite), DataField("healRate")]
    public float HealRate = 20f;

    [ViewVariables(VVAccess.ReadWrite), DataField("shouldStoreDevoured")]
    public bool ShouldStoreDevoured = true;

    [ViewVariables(VVAccess.ReadWrite), DataField("whitelist")]
    public EntityWhitelist? Whitelist = new()
    {
        Components = new[]
        {
            "MobState",
        }
    };

    [DataField("foodPreference")]
    public FoodPreference FoodPreference = FoodPreference.All;
}
