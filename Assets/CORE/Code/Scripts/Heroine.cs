using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class Heroine : MonoBehaviour
{
    private enum AnimatorStates { Idle = 0, JumpReady = 10, Jumping = 11, AttackA = 20, AttackB = 21, AttackC = 22, AttackD = 23, Sliding = 30, Grab = 40, Dieth = 50 }
    private enum AttackType { A, B, C, D, InJump }
    private enum InputMode { On, Off }
    private enum AnimationStates { True, False }


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

    [Header("Heroine class Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D physic;

    [Header("References")]
    [SerializeField] private Dagger daggerPrefab;


    private Vector2 _moveDirection;
    private float _jumpPower;
    private bool _isNextCombo;
    private AnimationStates _isComboAttackEnded = AnimationStates.False;
    private bool _inputOn = true;
    private AnimatorStates _state = AnimatorStates.Idle;

    private AnimatorStates State
    {
        get => _state;
        set
        {
            _state = value;
            animator.SetInteger("animationState", (int)_state);
        }
    }

    /// <summary>
    /// Обработка вводных данных
    /// </summary>
    private InputMode InputOn
    {
        get => _inputOn ? InputMode.On : InputMode.Off;
        set => _inputOn = value == InputMode.On ? true : false;
    }

    /// <summary>
    /// Свойство для аниматора. Указывает на завершение анимации атаки в комбо атаках
    /// </summary>
    private AnimationStates IsComboAttackEnded
    {
        get => _isComboAttackEnded;
        set => _isComboAttackEnded = value;
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
        if (!_inputOn) return;

        isGrounded = Physics2D.OverlapCircle(transform.position + checkerOffset, checkRadius, layerMask);

        if (!isGrounded) State = AnimatorStates.Jumping;

        switch (_state)
        {
            case AnimatorStates.Idle:
                MovementInputHandler();
                IdleHandler();
                break;
            case AnimatorStates.Jumping:
                MovementInputHandler();
                JumpingHandler();
                break;
            case AnimatorStates.AttackA:
            case AnimatorStates.AttackB:
            case AnimatorStates.AttackC:
            case AnimatorStates.AttackD:
                ComboAttackHandler();
                break;
            case AnimatorStates.Sliding:
                SlidingHandler();
                break;
            case AnimatorStates.Grab:
                GrabingHandler();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (moveOn) Move(_moveDirection);

        switch (_state)
        {
            case AnimatorStates.JumpReady:
                MovementInputHandler();
                JumpReadyHandler();
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + checkerOffset, checkRadius);
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
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            State = AnimatorStates.JumpReady;
            StopMove();
            moveOn = false;
            _jumpPower = 1f;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            State = AnimatorStates.AttackA;
            StopMove();
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
            State = AnimatorStates.Jumping;
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
            State = AnimatorStates.Idle;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _isNextCombo = true;
        }

        if (_isComboAttackEnded == AnimationStates.False) return;

        if (!_isNextCombo) State = AnimatorStates.Idle;
        else State = State switch
        {
            AnimatorStates.AttackA => AnimatorStates.AttackB,
            AnimatorStates.AttackB => AnimatorStates.AttackC,
            AnimatorStates.AttackC => AnimatorStates.AttackD,
            _ => AnimatorStates.AttackA,
        };

        _isNextCombo = false;
        _isComboAttackEnded = AnimationStates.False;
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
    private void TrhrowAttack()
    {
        if (!(State == AnimatorStates.Idle || State == AnimatorStates.Jumping)) return;

        Dagger dagger = Instantiate(daggerPrefab, this.transform.position, new Quaternion(0, 0, 0, 0));
        dagger.Throw(this.LookDirection, throwSpeed + (State == AnimatorStates.Jumping ? Mathf.Abs(_moveDirection.x) * moveSpeed : 0));
    }

    #endregion


    private void Move(Vector2 direction)
    {
        float moveX = _moveDirection.x * moveSpeed;
        physic.velocity = new Vector2(moveX, physic.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(moveX));
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

    private void Jump()
    {
        physic.velocity = jumpForce * Vector2.up * _jumpPower;
    }


    /// <summary>
    /// Получение урона
    /// </summary>
    public void Hurt(int attackDamage)
    {
        StopAllCoroutines();
    }

    public void StopMove()
    {
        _moveDirection.x = 0;
        physic.velocity = new Vector2(0, physic.velocity.y);
        animator.SetFloat("speed", 0f);
    }
}
