using System;
using UnityEngine;

public enum ProjectileLaunchResource
{
    None,
    Arrows,
    Mana
}

public class ProjectileLauncher : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int _fireRate = 60;
    [SerializeField] private float _projectileVelocity = 35f;
    [SerializeField] private float _projectileDamage = 10f;
    [SerializeField] private int _projectilePenetration;
    [SerializeField] private Transform _projectileSpawn;

    [Header("Projectile")]
    [SerializeField] private Mesh _projectileMesh;
    [SerializeField] private Material _projectileMaterial;
    [SerializeField] private ProjectileDamageType _damageType = ProjectileDamageType.Physical;
    [SerializeField] private LayerMask _projectileHitMask = ~0;
    [SerializeField] private bool _useGravity = true;

    [Header("Resource Cost")]
    [SerializeField] private Character _resourceOwner;
    [SerializeField] private ProjectileLaunchResource _resource = ProjectileLaunchResource.Arrows;
    [SerializeField] private bool _unlimitedResource;
    [SerializeField] private int _arrowsPerShot = 1;
    [SerializeField] private float _manaPerShot = 5f;

    [Header("Fire Mode")]
    [SerializeField] private bool _isSemiAuto = true;

    private bool _semiAutoTriggerLocked;
    private float _timeBetweenShots;
    private float _nextAllowedShotTime;

    public Mesh ProjectileMesh => _projectileMesh;
    public Material ProjectileMaterial => _projectileMaterial;
    public ProjectileDamageType DamageType => _damageType;
    public LayerMask ProjectileHitMask => _projectileHitMask;
    public ProjectileLaunchResource Resource => _resource;
    public bool UseGravity => _useGravity;
    public float TimeBetweenShots => _timeBetweenShots;
    public Transform ProjectileSpawn => _projectileSpawn;

    public event Action<ProjectileLauncher> OnShotRequested;

    private void Awake()
    {
        if (_resourceOwner == null)
            _resourceOwner = GetComponentInParent<Character>();

        _timeBetweenShots = _fireRate > 0 ? 60f / _fireRate : 999f;
    }

    public void TryShoot()
    {
        if (_isSemiAuto && _semiAutoTriggerLocked) return;
        Transform spawn = _projectileSpawn != null ? _projectileSpawn : transform;
        Shoot(spawn.forward);
    }

    public void TryShoot(Vector3 launchDirection)
    {
        if (_isSemiAuto && _semiAutoTriggerLocked) return;
        Shoot(launchDirection);
    }

    public void ReleaseTrigger() => _semiAutoTriggerLocked = false;

    public void SetResourceOwner(Character resourceOwner) => _resourceOwner = resourceOwner;

    public bool CanShoot()
    {
        if (LocalProjectilePool.Instance == null) return false;
        if (Time.time < _nextAllowedShotTime) return false;
        return HasResource();
    }

    private void Shoot(Vector3 launchDirection)
    {
        if (!CanShoot()) return;
        if (launchDirection.sqrMagnitude <= 0.0001f) return;

        _nextAllowedShotTime = Time.time + _timeBetweenShots;
        ConsumeResource();

        Transform spawn = _projectileSpawn != null ? _projectileSpawn : transform;
        float projectileVelocity = _resourceOwner != null ?
            _resourceOwner.GetProjectileVelocity(_projectileVelocity, _damageType) :
            _projectileVelocity;
        float projectileDamage = _resourceOwner != null ?
            _resourceOwner.GetProjectileDamage(_projectileDamage, _damageType) :
            _projectileDamage;

        LocalProjectilePool.Instance.SpawnProjectile(
            spawn.position,
            launchDirection.normalized * projectileVelocity,
            projectileDamage,
            _projectilePenetration,
            _damageType,
            _projectileMesh,
            _projectileMaterial,
            _projectileHitMask,
            _useGravity,
            gameObject);

        OnShotRequested?.Invoke(this);
        if (_isSemiAuto) _semiAutoTriggerLocked = true;
    }

    private bool HasResource()
    {
        if (_unlimitedResource) return true;

        return _resource switch
        {
            ProjectileLaunchResource.None => true,
            ProjectileLaunchResource.Arrows => _resourceOwner != null && _resourceOwner.HasArrows(_arrowsPerShot),
            ProjectileLaunchResource.Mana => _resourceOwner != null && _resourceOwner.HasMana(_resourceOwner.GetManaCost(_manaPerShot)),
            _ => false
        };
    }

    private void ConsumeResource()
    {
        if (_unlimitedResource) return;

        switch (_resource)
        {
            case ProjectileLaunchResource.Arrows:
                _resourceOwner.TrySpendArrows(_arrowsPerShot);
                break;
            case ProjectileLaunchResource.Mana:
                _resourceOwner.TrySpendMana(_resourceOwner.GetManaCost(_manaPerShot));
                break;
        }
    }
    public bool TryGetLaunchDirection(Vector3 startPoint, Vector3 endPoint, float accuracy,
        out Vector3 launchDirection, float maxSpreadAngleDegrees = 15f)
    {
        launchDirection = Vector3.zero;

        Vector3 displacement = endPoint - startPoint;

        // If gravity is disabled, just shoot directly at the endpoint.
        if (!_useGravity)
        {
            launchDirection = displacement.normalized;
            return true;
        }
        accuracy = Mathf.Clamp01(accuracy);
        Vector3 horizontalDisplacement = new(displacement.x, 0f, displacement.z);

        float horizontalDistance = horizontalDisplacement.magnitude;
        float verticalDistance = displacement.y;
        float gravity = LocalProjectilePool.Instance != null ?
            LocalProjectilePool.Instance.Gravity :
            Mathf.Abs(Physics.gravity.y);

        if (gravity <= 0f)
        {
            launchDirection = displacement.normalized;
            return true;
        }
        bool canReachTarget = true;

        if (horizontalDistance <= 0.001f) launchDirection = verticalDistance >= 0f ? Vector3.up : Vector3.down;
        else
        {
            float projectileVelocity = _resourceOwner != null ?
                _resourceOwner.GetProjectileVelocity(_projectileVelocity, _damageType) :
                _projectileVelocity;
            float speedSquared = projectileVelocity * projectileVelocity;
            float speedFourth = speedSquared * speedSquared;
            float discriminant =
                speedFourth -
                gravity * (
                    gravity * horizontalDistance * horizontalDistance +
                    2f * verticalDistance * speedSquared
                );
            Vector3 horizontalDirection = horizontalDisplacement.normalized;
            if (discriminant >= 0f)
            {
                // Low arc = fastest travel time.
                float sqrtDiscriminant = Mathf.Sqrt(discriminant);
                float angle = Mathf.Atan(
                    (speedSquared - sqrtDiscriminant) /
                    (gravity * horizontalDistance)
                );
                launchDirection = horizontalDirection * Mathf.Cos(angle) + Vector3.up * Mathf.Sin(angle);
            }
            else
            {
                // Target is unreachable, so maximize range instead.
                // 45 degrees gives max range on flat ground.
                float angle = 45f * Mathf.Deg2Rad;
                launchDirection = horizontalDirection * Mathf.Cos(angle) + Vector3.up * Mathf.Sin(angle);
                canReachTarget = false;
            }
        }
        launchDirection.Normalize();

        // Accuracy falloff by distance.
        // Close range = very accurate.
        // Max range = full accuracy penalty.
        float distance = displacement.magnitude;
        float rangePercent = 100f <= 0f ? 1f : Mathf.Clamp01(distance / 100f);
        float spreadDegrees = maxSpreadAngleDegrees * (1f - accuracy) * rangePercent;
        if (spreadDegrees > 0f)
        {
            Vector3 right = Vector3.Cross(launchDirection, Vector3.up);
            if (right.sqrMagnitude <= 0.001f) right = Vector3.Cross(launchDirection, Vector3.right);
            right.Normalize();
            Vector3 up = Vector3.Cross(right, launchDirection).normalized;

            float spreadRadius = Mathf.Tan(spreadDegrees * Mathf.Deg2Rad);
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * spreadRadius;

            launchDirection = launchDirection + right * randomOffset.x + up * randomOffset.y;
            launchDirection.Normalize();
        }

        return canReachTarget;
    }
}
