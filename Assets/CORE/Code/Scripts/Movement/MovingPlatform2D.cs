using System.Collections;
using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private Vector2 endPoint;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float delayTime = 1.0f;

    private Vector3 currentTarget;
    private bool movingToEnd = true;

    private void Start()
    {
        currentTarget = endPoint;
        StartCoroutine(MovePlatform());
    }

    private IEnumerator MovePlatform()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, currentTarget);
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);

            if (distance < 0.01f)
            {
                // Мы достигли текущей цели
                yield return new WaitForSeconds(delayTime);

                // Изменяем текущую цель
                currentTarget = movingToEnd ? startPoint : endPoint;
                movingToEnd = !movingToEnd;
            }

            yield return null;
        }
    }
}
