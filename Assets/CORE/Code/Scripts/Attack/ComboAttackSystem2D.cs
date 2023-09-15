using System.Linq;
using UnityEngine;
using Action = System.Action;

[RequireComponent(typeof(Animator))]

public class ComboAttackSystem2D : AttackSystem2D
{
    [SerializeField] private AttackData[] attackData;
    [SerializeField] private bool isLooping;

    private bool _isNextCombo;
    private int _attackIndex;

    private protected Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (!_attackOn) return;
        _attackOn = false;

        _attackIndex = 0;
        _attackZoneIndex = 0;

        animator.SetInteger("comboAttackStates", (int)attackData[_attackIndex].Id);
    }

    public void NextAttack()
    {
        _isNextCombo = true;
    }

    // Метод для системы Mecanim. Вызывать в конце анимации удара.
    private void CheckInput()
    {
        if (_isNextCombo)
        {
            if (isLooping)
            {
                _attackIndex = _attackIndex + 1 > attackData.Length - 1 ? 0 : _attackIndex + 1;
                animator.SetInteger("comboAttackStates", (int)attackData[_attackIndex].Id);
            }
            else if (_attackIndex + 1 <= attackData.Length - 1)
            {
                _attackIndex++;
                animator.SetInteger("comboAttackStates", (int)attackData[_attackIndex].Id);
            }
        }

        _isNextCombo = false;
    }
}
