using System.Linq;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

[System.Serializable]
public class PatrolMovingAl : MonoBehaviour
{
    public enum MovementMode { Walking, Flying }
    public enum MovementAlgoritm { EdgeToEdge, StartToPosition, PositionToPosition }


    private const float barrierDistance = .5f;

    public bool isOn = true;
    public MovementMode mode;
    public MovementAlgoritm movementAlgoritm;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public float rotateSeconds;
    public float moveSpeed = 0f;

    private bool _isGround;
    private bool _fastMoveOn;
    private bool _isMovingOn = true;
    private bool _isMoveToStart;
    private Vector2 _destination;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _physic;
    private Collider2D _collider;


    public bool IsOn { get => isOn; set => isOn = value; }
    public bool FastMoveOn { get => _fastMoveOn; set => _fastMoveOn = value; }
    public float Speed { get => _physic.velocity.magnitude / Time.fixedDeltaTime; }
    private bool IsReachedDestinationPosition
    {
        get
        {
            Vector2 currentPosition = _physic.position;

            if (UnityUtils.Approximately(currentPosition, _destination))
                return true;

            return Physics2D.RaycastAll(currentPosition, _destination - currentPosition, barrierDistance)
                .Where(e => e.transform.tag != this.tag)
                .Count() > 0;
        }
    }


    private void Awake()
    {
        _physic = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer is null) _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponentInChildren<Collider2D>();
        if (_collider is null) _collider = GetComponent<Collider2D>();

        _physic.gravityScale = mode == MovementMode.Flying ? 0f : 1f;

        if (movementAlgoritm == MovementAlgoritm.StartToPosition) startPosition = _physic.position;
        if (movementAlgoritm != MovementAlgoritm.EdgeToEdge) _destination = endPosition;
    }
    private void FixedUpdate()
    {
        if (!_isGround || !_isMovingOn || !isOn) return;

        if (movementAlgoritm == MovementAlgoritm.EdgeToEdge)
        {
            Vector2 groundPoint = CalcGroundPoint();
            Vector2 farGroundPoint = CalcFarGroundPoint();

            if (!UnityUtils.ApproximatelyEqual(groundPoint.y, farGroundPoint.y, 0.15f))
            {
                StartCoroutine(SlowRotate());
            }
            else
            {
                if (_spriteRenderer.flipX)
                    _physic.velocity = new Vector2(-(moveSpeed * Time.fixedDeltaTime), 0f);
                else
                    _physic.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, 0f);
            }
        }
        else
        {
            if (IsReachedDestinationPosition)
            {
                StartCoroutine(SlowRotate());
            }
            else
            {
                Vector2 currentPosition = this.transform.position;
                Vector2 moveDirection = (_destination - currentPosition).normalized;

                _physic.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
            }
        }

        if (_fastMoveOn) _physic.velocity *= 2;
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
        yield return new WaitForSeconds(rotateSeconds / 1.5f);

        _spriteRenderer.flipX = !_spriteRenderer.flipX;
        if (movementAlgoritm != MovementAlgoritm.EdgeToEdge)
        {
            _destination = _isMoveToStart ? endPosition : startPosition;
            _isMoveToStart = !_isMoveToStart;
        }

        yield return new WaitForSeconds(rotateSeconds / 3f);
        _isMovingOn = true;
    }
}