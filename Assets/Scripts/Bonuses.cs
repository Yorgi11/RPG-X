using UnityEngine;
[System.Serializable]
public class StatsBonus
{
    [Range(0, 100)] public int _healthBonus = 0;
    [Range(0, 100)] public int _staminaBonus = 0;
    [Range(0, 100)] public int _manaBonus = 0;
    [Range(0, 100)] public int _strengthBonus = 0;
    [Range(0, 100)] public int _enduranceBonus = 0;
    [Range(0, 100)] public int _intelligenceBonus = 0;
    [Range(0, 100)] public int _faithBonus = 0;
    [Range(0, 100)] public int _agilityBonus = 0;
    [Range(0, 100)] public int _carryWeightBonus = 0;
}
[System.Serializable]
public class ResourceBonus
{
    [Range(0f, 10f)] public float _healthRegen = 0;
    [Range(0f, 1f)] public float _staminaRegen = 0f;
    [Range(0f, 1f)] public float _manaRegen = 0f;
    [Range(0f, 1f)] public float _potionEffectiveness = 0f;
}
[System.Serializable]
public class MeleeBonus
{
    [Range(0f, 1f)] public float _meleeDamage = 0f;
    [Range(0f, 1f)] public float _lightAttackDamage = 0f;
    [Range(0f, 1f)] public float _heavyAttackDamage = 0f;
    [Range(0f, 1f)] public float _criticalHitChance = 0f;
    [Range(0f, 1f)] public float _criticalHitDamage = 0f;
    [Range(0f, 1f)] public float _attackSpeed = 0f;
}
[System.Serializable]
public class RangedBonus
{
    [Range(0f, 1f)] public float _rangedDamage = 0f;
    [Range(0f, 1f)] public float _projectileSpeed = 0f;
    [Range(0f, 1f)] public float _reloadSpeed = 0f;
    [Range(0f, 1f)] public float _drawSpeed = 0f;
    [Range(0f, 1f)] public float _headshotDamage = 0f;
    [Range(0f, 1f)] public float _ammoRecoveryChance = 0f;
}
[System.Serializable]
public class MagicBonus
{
    [Range(0f, 1f)] public float _magicDamage = 0f;
    [Range(0f, 1f)] public float _manaCost = 0f;
    [Range(0f, 1f)] public float _castSpeed = 0f;
    [Range(0f, 1f)] public float _spellProjectileSpeed = 0f;
    [Range(0f, 1f)] public float _AOESize = 0f;
    [Range(0, 100)] public float _manaRecoveryAfterKill = 0;
}
[System.Serializable]
public class XPBonus
{
    [Range(0, 2)] public int _upgradePoint = 0;
    [Range(0, 10)] public int _numLevelsPerUpgradePoint = 0;
    [Range(0f, 1f)] public float _xpGain = 0f;
    [Range(0f, 1f)] public float _combatXpGain = 0f;
    [Range(0f, 1f)] public float _questXpGain = 0f;
}
[System.Serializable]
public class DefenceBonus
{
    [Range(0f, 1f)] public float _armourEffectiveness = 0f;
    [Range(0f, 1f)] public float _physicalResistance = 0f;
    [Range(0f, 1f)] public float _magicResistance = 0f;
    [Range(0f, 1f)] public float _blockDamage = 0f;
    [Range(0f, 1f)] public float _parryWindow = 0f;
}
[System.Serializable]
public class WeaponBonus
{
    [Range(0f, 1f)] public float _swordDamage = 0f;
    [Range(0f, 1f)] public float _axeDamage = 0f;
    [Range(0f, 1f)] public float _spearDamage = 0f;
    [Range(0f, 1f)] public float _daggerDamage = 0f;
    [Range(0f, 1f)] public float _staffDamage = 0f;
    [Range(0f, 1f)] public float _wandDamage = 0f;
}
[System.Serializable]
public class StaminaBonus
{
    [Range(0f, 1f)] public float _attackStamina = 0f;
    [Range(0f, 1f)] public float _sprintStamina = 0f;
    [Range(0f, 1f)] public float _blockStamina = 0f;
    [Range(0, 100)] public float _staminaRecoveryAfterKill = 0;
    [Range(0, 100)] public float _staminaRecoveryAfterParry = 0;
}