using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Morlock : Enemy
{
    [Space] [Header("Morlock Settings")]

    [Header("Attack system")]
    [SerializeField] private float aggressionDistance;
    [SerializeField] private float attackDistance;

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

        Vector2 from = new Vector2(0f, this.transform.position.y);
        from.x = spriteRenderer.flipX ? _collider.bounds.min.x : _collider.bounds.max.x;
        Vector2 to = new Vector2(0, this.transform.position.y);
        to.x = spriteRenderer.flipX ? from.x - attackDistance : from.x + attackDistance;

        Gizmos.DrawLine(from, to);
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
        _attackOn = false;

        Transform playerTransform = Physics2D.RaycastAll(_forwardBodyPoint, _lookDirection, attackDistance)
            .Where(e => e.transform.tag != this.tag)
            .First().transform;

        if (playerTransform is not null && playerTransform.tag == "Player")
        {
            playerTransform.GetComponent<Heroine>().Hurt(atk);
        }

        StartCoroutine(AttackRecovery());
    }

    private IEnumerator AttackRecovery()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        _attackOn = true;
        movingSystem.IsOn = true;
    }
}
