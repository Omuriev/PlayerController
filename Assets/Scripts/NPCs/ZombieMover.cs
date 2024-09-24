using UnityEngine;

public class ZombieMover : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _gravityFactor = 1.2f;
    [SerializeField] private float _groundCheckRayLength = 0.5f;
    [SerializeField] private float _stepHeight = 0.3f;
    [SerializeField] private float _drag = 5f;
    [SerializeField] private float _speed = 30f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _upperStepPoint;
    [SerializeField] private Transform _lowerStepPoint;
    [SerializeField] private float _speedMultiplier = 10f;
    [SerializeField] private float _speedStairsMultiplier = 30f;
    [SerializeField] private float _slopeSpeedMultiplier = 6f;

    private Vector3 _targetPosition;
    private bool _isGround;
    private bool _isMove = true;
    private RaycastHit _slopeHit;

    private void Update()
    {
        _isGround = Physics.Raycast(transform.position, Vector3.down, _groundCheckRayLength, _groundLayer);

        if (_isMove)
        {
            Move();
        }
        else if (_rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            _isMove = true;
        }
            
        if (_isGround)
        {
            _rigidbody.drag = _drag;
        }
        else if (!_isGround)
        { 
            _rigidbody.drag = 0;
            _rigidbody.AddForce(Physics.gravity * _gravityFactor, ForceMode.Force);
        }
    }

    public void Move(Transform target)
    {
        _targetPosition = target.position;
        Move();
    }

    public void StopMove()
    {
        _rigidbody.velocity = Vector3.zero;
        _isMove = false;
    }

    private void Move()
    {
        Vector3 slopeMoveDirection;
        Vector3 targetDirection = (_targetPosition - transform.position).normalized;
        targetDirection.y = 0;

        if (OnSlope() && _isGround)
        {
            slopeMoveDirection = Vector3.ProjectOnPlane(targetDirection, _slopeHit.normal).normalized;

            _rigidbody.useGravity = false;
            _rigidbody.AddForce(slopeMoveDirection * _speed * _slopeSpeedMultiplier + Physics.gravity * _gravityFactor, ForceMode.Force);
        }
        else if (IsDetectStairs())
        {
            _rigidbody.useGravity = false;
            _rigidbody.AddForce(Vector3.up * _speedStairsMultiplier * _stepHeight * _speed, ForceMode.Force);
        }
        else
        {
            _rigidbody.useGravity = true;
            _rigidbody.AddForce(targetDirection * _speedMultiplier * _speed, ForceMode.Force);
        }

        LimitVelocity();
    }

    private void LimitVelocity()
    {
        Vector3 currentVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

        if (currentVelocity.magnitude > _speed)
        {
            Vector3 maximalVelocity = currentVelocity.normalized * _speed;
            _rigidbody.velocity = new Vector3(maximalVelocity.x, _rigidbody.velocity.y, maximalVelocity.z);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _groundCheckRayLength))
        {
            if (_slopeHit.normal != Vector3.up)
                return true;
        }

        return false;
    }

    private bool IsDetectStairs()
    {
        float lowerRayLength = 0.3f;
        float upperRayLength = 0.5f;
        float lowerRayCorrectedLength = lowerRayLength + 3f - _lowerStepPoint.localPosition.z;

        if (Physics.Raycast(_lowerStepPoint.position, _lowerStepPoint.forward, out _, lowerRayLength, _groundLayer))
        {
            return Physics.Raycast(_upperStepPoint.position, _upperStepPoint.forward, out _, upperRayLength, _groundLayer) == false;
        }

        return false;
    }
}
