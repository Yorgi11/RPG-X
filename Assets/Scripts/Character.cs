using System;
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
    private int _currentLevel = 1;
    private int _currentXp = 0;
    private int _currentRequiredXp = 0;
    private int _currentUpgradePoints = 0;
    
    private float _currentMana;

    public Class Class => _class;
    public BackStory BackStory => _backStory;
    public Stats CurrentStats => _currentStats;
    public int Arrows => _arrows;
    public float CurrentMana => _currentMana;
    public float MaxMana => _currentStats != null ? _currentStats._maxMana : 0f;
    public event Action<int> OnArrowsChanged;
    public event Action<float, float> OnManaChanged;

    private void Awake()
    {
        RebuildStats();
        _currentMana = MaxMana;
        _currentRequiredXp = _levelXp.GetRequiredXp(_currentLevel);
    }

    public void RebuildStats()
    {
        Stats classStats = _class != null ? _class._classStats : null;
        _currentStats = CopyStats(classStats);
        if (_backStory != null && _backStory._statsBonus != null) ApplyStatsBonus(_currentStats, _backStory._statsBonus);
        _currentMana = Mathf.Min(_currentMana, MaxMana);
    }

    public void AddXP(int xpToAdd)
    {
        _currentXp += xpToAdd;
        HandleXpChanged();
    }
    private void HandleXpChanged()
    {
        if (_currentXp >= _currentRequiredXp) _currentXp -= _currentRequiredXp;
        _currentLevel++;
        _currentRequiredXp = _levelXp.GetRequiredXp(_currentLevel);
        // update level
        // update required xp
        // send level up notification
        _currentUpgradePoints += 9;
        if (_currentXp >= _currentRequiredXp) HandleXpChanged();
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

    public bool TrySpendMana(float amount)
    {
        if (!HasMana(amount)) return false;
        if (amount <= 0f) return true;

        _currentMana -= amount;
        OnManaChanged?.Invoke(_currentMana, MaxMana);
        return true;
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
    private static void ApplyStatsBonus(Stats stats, StatsBonus bonus)
    {
        stats._maxHealth += bonus._healthBonus;
        stats._maxStamina += bonus._staminaBonus;
        stats._maxMana += bonus._manaBonus;
        stats._strength += bonus._strengthBonus;
        stats._endurance += bonus._enduranceBonus;
        stats._intelligence += bonus._intelligenceBonus;
        stats._faith += bonus._faithBonus;
        stats._agility += bonus._agilityBonus;
        stats._maxCarryWeight += bonus._carryWeightBonus;
    }
}
