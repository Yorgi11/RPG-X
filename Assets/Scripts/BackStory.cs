using UnityEngine;
[CreateAssetMenu(fileName = "New BackStory", menuName = "BackStory")]
public class BackStory: ScriptableObject
{
    public string _backStoryName;
    public string _backStoryDescription;
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