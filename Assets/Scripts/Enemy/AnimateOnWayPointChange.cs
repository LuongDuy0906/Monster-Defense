using UnityEngine;

public class AnimateOnWayPointChange : MonoBehaviour
{
    private Animator _animator;
    private Vector3 _lastTarget;
    private Enemy _enemy;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
        if(_animator == null) return;
        _lastTarget = _enemy.TargetPosition;
    }

    void Update()
    {
        Vector3 currentTarget = _enemy.TargetPosition;
        if(currentTarget != _lastTarget)
        {
            UpdateAnimation(currentTarget);
        }
    }

    private void UpdateAnimation(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            _animator.SetFloat("moveX", direction. x > 0 ? 1 : -1);
            _animator.SetFloat("moveY", 0);
        } else
        {
            _animator.SetFloat("moveX", 0);
            _animator.SetFloat("moveY", direction. y > 0 ? 1 : -1);
        }
    }
}
