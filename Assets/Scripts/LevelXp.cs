using UnityEngine;
[CreateAssetMenu(fileName = "New LevelXp", menuName = "LevelXp")]
public class LevelXp: ScriptableObject
{
    [SerializeField] private int[] _xpsToNextLevel;
    public int GetRequiredXp(int level) => _xpsToNextLevel[level - 1];
}