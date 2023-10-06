using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

[System.Serializable]
public class EnergyBall : Projectile2D
{
    private string[] tagsToSkipOnTriggerEnter = { "Enemy", "Area" };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().Hurt(Damage);
        }

        if (!tagsToSkipOnTriggerEnter.Contains(collision.tag))
        {
            Destroy(this.gameObject);
        }
    }
}
