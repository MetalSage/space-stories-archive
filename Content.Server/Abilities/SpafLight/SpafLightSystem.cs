using Content.Shared.Actions.Events; //my
using Content.Shared.Abilities.SpafLight; //my
using Content.Server.Emp;
using Content.Server.Ninja.Events;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.PowerCell;
using Content.Shared.Actions;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Ninja.Components;
using Content.Shared.Ninja.Systems;
using Content.Shared.Popups;
using Content.Shared.PowerCell.Components;
using Robust.Shared.Containers;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Content.Server.Chat.Systems;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Mobs.Systems;
using Content.Shared.Dataset;
using System.Numerics;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.NPC;
using Content.Server.NPC.HTN;
using Content.Server.NPC.Systems;
using Content.Server.Popups;
using Content.Shared.Atmos;
using Content.Shared.Dataset;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Pointing;
using Content.Shared.RatKing;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Content.Server.Light.EntitySystems;
using Content.Server.Light.Events;
using Content.Shared.Light.Components;
using Content.Server.Light.EntitySystems;
using System.Timers;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Content.Server.Abilities.SpafLight;

public sealed class SpafLightSystem : SharedSpafLightSystem // Creating a system for the operation of the button
{
    [Dependency] private readonly EmpSystem _emp = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly HungerSystem _hunger = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedStealthSystem _stealth = default!;
    [Dependency] private readonly ActionContainerSystem _actionContainer = default!;
    [Dependency] private readonly UnpoweredFlashlightSystem _unpoweredFlashlight = default!;
    [Dependency] private readonly PointLightSystem _pointLight = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpafLightComponent, SpafLightEvent>(OnSpafLight);
    }


    public float hunger_p = 0.0f; //Creating a field for transmitting information to another system. This will be needed later

    bool lighton = false;
    private void OnSpafLight(EntityUid uid, SpafLightComponent component, SpafLightEvent args) // creating an action to turn a person into a man
    {

        if (args.Handled)
            return;

        if (!TryComp<HungerComponent>(uid, out var hunger))
            return;

        //make sure the hunger doesn't go into the negatives
        if (hunger.CurrentHunger < component.HungerPerSpafLight)
        {
            _popup.PopupEntity(Loc.GetString("You don't have enough energy to perform an action. You need to eat"), uid, uid);
            return;
        }
        args.Handled = true;
        _hunger.ModifyHunger(uid, -component.HungerPerSpafLight, hunger); //taking away food

        var radius = 3.0f;
        var energy = 2.0f;
        var softness = 1.0f;

        if (_pointLight.TryGetLight(uid, out var pointLightComponent) && lighton == false)
        {
            _pointLight.SetEnabled(uid, true, pointLightComponent);
            _pointLight.SetRadius(uid, (float) radius, pointLightComponent);
            _pointLight.SetEnergy(uid, (float) energy, pointLightComponent);
            _pointLight.SetSoftness(uid, (float) softness, pointLightComponent);
            lighton = true;
        }
        else if(lighton == true)
        {
            var radius2 = 0.0f;
            var energy2 = 0.0f;
            var softness2 = 0.0f;
            _pointLight.SetEnabled(uid, true, pointLightComponent);
            _pointLight.SetRadius(uid, (float) radius2, pointLightComponent);
            _pointLight.SetEnergy(uid, (float) energy2, pointLightComponent);
            _pointLight.SetSoftness(uid, (float) softness2, pointLightComponent);
            lighton = false;
        }
    }
}
