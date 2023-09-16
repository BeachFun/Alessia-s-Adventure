using UnityEngine;

[RequireComponent(typeof(Animator))]

public class ComboAttackSystem2D : AttackSystem2D
{
    // Запечатывание поля
    [SerializeField] private bool isLooping;
    [SerializeField] private string AnimatorPropertyName = "comboAttackStates";

    private bool _isNextCombo;
    private protected Animator _animator;


    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (!AttackOn || !_attackOn) return;
        _attackOn = false;

        _attackZoneIndex = 0;

        _animator.SetInteger(AnimatorPropertyName, (int)attackData[_attackZoneIndex].Id);
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
                _attackZoneIndex = _attackZoneIndex + 1 > attackData.Length - 1 ? 0 : _attackZoneIndex + 1;
                _animator.SetInteger(AnimatorPropertyName, (int)attackData[_attackZoneIndex].Id);
            }
            else if (_attackZoneIndex + 1 <= attackData.Length - 1)
            {
                _attackZoneIndex++;
                _animator.SetInteger(AnimatorPropertyName, (int)attackData[_attackZoneIndex].Id);
            }
        }

        _isNextCombo = false;
    }
}
