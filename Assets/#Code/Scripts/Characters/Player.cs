using System.Linq;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AttackSystem2D))]
[RequireComponent(typeof(ComboAttackSystem2D))]
[RequireComponent(typeof(HeroMovementController2D))]
[RequireComponent(typeof(ShootSystem2D))]
[RequireComponent(typeof(PlayerInputController))]

public partial class Player : Character
{
    [SerializeField] private int daggerCount = 5;
    [Header("Energy System")]
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private float energy = 50;
    [Tooltip("Energy recovery speed per second")]
    [SerializeField] private float energyRecoverySpeed = 5;
    [SerializeField] private int attackEnergy = 10;
    [SerializeField] private int throwEnergy = 10;
    [Header("Sliding System")]
    [SerializeField] private float minSlidingTime = 0.7f;
    [SerializeField] private float slidingSpeed = 8f;

    private bool _inputOn = true;
    private float _height;
    private Vector2 _horizonatalVelocity;
    private AnimatorStates _state = AnimatorStates.Idle;

    private Collider2D _collider;
    private protected Animator _animator;
    private protected AttackSystem2D _attackController;
    private protected ComboAttackSystem2D _comboAttackController;
    private protected ShootSystem2D _shootController;
    private protected HeroMovementController2D _movementController;
    private protected PlayerInputController _inputController;


    public int HP
    {
        get => hp;
        private set
        {
            hp = value;
            Messenger<int, int>.Broadcast(GameEvents.PLAYER_HEALTH_CHANGED, value, MaxHP);
        }
    }
    public int DaggerCount
    {
        get => daggerCount;
        set
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
    public float MaxEnergy { get => maxEnergy; }
    public int MaxHP { get => maxHP; }

    private AnimatorStates CurrentState
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

        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _attackController = GetComponent<AttackSystem2D>();
        _comboAttackController = GetComponent<ComboAttackSystem2D>();
        _shootController = GetComponent<ShootSystem2D>();
        _movementController = GetComponent<HeroMovementController2D>();
        _inputController = GetComponent<PlayerInputController>();

        _height = _collider.bounds.max.y - _collider.bounds.min.y;

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

        if (!_movementController.IsGrounded)
        {
            _animator.SetFloat("speed", 0f);
            CurrentState = AnimatorStates.Jumping;
        }

        if (CurrentState == AnimatorStates.Idle)
        {
            MovementInputHandler();

            if (_inputController.SlideInput && Mathf.Abs(_horizonatalVelocity.x) > .3f)
            {
                StartCoroutine(SlidingRountine());
                CurrentState = AnimatorStates.Sliding;
                return;
            }

            if (_inputController.JumpInput && _movementController.IsGrounded)
            {
                StopMove();
                CurrentState = AnimatorStates.JumpReady;
                return;
            }

            if (_inputController.AttackInput)
            {
                Attack();
                return;
            }

            if (_inputController.ThrowInput)
            {
                StopMove();
                if (ThrowAttack() == -1) ResumeMove();
            }
        }

        if (CurrentState == AnimatorStates.Jumping)
        {
            MovementInputHandler();

            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump_Mid" && _movementController.IsGrounded)
            {
                CurrentState = AnimatorStates.Idle;
                return;
            }

            if (_inputController.AttackInput)
            {
                Attack();
                return;
            }

            if (_inputController.ThrowInput)
            {
                ThrowAttack();
            }
        }

        if (CurrentState == AnimatorStates.Combo)
        {
            if (_inputController.AttackInput)
            {
                Attack();
            }
        }
    }

    private void FixedUpdate()
    {
        // Energy Recovery
        if (CurrentState == AnimatorStates.Idle ||
            CurrentState == AnimatorStates.Jumping ||
            CurrentState == AnimatorStates.Grab)
        {
            if (Energy < MaxEnergy)
            {
                Energy += energyRecoverySpeed * Time.fixedDeltaTime;
                if (Energy > MaxEnergy) Energy = MaxEnergy;
            }
        }

        if (CurrentState == AnimatorStates.JumpReady)
        {
            MovementInputHandler();

            if (!_inputController.JumpInput || !_movementController.IsGrounded)
            {
                CurrentState = AnimatorStates.Jumping;
                _movementController.Pause = false;
                _movementController.Jump();
            }
            else
            {
                _movementController.JumpPowerUp();
            }
        }

        if (CurrentState == AnimatorStates.Sliding)
        {
            _movementController.Move(LookDirection * slidingSpeed, false);
        }
    }

    private void MovementInputHandler()
    {
        _horizonatalVelocity.x = _inputController.HorizontalInput;

        Move(_horizonatalVelocity);
    }


    private void StopMove()
    {
        _animator.SetFloat("speed", 0f);

        _movementController.Pause = true;
        _movementController.StopMovement(SnapAxis2D.X);
    }

    private void ResumeMove()
    {
        _movementController.Pause = false;
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
        CurrentState = AnimatorStates.Idle;
        ResumeMove();
    }

    private void GameIndicatorsStartedHandler()
    {
        // For initialized GUI indicators
        DaggerCount = DaggerCount;
        Energy = Energy;
        HP = HP;
    }

    private IEnumerator SlidingRountine()
    {
        yield return new WaitForSeconds(minSlidingTime);

        while (CurrentState == AnimatorStates.Sliding)
        {
            Vector2 origin = new(transform.position.x, _collider.bounds.min.y);

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.up, _height)
                .Where(e => e.collider.tag != this.tag || e.collider.tag != "MainCamera")
                .ToArray();

            if (hits.Length == 0) CurrentState = AnimatorStates.Idle;

            hits = Physics2D.RaycastAll(origin, LookDirection, _height)
                .Where(e => e.collider.tag != this.tag || e.collider.tag != "MainCamera")
                .ToArray();

            if (hits.Length != 0) CurrentState = AnimatorStates.Idle;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }


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
}

public partial class Player
{
    public void Move(Vector2 horizonatalVelocity)
    {
        if (horizonatalVelocity.x == 0)
        {
            _movementController.StopMovement(SnapAxis2D.X);
        }
        else
        {
            _movementController.Move(horizonatalVelocity * Time.fixedDeltaTime);
        }

        _animator.SetFloat("speed", Mathf.Abs(horizonatalVelocity.x));

        SpriteFlip(horizonatalVelocity);
    }

    public override void Attack()
    {
        if (Energy < attackEnergy) return;
        Energy -= attackEnergy;

        if (CurrentState == AnimatorStates.Idle)
        {
            StopMove();
            CurrentState = AnimatorStates.Combo;
            _comboAttackController.Attack();
            return;
        }
        if (CurrentState == AnimatorStates.Combo)
        {
            _comboAttackController.NextAttack();
        }
        if (CurrentState == AnimatorStates.Jumping)
        {
            _animator.SetTrigger("attackInJump");
        }
    }

    public int ThrowAttack()
    {
        // Бросает меч в направлении куда смотрит
        if (DaggerCount == 0 || Energy < throwEnergy) return -1;
        Energy -= throwEnergy;
        DaggerCount--;

        if (!(CurrentState == AnimatorStates.Idle || CurrentState == AnimatorStates.Jumping)) return 0;

        _shootController.Throw(LookDirection, 1 + (CurrentState == AnimatorStates.Jumping ? Mathf.Abs(_horizonatalVelocity.x) : 0f));

        return 1;
    }

    public override void Hurt(int attackDamage)
    {
        if (hp - (attackDamage - def) <= 0)
        {
            HP = 0;
            Death();
        }
        else
        {
            HP = HP - (attackDamage - def);
            _animator.SetTrigger("hit");
        }

        Debug.Log(HP);
    }

    public void Heal(int hp)
    {
        GameManagers.Audio?.PlaySound(AudioClipPool.Instance["Heal"], 0.2f);
        HP = hp + HP > MaxHP ? MaxHP : HP + hp;
    }

    public override void Death()
    {
        _animator.SetFloat("speed", 0f);
        CurrentState = AnimatorStates.Dieth;

        Messenger.Broadcast(GameEvents.LEVEL_FAILED);
    }
}
