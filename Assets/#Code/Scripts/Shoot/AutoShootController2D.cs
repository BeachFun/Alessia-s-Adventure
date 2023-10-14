using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Action = System.Action;

[System.Serializable]
public class AutoShootController2D : ShootSystem2D
{
    [SerializeField] private float shootMinDistance;
    [SerializeField] private float shootMaxDistance;
    [SerializeField] private string enemyTag;
    [SerializeField] private VisionMode detectionMode = VisionMode.Known;
    [SerializeField] private Transform enemyTransform;

    private string[] tagsToSkipOnDetecting = { "Enemy", "Area" };
    private VisionCone2D _vision;


    public event Action ActionBeforeShoot;


    private protected override void Start()
    {
        base.Start();

        _vision = GetComponentInChildren<VisionCone2D>();
        if (_vision is null) _vision = GetComponent<VisionCone2D>();
    }

    private void FixedUpdate()
    {
        if (!ShootOn || !_shootOn) return;

        Transform detectedTransform = null;

        if (detectionMode == VisionMode.Cone)
        {
            _vision.CheckVision();
            detectedTransform = _vision.DetectedObjects.Where(e => e.tag == enemyTag).First().transform;
            _enemyDirection = (detectedTransform.position - this.transform.position).normalized;
        }
        else if (this.enemyTransform != null)
        {
            _enemyDirection = (this.enemyTransform.position - this.transform.position).normalized;

            RaycastHit2D hit = Physics2D.RaycastAll(transform.position + _enemyDirection * shootMinDistance, _enemyDirection, shootMaxDistance)
                .Where(e => !tagsToSkipOnDetecting.Contains(e.transform.tag))
                .FirstOrDefault();

            if (hit.collider is not null) detectedTransform = hit.transform;
        }

        if (detectedTransform is not null && detectedTransform.tag == enemyTag)
        {
            ActionBeforeShoot?.Invoke();
            Throw(_enemyDirection, _speedMultiply);
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
        Throw(_enemyDirection, _speedMultiply);
    }
}
