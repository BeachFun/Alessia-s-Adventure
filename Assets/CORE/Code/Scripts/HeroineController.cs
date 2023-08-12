using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroineController : MonoBehaviour
{
    private enum MovementState { Idle, Running, Jumping, Falling };

    [Header("Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _lowJumpMultiplier;
    [SerializeField] private float _fallMultiplier;
    [SerializeField] private Vector2 _moveDirection;

    [Header("Ground Check")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _checkRadius;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Vector3 _checkerOffset;

    [Header("State")]
    [SerializeField] private MovementState _movementState;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        
        _isGrounded = Physics2D.OverlapCircle(transform.position + _checkerOffset, _checkRadius, _layerMask);

        GravityHandler();

        SpriteFlip(_moveDirection);
        UpdateAnimationState();

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) 
        {
            Jump();
        }

        if(Input.GetButtonDown("Fire1"))
        {
            _animator.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        Move(_moveDirection);
    }

    private void UpdateAnimationState()
    {
        if(_moveDirection.x > 0)
        {
            _movementState = MovementState.Running;
        }
        else if(_moveDirection.x < 0)
        {
            _movementState = MovementState.Running;
        }
        else
        {
            _movementState = MovementState.Idle;
        }

        if(_rigidbody.velocity.y > 0.1f)
        {
            _movementState = MovementState.Jumping;
        }
        else if(_rigidbody.velocity.y < -0.1f)
        {
            _movementState = MovementState.Falling;
        }

        _animator.SetInteger("State", (int)_movementState);
    }

    private void SpriteFlip(Vector2 direction)
    {
        if (direction.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    private void GravityHandler()
    {
        if(_rigidbody.velocity.y < -0.1f)
        {   
            _rigidbody.velocity += _fallMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
        else if(_rigidbody.velocity.y > 0.1f && !Input.GetKey(KeyCode.Space)) 
        {
            _rigidbody.velocity += _lowJumpMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
    }

    private void Move(Vector2 direction)
    {
        _rigidbody.velocity = new Vector2(direction.x * _moveSpeed, _rigidbody.velocity.y);
    }

    private void Jump()
    {
        _rigidbody.velocity = _jumpForce * Vector2.up;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + _checkerOffset, _checkRadius);
    }
}
