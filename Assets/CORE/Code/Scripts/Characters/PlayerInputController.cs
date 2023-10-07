using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private float horizontalInput;
    private bool jumpInput;
    private bool attackInput;
    private bool throwInput;
    private bool slideInput;

    public float HorizontalInput
    {
        get => horizontalInput;
    }
    public bool JumpInput
    {
        get => jumpInput;
    }
    public bool AttackInput
    {
        get => attackInput;
    }
    public bool ThrowInput
    {
        get => throwInput;
    }
    public bool SlideInput
    {
        get => slideInput;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetKey(KeyCode.Space);
        attackInput = Input.GetKeyDown(KeyCode.Mouse1);
        throwInput = Input.GetKeyDown(KeyCode.F);
        slideInput = Input.GetKeyDown(KeyCode.LeftShift);
    }
}
