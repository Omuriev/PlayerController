using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _maxDistanceToObject;
    [SerializeField] private ZombieMover _zombieMover;
    [SerializeField] private Rotator _rotator;

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, _target.position);

        _rotator.RotateToTarget(_target);

        if (_maxDistanceToObject < distance)
        {
            _zombieMover.Move(_target);
        }
        else
        {
            _zombieMover.StopMove();
        }
    }
}
