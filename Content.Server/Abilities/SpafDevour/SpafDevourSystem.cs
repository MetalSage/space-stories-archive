using Content.Shared.Actions.Events;
using System.Numerics;
using Content.Server.Chat.Systems;
using Content.Server.NPC;
using Content.Server.NPC.HTN;
using Content.Server.NPC.Systems;
using Content.Server.Popups;
using Content.Shared.Dataset;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Pointing;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Content.Server.Players;
using Content.Shared.Administration;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Server.Player;
using Robust.Shared.Console;
using Content.Shared.Cloning;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes; // I have connected a lot of names with the hope that they will be needed :)
using Content.Shared.Mobs.Systems;
using Content.Shared.Roles.Jobs;
using Content.Shared.Actions.Events;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Cuffs;
using Content.Server.Humanoid;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.Cuffs.Components;
using Content.Shared.Humanoid;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Preferences;
using Content.Shared.Ghost;
using Content.Shared.IdentityManagement;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Damage;
using Content.Server.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Polymorph;
using Robust.Shared.Prototypes;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.SpafDevour;
using Content.Shared.SpafDevour.Components;
using Content.Shared.Humanoid;


namespace Content.Server.SpafDevour;

public sealed class SpafDevourSystem : SharedSpafDevourSystem
{
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly HungerSystem _hunger = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpafDevourerComponent, SpafDevourDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, SpafDevourerComponent component, SpafDevourDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var ichorInjection = new Solution(component.Chemical, component.HealRate);

        if (component.FoodPreference == FoodPreference.All ||
            (component.FoodPreference == FoodPreference.Humanoid && HasComp<HumanoidAppearanceComponent>(args.Args.Target)))
        {
            ichorInjection.ScaleSolution(0.5f);

            if (component.ShouldStoreDevoured && args.Args.Target is not null)
            {
                component.Stomach.Insert(args.Args.Target.Value);
            }
            if (!TryComp<HungerComponent>(uid, out var hunger))
                return;

            float hung = 50.0f;
            _bloodstreamSystem.TryAddToChemicals(uid, ichorInjection);
            _hunger.ModifyHunger(uid, hung, hunger);
        }

        //TODO: Figure out a better way of removing structures via devour that still entails standing still and waiting for a DoAfter. Somehow.
        //If it's not human, it must be a structure
        else if (args.Args.Target != null)
        {
            QueueDel(args.Args.Target.Value);
        }

        _audioSystem.PlayPvs(component.SoundDevour, uid);
    }
}

