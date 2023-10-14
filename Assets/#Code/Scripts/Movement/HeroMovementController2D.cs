using UnityEngine;

public class HeroMovementController2D : MovementController2D, IDifferentJumpable
{
    [Header("Jump Settings")]
    [SerializeField] private protected float minJumpPower = 1f;
    [SerializeField] private protected float maxJumpPower = 1.5f;
    [SerializeField] private protected float powerStep = 0.035f;

    private bool _isJumpPowerChanging;
    private bool _isJumpPowerIncreasing;
    private float _jumpPower = 1f;


    public float JumpPower
    {
        get => _jumpPower;
        private set => _jumpPower = value;
    }
    public float MinJumpPower
    {
        get => minJumpPower;
        private set => minJumpPower = value;
    }
    public float MaxJumpPower
    {
        get => maxJumpPower;
    }
    public float PowerStep
    {
        get => powerStep;
    }


    /// <summary>
    /// Реализует динамическое изменение силы прыжка. Реагирует на паузу скрипта.
    /// </summary>
    private protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Pause) return;

        if (_isJumpPowerChanging)
        {
            float lastJumpPower = JumpPower;

            if (_isJumpPowerIncreasing) JumpPowerDown();
            else JumpPowerUp();

            if (lastJumpPower == JumpPower) _isJumpPowerIncreasing = !_isJumpPowerIncreasing;
        }
    }

    public virtual void JumpPowerDown()
    {
        JumpPower = JumpPower - powerStep <= 1f ? 1f : JumpPower - powerStep;
    }

    public virtual void JumpPowerUp()
    {
        JumpPower = JumpPower + powerStep >= MaxJumpPower ? MaxJumpPower : JumpPower + powerStep;
    }

    public override void Jump()
    {
        if (IsGrounded && UseGravity)
        {
            _physic.velocity = Vector2.up * JumpForce * JumpPower;
            JumpPower = minJumpPower;
        }
    }

    public void StartChangingJumpPower()
    {
        _isJumpPowerChanging = true;
    }

    public void EndChangingJumpPower()
    {
        _isJumpPowerChanging = false;
    }
}
