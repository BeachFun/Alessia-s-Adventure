using UnityEngine;

public class FinishArea2D : Area2D
{
    private protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is not null && collision.transform.tag == "Player")
        {
            Messenger.Broadcast(GameEvents.LEVEL_PASSED);
        }
    }
}
