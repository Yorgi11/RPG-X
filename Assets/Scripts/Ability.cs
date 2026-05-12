[System.Serializable]
public class Ability
{
    public string _abilityName;
    public string _abilityDescription;
    public float _durration;
    public float _cooldown;
    public StatsBonus _statsBonus;
    public ResourceBonus _resourceBonus;
    public MeleeBonus _meleeBonus;
    public RangedBonus _rangedBonus;
    public MagicBonus _magicBonus;
    public XPBonus _xpBonus;
    public DefenceBonus _defenceBonus;
    public WeaponBonus _weaponBonus;
    public StaminaBonus _staminaBonus;
}