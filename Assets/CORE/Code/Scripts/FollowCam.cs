using UnityEngine;

[RequireComponent(typeof(Camera))]

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Rigidbody2D target;
    [SerializeField] private float smoothTime = 0.2f;

    [SerializeField] private float offsetX = 1f;
    [SerializeField] private float offsetY = 1f;

    [SerializeField] private BoxCollider2D areaMovement;

    private float minX = -10f;
    private float maxX = 10f;
    private float minY = -10f;
    private float maxY = 10f;

    private Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        if (areaMovement is null) throw new System.NullReferenceException();

        Camera camera = GetComponent<Camera>();

        float cameraHalfWidth = camera.orthographicSize * camera.aspect;
        float cameraHalfHeight = camera.orthographicSize;

        minX = areaMovement.bounds.min.x + cameraHalfWidth;
        maxX = areaMovement.bounds.max.x - cameraHalfWidth;
        minY = areaMovement.bounds.min.y + cameraHalfHeight;
        maxY = areaMovement.bounds.max.y - cameraHalfHeight;
    }

    void LateUpdate()
    {
        // Учитываем направление игрока
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        if (horizontalDirection > 0)
        {
            offsetX = Mathf.Abs(offsetX);
        }
        else if (horizontalDirection < 0)
        {
            offsetX = -Mathf.Abs(offsetX);
        }

        // Ограничиваем позицию камеры
        Vector3 targetPosition = new Vector3(target.position.x + offsetX, target.position.y + offsetY, transform.position.z);
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
    }
}
