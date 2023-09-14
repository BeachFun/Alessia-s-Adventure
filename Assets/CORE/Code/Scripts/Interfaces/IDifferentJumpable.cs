public interface IDifferentJumpable : IJumpable
{
    float JumpPower { get; }
    float MaxJumpPower { get; }
    float PowerStep { get; }

    void JumpPowerUp();
    void JumpPowerDown();
}