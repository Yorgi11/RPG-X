using System;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    [SerializeField] private Class _class;
    [SerializeField] private BackStory _backStory;
    [SerializeField] private LevelXp _levelXp;
    [SerializeField] private int _arrows = 20;

    private Stats _currentStats;
    // inventory
    // equipment
    // abilities
    private readonly List<ActiveBonus> _activeBonuses = new();
    private readonly BonusModifierSet _currentBonuses = new();
    private int _currentLevel = 1;
    private int _currentXp = 0;
    private int _currentRequiredXp = 0;
    private int _currentUpgradePoints = 0;
    
    private float _currentHealth;
    private float _currentStamina;
    private float _currentMana;

    public Class Class => _class;
    public BackStory BackStory => _backStory;
    public Stats CurrentStats => _currentStats;
    public BonusModifierSet CurrentBonuses => _currentBonuses;
    public IReadOnlyList<ActiveBonus> ActiveBonuses => _activeBonuses;
    public int CurrentLevel => _currentLevel;
    public int CurrentXp => _currentXp;
    public int CurrentRequiredXp => _currentRequiredXp;
    public int CurrentUpgradePoints => _currentUpgradePoints;
    public int Arrows => _arrows;
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _currentStats != null ? _currentStats._maxHealth : 0f;
    public float CurrentStamina => _currentStamina;
    public float MaxStamina => _currentStats != null ? _currentStats._maxStamina : 0f;
    public float CurrentMana => _currentMana;
    public float MaxMana => _currentStats != null ? _currentStats._maxMana : 0f;
    public event Action<int> OnArrowsChanged;
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnStaminaChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<Stats> OnStatsChanged;
    public event Action<BonusModifierSet> OnBonusesChanged;
    public event Action<int> OnLevelChanged;
    public event Action<int, int> OnXpChanged;
    public event Action<int> OnUpgradePointsChanged;

    private void Awake()
    {
        RebuildStats();
        _currentHealth = MaxHealth;
        _currentStamina = MaxStamina;
        _currentMana = MaxMana;
        _currentRequiredXp = GetRequiredXp(_currentLevel);
    }

    private void Update()
    {
        if (_activeBonuses.Count <= 0) return;

        bool expiredAny = false;
        for (int i = _activeBonuses.Count - 1; i >= 0; i--)
        {
            ActiveBonus activeBonus = _activeBonuses[i];
            if (!activeBonus.IsTimed) continue;

            activeBonus._remainingDuration -= Time.deltaTime;
            if (!activeBonus.IsExpired) continue;

            _activeBonuses.RemoveAt(i);
            expiredAny = true;
        }

        if (expiredAny) RebuildStats();
    }

    public void RebuildStats()
    {
        Stats classStats = _class != null ? _class._classStats : null;
        _currentStats = CopyStats(classStats);

        _currentBonuses.Clear();
        AddClassBonuses(_currentBonuses, _class);
        AddBackStoryBonuses(_currentBonuses, _backStory);

        for (int i = 0; i < _activeBonuses.Count; i++)
            if (!_activeBonuses[i].IsExpired)
                _currentBonuses.Add(_activeBonuses[i]._definition);

        ApplyCoreStatBonuses(_currentStats, _currentBonuses);
        _currentHealth = Mathf.Min(_currentHealth, MaxHealth);
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
        _currentMana = Mathf.Min(_currentMana, MaxMana);
        OnStatsChanged?.Invoke(_currentStats);
        OnBonusesChanged?.Invoke(_currentBonuses);
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
        OnManaChanged?.Invoke(_currentMana, MaxMana);
    }

    public ActiveBonus AddBonus(BonusDefinition bonus, string runtimeSourceId = null, UnityEngine.Object sourceObject = null, float durationOverride = -1f)
    {
        if (bonus == null) return null;

        float duration = durationOverride >= 0f ? durationOverride : -1f;
        ActiveBonus activeBonus = new(bonus, runtimeSourceId, sourceObject, duration);
        _activeBonuses.Add(activeBonus);
        RebuildStats();
        return activeBonus;
    }

    public bool RemoveBonus(string bonusId)
    {
        if (string.IsNullOrWhiteSpace(bonusId)) return false;

        bool removedAny = false;
        for (int i = _activeBonuses.Count - 1; i >= 0; i--)
        {
            BonusDefinition definition = _activeBonuses[i]._definition;
            if (definition == null || definition._bonusId != bonusId) continue;

            _activeBonuses.RemoveAt(i);
            removedAny = true;
        }

        if (removedAny) RebuildStats();
        return removedAny;
    }

    public float GetBonusValue(BonusStat stat)
    {
        return _currentBonuses.GetBonusValue(stat);
    }

    public float ApplyBonus(BonusStat stat, float baseValue)
    {
        return _currentBonuses.GetFinalValue(stat, baseValue);
    }

    public float ApplyReduction(BonusStat stat, float baseValue)
    {
        return baseValue * _currentBonuses.GetReductionMultiplier(stat);
    }

    public float GetProjectileDamage(float baseDamage, ProjectileDamageType damageType)
    {
        float damage = baseDamage;
        damage *= 1f + _currentBonuses.GetModifierValue(BonusStat.RangedDamage);

        if (damageType == ProjectileDamageType.Magic)
            damage *= 1f + _currentBonuses.GetModifierValue(BonusStat.SpellDamage);

        return Mathf.Max(0f, damage);
    }

    public float GetProjectileVelocity(float baseVelocity, ProjectileDamageType damageType)
    {
        float velocity = baseVelocity * (1f + _currentBonuses.GetModifierValue(BonusStat.ProjectileSpeed));

        if (damageType == ProjectileDamageType.Magic)
            velocity *= 1f + _currentBonuses.GetModifierValue(BonusStat.SpellProjectileSpeed);

        return Mathf.Max(0f, velocity);
    }

    public float GetManaCost(float baseCost)
    {
        return Mathf.Max(0f, ApplyReduction(BonusStat.ManaCostReduction, baseCost));
    }

    public void AddXP(int xpToAdd)
    {
        if (xpToAdd <= 0) return;

        int adjustedXp = Mathf.RoundToInt(xpToAdd * (1f + _currentBonuses.GetModifierValue(BonusStat.XPGain)));
        _currentXp += Mathf.Max(0, adjustedXp);
        HandleXpChanged();
        OnXpChanged?.Invoke(_currentXp, _currentRequiredXp);
    }
    private void HandleXpChanged()
    {
        while (_currentRequiredXp > 0 && _currentXp >= _currentRequiredXp)
        {
            _currentXp -= _currentRequiredXp;
            _currentLevel++;
            _currentRequiredXp = GetRequiredXp(_currentLevel);
            AddUpgradePoints(GetUpgradePointsForLevel(_currentLevel));
            OnLevelChanged?.Invoke(_currentLevel);
        }
    }

    public bool HasArrows(int amount) => amount <= 0 || _arrows >= amount;
    public bool TrySpendArrows(int amount)
    {
        if (!HasArrows(amount)) return false;
        if (amount <= 0) return true;

        _arrows -= amount;
        OnArrowsChanged?.Invoke(_arrows);
        return true;
    }
    public bool HasMana(float amount) => amount <= 0f || _currentMana >= amount;

    public bool HasHealth(float amount) => amount <= 0f || _currentHealth > amount;

    public bool TrySpendHealth(float amount)
    {
        if (!HasHealth(amount)) return false;
        if (amount <= 0f) return true;

        _currentHealth -= amount;
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        return true;
    }

    public void ApplyHealing(float amount)
    {
        if (amount <= 0f) return;

        _currentHealth = Mathf.Min(MaxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
    }

    public bool HasStamina(float amount) => amount <= 0f || _currentStamina >= amount;

    public bool TrySpendStamina(float amount)
    {
        if (!HasStamina(amount)) return false;
        if (amount <= 0f) return true;

        _currentStamina -= amount;
        OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
        return true;
    }

    public void RestoreStamina(float amount)
    {
        if (amount <= 0f) return;

        _currentStamina = Mathf.Min(MaxStamina, _currentStamina + amount);
        OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
    }

    public bool TrySpendMana(float amount)
    {
        if (!HasMana(amount)) return false;
        if (amount <= 0f) return true;

        _currentMana -= amount;
        OnManaChanged?.Invoke(_currentMana, MaxMana);
        return true;
    }

    public void RestoreMana(float amount)
    {
        if (amount <= 0f) return;

        _currentMana = Mathf.Min(MaxMana, _currentMana + amount);
        OnManaChanged?.Invoke(_currentMana, MaxMana);
    }

    private int GetRequiredXp(int level)
    {
        return _levelXp != null ? _levelXp.GetRequiredXp(level) : 0;
    }

    private int GetUpgradePointsForLevel(int level)
    {
        int upgradePoints = 1 + Mathf.RoundToInt(GetBonusValue(BonusStat.UpgradePoints));
        int interval = Mathf.RoundToInt(GetBonusValue(BonusStat.UpgradePointEveryLevels));

        if (interval > 0 && level % interval == 0)
            upgradePoints++;

        return Mathf.Max(0, upgradePoints);
    }

    private void AddUpgradePoints(int amount)
    {
        if (amount <= 0) return;
        _currentUpgradePoints += amount;
        OnUpgradePointsChanged?.Invoke(_currentUpgradePoints);
    }

    private static Stats CopyStats(Stats source)
    {
        Stats stats = new();
        if (source == null) return stats;

        stats._maxHealth = source._maxHealth;
        stats._maxStamina = source._maxStamina;
        stats._maxMana = source._maxMana;
        stats._strength = source._strength;
        stats._endurance = source._endurance;
        stats._intelligence = source._intelligence;
        stats._faith = source._faith;
        stats._agility = source._agility;
        stats._maxCarryWeight = source._maxCarryWeight;
        return stats;
    }

    private static void ApplyCoreStatBonuses(Stats stats, BonusModifierSet bonuses)
    {
        stats._maxHealth = Mathf.Max(1, bonuses.GetFinalInt(BonusStat.MaxHealth, stats._maxHealth));
        stats._maxStamina = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.MaxStamina, stats._maxStamina));
        stats._maxMana = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.MaxMana, stats._maxMana));
        stats._strength = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.Strength, stats._strength));
        stats._endurance = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.Endurance, stats._endurance));
        stats._intelligence = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.Intelligence, stats._intelligence));
        stats._faith = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.Faith, stats._faith));
        stats._agility = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.Agility, stats._agility));
        stats._maxCarryWeight = Mathf.Max(0, bonuses.GetFinalInt(BonusStat.CarryWeight, stats._maxCarryWeight));
    }

    private static void AddClassBonuses(BonusModifierSet bonuses, Class characterClass)
    {
        if (characterClass == null) return;
        AddDefinitions(bonuses, characterClass._bonuses);
    }

    private static void AddBackStoryBonuses(BonusModifierSet bonuses, BackStory backStory)
    {
        if (backStory == null) return;
        AddDefinitions(bonuses, backStory._bonuses);
    }

    private static void AddDefinitions(BonusModifierSet bonuses, BonusDefinition[] definitions)
    {
        if (definitions == null) return;
        for (int i = 0; i < definitions.Length; i++)
            bonuses.Add(definitions[i]);
    }
}
