using UnityEngine;
[CreateAssetMenu(fileName = "New LevelXp", menuName = "LevelXp")]
public class LevelXp: ScriptableObject
{
    [SerializeField] private int[] _xpsToNextLevel;
    public int GetRequiredXp(int level)
    {
        if (_xpsToNextLevel == null || _xpsToNextLevel.Length == 0) return 0;
        int index = Mathf.Clamp(level - 1, 0, _xpsToNextLevel.Length - 1);
        return _xpsToNextLevel[index];
    }
}
