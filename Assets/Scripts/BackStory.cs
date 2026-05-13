using UnityEngine;
[CreateAssetMenu(fileName = "New BackStory", menuName = "BackStory")]
public class BackStory: ScriptableObject
{
    public string _backStoryName;
    public string _backStoryDescription;
    [Header("Backstory Bonuses")]
    public BonusDefinition[] _bonuses;
}
