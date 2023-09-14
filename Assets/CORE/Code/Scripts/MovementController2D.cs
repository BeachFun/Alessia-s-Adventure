using UnityEngine;

[RequireComponent(typeof(CharacterController))]

[System.Serializable]
public class MovementController2D : MonoBehaviour, IJumpable
{
    private protected const float gravity = 9.8f;

    [SerializeField] private protected bool isPaused = true;
    [SerializeField] private protected bool useGravity = true;
    [SerializeField] private protected float jumpForce = 0;

    private protected Vector2 _verticalVelocity;
    private protected CharacterController _character;


    public bool Pause
    {
        get => isPaused;
        set => isPaused = value;
    }
    public float JumpForce
    {
        get => jumpForce;
    }


    private protected virtual void Start()
    {
        _character = GetComponent<CharacterController>();
    }

    private protected virtual void FixedUpdate()
    {
        if (Pause) return;

        if (useGravity)
        {
            if (!_character.isGrounded)
            {
                _verticalVelocity.y -= gravity * Time.fixedDeltaTime;
            }
            else
            {
                _verticalVelocity.y = 0f;
            }

            _character.Move(_verticalVelocity * Time.fixedDeltaTime);
        }
    }

    public void TeleportTo(Vector2 position)
    {
        transform.position = position;
    }

    public void Move(Vector2 mv)
    {
        if (!isPaused) _character.Move(mv);
    }

    public virtual void Jump()
    {
        if (_character.isGrounded && useGravity)
        {
            _verticalVelocity.y += JumpForce;
        }
    }
}
