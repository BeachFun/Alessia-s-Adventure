using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Enemy3Controller : Enemy
{
    [Header("Enemy3 Settings")]
    [SerializeField] private float aggressionDistance;

    private string attackAnimationName;
    private Vector3 raycastDirection;

    private void FixedUpdate()
    {
        if (!isBusy)
        {
            Transform transform;

            transform = Physics2D.Raycast(this.transform.position, Vector3.left, aggressionDistance).transform;

            if (transform is not null && transform.tag == "Player")
            {
                attackAnimationName = "left_attack";
                raycastDirection = Vector3.left;
                StartCoroutine(Attack(transform));
            }

            transform = Physics2D.Raycast(this.transform.position, Vector3.right, aggressionDistance).transform;

            if (transform is not null && transform.tag == "Player")
            {
                attackAnimationName = "right_attack";
                raycastDirection = Vector3.right;
                StartCoroutine(Attack(transform));
            }
        }
    }

    private IEnumerator Attack(Transform playerTransform)
    {
        yield return null;

        isBusy = true;
        animator.SetTrigger(attackAnimationName);

        yield return new WaitForSeconds(atkSpeed / 2);

        playerTransform = Physics2D.Raycast(this.transform.position, raycastDirection, aggressionDistance).transform;
        if (playerTransform is not null && playerTransform.tag == "Player")
        {
            playerTransform.GetComponent<HeroineController>().Hurt(atk);
        }

        yield return new WaitForSeconds(atkSpeed / 2 + timeBetweenAttacks);

        isBusy = false;
    }
}
