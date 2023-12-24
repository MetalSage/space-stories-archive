using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Content.Shared.Tag;
using YamlDotNet.Core.Tokens;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Stories.FTLKey;


namespace Content.Server.Stories.FTLKey
{
    public sealed class FTLAccessConsoleSystem : EntitySystem
    {
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly TagSystem _tagSystem = default!;
        [Dependency] private readonly ShuttleConsoleSystem _shuttleConsoleSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<FTLAccessConsoleComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<FTLAccessConsoleComponent, ComponentRemove>(OnComponentRemove);

            SubscribeLocalEvent<FTLAccessConsoleComponent, EntInsertedIntoContainerMessage>(OnItemInserted);
            SubscribeLocalEvent<FTLAccessConsoleComponent, EntRemovedFromContainerMessage>(OnItemRemoved);
            SubscribeLocalEvent<FTLAccessConsoleComponent, AnchorStateChangedEvent>(OnAnchorChange);

        }

        private void OnComponentInit(EntityUid uid, FTLAccessConsoleComponent consl, ComponentInit args)
        {
            if (consl.KeyA != null)
                consl.FTLKeySlotA.StartingItem = consl.KeyA;
            if (consl.KeyB != null)
                consl.FTLKeySlotB.StartingItem = consl.KeyB;

            _itemSlotsSystem.AddItemSlot(uid, FTLAccessConsoleComponent.ConsoleFTLKeySlotA, consl.FTLKeySlotA);
            _itemSlotsSystem.AddItemSlot(uid, FTLAccessConsoleComponent.ConsoleFTLKeySlotB, consl.FTLKeySlotB);

            UpdateAccess(uid, consl);
        }

        private void OnComponentRemove(EntityUid uid, FTLAccessConsoleComponent consl, ComponentRemove args)
        {
            RemoveAccess(uid, consl);

            _itemSlotsSystem.RemoveItemSlot(uid, consl.FTLKeySlotA);
            _itemSlotsSystem.RemoveItemSlot(uid, consl.FTLKeySlotB);
        }

        private void OnItemInserted(EntityUid uid, FTLAccessConsoleComponent consl, EntInsertedIntoContainerMessage args)
        {
            UpdateAccess(uid, consl);
        }

        private void OnItemRemoved(EntityUid uid, FTLAccessConsoleComponent consl, EntRemovedFromContainerMessage args)
        {
            UpdateAccess(uid, consl, args.Entity);
        }

        private void OnAnchorChange(EntityUid uid, FTLAccessConsoleComponent consl, AnchorStateChangedEvent args)
        {
            if (args.Anchored) UpdateAccess(uid, consl);
            else RemoveAccess(uid, consl);
        }

        private void UpdateAccess(EntityUid uid, FTLAccessConsoleComponent consl, EntityUid? removing = null)
        {
            if (consl.FTLKeySlotA.ContainerSlot is not null && consl.FTLKeySlotA.ContainerSlot.ContainedEntity is not null)
                consl.FTLKeyA = (EntityUid) consl.FTLKeySlotA.ContainerSlot.ContainedEntity;
            if (consl.FTLKeySlotB.ContainerSlot is not null && consl.FTLKeySlotB.ContainerSlot.ContainedEntity is not null)
                consl.FTLKeyB = (EntityUid) consl.FTLKeySlotB.ContainerSlot.ContainedEntity;

            RemoveAccess(uid, consl);

            var tagComponent = GetTagComp(uid);

            if (tagComponent is null) return;

            if (removing is not null && removing != EntityUid.Invalid)
                RemoveFTLTags(tagComponent, _entityManager.GetComponent<FTLKeyComponent>((EntityUid) removing));

            if (consl.FTLKeyA != EntityUid.Invalid)
                AddFTLTags(tagComponent, _entityManager.GetComponent<FTLKeyComponent>(consl.FTLKeyA));
            if (consl.FTLKeyB != EntityUid.Invalid)
                AddFTLTags(tagComponent, _entityManager.GetComponent<FTLKeyComponent>(consl.FTLKeyB));

            UpdateConsoles(uid);
        }

        private void RemoveAccess(EntityUid uid, FTLAccessConsoleComponent consl)
        {
            var tagComponent = GetTagComp(uid);
            if (tagComponent is null) return;

            if (consl.FTLKeyA != EntityUid.Invalid)
                RemoveFTLTags(tagComponent, _entityManager.GetComponent<FTLKeyComponent>(consl.FTLKeyA));
            if (consl.FTLKeyB != EntityUid.Invalid)
                RemoveFTLTags(tagComponent, _entityManager.GetComponent<FTLKeyComponent>(consl.FTLKeyB));
        }

        private void AddFTLTags(TagComponent tagComponent, FTLKeyComponent keycomp)
        {
            if (keycomp.FTLKeys is null) return;
            _tagSystem.AddTags(tagComponent, keycomp.FTLKeys);
        }

        private void RemoveFTLTags(TagComponent tagComponent, FTLKeyComponent keycomp)
        {
            if (keycomp.FTLKeys is null) return;
            _tagSystem.RemoveTags(tagComponent, keycomp.FTLKeys);
        }


        private TagComponent? GetTagComp(EntityUid uid)
        {
            var trans = _entityManager.GetComponent<TransformComponent>(uid);
            if (trans.GridUid is null)
            {
                return null;
            }
            return EnsureComp<TagComponent>((EntityUid) trans.GridUid);
        }

        private void UpdateConsoles(EntityUid uid)
        {
            _shuttleConsoleSystem.RefreshShuttleConsoles(uid);
        }

    }

}
