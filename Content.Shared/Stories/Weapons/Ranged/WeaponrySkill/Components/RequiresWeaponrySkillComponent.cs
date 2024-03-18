using Content.Shared.Stories.Weapons.Ranged.WeaponrySkill.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Stories.Weapons.Ranged.WeaponrySkill.Components;

/// <summary>
/// Applies accuracy debuff if you don't have weaponry skill
/// </summary>

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(SharedWeaponrySkillSystem))]
public sealed partial class RequiresWeaponrySkillComponent : Component  
{
    [ViewVariables(VVAccess.ReadWrite), DataField("additionalMinAngle"), AutoNetworkedField]
    public Angle AdditionalMinAngle = Angle.FromDegrees(45);

    [ViewVariables(VVAccess.ReadWrite), DataField("additionalMaxAngle"), AutoNetworkedField]
    public Angle AdditionalMaxAngle = Angle.FromDegrees(45);

    [ViewVariables(VVAccess.ReadWrite), DataField("fireSpeedModifier"), AutoNetworkedField]
    public float FireSpeedModifier = 0.65f;

    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? WeaponEquipee; 
}
