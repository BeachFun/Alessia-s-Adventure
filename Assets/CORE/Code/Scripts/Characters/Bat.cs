using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PatrolMovingAl))]
[RequireComponent(typeof(ShootController2D))]

public class Bat : Enemy
{
    private PatrolMovingAl _movingSystem;
    private ShootController2D _shootSystem;


    private protected override void Start()
    {
        base.Start();

        _movingSystem = GetComponent<PatrolMovingAl>();
        _shootSystem = GetComponent<ShootController2D>();

        _attackSystem.ActionBeforeAttack += ActionBeforeAttackHandler;
        _attackSystem.ActionAfterAttack += ActionAfterAttackHandler;
        _shootSystem.ActionBeforeShoot += ActionBeforeShootHandler;
        _shootSystem.ActionAfterShoot += ActionAfterShootHandler;
    }

    public void Shoot()
    {
        _shootSystem.Shoot();
    }


    private void ActionBeforeAttackHandler()
    {
        _shootSystem.ShootOn = false;
        _movingSystem.Pause = true;
    }

    private void ActionAfterAttackHandler()
    {
        _shootSystem.ShootOn = true;
        _movingSystem.Pause = false;
    }

    private void ActionBeforeShootHandler()
    {
        _attackSystem.AttackOn = false;
        _movingSystem.Pause = true;
    }

    private void ActionAfterShootHandler()
    {
        _attackSystem.AttackOn = true;
        _movingSystem.Pause = false;
    }
}
