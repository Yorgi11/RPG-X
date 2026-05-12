using UnityEngine;

public enum ProjectileDamageType
{
    Physical,
    Magic
}

public readonly struct ProjectileDamageInfo
{
    public readonly float _damage;
    public readonly ProjectileDamageType _damageType;
    public readonly GameObject _source;
    public readonly Vector3 _hitPoint;
    public readonly Vector3 _hitNormal;

    public ProjectileDamageInfo(float damage, ProjectileDamageType damageType, GameObject source, Vector3 hitPoint, Vector3 hitNormal)
    {
        _damage = damage;
        _damageType = damageType;
        _source = source;
        _hitPoint = hitPoint;
        _hitNormal = hitNormal;
    }
}

public interface IProjectileDamageable
{
    void TakeProjectileDamage(ProjectileDamageInfo damageInfo);
}
