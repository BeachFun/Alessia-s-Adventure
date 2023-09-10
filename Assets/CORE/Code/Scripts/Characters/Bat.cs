using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PatrolMovingAl))]

public class Bat : Enemy
{
    [Space]
    [Header("Bat Settings")]

    [Header("Attack system")]
    [SerializeField] private bool attackOn;
    [SerializeField] private float attackDistance;
    [SerializeField] private Vector2 attackZoneSize;
    [SerializeField] private bool shootOn;
    [SerializeField] private float shootSpeed = 3;
    [SerializeField] private float playerCheckDistance;
    [SerializeField] protected float timeBetweenShoots = 1;
    [SerializeField] private EnergyBall energyBallPrefab;

    [Header("Components")]
    [SerializeField] private PatrolMovingAl movingSystem;

    [Header("References")]
    [Tooltip("Нужно установить ссылку на игрока на сцене, а не на префам игрока")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    private Vector2 _playerDirection;
    private bool _attackOn = true;
    private bool _shootOn = true;


    void FixedUpdate()
    {
        if (attackOn && _attackOn)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(physic.position, attackZoneSize, 0, spriteRenderer.flipX ? Vector2.left : Vector2.right, attackDistance)
                .Where(e => e.transform.tag != this.tag)
                .ToArray();

            if (hits.Length > 0 && hits.Any(e => e.transform.tag == "Player"))
            {
                _attackOn = false;
                ChangeStateToAttack();
                animator.SetTrigger("attack2");
            }
        }

        if (shootOn && _shootOn)
        {
            _playerDirection = (playerRigidbody.position - physic.position).normalized;
            Transform playerTransform = Physics2D.Raycast(physic.position + _playerDirection, _playerDirection, playerCheckDistance).transform;

            if (playerTransform is not null && playerTransform.tag == "Player")
            {
                _shootOn = false;
                ChangeStateToAttack();
                animator.SetTrigger("attack1");
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

    private void Shoot()
    {
        EnergyBall energyBall = Instantiate(energyBallPrefab, this.transform.position, new Quaternion(0f, 0f, 0f, 0f));
        energyBall.power = atk;
        energyBall.AddForce(_playerDirection, shootSpeed);

        StartCoroutine(ShootRecoverRoutine());
    }

    private IEnumerator ShootRecoverRoutine()
    {
        movingSystem.IsOn = true;

        yield return new WaitForSeconds(timeBetweenShoots);

        _shootOn = true;
    }

    private void Attack()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(physic.position, attackZoneSize, 0, spriteRenderer.flipX ? Vector2.left : Vector2.right, attackDistance)
            .Where(e => e.transform.tag != this.tag)
            .ToArray();

        Transform playerTransform = hits.Length > 0 ? hits.Where(e => e.transform.tag == "Player").First().transform : null;

        if (playerTransform is not null)
            playerTransform.GetComponent<Heroine>().Hurt(atk);

        StartCoroutine(AttackRecoveryRoutine());
    }

    private IEnumerator AttackRecoveryRoutine()
    {
        movingSystem.IsOn = true;

        yield return new WaitForSeconds(timeBetweenAttacks);

        _attackOn = true;
    }


    private void ChangeStateToAttack()
    {
        movingSystem.IsOn = false;
        physic.velocity = Vector2.zero;
    }
}
