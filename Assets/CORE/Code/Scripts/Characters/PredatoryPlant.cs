using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PredatoryPlant : Enemy
{
    [Header("PredatoryPlant Settings")]
    [SerializeField] private bool attackOn;
    [SerializeField] private float attackDistance;
    [SerializeField] private Vector2 attackZoneSize;

    private bool _attackOn = true;
    private Vector3 _raycastDirection;

    private void FixedUpdate()
    {
        if (!attackOn || !_attackOn) return;

        RaycastHit2D[] hits;
        Transform playerTransform;
        Vector2 origin = new Vector2(0, physic.position.y);

        // Проверка слева
        origin.x = _collider.bounds.min.x;
        hits = Physics2D.BoxCastAll(origin, attackZoneSize, 0, Vector2.left, attackDistance)
            .Where(e => e.transform.tag == "Player")
            .ToArray();

        playerTransform = hits.Length > 0 ? hits.First().transform : null;
        if (playerTransform is not null)
        {
            _raycastDirection = Vector3.left;
            _attackOn = false;
            animator.SetTrigger("left_attack");
        }

        // Проверка справа
        origin.x = _collider.bounds.max.x;
        hits = Physics2D.BoxCastAll(origin, attackZoneSize, 0, Vector2.right, attackDistance)
            .Where(e => e.transform.tag == "Player")
            .ToArray();

        playerTransform = hits.Length > 0 ? hits.First().transform : null;
        if (playerTransform is not null)
        {
            _raycastDirection = Vector3.right;
            _attackOn = false;
            animator.SetTrigger("right_attack");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 center = new Vector3(0f, physic.position.y);
        center.x = _collider.bounds.min.x - (attackDistance / 2);
        Gizmos.DrawWireCube(center, attackZoneSize);
        center.x = _collider.bounds.max.x + (attackDistance / 2);
        Gizmos.DrawWireCube(center, attackZoneSize);
    }

    private void Attack()
    {
        Vector2 origin = new Vector2(0, physic.position.y);
        origin.x = _raycastDirection == Vector3.left ? _collider.bounds.min.x : _collider.bounds.max.x;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, attackZoneSize, 0, _raycastDirection, attackDistance)
            .Where(e => e.transform.tag == "Player")
            .ToArray();

        Transform playerTransform = hits.Length > 0 ? hits.First().transform : null;

        if (playerTransform is not null)
            playerTransform.GetComponent<Heroine>().Hurt(atk);

        StartCoroutine(AttackRecoverRoutine());
    }

    private IEnumerator AttackRecoverRoutine()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        _attackOn = true;
    }
}
