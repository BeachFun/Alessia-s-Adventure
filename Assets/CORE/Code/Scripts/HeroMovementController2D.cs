using UnityEngine;

public class HeroMovementController2D : MovementController2D, IDifferentJumpable
{
    [SerializeField] private protected float moveSpeed = 0f;
    [SerializeField] private protected float minJumpPower = 1f;
    [SerializeField] private protected float maxJumpPower = 1.5f;
    [SerializeField] private protected float powerStep = 0.05f;

    private bool _isJumpPowerChanging;
    private bool _isJumpPowerIncreasing;
    private float _jumpPower;

    public bool IsGrounded
    {
        get => _character.isGrounded;
    }
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

    /// <summary>
    /// Передвижение на расстояние с учетом скорости
    /// </summary>
    public override void Move(Vector2 mv)
    {
        base.Move(mv * moveSpeed);
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
        if (_character.isGrounded && useGravity)
        {
            _verticalVelocity.y += JumpForce * JumpPower;
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
