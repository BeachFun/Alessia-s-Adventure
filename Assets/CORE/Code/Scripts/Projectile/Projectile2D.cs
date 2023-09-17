using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Projectile2D : MonoBehaviour
{
    public int Power;
    private Rigidbody2D physic;

    private protected virtual void Awake()
    {
        physic = GetComponent<Rigidbody2D>();
    }

    public void AddForce(Vector2 force, float speed)
    {
        physic.AddForce(force * speed, ForceMode2D.Force);
    }
}
