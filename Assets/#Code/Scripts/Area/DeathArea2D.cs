using UnityEngine;

public class DeathArea2D : Area2D
{
    private protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && (collision.transform.tag == "Player" || collision.transform.tag == "Enemy"))
        {
            collision.transform.GetComponent<Character>().Death();
        }
    }
}
