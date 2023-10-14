using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is not null && collision.tag == "Player")
        {
            Messenger<string, int>.Broadcast(GameEvents.ITEM_COLLECTED, itemName, 1);
            Destroy(this.gameObject);
        }
    }
}
