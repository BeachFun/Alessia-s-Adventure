using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PatrolMovingAl))]

public class Morlock : Enemy
{
    [Space] [Header("Morlock Settings")]

    [Header("Attack system")]
    [SerializeField] private float aggressionDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private Vector2 attackZoneSize;

    [Header("Components")]
    [SerializeField] private PatrolMovingAl movingSystem;

    private bool _attackOn = true;
    private Vector2 _lookDirection;
    private Vector2 _forwardBodyPoint;


    private void FixedUpdate()
    {
        if (_isBusy) return;

        animator.SetFloat("speed", movingSystem.Speed);

        if (!_attackOn) return;

        Transform playerTransform = FindTransform(attackDistance);

        if (playerTransform is not null && playerTransform.tag == "Player")
        {
            movingSystem.IsOn = false;
            _attackOn = false;
            animator.SetFloat("speed", 0);
            animator.SetTrigger("attack");
        }
        else
        {
            playerTransform = FindTransform(aggressionDistance);

            if (playerTransform is null)
            {
                movingSystem.FastMoveOn = false;
                animator.speed = 1f;
            }
            else if (playerTransform.tag == "Player")
            {
                movingSystem.FastMoveOn = true;
                animator.speed = 2f;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector2 center = new Vector2(0f, this.transform.position.y);
        center.x = spriteRenderer.flipX ? _collider.bounds.min.x - attackDistance : _collider.bounds.max.x + attackDistance;

        Gizmos.DrawWireCube(center, attackZoneSize);
    }

    private Transform FindTransform(float distance)
    {
        // TODO: Придумать более достойное название для метода

        _forwardBodyPoint = new Vector2(0f, this.transform.position.y);
        _forwardBodyPoint.x = spriteRenderer.flipX ? _collider.bounds.min.x : _collider.bounds.max.x;

        _lookDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        IEnumerable<RaycastHit2D> hits = Physics2D.RaycastAll(_forwardBodyPoint, _lookDirection, distance)
            .Where(e => e.transform.tag != this.tag);

        if (hits.Count() == 0) return null;

        RaycastHit2D hit = hits.First();

        return hit.collider is not null ? hit.transform : null;
    }

    private void Attack()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(physic.position, attackZoneSize, 0, _lookDirection, attackDistance)
           .Where(e => e.transform.tag != this.tag)
           .ToArray();

        Transform playerTransform = hits.Length > 0 ? hits.Where(e => e.transform.tag == "Player").First().transform : null;

        if (playerTransform is not null)
            playerTransform.GetComponent<Heroine>().Hurt(atk);

        StartCoroutine(AttackRecoveryRoutine());
    }

    private IEnumerator AttackRecoveryRoutine()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        _attackOn = true;
        movingSystem.IsOn = true;
    }
}
