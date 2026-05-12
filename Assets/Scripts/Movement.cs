using UnityEngine;
public enum MovementType { Crouching, Walking, Running }
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float[] _moveSpeeds;
    [SerializeField] private float _moveAcceleration = 6f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 0.1f;
    [SerializeField] private float _groundCheckRadius = 0.5f;
    [SerializeField] private Transform _groundCheckPoint;
    private bool _isGrounded = true;
    private Rigidbody _rb;
    private Collider _collider;
    public Vector3 FlatVelocity => new(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<Collider>();
        _rb.freezeRotation = true;
    }
    public void CheckGrounded(ref Transform transform)
    {
        Vector3 checkCenter = GetGroundCheckCenter();
        _isGrounded = Physics.CheckSphere(
            checkCenter,
            _groundCheckRadius,
            _groundLayer,
            QueryTriggerInteraction.Ignore);
    }
    private Vector3 GetGroundCheckCenter()
    {
        if (_groundCheckPoint != null) return _groundCheckPoint.position;
        if (_collider == null) return _rb.position + Vector3.down * _groundCheckDistance;

        Bounds bounds = _collider.bounds;
        return new Vector3(bounds.center.x, bounds.min.y + _groundCheckDistance, bounds.center.z);
    }
    public void OnMove(Vector2 moveDirection, ref Transform transform, MovementType movementType = MovementType.Walking)
    {
        float moveSpeed = _moveSpeeds[(int)movementType];
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
        Vector3 targetVelocity = (right * moveDirection.x + forward * moveDirection.y) * moveSpeed;
        Vector3 acceleration = _moveAcceleration * (targetVelocity - FlatVelocity);
        acceleration.y = 0f;
        _rb.AddForce(acceleration, ForceMode.Acceleration);
    }
    public bool OnJump()
    {
        if (!_isGrounded) return false;
        _rb.linearVelocity = FlatVelocity;
        _rb.AddForce(Vector3.up * Mathf.Sqrt(_jumpHeight * 2 * Physics.gravity.magnitude), ForceMode.VelocityChange);
        _isGrounded = false;
        return true;
    }
}
