using Content.Server.Administration.Logs;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Database;
using Content.Shared.Implants.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio.Systems;
using Content.Shared.Chemistry;
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
                _popup.PopupEntity(Loc.GetString("inject-trigger-empty-message"), user, user);
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
                _popup.PopupEntity(Loc.GetString("inject-trigger-cant-inject-message", ("target", Identity.Entity(user, _entMan))), user, user);
                return;
            }

            _audio.PlayPvs(component.InjectSound, user);

            component.NotUsedSolutions[injectSolName] = false;

            var realTransferAmount = FixedPoint2.Min(injectSolution.Volume, targetSolution.AvailableVolume);

            if (realTransferAmount <= 0)
            {
                _popup.PopupEntity(Loc.GetString("inject-trigger-empty-capsule-message"), user, user);
                return;
            }

            // Move units from injectsolution to targetSolution
            var removedSolution = _solutionContainer.SplitSolution((Entity<SolutionComponent>) injectSoln, realTransferAmount);

            if (!targetSolution.CanAddSolution(removedSolution))
            {
                _popup.PopupEntity(Loc.GetString("inject-trigger-cant-inject-message", ("target", Identity.Entity(user, _entMan))), user, user);
                return;
            }

            _reactiveSystem.DoEntityReaction(user, removedSolution, ReactionMethod.Injection);
            _solutionContainers.TryAddSolution(targetSoln.Value, removedSolution);

            _popup.PopupEntity(Loc.GetString("inject-trigger-feel-prick-message"), user, user);

            // same LogType as syringes...
            _adminLogger.Add(LogType.ForceFeed, $"{_entMan.ToPrettyString(user):user} used inject implant with a solution {SolutionContainerSystem.ToPrettyString(removedSolution):removedSolution}");


            args.Handled = true;
        }

    }
}
