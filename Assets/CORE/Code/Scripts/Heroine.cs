using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class Heroine : MonoBehaviour
{
    private enum AnimationStates { Idle = 0, JumpStart = 10, Jumping = 11, ComboAttack = 20, Sliding = 30, Grab = 40, Dieth = 50 }
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
    [SerializeField] private float lowJumpMultiplier;
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

        switch (_state)
        {
            case AnimationStates.Idle:
                Movement();
                IdleHandler();
                break;
            case AnimationStates.JumpStart:
            case AnimationStates.Jumping:
                Movement();
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
            case AnimationStates.Dieth:
                DiethHandler();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (moveOn) Move(_moveDirection);
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


    private void Movement()
    {
        _moveDirection.x = Input.GetAxisRaw("Horizontal");

        if (physic.velocity.y < -0.1f)
        {
            physic.velocity += fallMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
        else if (physic.velocity.y > 0.1f && !Input.GetKey(KeyCode.Space))
        {
            physic.velocity += lowJumpMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }

        SpriteFlip(_moveDirection);
    }


    #region Обработчики нажатий на клавиши

    private void IdleHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            State = AnimationStates.JumpStart;
            moveOn = false;
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

    private void JumpStartHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            State = AnimationStates.JumpStart;
            moveOn = false;
            return;
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

    private void DiethHandler()
    {

    }

    #endregion


    #region Методы для анимаций

    private void Jump()
    {
        physic.velocity = jumpForce * Vector2.up;
    }

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
        dagger.Throw(this.LookDirection, throwSpeed);
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
}
