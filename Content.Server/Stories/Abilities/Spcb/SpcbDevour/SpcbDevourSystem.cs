using Content.Shared.Actions.Events;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Server.Humanoid;
using Content.Shared.Interaction;
using Content.Server.Popups;
using Content.Shared.Popups;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.SpcbDevour;
using Content.Shared.SpcbDevour.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.Serialization.Manager;
using Content.Shared.Spcb.Components;
using Content.Shared.Abilities.SpcbNew;
using Content.Shared.Abilities.SpcbExit;


namespace Content.Server.SpcbDevour;

public sealed class SpcbDevourSystem : SharedSpcbDevourSystem
{
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly HungerSystem _hunger = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly IEntityManager _entities = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpcbDevourerComponent, SpcbDevourDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, SpcbDevourerComponent component, SpcbDevourDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        var omnizineInjection = new Solution(component.Chemical, component.HealRate);

        if (component.FoodPreference == FoodPreference.All ||
            (component.FoodPreference == FoodPreference.Humanoid && HasComp<HumanoidAppearanceComponent>(args.Args.Target))) // List of allowed things in the prototype entity
        {
            omnizineInjection.ScaleSolution(0.5f);
            if (args.Args.Target != null) {
                if (_mindSystem.TryGetMind(uid, out var mindId, out var mind))
                {
                    _mindSystem.TransferTo(mindId, args.Args.Target, mind: mind);
                    _bloodstreamSystem.TryAddToChemicals(args.Args.Target.Value, omnizineInjection);
                    AddComp<SpcbComponent>(args.Args.Target.Value);
                    AddComp<SpcbNewComponent>(args.Args.Target.Value);
                    AddComp<SpcbExitComponent>(args.Args.Target.Value);
                    QueueDel(uid);
                }
            }
        }

        else if (args.Args.Target != null)
        {
            _popup.PopupEntity(Loc.GetString("It's not humanoid"), uid, uid);
            return;
        }

    }
}

