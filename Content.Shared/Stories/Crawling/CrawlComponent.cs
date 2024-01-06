using Robust.Shared.GameStates;

namespace Content.Shared.Stories.Crawling;
/// <summary>
/// Component providing to entities ability to crawl
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedCrawlingController))]
public sealed partial class CrawlComponent : Component
{
    /// <summary>
    /// Is entity crawling right now
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool Crawling { get; set; } = false;

    /// <summary>
    /// Walk speed modificator which applies while entity crawling
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float WalkSpeedModifier = 0.5f;

    /// <summary>
    /// Walk speed modificator which applies while entity crawling
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float SprintSpeedModifier = 0.5f;

    /// <summary>
    /// Delay used in do after to lie down
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan LieDownDelay = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Delay used in do after to get up
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan GetUpDelay = TimeSpan.FromSeconds(1);
}
