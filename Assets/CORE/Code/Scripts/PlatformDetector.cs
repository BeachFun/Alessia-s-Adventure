using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            // Найден объект платформы, делаем игрока дочерним объектом платформы
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            // Покинули платформу, открепляем игрока от платформы
            transform.parent = null;
        }
    }
}
