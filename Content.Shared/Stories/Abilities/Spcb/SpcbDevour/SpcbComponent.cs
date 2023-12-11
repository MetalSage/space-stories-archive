using Robust.Shared.GameStates;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;

namespace Content.Shared.Spcb.Components;


[RegisterComponent, NetworkedComponent]
public sealed partial class SpcbComponent : Component
{

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<StatusIconPrototype> SpcbStatusIcon = "SpcbFaction";
}
