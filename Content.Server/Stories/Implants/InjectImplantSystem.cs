using Content.Server.Administration.Logs;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Server.Explosion.Components;
using Content.Server.Flash;
using Content.Server.Flash.Components;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Database;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Payload.Components;
using Content.Shared.Radio;
using Content.Shared.Slippery;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Trigger;
using Content.Shared.FixedPoint;
using Content.Shared.Weapons.Ranged.Events;
using JetBrains.Annotations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Server.Administration.Logs;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Shared.Chemistry;
using Content.Shared.CombatMode;
using Content.Shared.DoAfter;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Audio.Systems;
using Content.Shared.IdentityManagement;

using Content.Server.Explosion.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Popups;
using System.Linq;

namespace Content.Server.Stories.Implants
{

    public sealed partial class InjectImplantSystem : EntitySystem
    {
        [Dependency] private readonly IAdminLogManager _adminLogger = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly SolutionContainerSystem _solutionContainer = default!;
        [Dependency] private readonly IEntityManager _entMan = default!;
        [Dependency] private readonly ReactiveSystem _reactiveSystem = default!;
        [Dependency] private readonly SolutionContainerSystem _solutionContainers = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<InjectOnTriggerComponent, TriggerEvent>(HandleInjectTrigger);
        }

        private void HandleInjectTrigger(EntityUid uid, InjectOnTriggerComponent component, TriggerEvent args)
        {

            // Get user uid
            if (!TryComp<SubdermalImplantComponent>(uid, out var implantComp) || implantComp.ImplantedEntity is null)
            {
                return;
            }
            var user = (EntityUid) implantComp.ImplantedEntity;


            // Geting inject solutions form many avaible solutions
            if (!component.NotUsedSolutions.ContainsValue(true))
            {
                // Have no capsules
                _popup.PopupCursor(Loc.GetString("inject-trigger-empty-message"), user);
                return;
            }

            var injectSolName = component.NotUsedSolutions.Keys.First<string>();
            foreach (var (solName, valid) in component.NotUsedSolutions)
            {
                if (valid) injectSolName = solName;
            }

            if (!_solutionContainer.TryGetSolution(uid, injectSolName, out var injectSoln, out var injectSolution))
            {
                return;
            }

            // Try get insert solution
            if (!_solutionContainer.TryGetInjectableSolution(user, out var targetSoln, out var targetSolution))
            {
                _popup.PopupCursor(Loc.GetString("inject-trigger-cant-inject-message", ("target", Identity.Entity(user, _entMan))), user);
                return;
            }

            _audio.PlayPvs(component.InjectSound, user);

            component.NotUsedSolutions[injectSolName] = false;

            var realTransferAmount = FixedPoint2.Min(injectSolution.Volume, targetSolution.AvailableVolume);

            if (realTransferAmount <= 0)
            {
                _popup.PopupCursor(Loc.GetString("inject-trigger-empty-capsule-message"), user);
                return;
            }

            // Move units from injectsolution to targetSolution
            var removedSolution = _solutionContainer.SplitSolution((Entity<SolutionComponent>) injectSoln, realTransferAmount);

            if (!targetSolution.CanAddSolution(removedSolution))
            {
                _popup.PopupCursor(Loc.GetString("inject-trigger-cant-inject-message", ("target", Identity.Entity(user, _entMan))), user);
                return;
            }

            _reactiveSystem.DoEntityReaction(user, removedSolution, ReactionMethod.Injection);
            _solutionContainers.TryAddSolution(targetSoln.Value, removedSolution);

            _popup.PopupCursor(Loc.GetString("inject-trigger-feel-prick-message"), user);

            // same LogType as syringes...
            _adminLogger.Add(LogType.ForceFeed, $"{_entMan.ToPrettyString(user):user} used inject implant with a solution {SolutionContainerSystem.ToPrettyString(removedSolution):removedSolution}");


            args.Handled = true;
        }

    }
}
