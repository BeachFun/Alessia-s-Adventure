using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]

[System.Serializable]
public class Dagger : Projectile2D
{
    private void FixedUpdate()
    {
        _physic.rotation = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().Hurt(Damage);
        }

        if (collision.gameObject.tag != "Player")
        {
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }
}
