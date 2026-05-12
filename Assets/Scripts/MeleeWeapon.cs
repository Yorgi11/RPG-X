using System.Collections;
using QF_Tools.QF_Utilities;
using UnityEngine;
public class MeleeWeapon : MonoBehaviour
{
    [System.Serializable]
    public struct AttackKeyFrame
    {
        public Vector3 _pos;
        public Vector3 _rot;
    }
    [Header("Stats")]
    [SerializeField] private int _attackRate = 60;
    [SerializeField] private float _lightDamage = 10f;
    [SerializeField] private float _heavyDamage = 40f;
    [SerializeField] private Transform _damageBox;
    [SerializeField] private AttackKeyFrame[] _attackPath;
    private Coroutine _swingCoroutine;
    private bool _swingComplete = true;
    public bool IsSwinging => !_swingComplete;

    public void Swing()
    {
        if (!_swingComplete || _attackPath == null || _attackPath.Length == 0) return;
        if (_swingCoroutine != null) StopCoroutine(_swingCoroutine);
        _swingCoroutine = StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        Vector3[] positions = new Vector3[_attackPath.Length];
        Vector3[] rotations = new Vector3[_attackPath.Length];

        for (int i = 0; i < _attackPath.Length; i++)
        {
            positions[i] = _attackPath[i]._pos;
            rotations[i] = _attackPath[i]._rot;
        }

        float duration = _attackRate > 0 ? 60f / _attackRate : 0f;

        yield return QF_Coroutines.LerpOverSequenceOverTime(
            positions, rotations, duration,
            QF_Coroutines.LerpVector3, QF_Coroutines.LerpVector3,
            QF_Coroutines.DistVector3, QF_Coroutines.DistVector3,
            values => { transform.SetLocalPositionAndRotation(values.Item1, Quaternion.Euler(values.Item2)); },
            onComplete: value => _swingComplete = value);
        _swingCoroutine = null;
    }
}
