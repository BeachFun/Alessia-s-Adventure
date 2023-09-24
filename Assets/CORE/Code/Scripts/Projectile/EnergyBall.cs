using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

[System.Serializable]
public class EnergyBall : Projectile2D
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().Hurt(Damage);
        }
        if (collision.gameObject.GetComponent<Bat>() is null)
        {
            Destroy(this.gameObject);
        }
    }
}
