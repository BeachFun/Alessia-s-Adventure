using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Bat : Enemy
{
    [Header("Moving system")]
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private Vector2 endPoint;
    [SerializeField] private float rotateSeconds;
    [SerializeField] private bool isStand;

    private bool _isMoveBack;
    private Vector2 _destination;


    private void FixedUpdate()
    {
        if (!isStand)
        {
            Vector2 currPosition = new Vector2(this.transform.position.x, this.transform.position.y);

            if (startPoint == currPosition)
                StartCoroutine(SlowRotate());

            physic.velocity = (_destination - currPosition) * moveSpeed * Time.fixedDeltaTime;
        }
    }

    private IEnumerator SlowRotate()
    {
        isStand = true;

        yield return new WaitForSeconds(rotateSeconds / 1.5f);

        _destination = _isMoveBack ? endPoint : startPoint;
        spriteRenderer.flipX = _destination.x < this.transform.position.x ? true : false;

        yield return new WaitForSeconds(rotateSeconds / 3f);

        isStand = false;
    }
}
