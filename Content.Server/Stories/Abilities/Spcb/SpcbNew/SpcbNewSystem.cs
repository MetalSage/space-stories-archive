using Content.Shared.Actions.Events;
using Content.Shared.Abilities.SpcbNew;
using Content.Server.Popups;
using Content.Shared.Popups;
using Content.Server.Chat.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using System;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Spcb.Components;
using System.Threading;
using Content.Shared.Chemistry.Components;
using System.Threading.Tasks;
using Content.Server.Body.Systems;

namespace Content.Server.Abilities.SpcbNew;

public sealed class SpcbNewSystem : SharedSpcbNewSystem // Creating a system for the operation of the button
{
    [Dependency] private readonly HungerSystem _hunger = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpcbNewComponent, SpcbNewEvent>(OnSpcbNew); // Tracking events
    }


    private void OnSpcbNew(EntityUid uid, SpcbNewComponent component, SpcbNewEvent args) // Creating an action that the object will perform after registering the event. In this case, we create an egg
    {

        if (args.Handled)
            return;

        var ipekakInjection = new Solution(component.Chemical, component.HealRate);
        _bloodstreamSystem.TryAddToChemicals(uid, ipekakInjection);

        var child = Spawn(component.TransMobSpawnId, Transform(uid).Coordinates);
    }
}
