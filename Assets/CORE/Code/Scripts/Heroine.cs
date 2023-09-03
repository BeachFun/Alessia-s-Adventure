using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class Heroine : MonoBehaviour
{
    private enum AnimationStates { Idle = 0, JumpReady = 10, Jumping = 11, ComboAttack = 20, Sliding = 30, Grab = 40, Dieth = 50 }
    private enum AttackType { A, B, C, D, InJump }
    private enum InputMode { On, Off }


    [Header("Characteristics")]
    [SerializeField] private int hp = 5;
    [SerializeField] private int atk = 3;
    [SerializeField] private int def = 1;

    [Header("Movement Settings")]
    [SerializeField] private bool moveOn = true;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float jumpForce;
    [SerializeField] [Range(.005f, .025f)] private float jumpMultiplyStepByFixedFrame = .01f;
    [SerializeField] [Range(1f, 3f)] private float jumpMultiplyLimit = 1.5f;
    [SerializeField] private float fallMultiplier;

    [Header("Attack Syatem")]
    [SerializeField] private float attackInJumpDistance;
    [SerializeField] private float attackADistance;
    [SerializeField] private float attackBDistance;
    [SerializeField] private float attackCDistance;
    [SerializeField] private float attackDDistance;
    [SerializeField] private float throwSpeed;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 checkerOffset;

    [Header("Other Settings")]
    [SerializeField] private bool inputOn = true;
    [SerializeField] private AnimationStates _state = AnimationStates.Idle;

    [Header("Heroine class Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D physic;

    [Header("References")]
    [SerializeField] private Dagger daggerPrefab;


    private Vector2 _moveDirection;
    private float _jumpPower;


    private AnimationStates State
    {
        get => _state;
        set
        {
            _state = value;
            animator.SetInteger("animationState", (int)_state);
        }
    }

    /// <summary>
    /// Направление взгляда
    /// </summary>
    public Vector2 LookDirection
    {
        get => spriteRenderer.flipX ? Vector2.left : Vector2.right;
    }


    private void Update()
    {
        if (!inputOn) return;

        isGrounded = Physics2D.OverlapCircle(transform.position + checkerOffset, checkRadius, layerMask);

        if (!isGrounded) State = AnimationStates.Jumping;

        switch (_state)
        {
            case AnimationStates.Idle:
                MovementInputHandler();
                IdleHandler();
                break;
            case AnimationStates.Jumping:
                MovementInputHandler();
                JumpingHandler();
                break;
            case AnimationStates.ComboAttack:
                ComboAttackHandler();
                break;
            case AnimationStates.Sliding:
                SlidingHandler();
                break;
            case AnimationStates.Grab:
                GrabingHandler();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (moveOn) Move(_moveDirection);

        switch (_state)
        {
            case AnimationStates.JumpReady:
                MovementInputHandler();
                JumpReadyHandler();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + checkerOffset, checkRadius);
    }


    private void SpriteFlip(Vector2 direction)
    {
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }



    #region Обработчики нажатий на клавиши

    private void MovementInputHandler()
    {
        _moveDirection.x = Input.GetAxisRaw("Horizontal");

        if (physic.velocity.y < -0.1f)
        {
            physic.velocity += fallMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }

        SpriteFlip(_moveDirection);
    }

    private void IdleHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            State = AnimationStates.JumpReady;
            StopMove();
            moveOn = false;
            _jumpPower = 1f;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            State = AnimationStates.ComboAttack;
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            StopMove();
            animator.SetTrigger("throwAttack");
        }
    }

    private void JumpReadyHandler()
    {
        if (!Input.GetKey(KeyCode.Space) || !isGrounded)
        {
            State = AnimationStates.Jumping;
            moveOn = true;
            Jump();
        }
        else if (_jumpPower < jumpMultiplyLimit)
        {
            _jumpPower += jumpMultiplyStepByFixedFrame;
        }
    }

    private void JumpingHandler()
    {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump_Mid" && isGrounded)
        {
            State = AnimationStates.Idle;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("attackInJump");
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("throwAttack");
        }
    }

    private void ComboAttackHandler()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0))
        {
            State = AnimationStates.Idle;
        }
    }

    private void SlidingHandler()
    {

    }

    private void GrabingHandler()
    {

    }

    #endregion


    #region Методы для анимаций

    private void Attack(AttackType attackType)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, this.LookDirection, attackType switch
        {
            AttackType.A => attackADistance,
            AttackType.B => attackBDistance,
            AttackType.C => attackCDistance,
            AttackType.D => attackDDistance,
            _ => attackInJumpDistance
        });

        hits.Where(e => e.transform.gameObject.GetComponent<Enemy>() is not null)
            .Select(e => e.transform.gameObject.GetComponent<Enemy>())
            .ToList()
            .ForEach(e => e.Hurt(atk));
    }

    /// <summary>
    /// Бросает меч в направлении куда смотрит
    /// </summary>
    public void TrhrowAttack()
    {
        if (!(State == AnimationStates.Idle || State == AnimationStates.Jumping)) return;

        Dagger dagger = Instantiate(daggerPrefab, this.transform.position, new Quaternion(0, 0, 0, 0));
        dagger.Throw(this.LookDirection, throwSpeed + (State == AnimationStates.Jumping ? Mathf.Abs(_moveDirection.x) * moveSpeed : 0));
    }

    /// <summary>
    /// Включает/Отключает функцию обработки вводных данных
    /// </summary>
    private void SetInputOn(InputMode mode) => inputOn = mode == InputMode.On ? true : false; 

    #endregion


    /// <summary>
    /// Получение урона
    /// </summary>
    public void Hurt(int attackDamage)
    {
        StopAllCoroutines();
    }

    private void Move(Vector2 direction)
    {
        float moveX = _moveDirection.x * moveSpeed;
        physic.velocity = new Vector2(moveX, physic.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(moveX));
    }

    public void StopMove()
    {
        _moveDirection.x = 0;
        physic.velocity = new Vector2(0, physic.velocity.y);
        animator.SetFloat("speed", 0f);
    }

    private void Jump()
    {
        physic.velocity = jumpForce * Vector2.up * _jumpPower;
    }

}
