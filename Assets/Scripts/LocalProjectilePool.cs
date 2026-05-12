using System.Collections.Generic;
using UnityEngine;

public class LocalProjectilePool : MonoBehaviour
{
    public static LocalProjectilePool Instance { get; private set; }

    [SerializeField] private int _initialPoolSize = 64;
    [SerializeField] private float _maxFlightTime = 3f;
    [SerializeField] private float _gravity = 9.81f;
    public float Gravity => _gravity;

    private class LocalProjectile
    {
        public bool _useGravity = true;
        public int _penetration;
        public float _damage;
        public float _currentFlightTime;
        public Vector3 _position;
        public Vector3 _lastPosition;
        public Vector3 _velocity;
        public ProjectileDamageType _damageType;
        public LayerMask _hitMask;
        public GameObject _source;
        public GameObject _visualObject;
        public Transform _visualTransform;
        public MeshFilter _meshFilter;
        public MeshRenderer _meshRenderer;
        public readonly List<IProjectileDamageable> _hitObjs = new(6);

        public void Reset()
        {
            _useGravity = true;
            _penetration = 0;
            _damage = 0f;
            _currentFlightTime = 0f;
            _position = Vector3.zero;
            _lastPosition = Vector3.zero;
            _velocity = Vector3.zero;
            _damageType = ProjectileDamageType.Physical;
            _hitMask = ~0;
            _source = null;
            _hitObjs.Clear();

            if (_meshFilter != null) _meshFilter.sharedMesh = null;
            if (_meshRenderer != null) _meshRenderer.sharedMaterial = null;
            if (_visualObject != null) _visualObject.SetActive(false);
        }
    }

    private readonly List<LocalProjectile> _reserveProjectiles = new();
    private readonly List<LocalProjectile> _activeProjectiles = new();
    private readonly RaycastHit[] _hits = new RaycastHit[16];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        for (int i = 0; i < _initialPoolSize; i++)
            AddProjectile();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update() => UpdateProjectiles(Time.deltaTime);

    private void AddProjectile()
    {
        GameObject visual = new("Local Projectile");
        visual.transform.SetParent(transform);
        visual.SetActive(false);

        MeshFilter meshFilter = visual.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = visual.AddComponent<MeshRenderer>();

        LocalProjectile projectile = new()
        {
            _visualObject = visual,
            _visualTransform = visual.transform,
            _meshFilter = meshFilter,
            _meshRenderer = meshRenderer
        };

        projectile.Reset();
        _reserveProjectiles.Add(projectile);
    }

    private void RemoveProjectileAt(int i)
    {
        LocalProjectile projectile = _activeProjectiles[i];
        _activeProjectiles.RemoveAt(i);
        projectile.Reset();
        _reserveProjectiles.Add(projectile);
    }

    public void SpawnProjectile(
        Vector3 position,
        Vector3 velocity,
        float damage,
        int penetration,
        ProjectileDamageType damageType,
        Mesh visualMesh,
        Material visualMaterial,
        LayerMask hitMask,
        bool useGravity = true,
        GameObject source = null)
    {
        if (_reserveProjectiles.Count <= 0)
            AddProjectile();

        int last = _reserveProjectiles.Count - 1;
        LocalProjectile projectile = _reserveProjectiles[last];
        _reserveProjectiles.RemoveAt(last);

        projectile.Reset();
        projectile._position = position;
        projectile._lastPosition = position;
        projectile._velocity = velocity;
        projectile._damage = damage;
        projectile._penetration = penetration;
        projectile._damageType = damageType;
        projectile._hitMask = hitMask;
        projectile._useGravity = useGravity;
        projectile._source = source;
        projectile._meshFilter.sharedMesh = visualMesh;
        projectile._meshRenderer.sharedMaterial = visualMaterial;
        projectile._visualTransform.SetPositionAndRotation(position, VelocityRotation(velocity));

        projectile._visualObject.SetActive(visualMesh != null);

        _activeProjectiles.Add(projectile);
    }

    private void UpdateProjectiles(float dt)
    {
        if (_activeProjectiles.Count <= 0) return;

        for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
        {
            LocalProjectile projectile = _activeProjectiles[i];
            projectile._currentFlightTime += dt;

            if (projectile._currentFlightTime > _maxFlightTime ||
                projectile._damage <= 0.1f ||
                projectile._velocity.sqrMagnitude <= 0.01f)
            {
                RemoveProjectileAt(i);
                continue;
            }

            projectile._lastPosition = projectile._position;
            if (projectile._useGravity)
                projectile._velocity += Vector3.down * (_gravity * dt);
            projectile._position += projectile._velocity * dt;

            Vector3 travel = projectile._position - projectile._lastPosition;
            float travelSqr = travel.sqrMagnitude;
            if (travelSqr <= 0.00000001f) continue;

            float travelDistance = Mathf.Sqrt(travelSqr);
            int hitCount = Physics.RaycastNonAlloc(
                projectile._lastPosition,
                travel / travelDistance,
                _hits,
                travelDistance,
                projectile._hitMask,
                QueryTriggerInteraction.Ignore);

            if (hitCount > 1)
                System.Array.Sort(_hits, 0, hitCount, RaycastHitDistanceComparer.Instance);

            bool projectileRemoved = false;

            for (int h = 0; h < hitCount; h++)
            {
                Collider col = _hits[h].collider;
                if (!col) continue;

                IProjectileDamageable damageable = col.GetComponentInParent<IProjectileDamageable>();
                if (damageable == null || AlreadyHit(projectile._hitObjs, damageable)) continue;

                projectile._hitObjs.Add(damageable);
                damageable.TakeProjectileDamage(new ProjectileDamageInfo(
                    projectile._damage,
                    projectile._damageType,
                    projectile._source,
                    _hits[h].point,
                    _hits[h].normal));

                projectile._position = _hits[h].point;
                projectile._velocity *= 0.8f;
                projectile._damage *= 0.8f;

                if (--projectile._penetration < 0 ||
                    projectile._damage <= 0.1f ||
                    projectile._velocity.sqrMagnitude <= 0.01f)
                {
                    RemoveProjectileAt(i);
                    projectileRemoved = true;
                    break;
                }
            }

            if (projectileRemoved) continue;

            projectile._visualTransform.SetPositionAndRotation(projectile._position, VelocityRotation(projectile._velocity));

        }
    }

    private static Quaternion VelocityRotation(Vector3 velocity)
    {
        if (velocity.sqrMagnitude <= 0.0001f) return Quaternion.identity;
        return Quaternion.LookRotation(velocity.normalized, Vector3.up);
    }

    private static bool AlreadyHit(List<IProjectileDamageable> hitObjs, IProjectileDamageable obj)
    {
        for (int i = 0; i < hitObjs.Count; i++)
            if (hitObjs[i] == obj) return true;
        return false;
    }

    private sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public static readonly RaycastHitDistanceComparer Instance = new();
        public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
    }
}
