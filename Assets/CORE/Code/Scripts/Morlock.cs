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

    private Vector2 lookDirection;
    private bool _isAggressiveWalk;
    private bool _movementOn = true;
    private bool _attackOn = true;
    private bool _isGround;
    private MorlockState _state;


    void FixedUpdate()
    {
        if (!_isBusy && _isGround)
        {
            if (!_attackOn && _state == MorlockState.Idle) _movementOn = true;

            // Cheking environment
            RaycastHit2D hitBottom, hitFar; // поверхность под персонажем и чуть дальше персонажа
            Vector2 bottomBodyPoint, forwardBodyPoint;

            bottomBodyPoint = new Vector2(this.transform.position.x, collider.bounds.min.y);

            if (moveDirection == MoveDirection.Left)
            {
                forwardBodyPoint = new Vector2(collider.bounds.min.x, this.transform.position.y);
                hitFar = Physics2D.Raycast(forwardBodyPoint, new Vector2(-0.7f, -1f), rayDistanceFarGroundCheck);
            }
            else
            {
                forwardBodyPoint = new Vector2(collider.bounds.max.x, this.transform.position.y);
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
                    _isAggressiveWalk = false;
                    animator.speed = 1f;
                }
                else if (playerTransform.tag == "Player")
                {
                    _isAggressiveWalk = true;
                    animator.speed = 2f;
                }
            }

            // Moving
            if (_movementOn)
            {
                hitBottom = Physics2D.Raycast(bottomBodyPoint, Vector2.down, rayDistanceGroundCheck);

                if (hitFar.collider is null || !UnityUtils.Approximately(hitBottom.point, hitFar.point, SnapAxis2D.Y))
                {
                    StartCoroutine(SlowRotate());
                }
                else
                {
                    if (moveDirection == MoveDirection.Left)
                        physic.velocity = new Vector2(-(moveSpeed * Time.fixedDeltaTime), 0);
                    else
                        physic.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, 0);

                    if (_isAggressiveWalk)
                        physic.velocity *= 2; // если увидел игрока, то ускоренное движение к игроку

                    animator.SetFloat("speed", physic.velocity.magnitude / Time.fixedDeltaTime);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _isGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _isGround = false;
        }
    }


    private IEnumerator Attack(Transform transform, Vector2 forwardPoint)
    {
        _isBusy = true;
        _attackOn = false;

        animator.SetFloat("speed", 0);
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(hurtSpeed);

        Transform playerTransform = Physics2D.Raycast(forwardPoint, lookDirection, attackDistance).transform;
        if (playerTransform is not null && playerTransform.tag == "Player")
        {
            transform.GetComponent<Heroine>().Hurt(atk);
        }

        _isBusy = false;

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
