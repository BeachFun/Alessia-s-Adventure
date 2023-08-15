using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Morlock : Enemy
{
    private enum MorlockState { Idle, Walk, Rotate, Attack, Hit }


    [Space] [Header("Morlock Settings")]

    [Header("Attack system")]
    [SerializeField] private float aggressionDistance;
    [SerializeField] private float attackDistance;

    [Header("Movement system")]
    [SerializeField] private float rayDistanceGroundCheck = 1f;
    [SerializeField] private float rayDistanceFarGroundCheck = 2f;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Left;
    [SerializeField] private float rotateSeconds;
    [SerializeField] private bool isAggressiveWalk;

    [Header("Other")]
    [SerializeField] private bool isGround;

    [Header("Morlock class Components")]
    [SerializeField] private BoxCollider2D boxCollider2D;

    private Vector2 lookDirection;
    private bool _movementOn = true;
    private bool _attackOn = true;
    private MorlockState _state;


    protected override void Start()
    {
        base.Start();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!isBusy && isGround)
        {
            if (!_attackOn && _state == MorlockState.Idle) _movementOn = true;

            // Cheking environment
            RaycastHit2D hitBottom, hitFar; // поверхность под персонажем и чуть дальше персонажа
            Vector2 bottomBodyPoint, forwardBodyPoint;

            bottomBodyPoint = new Vector2(this.transform.position.x, boxCollider2D.bounds.min.y);

            if (moveDirection == MoveDirection.Left)
            {
                forwardBodyPoint = new Vector2(boxCollider2D.bounds.min.x, this.transform.position.y);
                hitFar = Physics2D.Raycast(forwardBodyPoint, new Vector2(-0.7f, -1f), rayDistanceFarGroundCheck);
            }
            else
            {
                forwardBodyPoint = new Vector2(boxCollider2D.bounds.max.x, this.transform.position.y);
                hitFar = Physics2D.Raycast(forwardBodyPoint, new Vector2(0.7f, -1f), rayDistanceFarGroundCheck);
            }


            // Attacked, check player
            lookDirection = moveDirection == MoveDirection.Left ? Vector2.left : Vector2.right;
            Transform playerTransform = Physics2D.Raycast(forwardBodyPoint, lookDirection, attackDistance).transform;

            if (playerTransform is not null && playerTransform.tag == "Player")
            {
                _movementOn = false;

                if (_attackOn) StartCoroutine(Attack(playerTransform, forwardBodyPoint));
            }
            else
            {
                playerTransform = Physics2D.Raycast(forwardBodyPoint, lookDirection, aggressionDistance).transform;
                if (playerTransform is null)
                {
                    isAggressiveWalk = false;
                    animator.speed = 1f;
                }
                else if (playerTransform.tag == "Player")
                {
                    isAggressiveWalk = true;
                    animator.speed = 2f;
                }
            }

            // Moving
            if (_movementOn)
            {
                hitBottom = Physics2D.Raycast(bottomBodyPoint, Vector2.down, rayDistanceGroundCheck);

                if (hitFar.collider is null || !Mathf.Approximately(Mathf.Round(hitBottom.point.y), Mathf.Round(hitFar.point.y)))
                {
                    StartCoroutine(SlowRotate());
                }
                else
                {
                    if (moveDirection == MoveDirection.Left)
                        physic.velocity = new Vector2(-(moveSpeed * Time.fixedDeltaTime), 0);
                    else
                        physic.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, 0);

                    if (isAggressiveWalk)
                        physic.velocity *= 2; // если увидел игрока, то ускоренное движение к игроку

                    animator.SetFloat("speed", physic.velocity.magnitude / Time.fixedDeltaTime);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isGround = false;
        }
    }


    private IEnumerator Attack(Transform transform, Vector2 forwardPoint)
    {
        isBusy = true;
        _attackOn = false;

        animator.SetFloat("speed", 0);
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(hurtSpeed);

        Transform playerTransform = Physics2D.Raycast(forwardPoint, lookDirection, attackDistance).transform;
        if (playerTransform is not null && playerTransform.tag == "Player")
        {
            transform.GetComponent<HeroineController>().Hurt(atk);
        }

        isBusy = false;

        yield return new WaitForSeconds(timeBetweenAttacks);

        _attackOn = true;
    }

    private IEnumerator SlowRotate()
    {
        _state = MorlockState.Rotate;
        _movementOn = false;

        animator.SetFloat("speed", 0f);

        yield return new WaitForSeconds(rotateSeconds / 1.5f);

        if (moveDirection == MoveDirection.Left)
        {
            spriteRenderer.flipX = false;
            moveDirection = MoveDirection.Right;
        }
        else
        {
            spriteRenderer.flipX = true;
            moveDirection = MoveDirection.Left;
        }

        yield return new WaitForSeconds(rotateSeconds / 3f);

        _movementOn = true;
        _state = MorlockState.Idle;
    }
}
