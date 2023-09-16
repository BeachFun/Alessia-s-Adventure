using System.Linq;
using UnityEngine;
using Action = System.Action;

[RequireComponent(typeof(Animator))]

[System.Serializable]
public class AutoShootController2D : ShootSystem2D
{
    [SerializeField] private float shootMinDistance;
    [SerializeField] private float shootMaxDistance;
    [SerializeField] private string enemyTag;
    [SerializeField] private VisionMode detectionMode = VisionMode.Known;
    [SerializeField] private Transform enemyTransform;

    private VisionCone2D _vision;
    private protected Animator _animator;


    public event Action ActionBeforeShoot;


    private protected void Start()
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
}
