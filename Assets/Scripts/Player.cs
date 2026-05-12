using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Character))]
public class Player : MonoBehaviour
{
    [SerializeField] private Transform _rangedSpawnPoint;
    [SerializeField] private ProjectileLauncher _projectileLauncher;
    [SerializeField] private MeleeWeapon _meleeWeapon;
    [SerializeField] private LayerMask _targetLayers;
    [Header("Camera Settings")]
    [SerializeField] private float _mouseSensitivity = 12f;
    [SerializeField] private float _cameraMoveSpeed = 5f;
    [SerializeField] private float _cameraLowerLimit = -15f;
    [SerializeField] private float _cameraUpperLimit = 90f;
    [SerializeField] private Vector3 _cameraOffset;
    [Header("Jump Settings")]
    [SerializeField] private float _jumpBufferTime = 0.25f;
    [Header("Temp")]
    [Range(0f, 1f)] private float _accuracy = 1f;
    
    private float _dt;
    private float _jumpPressedTime = float.NegativeInfinity;
    private Transform _transform;
    private Transform _cameraTransform;
    private Transform _cameraPivotTransform;
    private Character _character;
    private Movement _movement;
    private MovementType _currentMovementType = MovementType.Walking;
    private InputSystem_Actions _inputActions;
    public Character Character => _character;
    void OnEnable() { _inputActions = new(); _inputActions.Enable(); }
    void OnDisable() => _inputActions.Disable();
    void Awake()
    {
        _movement = GetComponent<Movement>();
        _character = GetComponent<Character>();

        _transform = transform;
        _yRot = _transform.eulerAngles.y;
        _cameraTransform = Camera.main.transform;
        _cameraPivotTransform = _cameraTransform.parent;
        if (_projectileLauncher == null && _rangedSpawnPoint != null)
            _projectileLauncher = _rangedSpawnPoint.GetComponentInChildren<ProjectileLauncher>();
        if (_projectileLauncher != null) _projectileLauncher.SetResourceOwner(_character);
        ToggleMouse();
    }
    private void Update()
    {
        _dt = Time.deltaTime;
        _currentMovementType = _inputActions.Player.Crouch.IsPressed() ?
             MovementType.Crouching : _inputActions.Player.Sprint.IsPressed() ? MovementType.Running : MovementType.Walking;

        if (_inputActions.Player.Attack.IsPressed())
        {
            if (_projectileLauncher != null)
            {
                Vector3 target = GetRangedTarget();
                Transform spawn = _projectileLauncher.ProjectileSpawn != null ? _projectileLauncher.ProjectileSpawn : _rangedSpawnPoint;
                _projectileLauncher.TryGetLaunchDirection(spawn.position, target, _accuracy, out Vector3 dir);
                _rangedSpawnPoint.LookAt(_rangedSpawnPoint.position + dir, Vector3.up);
                _projectileLauncher.TryShoot(dir);
            }
            if (_meleeWeapon != null) _meleeWeapon.Swing();
        }

        if (_inputActions.Player.Attack.WasReleasedThisFrame() && _projectileLauncher != null)
            _projectileLauncher.ReleaseTrigger();

        if (_inputActions.Player.Jump.WasPressedThisFrame()) _jumpPressedTime = Time.realtimeSinceStartup;
    }
    private void FixedUpdate()
    {
        _movement.CheckGrounded(ref _transform);
        _movement.OnMove(_inputActions.Player.Move.ReadValue<Vector2>(), ref _transform, _currentMovementType);
        if (Time.realtimeSinceStartup - _jumpPressedTime <= _jumpBufferTime && _movement.OnJump())
        _jumpPressedTime = float.NegativeInfinity;
    }
    private void LateUpdate()
    {
        OnHandleCamera();
    }
    private void ToggleMouse()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
    private float _xRot, _yRot;
    private void OnHandleCamera()
    {
        _cameraTransform.localPosition =
        Vector3.Lerp(_cameraTransform.localPosition, _cameraOffset, _cameraMoveSpeed * _dt);
        Vector2 mouseDelta = _inputActions.Player.Look.ReadValue<Vector2>();
        _xRot -= mouseDelta.y * _mouseSensitivity * _dt;
        _yRot += mouseDelta.x * _mouseSensitivity * _dt;
        _xRot = Mathf.Clamp(_xRot, _cameraLowerLimit, _cameraUpperLimit);
        _yRot = Mathf.Repeat(_yRot, 360f);
        _cameraPivotTransform.localRotation = Quaternion.Euler(_xRot, 0f, 0f);
        _transform.rotation = Quaternion.Euler(0f, _yRot, 0f);
    }
    private readonly RaycastHit[] _hits = new RaycastHit[10];
    private Vector3 GetRangedTarget()
    {
        int count = Physics.RaycastNonAlloc(_cameraTransform.position, _cameraTransform.forward,
        _hits, 100f, _targetLayers, QueryTriggerInteraction.Ignore);
        Vector3 hit;
        if (count > 0)
        {
            System.Array.Sort(_hits, 0, count, RaycastHitDistanceComparer.Instance);
            hit = _hits[0].point;
        }
        else hit = _cameraTransform.position + 100f * _cameraTransform.forward;
        System.Array.Clear(_hits, 0, _hits.Length);
        Debug.DrawLine(_cameraTransform.position, hit, Color.red, 5f);
        return hit;
    }
    private sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public static readonly RaycastHitDistanceComparer Instance = new();
        public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
    }
}
