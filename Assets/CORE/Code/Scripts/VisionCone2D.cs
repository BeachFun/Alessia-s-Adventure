using UnityEngine;

public class VisionCone2D : MonoBehaviour
{
    public float visionAngle = 60.0f;
    public float maxDistance = 10.0f;
    public LayerMask targetLayer;
    public Transform eyesTransform;

    private void Update()
    {
        CheckVision();
    }

    public void CheckVision()
    {
        // Настройте лучи для проверки видимости в соответствии с параметрами зрения
        Vector2 direction = eyesTransform.right;
        float halfAngle = visionAngle / 2.0f;

        for (float angle = -halfAngle; angle <= halfAngle; angle += 1.0f)
        {
            Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * direction;

            RaycastHit2D hit = Physics2D.Raycast(eyesTransform.position, rayDirection, maxDistance, targetLayer);

            if (hit.collider != null)
            {
                // Обработайте объект, который был обнаружен в зоне видимости
                GameObject target = hit.collider.gameObject;
                Debug.Log("Object " + target.name + " is in vision cone.");
            }
        }
    }
}
