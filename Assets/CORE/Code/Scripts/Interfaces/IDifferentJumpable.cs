public interface IDifferentJumpable : IJumpable
{
    float MinJumpPower { get; }
    float MaxJumpPower { get; }
    float PowerStep { get; }

    void JumpPowerUp();
    void JumpPowerDown();
}