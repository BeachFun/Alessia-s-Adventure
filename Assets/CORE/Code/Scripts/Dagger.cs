using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]

public class Dagger : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float lifeTime;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D physic;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        physic = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(LifeTimeHandler());
    }

    private void FixedUpdate()
    {
        physic.rotation = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().Hurt(damage);
        }

        if (collision.gameObject.tag != "Player")
        {
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }


    public void Throw(Vector2 direction, float throwSpeed)
    {
        var physic = GetComponent<Rigidbody2D>();
        physic.velocity = direction * throwSpeed;
        physic.AddForce(direction * throwSpeed, ForceMode2D.Force);

        SpriteFlip(direction);
    }

    private void SpriteFlip(Vector2 direction)
    {
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }


    private IEnumerator LifeTimeHandler()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(this.gameObject);
    }
}
