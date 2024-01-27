using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Content.Shared.Tag;
using Content.Server.Shuttles.Systems;
using Content.Shared.Stories.ConsoleLock;

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

            //Locking
            SubscribeLocalEvent<FTLAccessConsoleComponent, ConsoleLockToggledEvent>(OnLockToggled);
        }

        private void OnComponentInit(EntityUid uid, FTLAccessConsoleComponent consl, ComponentInit args)
        {

            foreach (var slot in consl.Slots)
            {
                _itemSlotsSystem.AddItemSlot(uid, slot.Key, slot.Value);
            }

            UpdateAccess(uid, consl);
        }

        private void OnComponentRemove(EntityUid uid, FTLAccessConsoleComponent consl, ComponentRemove args)
        {
            RemoveAccess(uid, consl);

            foreach (var slot in consl.Slots.Values)
            {
                _itemSlotsSystem.RemoveItemSlot(uid, slot);
            }
        }

        private void OnItemInserted(EntityUid uid, FTLAccessConsoleComponent consl, EntInsertedIntoContainerMessage args)
        {
            var xform = Transform(uid);
            if (!xform.Anchored) return;

            UpdateAccess(uid, consl);
        }

        private void OnItemRemoved(EntityUid uid, FTLAccessConsoleComponent consl, EntRemovedFromContainerMessage args)
        {
            var xform = Transform(uid);
            if (!xform.Anchored) return;

            RemoveCurrentAccess(uid, consl, args.Entity);
            UpdateAccess(uid, consl);
        }

        private void OnAnchorChange(EntityUid uid, FTLAccessConsoleComponent consl, AnchorStateChangedEvent args)
        {
            if (args.Anchored) UpdateAccess(uid, consl);
            else RemoveAccess(uid, consl);
        }

        private void OnLockToggled(EntityUid uid, FTLAccessConsoleComponent consl, ConsoleLockToggledEvent args)
        {
            if (args.Locked)
            {
                foreach (var slot in consl.Slots.Values)
                {
                    _itemSlotsSystem.SetLock(uid, slot, true);
                }
            }
            else
            {
                foreach (var slot in consl.Slots.Values)
                {
                    _itemSlotsSystem.SetLock(uid, slot, false);
                }
            }
        }

        /// <summary>
        /// Sets Access from inserted Keys
        /// </summary>
        private void UpdateAccess(EntityUid uid, FTLAccessConsoleComponent consl)
        {
            foreach (var slot in consl.Slots.Values)
            {
                if (slot.ContainerSlot is not null && slot.ContainerSlot.ContainedEntity is not null)
                    AddCurrentAccess(uid, consl, (EntityUid) slot.ContainerSlot.ContainedEntity);
            }

            UpdateConsole(uid);
        }

        /// <summary>
        /// Remove access of all inserted keys
        /// </summary>

        private void RemoveAccess(EntityUid uid, FTLAccessConsoleComponent consl)
        {
            foreach (var slot in consl.Slots.Values)
            {
                if (slot.ContainerSlot is not null && slot.ContainerSlot.ContainedEntity is not null)
                    RemoveCurrentAccess(uid, consl, (EntityUid) slot.ContainerSlot.ContainedEntity);
            }

        }

        /// <summary>
        /// Add access of current Key
        /// </summary>
        private void AddCurrentAccess(EntityUid uid, FTLAccessConsoleComponent consl, EntityUid added)
        {
            var xform = Transform(uid);
            if (xform.GridUid is null) return;

            var tagComp = EnsureComp<TagComponent>(uid);

            if (!TryComp<FTLKeyComponent>(added, out var keyComp) || keyComp.FTLKeys is null) return;
            _tagSystem.AddTags(tagComp, keyComp.FTLKeys);
        }

        /// <summary>
        /// Remove access of current Key
        /// </summary>
        private void RemoveCurrentAccess(EntityUid uid, FTLAccessConsoleComponent consl, EntityUid removed)
        {
            var xform = Transform(uid);
            if (xform.GridUid is null) return;

            var tagComp = EnsureComp<TagComponent>(uid);

            if (!TryComp<FTLKeyComponent>(removed, out var keyComp) || keyComp.FTLKeys is null) return;
            _tagSystem.RemoveTags(tagComp, keyComp.FTLKeys);
        }

        private void UpdateConsole(EntityUid uid)
        {
            _shuttleConsoleSystem.RefreshShuttleConsoles(uid);
        }
    }
}
