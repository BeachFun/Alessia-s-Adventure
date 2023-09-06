using System.Linq;
using System.Collections;
using UnityEngine;

public class PatrolMovingAl : MonoBehaviour
{
    [SerializeField] private bool isOn = true;
    [Header("Settings")]
    [SerializeField] private float rotateSeconds;
    [SerializeField] private float moveSpeed = 0f;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _physic;
    private Collider2D _collider;
    private bool _isGround;
    private bool _fastMoveOn;
    private bool _isMovingOn = true;


    public bool IsOn
    {
        get => isOn;
        set => isOn = value;
    }
    public bool FastMoveOn
    {
        get => _fastMoveOn;
        set => _fastMoveOn = value;
    }
    public float Speed
    {
        get => _physic.velocity.magnitude / Time.fixedDeltaTime;
    }


    private void Awake()
    {
        _physic = GetComponent<Rigidbody2D>();

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer is null) _spriteRenderer = GetComponent<SpriteRenderer>();

        _collider = GetComponentInChildren<Collider2D>();
        if (_collider is null) _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (!_isGround || !_isMovingOn || !isOn) return;

        Vector2 groundPoint = CalcGroundPoint();
        Vector2 farGroundPoint = CalcFarGroundPoint();

        if (!UnityUtils.ApproximatelyEqual(groundPoint.y, farGroundPoint.y, 0.15f))
        {
            StartCoroutine(SlowRotate());
        }
        else
        {
            if (_spriteRenderer.flipX)
                _physic.velocity = new Vector2(-(moveSpeed * Time.fixedDeltaTime), 0);
            else
                _physic.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, 0);

            if (_fastMoveOn) _physic.velocity *= 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) _isGround = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) _isGround = false;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(new Vector2(_collider.bounds.min.x, _collider.bounds.max.y), new Vector2(-0.7f, -1f));
    //    Gizmos.DrawRay(new Vector2(_collider.bounds.max.x, _collider.bounds.max.y), new Vector2(0.7f, -1f));
    //    Gizmos.DrawRay(new Vector2(this.transform.position.x, _collider.bounds.min.y), Vector2.down);
    //}

    private Vector2 CalcGroundPoint()
    {
        Vector2 bottomBodyPoint = new Vector2(this.transform.position.x, _collider.bounds.min.y);

        return Physics2D.RaycastAll(bottomBodyPoint, Vector2.down).Where(e => e.transform.tag != this.tag).First().point;
    }

    private Vector2 CalcFarGroundPoint()
    {
        Vector2 forwardBodyPoint, direction;

        if (_spriteRenderer.flipX)
        {
            forwardBodyPoint = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y);
            direction = new Vector2(-0.7f, -1f);
        }
        else
        {
            forwardBodyPoint = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y);
            direction = new Vector2(0.7f, -1f);
        }

        return Physics2D.RaycastAll(forwardBodyPoint, direction).Where(e => e.transform.tag != "Enemy").First().point;
    }

    private IEnumerator SlowRotate()
    {
        _isMovingOn = false;
        _physic.velocity = Vector2.zero;
        yield return new WaitForSeconds(rotateSeconds / 1.5f);
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
        yield return new WaitForSeconds(rotateSeconds / 3f);
        _isMovingOn = true;
    }
}
