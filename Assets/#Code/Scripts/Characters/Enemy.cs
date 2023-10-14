using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MovementController2D))]
[RequireComponent(typeof(AutoAttackController2D))]

public class Enemy : Character
{
    private protected bool _isBusy;
    private protected Animator _animator;
    private protected MovementController2D _movement;
    private protected AutoAttackController2D _attackSystem;

    private protected Bool IsBusy
    {
        get => _isBusy ? Bool.True : Bool.False;
        set => _isBusy = value == Bool.True ? true : false;
    }


    private protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _movement = GetComponent<MovementController2D>();
        _attackSystem = GetComponent<AutoAttackController2D>();
    }

    private protected virtual void FixedUpdate()
    {
        _attackSystem.IsRotated = _spriteRenderer.flipX;
    }

    public override void Attack()
    {
        _attackSystem.Attack();
    }

    public override void Hurt(int damage)
    {
        hp = damage > def ? hp - (damage - def) : hp;

        if (hp <= 0) Death();
        else _animator.SetTrigger("hit");
    }

    public override void Flip()
    {
        base.Flip();

        _attackSystem.IsRotated = _spriteRenderer.flipX;
    }

    public override void Death()
    {
        base.Death();

        Messenger.Broadcast(GameEvents.ENEMY_KILLED);
    }
}

public enum Bool
{
    True,
    False
}
