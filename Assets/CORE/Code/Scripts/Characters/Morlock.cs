using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PatrolMovingAl))]
[RequireComponent(typeof(VisionCone2D))]

public class Morlock : Enemy
{
    private PatrolMovingAl _movingSystem;
    private VisionCone2D _vision2D;


    private protected override void Start()
    {
        base.Start();

        _movingSystem = GetComponent<PatrolMovingAl>();
        _vision2D = GetComponent<VisionCone2D>();

        _attackSystem.ActionBeforeAttack += ActionBeforeAttackHandler;
        _attackSystem.ActionAfterAttack += ActionAfterAttackHandler;
    }

    private void OnDestroy()
    {
        _attackSystem.ActionBeforeAttack -= ActionBeforeAttackHandler;
        _attackSystem.ActionAfterAttack -= ActionAfterAttackHandler;
    }

    private protected override void FixedUpdate()
    {
        base.FixedUpdate();

        _vision2D.CheckVision();

        Transform playerTransform = null;
        if (_vision2D.DetectedObjects.Count > 0) playerTransform = _vision2D.DetectedObjects[0].transform;

        if (playerTransform is not null && playerTransform.tag == playerTag)
        {
            _movingSystem.FastMoveOn = true;
            _animator.speed = 2f;
        }
        else
        {
            _movingSystem.FastMoveOn = false;
            _animator.speed = 1f;
        }

        _animator.SetFloat("speed", _movingSystem.Speed);
    }

    private void ActionBeforeAttackHandler()
    {
        _movingSystem.Pause = true;
        _animator.SetFloat("speed", 0);
    }

    private void ActionAfterAttackHandler()
    {
        _movingSystem.Pause = false;
    }
}
