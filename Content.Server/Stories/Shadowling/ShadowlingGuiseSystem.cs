using Content.Server.Stealth;
using Content.Shared.SpaceStories.Shadowling;
using Robust.Shared.Timing;

namespace Content.Server.SpaceStories.Shadowling;

public sealed class ShadowlingGuiseSystem : EntitySystem
{
    [Dependency] private readonly StealthSystem _stealth = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShadowlingComponent, ShadowlingGuiseEvent>(OnGuise);
    }

    private void OnGuise(EntityUid uid, ShadowlingComponent component, ref ShadowlingGuiseEvent ev)
    {
        var curTime = _timing.CurTime;
        component.GuiseEndsAt = curTime.Add(component.GuiseEndsIn);
        _stealth.SetEnabled(uid, true);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;

        var query = EntityQueryEnumerator<ShadowlingComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.GuiseEndsAt > curTime)
            {
                _stealth.SetEnabled(uid, false);
            }
        }
    }
}
