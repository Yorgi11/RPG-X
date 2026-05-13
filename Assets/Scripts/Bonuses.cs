using System;
using System.Collections.Generic;
using UnityEngine;

public enum BonusSourceType
{
    Class,
    Backstory,
    Skill,
    Equipment,
    Consumable,
    TemporaryBuff,
    PermanentQuestReward,
    FactionReward,
    RegionEffect,
    StatusEffect,
    Enchantment,
    DifficultyModifier
}

public enum BonusDurationType
{
    Permanent,
    WhileEquipped,
    Timed,
    UntilRest,
    UntilRegionExit,
    WhileInRegion,
    WhileBuffActive,
    Conditional
}

public enum BonusOperationType
{
    FlatAdd,
    FlatSubtract,
    PercentAdd,
    PercentSubtract,
    Multiplier,
    Override,
    ConditionalModifier,
    TriggeredEffect
}

public enum BonusStackingRule
{
    Additive,
    Multiplicative,
    FlatAdditive,
    HighestValueOnly,
    UniqueEffect,
    ConditionalTrigger,
    CappedTotal
}

public enum BonusStat
{
    MaxHealth,
    MaxStamina,
    MaxMana,
    Strength,
    Endurance,
    Intelligence,
    Faith,
    Agility,
    CarryWeight,
    HealthRegeneration,
    StaminaRegeneration,
    ManaRegeneration,
    PotionEffectiveness,
    MeleeDamage,
    LightAttackDamage,
    HeavyAttackDamage,
    CriticalHitChance,
    CriticalHitDamage,
    AttackSpeed,
    RangedDamage,
    ProjectileSpeed,
    ReloadSpeed,
    DrawSpeed,
    HeadshotDamage,
    AmmoRecoveryChance,
    SpellDamage,
    HolyDamage,
    HealingEffectiveness,
    ManaCostReduction,
    CastSpeed,
    SpellProjectileSpeed,
    SpellAreaSize,
    ManaRecoveryAfterKill,
    XPGain,
    CombatXPGain,
    QuestXPGain,
    UpgradePoints,
    UpgradePointEveryLevels,
    Defense,
    ArmourEffectiveness,
    PhysicalResistance,
    MagicResistance,
    BlockEfficiency,
    ParryWindow,
    SwordDamage,
    AxeDamage,
    SpearDamage,
    DaggerDamage,
    StaffDamage,
    WandDamage,
    AttackStaminaCostReduction,
    SprintStaminaCostReduction,
    BlockStaminaCostReduction,
    StaminaRecoveryAfterKill,
    StaminaRecoveryAfterParry,
    SellPriceMultiplier,
    GatheringYield,
    FoodHealingItemEffectiveness,
    RepairCostReduction,
    DurabilityLossReduction,
    DodgeCostReduction,
    AbilityCooldownReduction
}

[Serializable]
public class BonusDefinition
{
    public string _bonusId;
    public string _displayName;
    [TextArea] public string _description;
    public BonusStat _affectedStat;
    public BonusOperationType _operationType = BonusOperationType.FlatAdd;
    public float _value;
    public BonusSourceType _sourceType;
    public BonusDurationType _durationType = BonusDurationType.Permanent;
    public BonusStackingRule _stackingRule = BonusStackingRule.Additive;
    public string _condition;
    public int _priority;
    public string[] _tags;
}

public sealed class ActiveBonus
{
    public readonly BonusDefinition _definition;
    public readonly string _runtimeSourceId;
    public readonly UnityEngine.Object _sourceObject;
    public float _remainingDuration;

    public ActiveBonus(BonusDefinition definition, string runtimeSourceId, UnityEngine.Object sourceObject, float remainingDuration = -1f)
    {
        _definition = definition;
        _runtimeSourceId = runtimeSourceId;
        _sourceObject = sourceObject;
        _remainingDuration = remainingDuration;
    }

    public bool IsTimed => _definition != null && _definition._durationType == BonusDurationType.Timed;
    public bool IsExpired => IsTimed && _remainingDuration <= 0f;
}

public sealed class BonusModifierSet
{
    private struct OverrideValue
    {
        public float _value;
        public int _priority;
        public bool _hasValue;
    }

    private readonly Dictionary<BonusStat, float> _flatAdds = new();
    private readonly Dictionary<BonusStat, float> _percentAdds = new();
    private readonly Dictionary<BonusStat, float> _multipliers = new();
    private readonly Dictionary<BonusStat, float> _highestValues = new();
    private readonly Dictionary<BonusStat, HashSet<string>> _uniqueIds = new();
    private readonly Dictionary<BonusStat, OverrideValue> _overrides = new();

    public void Clear()
    {
        _flatAdds.Clear();
        _percentAdds.Clear();
        _multipliers.Clear();
        _highestValues.Clear();
        _uniqueIds.Clear();
        _overrides.Clear();
    }

    public void Add(BonusDefinition bonus)
    {
        if (bonus == null) return;
        if (bonus._operationType == BonusOperationType.TriggeredEffect) return;
        if (!string.IsNullOrWhiteSpace(bonus._condition)) return;

        if (bonus._stackingRule == BonusStackingRule.UniqueEffect && !RegisterUniqueBonus(bonus))
            return;

        switch (bonus._operationType)
        {
            case BonusOperationType.FlatAdd:
                AddTo(_flatAdds, bonus._affectedStat, bonus._value);
                break;
            case BonusOperationType.FlatSubtract:
                AddTo(_flatAdds, bonus._affectedStat, -bonus._value);
                break;
            case BonusOperationType.PercentAdd:
                AddTo(_percentAdds, bonus._affectedStat, bonus._value);
                break;
            case BonusOperationType.PercentSubtract:
                AddTo(_percentAdds, bonus._affectedStat, -bonus._value);
                break;
            case BonusOperationType.Multiplier:
                MultiplyInto(_multipliers, bonus._affectedStat, bonus._value);
                break;
            case BonusOperationType.Override:
                SetOverride(bonus);
                break;
            case BonusOperationType.ConditionalModifier:
                if (string.IsNullOrWhiteSpace(bonus._condition))
                    AddTo(_percentAdds, bonus._affectedStat, bonus._value);
                break;
        }

        if (bonus._stackingRule == BonusStackingRule.HighestValueOnly)
            SetHighestValue(bonus._affectedStat, bonus._value);
    }

    public float GetFinalValue(BonusStat stat, float baseValue = 0f)
    {
        if (_overrides.TryGetValue(stat, out OverrideValue overrideValue) && overrideValue._hasValue)
            return ApplyFinalCap(stat, overrideValue._value, baseValue);

        if (_highestValues.TryGetValue(stat, out float highestValue))
            return ApplyFinalCap(stat, highestValue, baseValue);

        float value = baseValue + GetRaw(_flatAdds, stat);
        value *= 1f + GetRaw(_percentAdds, stat);
        value *= GetRaw(_multipliers, stat, 1f);
        return ApplyFinalCap(stat, value, baseValue);
    }

    public float GetBonusValue(BonusStat stat)
    {
        return GetFinalValue(stat, 0f);
    }

    public float GetModifierValue(BonusStat stat)
    {
        float value = GetFinalValue(stat, 1f) - 1f;
        return ApplyModifierCap(stat, value);
    }

    public int GetFinalInt(BonusStat stat, int baseValue)
    {
        return Mathf.RoundToInt(GetFinalValue(stat, baseValue));
    }

    public float GetReductionMultiplier(BonusStat stat)
    {
        return 1f - Mathf.Clamp01(GetModifierValue(stat));
    }

    private bool RegisterUniqueBonus(BonusDefinition bonus)
    {
        if (string.IsNullOrWhiteSpace(bonus._bonusId)) return true;
        if (!_uniqueIds.TryGetValue(bonus._affectedStat, out HashSet<string> ids))
        {
            ids = new HashSet<string>();
            _uniqueIds[bonus._affectedStat] = ids;
        }

        return ids.Add(bonus._bonusId);
    }

    private void SetOverride(BonusDefinition bonus)
    {
        if (!_overrides.TryGetValue(bonus._affectedStat, out OverrideValue existing) ||
            !existing._hasValue ||
            bonus._priority >= existing._priority)
        {
            _overrides[bonus._affectedStat] = new OverrideValue
            {
                _value = bonus._value,
                _priority = bonus._priority,
                _hasValue = true
            };
        }
    }

    private void SetHighestValue(BonusStat stat, float value)
    {
        if (!_highestValues.TryGetValue(stat, out float existing) || value > existing)
            _highestValues[stat] = value;
    }

    private static void AddTo(Dictionary<BonusStat, float> values, BonusStat stat, float value)
    {
        values.TryGetValue(stat, out float current);
        values[stat] = current + value;
    }

    private static void MultiplyInto(Dictionary<BonusStat, float> values, BonusStat stat, float value)
    {
        if (Mathf.Approximately(value, 0f)) return;
        values.TryGetValue(stat, out float current);
        if (Mathf.Approximately(current, 0f)) current = 1f;
        values[stat] = current * value;
    }

    private static float GetRaw(Dictionary<BonusStat, float> values, BonusStat stat, float fallback = 0f)
    {
        return values.TryGetValue(stat, out float value) ? value : fallback;
    }

    private static float ApplyFinalCap(BonusStat stat, float value, float baseValue)
    {
        if (!Mathf.Approximately(baseValue, 0f) && IsCappedModifierStat(stat))
            return value;

        return stat switch
        {
            BonusStat.MeleeDamage => ApplyModifierCap(stat, value),
            BonusStat.RangedDamage => ApplyModifierCap(stat, value),
            BonusStat.SpellDamage => ApplyModifierCap(stat, value),
            BonusStat.HolyDamage => ApplyModifierCap(stat, value),
            BonusStat.ManaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.AttackStaminaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.SprintStaminaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.BlockStaminaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.DodgeCostReduction => Mathf.Clamp(value, 0f, 0.4f),
            BonusStat.BlockEfficiency => Mathf.Clamp(value, 0f, 0.6f),
            BonusStat.SellPriceMultiplier => Mathf.Clamp(value, 0f, 0.6f),
            BonusStat.XPGain => Mathf.Clamp(value, -1f, 0.25f),
            BonusStat.CombatXPGain => Mathf.Clamp(value, -1f, 0.25f),
            BonusStat.QuestXPGain => Mathf.Clamp(value, -1f, 0.25f),
            BonusStat.RepairCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.DurabilityLossReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.AbilityCooldownReduction => Mathf.Clamp(value, 0f, 0.4f),
            _ => value
        };
    }

    private static float ApplyModifierCap(BonusStat stat, float value)
    {
        return stat switch
        {
            BonusStat.MeleeDamage => Mathf.Clamp(value, -1f, 0.75f),
            BonusStat.RangedDamage => Mathf.Clamp(value, -1f, 0.75f),
            BonusStat.SpellDamage => Mathf.Clamp(value, -1f, 0.75f),
            BonusStat.HolyDamage => Mathf.Clamp(value, -1f, 1f),
            BonusStat.ManaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.AttackStaminaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.SprintStaminaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.BlockStaminaCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.DodgeCostReduction => Mathf.Clamp(value, 0f, 0.4f),
            BonusStat.BlockEfficiency => Mathf.Clamp(value, 0f, 0.6f),
            BonusStat.XPGain => Mathf.Clamp(value, -1f, 0.25f),
            BonusStat.CombatXPGain => Mathf.Clamp(value, -1f, 0.25f),
            BonusStat.QuestXPGain => Mathf.Clamp(value, -1f, 0.25f),
            BonusStat.RepairCostReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.DurabilityLossReduction => Mathf.Clamp(value, 0f, 0.5f),
            BonusStat.AbilityCooldownReduction => Mathf.Clamp(value, 0f, 0.4f),
            _ => value
        };
    }

    private static bool IsCappedModifierStat(BonusStat stat)
    {
        return stat switch
        {
            BonusStat.MeleeDamage => true,
            BonusStat.RangedDamage => true,
            BonusStat.SpellDamage => true,
            BonusStat.HolyDamage => true,
            BonusStat.ManaCostReduction => true,
            BonusStat.AttackStaminaCostReduction => true,
            BonusStat.SprintStaminaCostReduction => true,
            BonusStat.BlockStaminaCostReduction => true,
            BonusStat.DodgeCostReduction => true,
            BonusStat.BlockEfficiency => true,
            BonusStat.XPGain => true,
            BonusStat.CombatXPGain => true,
            BonusStat.QuestXPGain => true,
            BonusStat.RepairCostReduction => true,
            BonusStat.DurabilityLossReduction => true,
            BonusStat.AbilityCooldownReduction => true,
            _ => false
        };
    }
}
