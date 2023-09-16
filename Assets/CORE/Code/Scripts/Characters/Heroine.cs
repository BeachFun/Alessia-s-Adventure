using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AttackSystem2D))]
[RequireComponent(typeof(ComboAttackSystem2D))]
[RequireComponent(typeof(HeroMovementController2D))]
[RequireComponent(typeof(ShootSystem2D))]

public class Heroine : Character
{
    private enum AnimatorStates
    {
        Idle = 0,
        JumpReady = 10,
        Jumping = 11,
        Combo = 20,
        Sliding = 30,
        Grab = 40,
        Dieth = 50
    }
    private enum InputMode { On, Off }


    private bool _inputOn = true;
    private Vector2 _horizonatalVelocity;
    private AnimatorStates _state = AnimatorStates.Idle;

    private protected Animator _animator;
    private protected AttackSystem2D _attackController;
    private protected ComboAttackSystem2D _comboAttackController;
    private protected ShootSystem2D _shootController;
    private protected HeroMovementController2D _movementController;



    private AnimatorStates State
    {
        get => _state;
        set
        {
            _state = value;
            _animator.SetInteger("state", (int)_state);
        }
    }

    /// <summary>
    /// Включение/Отключение управления персонажем
    /// </summary>
    private InputMode InputOn
    {
        get => _inputOn ? InputMode.On : InputMode.Off;
        set => _inputOn = value == InputMode.On ? true : false;
    }

    /// <summary>
    /// Направление взгляда
    /// </summary>
    public Vector2 LookDirection
    {
        get => _spriteRenderer.flipX ? Vector2.left : Vector2.right;
    }


    private protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _attackController = GetComponent<AttackSystem2D>();
        _comboAttackController = GetComponent<ComboAttackSystem2D>();
        _shootController = GetComponent<ShootSystem2D>();
        _movementController = GetComponent<HeroMovementController2D>();
    }

    private void Update()
    {
        if (!_inputOn) return;

        if (!_movementController.IsGrounded) State = AnimatorStates.Jumping;

        if (State == AnimatorStates.Idle)
        {
            MovementInputHandler();

            if (Input.GetKey(KeyCode.Space) && _movementController.IsGrounded)
            {
                StopMove();
                State = AnimatorStates.JumpReady;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Attack();
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                StopMove();
                _animator.SetTrigger("throwAttack");
            }
        }

        if (State == AnimatorStates.Jumping)
        {
            MovementInputHandler();

            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump_Mid" && _movementController.IsGrounded)
            {
                State = AnimatorStates.Idle;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Attack();
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                ThrowAttack();
            }
        }

        if (State == AnimatorStates.Combo)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Attack();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_inputOn) return;

        if (_state == AnimatorStates.JumpReady)
        {
            MovementInputHandler();

            if (!Input.GetKey(KeyCode.Space) || !_movementController.IsGrounded)
            {
                State = AnimatorStates.Jumping;
                _movementController.Pause = false;
                _movementController.Jump();
            }
            else
            {
                _movementController.JumpPowerUp();
            }
        }
    }

    private void MovementInputHandler()
    {
        _horizonatalVelocity.x = Input.GetAxisRaw("Horizontal");

        _movementController.Move(_horizonatalVelocity * Time.deltaTime);
        _animator.SetFloat("speed", Mathf.Abs(_horizonatalVelocity.x));

        SpriteFlip(_horizonatalVelocity);
    }


    /// <summary>
    /// Получение урона
    /// </summary>
    public override void Hurt(int attackDamage)
    {
        if (hp - (attackDamage - def) <= 0)
        {
            hp = 0;
            State = AnimatorStates.Dieth;
        }
        else
        {
            hp = hp - (attackDamage - def);
            _animator.SetTrigger("hit");
        }

        Debug.Log(hp);
    }

    public void StopMove()
    {
        _animator.SetFloat("speed", 0f);
        _movementController.Pause = true;
    }

    public override void Attack()
    {
        if (State == AnimatorStates.Idle)
        {
            StopMove();
            State = AnimatorStates.Combo;
            _comboAttackController.Attack();
            return;
        }
        if (State == AnimatorStates.Combo)
        {
            _comboAttackController.NextAttack();
        }
        if (State == AnimatorStates.Jumping)
        {
            _animator.SetTrigger("attackInJump");
        }
    }

    // Бросает меч в направлении куда смотрит
    public void ThrowAttack()
    {
        if (!(State == AnimatorStates.Idle || State == AnimatorStates.Jumping)) return;

        _shootController.ReleaseProjectile(LookDirection, 1 + (State == AnimatorStates.Jumping ? Mathf.Abs(_horizonatalVelocity.x) : 0f));
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
}
