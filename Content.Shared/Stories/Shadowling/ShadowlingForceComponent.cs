using Content.Shared.Actions;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.SpaceStories.Shadowling;
[RegisterComponent]
public sealed partial class ShadowlingForceComponent : Component
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    /// <summary>
    /// Связанные рабы
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField("slaves")]
    public List<EntityUid> Slaves = new();

    /// <summary>
    /// Активен ли shadow walk
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField("inShadowWalk")]
    public bool InShadowWalk = false;

    /// <summary>
    /// Когда shadow walk окончится
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField("shadowWalkEndsAt", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan ShadowWalkEndsAt = default!;

    /// <summary>
    /// Через сколько shadow walk окончится отсчитывая от активации
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField("shadowWalkEndsIn", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan ShadowWalkEndsIn = TimeSpan.FromSeconds(5);

    [ViewVariables(VVAccess.ReadOnly)]
    [DataField("icyVeinsReagentId", customTypeSerializer: typeof(PrototypeIdSerializer<ReagentPrototype>))]
    public string IcyVeinsReagentId = "IceOil";

    /// <summary>
    /// Какая связь у существа с силой.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("forceType")]
    public ShadowlingForceType ForceType
    {
        get => _forceType;
        set
        {
            _forceType = value;
            Actions.TryGetValue(value, out var toGrant);
            var ev = new ShadowlingForceTypeChangeEvent(Owner, value, toGrant);
            _entityManager.EventBus.RaiseLocalEvent(Owner, ref ev, true);
        }
    }
    private ShadowlingForceType _forceType = ShadowlingForceType.ShadowlingHatch;

    /// <summary>
    /// Способности у существа в зависимости от типа силы.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("actions")]
    public Dictionary<ShadowlingForceType, List<string>> Actions = new()
    {
        {
            ShadowlingForceType.ShadowlingHatch,
            new() {
                "ActionShadowlingHatch" // Раскрыться
            }
        },
        {
            ShadowlingForceType.ShadowlingBasic,
            new() {
                "ActionShadowlingEnthrall", // Поработить
                "ActionShadowlingGlare", // Вспышка
                "ActionShadowlingVeil", // No more lights (TM)
                "ActionShadowlingShadowWalk", // Шаг тени
                "ActionShadowlingIcyVeins", // Стынущие жилы, вводит ледяное масло
                "ActionShadowlingCollectiveMind", // Узнать количество рабов, если их достаточно, то повысить уровень
                "ActionShadowlingRapidReHatch", // Позволяет тенеморфу восстановится
                "ActionShadowlingDestroyEngines", // Принести в жертву члена экипажа для задержания эвака на 10 минут
            }
        },
        {
            ShadowlingForceType.ShadowlingBeginning,
            new () {
                "ActionShadowlingEnthrall", // Поработить
                "ActionShadowlingGlare", // Вспышка
                "ActionShadowlingVeil", // No more lights (TM)
                "ActionShadowlingShadowWalk", // Шаг тени
                "ActionShadowlingIcyVeins", // Стынущие жилы, вводит ледяное масло
                "ActionShadowlingCollectiveMind", // Узнать количество рабов, если их достаточно, то повысить уровень
                "ActionShadowlingRapidReHatch", // Позволяет тенеморфу восстановится
                "ActionShadowlingDestroyEngines", // Принести в жертву члена экипажа для задержания эвака на 10 минут
                "ActionShadowlingSonicScreech", // Оглушает всех в округе, выбивает окна
            }
        },
        {
            ShadowlingForceType.ShadowlingMedium,
            new () {
                "ActionShadowlingEnthrall", // Поработить
                "ActionShadowlingGlare", // Вспышка
                "ActionShadowlingVeil", // No more lights (TM)
                "ActionShadowlingShadowWalk", // Шаг тени
                "ActionShadowlingIcyVeins", // Стынущие жилы, вводит ледяное масло
                "ActionShadowlingCollectiveMind", // Узнать количество рабов, если их достаточно, то повысить уровень
                "ActionShadowlingRapidReHatch", // Позволяет тенеморфу восстановится
                "ActionShadowlingDestroyEngines", // Принести в жертву члена экипажа для задержания эвака на 10 минут
                "ActionShadowlingSonicScreech", // Оглушает всех в округе, выбивает окна
                "ActionShadowlingBlindnessSmoke", // Непроглядный чёрный дым, позволяет тенеморфам и треллам регенировать
            }
        },
        {
            ShadowlingForceType.ShadowlingHigh,
            new() {
                "ActionShadowlingEnthrall", // Поработить
                "ActionShadowlingGlare", // Вспышка
                "ActionShadowlingVeil", // No more lights (TM)
                "ActionShadowlingShadowWalk", // Шаг тени
                "ActionShadowlingIcyVeins", // Стынущие жилы, вводит ледяное масло
                "ActionShadowlingCollectiveMind", // Узнать количество рабов, если их достаточно, то повысить уровень
                "ActionShadowlingRapidReHatch", // Позволяет тенеморфу восстановится
                "ActionShadowlingDestroyEngines", // Принести в жертву члена экипажа для задержания эвака на 10 минут
                "ActionShadowlingSonicScreech", // Оглушает всех в округе, выбивает окна
                "ActionShadowlingBlindnessSmoke", // Непроглядный чёрный дым, позволяет тенеморфам и треллам регенировать
                "ActionShadowlingBlackRecuperation", // Чёрная медицина, воскрешает раба, если тот в крите/мёртв, делает живого раба низшим тенелингом
            }
        },
        {
            ShadowlingForceType.ShadowlingFinal,
            new() {
                "ActionShadowlingEnthrall", // Поработить
                "ActionShadowlingGlare", // Вспышка
                "ActionShadowlingVeil", // No more lights (TM)
                "ActionShadowlingShadowWalk", // Шаг тени
                "ActionShadowlingIcyVeins", // Стынущие жилы, вводит ледяное масло
                "ActionShadowlingCollectiveMind", // Узнать количество рабов, если их достаточно, то повысить уровень
                "ActionShadowlingRapidReHatch", // Позволяет тенеморфу восстановится
                "ActionShadowlingDestroyEngines", // Принести в жертву члена экипажа для задержания эвака на 10 минут
                "ActionShadowlingSonicScreech", // Оглушает всех в округе, выбивает окна
                "ActionShadowlingBlindnessSmoke", // Непроглядный чёрный дым, позволяет тенеморфам и треллам регенировать
                "ActionShadowlingBlackRecuperation", // Чёрная медицина, воскрешает раба, если тот в крите/мёртв, делает живого раба низшим тенелингом
                "ActionShadowlingAscendance" // Стань богом, уничтожь всё живое
            }
        },
        {
            ShadowlingForceType.ShadowlingOverlord,
            new() {
                "ActionShadowlingHypnosis", // Гипноз
                "ActionShadowlingGlare", // Вспышка
                "ActionShadowlingVeil", // No more lights (TM)
                "ActionShadowlingPlaneShift", // Смещение, теневой шаг, но без ограничений
                "ActionShadowlingIcyVeins", // Стынущие жилы, вводит ледяное масло
                "ActionShadowlingCollectiveMind", // Узнать количество рабов, если их достаточно, то повысить уровень
                "ActionShadowlingRapidReHatch", // Позволяет тенеморфу восстановится
                "ActionShadowlingDestroyEngines", // Принести в жертву члена экипажа для задержания эвака на 10 минут
                "ActionShadowlingSonicScreech", // Оглушает всех в округе, выбивает окна
                "ActionShadowlingBlindnessSmoke", // Непроглядный чёрный дым, позволяет тенеморфам и треллам регенировать
                "ActionShadowlingBlackRecuperation", // Чёрная медицина, воскрешает раба, если тот в крите/мёртв, делает живого раба низшим тенелингом
                "ActionShadowlingAnnihilate", // Вы стали богом, вы можете заставить разорваться членов экипажа просто силой мысли
                "ActionShadowlingLightningStorm", // Грозовой шторм, уничтожьте электричеством всё в округе
                "ActionShadowlingAscendantBroadcast", // Послание бога, которое не может не услышать никто
            }
        },
        {
            ShadowlingForceType.ShadowlingSlave,
            new() {
                "ActionShadowlingGuise", // Сокройте своё присутствие на короткий промежуток времени
            }
        },
        {
            ShadowlingForceType.ShadowlingTrell,
            new() {
                "ActionShadowlingShadowWalk", // Теневой шаг, прямо как у истинного тенелинга
            }
        }
    };

    /// <summary>
    /// Способности которые сила дала существу.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("grantedActions")]
    public List<EntityUid> GrantedActions = new();
}

public enum ShadowlingForceType : byte
{
    ShadowlingSlave, // You're just a slave
    ShadowlingTrell, // You're gained with a little power of a true shadowling

    ShadowlingHatch, // You can only hatch yourself
    ShadowlingBasic, // Almost all common powers
    ShadowlingBeginning, // Basic shadowling but with 3 slaves
    ShadowlingMedium, // Beginning shadowling but with 5 slaves
    ShadowlingHigh, // Medium shadowling but with 9 slaves
    ShadowlingFinal, // You're able to become an overlord (requires 15 slaves)
    ShadowlingOverlord, // Absolute power!
}

[ByRefEvent]
public readonly record struct ShadowlingForceTypeChangeEvent(EntityUid Uid, ShadowlingForceType ShadowlingForceType, List<string>? NewActions);

public sealed partial class ShadowlingEnthrallEvent : EntityTargetActionEvent
{
}

public sealed partial class ShadowlingGlareEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingVeilEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingShadowWalkEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingIcyVeinsEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingCollectiveMindEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingRapidReHatchEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingDestroyEnginesEvent : EntityTargetActionEvent
{
}

public sealed partial class ShadowlingSonicScreechEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingBlindnessSmokeEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingBlackRecuperationEvent : EntityTargetActionEvent
{
}

public sealed partial class ShadowlingAnnihilateEvent : EntityTargetActionEvent
{
}

public sealed partial class ShadowlingLightningStormEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingAscendantBroadcastEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingPlaneShiftEvent : InstantActionEvent
{
}

public sealed partial class ShadowlingGuiseEvent : InstantActionEvent
{
}
