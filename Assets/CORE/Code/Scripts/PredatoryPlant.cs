using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PredatoryPlant : Enemy
{
    [Header("PredatoryPlant Settings")]
    [SerializeField] private float attackDistance;

    private string attackAnimationName;
    private Vector3 raycastDirection;

    private void FixedUpdate()
    {
        if (!isBusy)
        {
            Transform transform;

            transform = Physics2D.Raycast(this.transform.position, Vector3.left, attackDistance).transform;

            if (transform is not null && transform.tag == "Player")
            {
                attackAnimationName = "left_attack";
                raycastDirection = Vector3.left;
                StartCoroutine(Attack(transform));
            }

            transform = Physics2D.Raycast(this.transform.position, Vector3.right, attackDistance).transform;

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

        yield return new WaitForSeconds(hurtSpeed / 1.5f);

        playerTransform = Physics2D.Raycast(this.transform.position, raycastDirection, attackDistance).transform;
        if (playerTransform is not null && playerTransform.tag == "Player")
        {
            playerTransform.GetComponent<HeroineController>().Hurt(atk);
        }

        yield return new WaitForSeconds(hurtSpeed / 3 + timeBetweenAttacks);

        isBusy = false;
    }
}
