using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PredatoryPlant : Enemy
{
    [Header("PredatoryPlant Settings")]
    [SerializeField] private float attackDistance;

    private Vector3 _raycastDirection;

    private void FixedUpdate()
    {
        this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        if (!isBusy)
        {
            Transform transform;

            Vector2 origin = new Vector2(0, this.transform.position.y);

            origin.x = collider.bounds.min.x;
            transform = Physics2D.Raycast(origin, Vector2.left, attackDistance).transform;

            if (transform is not null && transform.tag == "Player")
            {
                _raycastDirection = Vector3.left;
                animator.SetTrigger("left_attack");
            }

            origin.x = collider.bounds.max.x;
            transform = Physics2D.Raycast(origin, Vector2.right, attackDistance).transform;

            if (transform is not null && transform.tag == "Player")
            {
                _raycastDirection = Vector3.right;
                animator.SetTrigger("right_attack");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(collider.bounds.min.x, this.transform.position.y), this.transform.position - new Vector3(attackDistance, 0f));
        Gizmos.DrawLine(new Vector3(collider.bounds.max.x, this.transform.position.y), this.transform.position + new Vector3(attackDistance, 0f));
    }

    private void Attack()
    {
        Vector2 origin = new Vector2(0, this.transform.position.y);
        origin.x = _raycastDirection == Vector3.left ? collider.bounds.min.x : collider.bounds.max.x;

        Transform playerTransform = Physics2D.Raycast(origin, _raycastDirection, attackDistance).transform;
        if (playerTransform is not null && playerTransform.tag == "Player")
        {
            playerTransform.GetComponent<Heroine>().Hurt(atk);
        }
    }
}
