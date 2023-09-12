using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AttackController2D))]

public class Enemy : Character
{
    [SerializeField] private protected string playerTag = "Player";

    private protected bool _isBusy;
    private protected Animator _animator;
    private protected AttackController2D _attackSystem;


    private protected Bool IsBusy
    {
        get => _isBusy ? Bool.True : Bool.False;
        set => _isBusy = value == Bool.True ? true : false;
    }


    private protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _attackSystem = GetComponent<AttackController2D>();
    }

    public override void Attack()
    {
        _attackSystem.Attack();
    }

    public override void Hurt(int damage)
    {
        hp = damage > def ? hp - (damage - def) : hp;

        if (hp < 0) Dieth();
        else _animator.SetTrigger("hit");
    }
}

public enum Bool
{
    True,
    False
}
