using UnityEngine;
[CreateAssetMenu(fileName = "New Class", menuName = "Class")]
public class Class : ScriptableObject
{
    public string _className;
    public string _classDescription;
    public Stats _classStats;
    [Header("Class Bonuses")]
    public ResourceBonus _resourceBonus;
    public MeleeBonus _meleeBonus;
    public RangedBonus _rangedBonus;
    public MagicBonus _magicBonus;
    public XPBonus _xpBonus;
    public DefenceBonus _defenceBonus;
    public WeaponBonus _weaponBonus;
    public StaminaBonus _staminaBonus;
    [Header("Class Abilities")]
    public Ability[] _abilities;
    // starting equipment
    // prefered equipment/weapon types
    // weaknesses
    // dialogue tags
}