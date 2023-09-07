using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PatrolMovingAl))]

public class Bat : Enemy
{
    private enum BatState { Idle, Move, Attack, Rotation }


    [Space]
    [Header("Bat Settings")]

    [Header("Attack system")]
    [SerializeField] private bool attackOn;
    [SerializeField] private float shootSpeed = 3;
    [SerializeField] protected float timeBetweenShoots = 1;
    [SerializeField] private EnergyBall energyBallPrefab;

    [Header("Components")]
    [SerializeField] private PatrolMovingAl movingSystem;

    [Header("References")]
    [Tooltip("Нужно установить ссылку на игрока на сцене, а не на префам игрока")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    private Vector2 _playerDirection;
    private BatState _state;
    private bool _attackOn = true;
    private bool _shootOn = true;


    void FixedUpdate()
    {
        if (_state == BatState.Idle)
        {
            if (!attackOn) return;

            if (_attackOn)
            {

            }

            if (_shootOn)
            {
                _playerDirection = (playerRigidbody.position - physic.position).normalized;
                Transform playerTransform = Physics2D.Raycast(physic.position + _playerDirection, _playerDirection).transform;

                if (playerTransform is not null && playerTransform.tag == "Player")
                {
                    _state = BatState.Attack;
                    _shootOn = false;
                    movingSystem.IsOn = false;
                    physic.velocity = Vector2.zero;
                    animator.SetTrigger("attack1");
                }
            }
        }
    }

    private void Shoot()
    {
        EnergyBall energyBall = Instantiate(energyBallPrefab, this.transform.position, new Quaternion(0f, 0f, 0f, 0f));
        energyBall.power = atk;
        energyBall.AddForce(_playerDirection, shootSpeed);

        _state = BatState.Idle;

        StartCoroutine(ShootRecover());
    }

    private IEnumerator ShootRecover()
    {
        movingSystem.IsOn = true;

        yield return new WaitForSeconds(timeBetweenShoots);

        _shootOn = true;
    }

    private void Attack()
    {
        
    }
}
