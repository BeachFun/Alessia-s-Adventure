using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionCone2D : MonoBehaviour
{
    [SerializeField] private float visionAngle = 60.0f;
    [SerializeField] private float maxDistance = 10.0f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform eyesTransform;
    [SerializeField] private bool detectMultipleObjects = false;
    [SerializeField] private bool isRotated = false;
    [SerializeField] private bool shouldCheckVision = false;

    private List<GameObject> _detectedObjects = new List<GameObject>();

    public bool IsRatated
    {
        get => isRotated;
        set => isRotated = value;
    }
    public List<GameObject> DetectedObjects
    {
        get => _detectedObjects;
    }

    private void Update()
    {
        if (shouldCheckVision) CheckVision();
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = eyesTransform.right;
        float halfAngle = visionAngle / 2.0f;

        for (float angle = -halfAngle; angle <= halfAngle; angle += 1.0f)
        {
            Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * direction * (IsRatated ? -1 : 1);
            Gizmos.DrawRay(eyesTransform.position, rayDirection);
        }
    }

    public void CheckVision()
    {
        // Настройте лучи для проверки видимости в соответствии с параметрами зрения
        Vector2 direction = eyesTransform.right;
        float halfAngle = visionAngle / 2.0f;

        _detectedObjects.Clear();

        if (!detectMultipleObjects)
        {
            for (float angle = -halfAngle; angle <= halfAngle; angle += 1.0f)
            {
                Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * direction * (IsRatated ? -1 : 1);

                RaycastHit2D hit = Physics2D.Raycast(eyesTransform.position, rayDirection, maxDistance, targetLayer);

                if (hit.collider != null) _detectedObjects.Add(hit.collider.gameObject);
            }
        }
        else
        {
            for (float angle = -halfAngle; angle <= halfAngle; angle += 1.0f)
            {
                Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * direction * (IsRatated ? -1 : 1);

                IEnumerable<GameObject> hits = Physics2D.RaycastAll(eyesTransform.position, rayDirection, maxDistance, targetLayer)
                    .Where(e => e.collider != null)
                    .Select(e => e.collider.gameObject);

                if (hits.Count() > 0) _detectedObjects.AddRange(hits);
            }
        }
    }

    public void VisionStart() => shouldCheckVision = true;

    public void VisionEnd() => shouldCheckVision = false;
}

public enum VisionMode
{
    Cone,
    Known
}
