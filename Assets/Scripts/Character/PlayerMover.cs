using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private const string IsWalkCommand = "isWalk";
    private const string IsJumpCommand = "isJump";
    private const string SpeedAnimationValue = "Speed";
    private const string AngleAnimationValue = "Angle";


    [SerializeField] private Player _player;
    [SerializeField] private float _rotationSpeed = 350f;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _strafeSpeed = 3f;
    [SerializeField] private float _jumpSpeed = 7;
    [SerializeField] private float _gravityFactor = 2;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _distanceToGround = 0.1f;
    [SerializeField] private float _verticalVelocityMultiplier = 10f;

    private Vector2 _moveDirection;
    private Vector3 _verticalVelocity;

    private void Update()
    {
        _moveDirection = _player.PlayerInput.Player.Move.ReadValue<Vector2>();

        Move();
    }

    private void Move()
    {
        Vector3 forward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;

        Vector3 playerSpeed = forward * _moveDirection.y * _speed + right * _moveDirection.x * _strafeSpeed;
        _player.Animator.SetBool(IsWalkCommand, playerSpeed.sqrMagnitude > 0.1f);

        Vector3 localPlayerSpeed = transform.InverseTransformVector(playerSpeed);

        _player.Animator.SetFloat(SpeedAnimationValue, localPlayerSpeed.z);
        _player.Animator.SetFloat(AngleAnimationValue, localPlayerSpeed.x);

        Rotate(forward, playerSpeed);

        if (_player.CharacterController.isGrounded)
        {
            if (_player.PlayerInput.Player.Jump.IsPressed())
            {
                _verticalVelocity = Vector3.up * _jumpSpeed;
                _player.Animator.SetBool(IsJumpCommand, true);
            }
            else
            {
                _verticalVelocity = Vector3.down * _verticalVelocityMultiplier;
            }

            _player.CharacterController.Move((playerSpeed + _verticalVelocity) * Time.deltaTime);
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.down, _distanceToGround))
            {
                _player.Animator.SetBool(IsJumpCommand, false);
            }

            Vector3 horizontalVelocity = _player.CharacterController.velocity;
            horizontalVelocity.y = 0;

            _verticalVelocity += Physics.gravity * Time.deltaTime * _gravityFactor;
            _player.CharacterController.Move((horizontalVelocity + _verticalVelocity) * Time.deltaTime);
        }
    }

    private void Rotate(Vector3 forward, Vector3 playerSpeed)
    {
        if (playerSpeed != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(forward, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}
