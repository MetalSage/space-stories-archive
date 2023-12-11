using Content.Shared.Actions.Events;
using Content.Shared.Abilities.SpcbExit;
using Content.Server.Popups;
using Content.Shared.Popups;
using Content.Server.Chat.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using System;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Spcb.Components;
using Content.Shared.Abilities.SpcbNew;

namespace Content.Server.Abilities.SpcbExit;

public sealed class SpcbExitSystem : SharedSpcbExitSystem // Creating a system for the operation of the button
{
    [Dependency] private readonly HungerSystem _hunger = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpcbExitComponent, SpcbExitEvent>(OnSpcbExit); // Tracking events
    }


    private void OnSpcbExit(EntityUid uid, SpcbExitComponent component, SpcbExitEvent args) // Creating an action that the object will perform after registering the event. In this case, we create an egg
    {

        if (args.Handled)
            return;


        var child = Spawn(component.TransMobSpawnId, Transform(uid).Coordinates);

        if (_mindSystem.TryGetMind(uid, out var mindId, out var mind))
        {
            _mindSystem.TransferTo(mindId, child, mind: mind);
            RemComp<SpcbComponent>(uid);
            RemComp<SpcbExitComponent>(uid);
            RemComp<SpcbNewComponent>(uid);
        }
    }
}
