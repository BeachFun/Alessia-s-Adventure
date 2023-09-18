using UnityEngine;
using Action = System.Action;

[RequireComponent(typeof(Animator))]

public class AutoAttackController2D : AttackSystem2D
{
    private Animator _animator;

    public event Action ActionBeforeAttack;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!AttackOn || !_attackOn) return;

        for (int i = 0; i < attackData.Length; i++)
        {
            Vector2 direction = IsRotated ? Vector2.left : Vector2.right;

            RaycastHit2D[] hits = CheckZone(i);

            if (hits.Length > 0)
            {
                ActionBeforeAttack?.Invoke();
                _attackOn = false;
                _attackZoneIndex = i;
                Attack();
                return;
            }
        }

        _attackZoneIndex = -1;
    }

    public void Attack()
    {
        if (!_attackOn) return;

        if (_attackZoneIndex != -1) _animator.SetTrigger(attackData[_attackZoneIndex].NameAnimatorProperty);
    }

    // Метод для системы Mecanim. Вызывать в момент удара.
    private protected void Damage2()
    {
        base.Damage();
    }
}
