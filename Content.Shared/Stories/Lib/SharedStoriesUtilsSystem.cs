using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rejuvenate;
using Content.Shared.Stories.Lib.Incorporeal;
using Content.Shared.Stories.Lib.Invisibility;
using Robust.Shared.Player;

namespace Content.Shared.Stories.Lib;

/// <summary>
/// A system that combines common methods from systems made by Space Stories
/// And containing shortcuts for Space Wizards code
/// </summary>
public abstract partial class SharedStoriesUtilsSystem : EntitySystem
{
    [Dependency] protected readonly IEntityManager _entity = default!;
    [Dependency] protected readonly MobStateSystem _mobState = default!;

    public IEnumerable<EntityUid> GetAliveMobList()
    {
        var entities = _entity.AllEntityQueryEnumerator<MobStateComponent>();

        while (entities.MoveNext(out var uid, out var mobState))
        {
            if (mobState.CurrentState == MobState.Alive)
                yield return uid;
        }
    }

    public bool IsMob(EntityUid uid)
    {
        return HasComp<MobStateComponent>(uid);
    }

    public bool IsIncorporeal(EntityUid uid)
    {
        return HasComp<IncorporealComponent>(uid);
    }

    public bool IsInConsciousness(EntityUid uid)
    {
        if (!TryComp<ActorComponent>(uid, out var actor))
            return false;

        return actor.PlayerSession == null;
    }
}
