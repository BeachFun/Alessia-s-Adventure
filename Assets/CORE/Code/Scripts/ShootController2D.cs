using System.Linq;
using System.Collections;
using UnityEngine;
using Action = System.Action;

[RequireComponent(typeof(Animator))]

public class ShootController2D : MonoBehaviour
{
    [SerializeField] private bool shootOn;
    [SerializeField] private int shootDamage;
    [SerializeField] private float shootMinDistance;
    [SerializeField] private float shootMaxDistance;
    [SerializeField] private float timeBetweenShoots;
    [SerializeField] private float shootSpeed;
    [SerializeField] private string shootAnimationName;
    [SerializeField] private string enemyTag;
    [SerializeField] private VisionMode detectionMode = VisionMode.Known;
    [SerializeField] private Projectile2D projectile;
    [SerializeField] private Transform enemyTransform;

    private bool _shootOn;
    private Vector3 _enemyDirection;
    private Animator _animator;
    private VisionCone2D _vision;

    public bool ShootOn { get => shootOn; set => shootOn = value; }


    public event Action ActionBeforeShoot;
    public event Action ActionAfterShoot;


    private void Start()
    {
        _animator = GetComponent<Animator>();

        _vision = GetComponentInChildren<VisionCone2D>();
        if (_vision is null) _vision = GetComponent<VisionCone2D>();
    }

    private void FixedUpdate()
    {
        if (!ShootOn || !_shootOn) return;

        Transform enemyTransform;

        if (detectionMode == VisionMode.Cone)
        {
            _vision.CheckVision();
            enemyTransform = _vision.DetectedObjects.Where(e => e.tag == enemyTag).First().transform;
            _enemyDirection = (enemyTransform.position - this.transform.position).normalized;
        }
        else
        {
            _enemyDirection = (this.enemyTransform.position - this.transform.position).normalized;
            enemyTransform = Physics2D.Raycast(transform.position + _enemyDirection * shootMinDistance, _enemyDirection, shootMaxDistance).transform;
        }

        if (enemyTransform is not null && enemyTransform.tag == enemyTag)
        {
            _shootOn = false;
            ActionBeforeShoot.Invoke();
            Shoot();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionMode == VisionMode.Cone)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + Vector3.left * shootMinDistance, Vector3.left);
        }
        else
        {
            // TODO: finish this algorithm
        }
    }

    public void Shoot()
    {
        _animator.SetTrigger(shootAnimationName);
    }

    private void ReleaseProjectile()
    {
        Projectile2D projectile = Instantiate(this.projectile, this.transform.position, new Quaternion(0f, 0f, 0f, 0f));
        projectile.Power = shootDamage;
        projectile.AddForce(_enemyDirection, shootSpeed);

        StartCoroutine(ShootRecoverRoutine());
    }

    private IEnumerator ShootRecoverRoutine()
    {
        ActionAfterShoot.Invoke();

        yield return new WaitForSeconds(timeBetweenShoots);

        _shootOn = true;
    }
}
