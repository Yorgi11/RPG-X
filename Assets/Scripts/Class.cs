using UnityEngine;
[CreateAssetMenu(fileName = "New Class", menuName = "Class")]
public class Class : ScriptableObject
{
    public string _className;
    public string _classDescription;
    public Stats _classStats;
    [Header("Class Bonuses")]
    public BonusDefinition[] _bonuses;
    [Header("Class Abilities")]
    public AbilityDefinition[] _startingAbilities;
    public AbilityLoadoutEntry[] _defaultLoadout;
    // starting equipment
    // prefered equipment/weapon types
    // weaknesses
    // dialogue tags
}
