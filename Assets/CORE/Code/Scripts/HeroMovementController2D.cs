using UnityEngine;

public class HeroMovementController2D : MovementController2D, IDifferentJumpable
{
    [SerializeField] private protected float jumpPower = 1f;
    [SerializeField] private protected float maxJumpPower = 1.5f;
    [SerializeField] private protected float powerStep = 0.05f;

    private bool _isJumpPowerChanging;
    private bool _isJumpPowerIncreasing;

    public float JumpPower
    {
        get => jumpPower;
        private set => jumpPower = value;
    }
    public float MaxJumpPower
    {
        get => maxJumpPower;
    }
    public float PowerStep
    {
        get => powerStep;
    }

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
        if (_character.isGrounded && useGravity)
        {
            _verticalVelocity.y += JumpForce * jumpPower;
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
