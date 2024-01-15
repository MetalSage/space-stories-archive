using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared.SpaceStories.Bioluminescence;

[RegisterComponent]
[NetworkedComponent]
public sealed partial class BioluminescenceComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("action")]
    public string Action = "TurnBioluminescenceAction";
}

public sealed partial class TurnBioluminescenceEvent : InstantActionEvent
{
}
