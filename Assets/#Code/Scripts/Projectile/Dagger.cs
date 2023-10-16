using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]

[System.Serializable]
public class Dagger : Projectile2D
{
    private string[] tagsToSkipOnTriggerEnter = { "Player", "Area", "Collectable" };

    private void FixedUpdate()
    {
        _physic.rotation = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            try
            {
                collision.gameObject.GetComponent<Enemy>().Hurt(Damage);
            }
            catch { Debug.LogWarning("Error this"); }
        }

        if (!tagsToSkipOnTriggerEnter.Contains(collision.tag))
        {
            Debug.Log($"Dagger destroy {collision.tag}");
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }
}
