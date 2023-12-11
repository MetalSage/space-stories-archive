using Content.Shared.SpaceStories.Shadowling;

namespace Content.Server.SpaceStories.Shadowling;
public sealed class ShadowlingForceSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public List<EntityUid> GetEntitiesAroundShadowling<TFilter>(EntityUid uid, float radius, bool filterTrells = true) where TFilter : IComponent
    {
        List<EntityUid> result = new() { };

        if (!TryComp<TransformComponent>(uid, out var transform))
            return result;

        foreach (var entity in _entityLookup.GetEntitiesInRange(transform.Coordinates, radius))
        {
            if (!TryComp<TFilter>(entity, out var _))
                continue;
            if (filterTrells && TryComp<ShadowlingForceComponent>(entity, out var _))
                continue;

            result.Add(entity);
        }

        return result;
    }

    public void ChangeForceType(EntityUid uid, ShadowlingForceComponent component, ShadowlingForceType forceType)
    {
        component.ForceType = forceType;
        Dirty(uid, component);
    }
}
