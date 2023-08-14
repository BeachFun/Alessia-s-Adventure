using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Morlock : Enemy
{
    private enum MoveDirection { Left, Right }


    [Space] [Header("Morlock Settings")]

    [Header("Attack system")]
    [SerializeField] private float aggressionDistance;
    [SerializeField] private float attackDistance;

    [Header("Moving system")]
    [SerializeField] private bool isGround;
    [SerializeField] private float rayDistanceGroundCheck = 1f;
    [SerializeField] private float rayDistanceFarGroundCheck = 2f;
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Left;
    [SerializeField] private float rotateSeconds;

    private bool raycastOn = true;
    private bool isAggressiveWalk;


    private void FixedUpdate()
    {
        if (raycastOn)
        {
            // Movement, check ground
            if (isGround)
            {
                RaycastHit2D hitBottom, hitFar; // поверхность под персонажем и чуть дальше персонажа
                Vector2 bottomPoint = new Vector2(this.transform.position.x, collider2D.bounds.min.y);
                Vector2 forwardPoint;

                if (moveDirection == MoveDirection.Left)
                {
                    forwardPoint = new Vector2(collider2D.bounds.min.x, this.transform.position.y);
                    hitFar = Physics2D.Raycast(forwardPoint, new Vector2(-0.7f, -1f), rayDistanceFarGroundCheck);
                }
                else
                {
                    forwardPoint = new Vector2(collider2D.bounds.max.x, this.transform.position.y);
                    hitFar = Physics2D.Raycast(forwardPoint, new Vector2(0.7f, -1f), rayDistanceFarGroundCheck);
                }

                hitBottom = Physics2D.Raycast(bottomPoint, Vector2.down, rayDistanceGroundCheck);

                if (hitFar.collider is null || Mathf.Approximately(hitBottom.point.y, hitFar.point.y))
                {
                    StartCoroutine(SlowRotate());
                }
                else
                {
                    if (moveDirection == MoveDirection.Left)
                        physic.velocity = new Vector2(-(moveSpeed * Time.fixedDeltaTime), 0);
                    else
                        physic.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, 0);

                    animator.SetFloat("speed", physic.velocity.magnitude / Time.fixedDeltaTime);
                }
            }

            // Attacked, check player

            Transform playerTransform;

            if (moveDirection == MoveDirection.Left)
                playerTransform = Physics2D.Raycast(this.transform.position, Vector2.left, aggressionDistance).transform;
            else
                playerTransform = Physics2D.Raycast(this.transform.position, Vector2.right, aggressionDistance).transform;

            if (playerTransform is null)
                isAggressiveWalk = false;
            else if (playerTransform.tag == "Player")
                isAggressiveWalk = true;
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


    private IEnumerator Attack(Transform transform)
    {
        yield return null;
    }

    private IEnumerator SlowRotate()
    {
        raycastOn = false;
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

        raycastOn = true;
    }
}
