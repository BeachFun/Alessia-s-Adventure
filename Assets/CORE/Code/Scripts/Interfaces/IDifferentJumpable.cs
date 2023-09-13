public interface IDifferentJumpable : IJumpable
{
    float JumpPower { get; set; }

    void JumpPowerUp();
    void JumpPowerDown();
}