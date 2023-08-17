using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class EnergyBall : MonoBehaviour
{
    public int power;
    private Rigidbody2D physic;

    private void Awake()
    {
        physic = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HeroineController>().Hurt(power);
        }
        if (collision.gameObject.GetComponent<Bat>() is null)
        {
            Destroy(this.gameObject);
        }
    }

    public void AddForce(Vector2 force, float speed)
    {
        physic.AddForce(force * speed, ForceMode2D.Force);
    }
}
