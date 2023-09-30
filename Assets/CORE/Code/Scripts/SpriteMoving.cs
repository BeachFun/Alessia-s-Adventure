using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]

public class SpriteMoving : MonoBehaviour
{
    [SerializeField] private float speed = 0;

    private float pos = 0;
    private RawImage image;

    private const float _delay = 0.05f;


    private void Start()
    {
        image = GetComponent<RawImage>();
        StartCoroutine(MoveSprite());
    }

    private IEnumerator MoveSprite()
    {
        yield return null;

        while (true)
        {
            pos += speed;

            if (pos > 1.0f)
                pos = 0;

            image.uvRect = new Rect(pos, 0, 1, 1);

            yield return new WaitForSeconds(_delay);
        }
    }
}
