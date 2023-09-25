using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AttackSystem2D))]
[RequireComponent(typeof(ComboAttackSystem2D))]
[RequireComponent(typeof(HeroMovementController2D))]
[RequireComponent(typeof(ShootSystem2D))]

public class Player : Character
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

    [SerializeField] private int daggerCount = 5;
    [Header("Energy System")]
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private float energy = 50;
    [Tooltip("Energy recovery speed per second")]
    [SerializeField] private float energyRecoverySpeed = 5;
    [SerializeField] private int attackEnergy = 10;
    [SerializeField] private int throwEnergy = 10;

    private bool _inputOn = true;
    private Vector2 _horizonatalVelocity;
    private AnimatorStates _state = AnimatorStates.Idle;

    private protected Animator _animator;
    private protected AttackSystem2D _attackController;
    private protected ComboAttackSystem2D _comboAttackController;
    private protected ShootSystem2D _shootController;
    private protected HeroMovementController2D _movementController;


    public int HP
    {
        get => hp;
        private set
        {
            hp = value;
            Messenger<int>.Broadcast(GameEvents.PLAYER_HEALTH_CHANGED, value);
        }
    }
    public int DaggerCount
    {
        get => daggerCount;
        private set
        {
            daggerCount = value;
            Messenger<int>.Broadcast(GameEvents.PLAYER_DAGGER_CHANGED, value);
        }
    }
    public float Energy
    {
        get => energy;
        private set
        {
            energy = value;
            Messenger<float, float>.Broadcast(GameEvents.PLAYER_ENERGY_CHANGED, value, MaxEnergy);
        }
    }
    public float MaxEnergy
    {
        get => maxEnergy;
    }

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


    private void Awake()
    {
        Messenger.AddListener(GameEvents.GAME_INDICATORS_STARTED, GameIndicatorsStartedHandler);
    }

    private protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _attackController = GetComponent<AttackSystem2D>();
        _comboAttackController = GetComponent<ComboAttackSystem2D>();
        _shootController = GetComponent<ShootSystem2D>();
        _movementController = GetComponent<HeroMovementController2D>();

        _comboAttackController.ComboEnded += ActionAfterComboAttack;
        _shootController.ActionAfterShoot += ResumeMove;
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvents.GAME_INDICATORS_STARTED, GameIndicatorsStartedHandler);

        _comboAttackController.ComboEnded -= ActionAfterComboAttack;
        _shootController.ActionAfterShoot -= ResumeMove;
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
                ThrowAttack();
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
        // Energy Recovery
        if (_state == AnimatorStates.Idle || 
            _state == AnimatorStates.Jumping || 
            _state == AnimatorStates.Grab)
        {
            if (Energy < MaxEnergy)
            {
                Energy += energyRecoverySpeed * Time.fixedDeltaTime;
                if (Energy > MaxEnergy) Energy = MaxEnergy;
            }
        }

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

        if (_horizonatalVelocity.x == 0)
        {
            _movementController.StopMovement(SnapAxis2D.X);
        }
        else
        {
            _movementController.Move(_horizonatalVelocity * Time.fixedDeltaTime);
        }

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

    public override void Dieth()
    {
        State = AnimatorStates.Dieth;
    }


    public void StopMove()
    {
        _animator.SetFloat("speed", 0f);

        _movementController.Pause = true;
        _movementController.StopMovement(SnapAxis2D.X);
    }

    public void ResumeMove()
    {
        _movementController.Pause = false;
    }

    public override void Attack()
    {
        if (Energy < attackEnergy) return;
        Energy -= attackEnergy;

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
        if (DaggerCount == 0 || Energy < throwEnergy) return;
        Energy -= throwEnergy;
        DaggerCount--;

        if (!(State == AnimatorStates.Idle || State == AnimatorStates.Jumping)) return;

        _shootController.Throw(LookDirection, 1 + (State == AnimatorStates.Jumping ? Mathf.Abs(_horizonatalVelocity.x) : 0f));
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

        _attackController.IsRotated = _spriteRenderer.flipX;
        _comboAttackController.IsRotated = _spriteRenderer.flipX;
    }

    private void ActionAfterComboAttack()
    {
        State = AnimatorStates.Idle;
        ResumeMove();
    }

    private void GameIndicatorsStartedHandler()
    {
        // For initialized GUI indicators
        DaggerCount = DaggerCount;
        Energy = Energy;
        HP = HP;
    }
}
