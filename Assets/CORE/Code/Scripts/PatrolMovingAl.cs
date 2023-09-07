using System.Linq;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PatrolMovingAl : MonoBehaviour
{
    public enum Mode { EdgeToEdge, StartToPoint, PointToPoint }


    private bool s_isOn = true;
    private Mode s_mode;
    private Vector2 s_startPosition;
    private Vector2 s_endPosition;
    private float s_rotateSeconds;
    private float s_moveSpeed = 0f;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _physic;
    private Collider2D _collider;
    private bool _isGround;
    private bool _fastMoveOn;
    private bool _isMovingOn = true;


    #region Свойства для инспектора
    public bool IsOn { get => s_isOn; set => s_isOn = value; }
    public Mode MovementMode { get => s_mode; set => s_mode = value; }
    public Vector2 StartPosition { get => s_startPosition; set => s_startPosition = value; }
    public Vector2 EndPosition { get => s_endPosition; set => s_endPosition = value; }
    public float RotateSeconds { get => s_rotateSeconds; set => s_rotateSeconds = value; }
    public float MoveSpeed { get => s_moveSpeed; set => s_moveSpeed = value; }
    #endregion
    public bool FastMoveOn { get => _fastMoveOn; set => _fastMoveOn = value; }
    public float Speed { get => _physic.velocity.magnitude / Time.fixedDeltaTime; }


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
        if (!_isGround || !_isMovingOn || !s_isOn) return;

        Vector2 groundPoint = CalcGroundPoint();
        Vector2 farGroundPoint = CalcFarGroundPoint();

        if (!UnityUtils.ApproximatelyEqual(groundPoint.y, farGroundPoint.y, 0.15f))
        {
            StartCoroutine(SlowRotate());
        }
        else
        {
            if (_spriteRenderer.flipX)
                _physic.velocity = new Vector2(-(s_moveSpeed * Time.fixedDeltaTime), 0);
            else
                _physic.velocity = new Vector2(s_moveSpeed * Time.fixedDeltaTime, 0);

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
        yield return new WaitForSeconds(s_rotateSeconds / 1.5f);
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
        yield return new WaitForSeconds(s_rotateSeconds / 3f);
        _isMovingOn = true;
    }
}