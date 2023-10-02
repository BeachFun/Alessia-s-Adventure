using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public abstract class Area2D : MonoBehaviour
{
    private protected BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private protected virtual void OnTriggerEnter2D(Collider2D collision) { }
    private protected virtual void OnTriggerStay2D(Collider2D collision) { }
    private protected virtual void OnTriggerExit2D(Collider2D collision) { }
}
