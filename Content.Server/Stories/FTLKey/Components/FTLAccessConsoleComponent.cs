using Robust.Shared.Prototypes;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Stories.FTLKey;

[RegisterComponent]
public sealed partial class FTLAccessConsoleComponent : Component
{

    [DataField("slots")]
    public Dictionary<string, ItemSlot> Slots = new();
}

