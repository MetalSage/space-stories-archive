using Robust.Shared.Audio;

namespace Content.Server.Stories.Implants;

[RegisterComponent]
public sealed partial class InjectOnTriggerComponent : Component
{
    [DataField("solutions")]
    public Dictionary<string, bool> NotUsedSolutions = new();

    [DataField("injectSound")]
    public SoundSpecifier InjectSound = new SoundPathSpecifier("/Audio/Items/hypospray.ogg");
}
