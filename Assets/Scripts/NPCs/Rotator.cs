using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _rotationSpeed;

    private float _angleStep = 1f;

    public void RotateToTarget(Transform target)
    {
        Vector3 targetDirection = (target.position - transform.position).normalized;
        targetDirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(targetDirection);
        float angle = rotation.eulerAngles.y;
        float difference = Mathf.Abs(transform.rotation.eulerAngles.y - angle);

        if (difference > _angleStep)
            _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime));
    }
}
