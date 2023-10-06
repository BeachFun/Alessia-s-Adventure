using UnityEngine;

public class ActivateArea2D : Area2D
{
    [SerializeField] private bool isReusable;
    [SerializeField] private GameObject refObject;

    private bool _isUsed;

    private protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isUsed) return;

        if (collision != null && collision.transform.tag == "Player")
        {
            refObject.SetActive(true);

            if (!isReusable) _isUsed = true;
        }
    }
}
