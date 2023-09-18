using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Projectile2D : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private protected int _damage;
    private protected Rigidbody2D _physic;
    private SpriteRenderer _spriteRenderer;

    public int Damage
    {
        get => _damage;
        set => _damage = value;
    }

    private protected virtual void Awake()
    {
        _physic = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(LifeTimeHandler());
    }

    public void AddForce(Vector2 force, float speed)
    {
        _physic.AddForce(force * speed, ForceMode2D.Force);

        SpriteFlip(force);
    }

    private void SpriteFlip(Vector2 direction)
    {
        if (direction.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    private IEnumerator LifeTimeHandler()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(this.gameObject);
    }
}
