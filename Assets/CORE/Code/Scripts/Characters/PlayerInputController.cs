using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private float _horizontalInput;
    private bool _jumpInput;
    private bool _attackInput;
    private bool _throwInput;
    private bool _slideInput;

    public float HorizontalInput
    {
        get => _horizontalInput;
    }
    public bool JumpInput
    {
        get => _jumpInput;
    }
    public bool AttackInput
    {
        get => _attackInput;
    }
    public bool ThrowInput
    {
        get => _throwInput;
    }
    public bool SlideInput
    {
        get => _slideInput;
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _jumpInput = Input.GetKey(KeyCode.Space);
        _attackInput = Input.GetKeyDown(KeyCode.Mouse1);
        _throwInput = Input.GetKeyDown(KeyCode.F);
        _slideInput = Input.GetKeyDown(KeyCode.LeftShift);
    }
}
