using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Bat : Enemy
{
    private enum BatState { Idle, Move, Attack, Rotation }


    [Space][Header("Bat Settings")]

    [Header("Moving system")]
    [SerializeField] private Vector2 finishPos;
    [SerializeField] private float barrierDistance = 1;
    [SerializeField] private float rotateSeconds;

    [Header("Attack system")]
    [SerializeField] private bool attackOn;
    [SerializeField] private float shootSpeed = 3;
    [SerializeField] private EnergyBall energyBallPrefab;

    [Header("Bat class Components")]
    [SerializeField] private BoxCollider2D boxCollider2D;

    [Header("References")]
    [Tooltip("Нужно установить ссылку на игрока на сцене, а не на префам игрока")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    private bool _isMoveBack;
    private Vector2 _startPos;
    private Vector2 _destination;
    private BatState _state;
    private bool _attackOn = true;


    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        Initialize();
    }

    void FixedUpdate()
    {
        physic.rotation = 0;

        if (_state == BatState.Idle)
        {
            Vector2 currentPosition = this.transform.position;
            Vector2 moveDirection = (_destination - currentPosition).normalized;
            physic.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;

            //Transform barrierTransform = Physics2D.Raycast(currentPosition, moveDirection).transform;
            //if (barrierTransform is not null)
            //{
            //    float distance = Vector2.Distance(currentPosition, barrierTransform.position);
            //    if (distance < barrierDistance)
            //    {
            //        SlowRotate();
            //        return;
            //    }
            //}

            if (UnityUtils.Approximately(currentPosition, _destination))
                SlowRotate();

            if (attackOn & _attackOn)
            {
                Vector2 attackDirection = (playerRigidbody.position - physic.position).normalized;
                Transform playerTransform = Physics2D.Raycast(physic.position + attackDirection, attackDirection).transform;

                if (playerTransform is not null && playerTransform.tag == "Player")
                {
                    Attack(attackDirection);
                }
            }
        }
    }


    private void Initialize()
    {
        _startPos = this.transform.position;
        _destination = finishPos;

        Vector2 direction = (_destination - _startPos).normalized;
        if (direction.x > 0) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
    }


    private void SlowRotate()
    {
        _state = BatState.Rotation;
        physic.velocity = Vector2.zero;

        StartCoroutine(SlowRotateCoroutine());
    }

    private IEnumerator SlowRotateCoroutine()
    {
        yield return new WaitForSeconds(rotateSeconds / 1.5f);

        _destination = _isMoveBack ? finishPos : _startPos;
        _isMoveBack = !_isMoveBack;
        spriteRenderer.flipX = _destination.x < this.transform.position.x ? true : false;

        yield return new WaitForSeconds(rotateSeconds / 3f);

        _state = BatState.Idle;
    }

    private void Attack(Vector2 direction)
    {
        _state = BatState.Attack;
        _attackOn = false;
        physic.velocity = Vector2.zero;

        StartCoroutine(AttackCoroutine(direction));
    }

    private IEnumerator AttackCoroutine(Vector2 direction)
    {
        animator.SetTrigger("attack1");
        float length = UnityUtils.AnimationPlayDuration(animator);

        yield return new WaitForSeconds(length);

        EnergyBall energyBall = Instantiate(energyBallPrefab, this.transform.position, new Quaternion(0f, 0f, 0f, 0f));
        energyBall.power = atk;
        energyBall.AddForce(direction, shootSpeed);

        _state = BatState.Idle;

        yield return new WaitForSeconds(timeBetweenAttacks);

        _attackOn = true;
    }
}
