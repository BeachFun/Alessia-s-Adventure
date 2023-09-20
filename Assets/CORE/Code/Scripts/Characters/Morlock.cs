using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PatrolMovingAl))]
[RequireComponent(typeof(VisionCone2D))]

public class Morlock : Enemy
{
    [SerializeField] private protected string playerTag = "Player";

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

        _vision2D.IsRatated = _spriteRenderer.flipX;
        _vision2D.CheckVision();

        Transform playerTransform = null;
        if (_vision2D.DetectedObjects.Any(e => e.tag == playerTag))
        {
            playerTransform = _vision2D.DetectedObjects.First(e => e.tag == playerTag).transform;
        }

        if (playerTransform is not null)
        {
            _movingSystem.FastMoveOn = true;
            _movingSystem.IsRotating = false;
            _animator.speed = 2f;
        }
        else
        {
            _movingSystem.FastMoveOn = false;
            _movingSystem.IsRotating = true;
            _animator.speed = 1f;
        }

        _animator.SetFloat("speed", _movingSystem.Speed);
    }


    private void ActionBeforeAttackHandler()
    {
        _movingSystem.StopMovement(SnapAxis2D.X);
        _movingSystem.Pause = true;
        _animator.SetFloat("speed", 0);
    }

    private void ActionAfterAttackHandler()
    {
        _movingSystem.Pause = false;
    }
}
