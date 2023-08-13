using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Enemy3Controller : Enemy
{
    [Header("Enemy3 Settings")]
    [SerializeField] private float aggressionDistance;

    private string attackAnimationName;

    private void FixedUpdate()
    {
        if (!isBusy)
        {
            RaycastHit2D hit;

            hit = Physics2D.Raycast(transform.position, Vector3.left, aggressionDistance);

            if (hit.transform is not null && hit.transform.tag == "Player")
            {
                attackAnimationName = "left_attack";
                StartCoroutine(Attack());
            }

            hit = Physics2D.Raycast(transform.position, Vector3.right, aggressionDistance);

            if (hit.transform is not null && hit.transform.tag == "Player")
            {
                attackAnimationName = "right_attack";
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        yield return null;

        isBusy = true;

        animator.SetTrigger(attackAnimationName);

        while (UnityUtils.IsAnimationPlaying(animator, attackAnimationName))
        {
            yield return new WaitForSeconds(.05f);
        }

        isBusy = false;
    }
}
