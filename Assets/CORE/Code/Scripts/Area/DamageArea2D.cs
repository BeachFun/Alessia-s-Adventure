using System.Collections;
using UnityEngine;

public class DamageArea2D : Area2D
{
    [SerializeField] private int damage = 10;
    [Tooltip("Промежуток между получениями урона")]
    [SerializeField] private float damageSpan = 1.7f;

    private Player _player;
    private Coroutine _coroutine;

    private protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is not null && collision.transform.tag == "Player")
        {
            _player = collision.GetComponent<Player>();
            _coroutine = StartCoroutine(DamageRoutine());
        }
    }

    private protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision is not null && collision.transform.tag == "Player")
        {
            _player = null;
            StopCoroutine(_coroutine);
        }
    }

    private IEnumerator DamageRoutine()
    {
        while (true)
        {
            _player.Hurt(damage);

            yield return new WaitForSeconds(damageSpan);
        }
    }
}
