using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

[System.Serializable]
public class MovementController2D : MonoBehaviour, IForceReceiver2D, IJumpable
{
    [SerializeField] private protected bool isPaused;
    [SerializeField] private protected float moveSpeed;
    [SerializeField] private protected float jumpForce = 0;
    [SerializeField] private protected float fallSpeedMultiplier = 1f;
    [Header("Ground Check")]
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 checkerOffset;


    private protected bool _isGrounded;
    private protected Rigidbody2D _physic;
    private protected Collider2D _collider;


    public bool Pause
    {
        get => isPaused;
        set
        {
            isPaused = value;
            StopMovement(SnapAxis2D.All);
        }
    }
    public float JumpForce
    {
        get => jumpForce;
    }
    public bool IsGrounded
    {
        get => _isGrounded;
    }
    public bool UseGravity
    {
        get => _physic.gravityScale != 0;
    }

    private protected virtual void Start()
    {
        _physic = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private protected virtual void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(transform.position + checkerOffset, checkRadius, layerMask);
    }

    private protected virtual void FixedUpdate()
    {
        if (_physic.velocity.y < -0.1f)
        {
            _physic.velocity += fallSpeedMultiplier * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;

        Gizmos.DrawWireSphere(transform.position + checkerOffset, checkRadius);
    }


    public virtual void Jump()
    {
        if (IsGrounded && UseGravity)
        {
            _physic.velocity += Vector2.up * JumpForce;
        }
    }


    public void TeleportTo(Vector2 position)
    {
        if (!isPaused) transform.position = position;
    }

    public virtual void MoveTo(Vector2 mv)
    {
        if (!isPaused) _physic.position += mv;
    }

    public void Move(Vector2 direction)
    {
        if (isPaused) return;

        float moveX = direction.x != 0f ? direction.x * moveSpeed : _physic.velocity.x;
        float moveY = direction.y != 0f ? direction.y * moveSpeed : _physic.velocity.y;

        _physic.velocity = new Vector2(moveX, moveY);
    }

    public void AddForce(Vector2 force, ForceMode2D mode)
    {
        if (!isPaused) _physic.AddForce(force * moveSpeed, mode);
    }

    public void AddForceAtPosition(Vector2 force, Vector2 position, ForceMode2D mode)
    {
        if (!isPaused) _physic.AddForceAtPosition(force * moveSpeed, position, mode);
    }


    public void StopMovement(SnapAxis2D axis)
    {
        if (axis == SnapAxis2D.X) _physic.velocity = new Vector2(0f, _physic.velocity.y);
        if (axis == SnapAxis2D.Y) _physic.velocity = new Vector2(_physic.velocity.x, 0f);
        if (axis == SnapAxis2D.All) _physic.velocity = Vector2.zero;
    }
}
