using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool PhysicsBasedProjectile = false;
    public float ProjectileSpeed = 10f;
    public Vector3 ProjectileDirection = Vector3.zero;
    public float PenetrationStrength = 100f;
    private Rigidbody _rb;
    public List<Vector3> WayPoints = new List<Vector3>();
    public int _currentWaypoint = 0;
    private Vector3 _targetVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (!_rb) Destroy(this.gameObject);
        if (PhysicsBasedProjectile)
        {
            _rb.useGravity = true;
        }
    }

    private void Update()
    {
        if (WayPoints.Count > 0)
        {
            Vector3 direction = WayPoints[_currentWaypoint] - transform.position;
            direction = direction.normalized;
            Vector3 pastPoint = WayPoints[_currentWaypoint] - (direction * .1f);
            bool passed = Vector3.Dot((WayPoints[_currentWaypoint] - transform.position).normalized, (pastPoint - WayPoints[_currentWaypoint]).normalized) > 0;

            _targetVelocity = direction * ProjectileSpeed;

            if (Vector3.Distance(transform.position, WayPoints[_currentWaypoint]) <= .1f || passed)
            {
                _currentWaypoint++;

                if (_currentWaypoint > WayPoints.Count - 1)
                {
                    Destroy(this.gameObject);
                    //end of line
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (WayPoints.Count > 0)
        {
            HandleWayPoints();
        }
    }

    public void InitializeProjectile(Vector3 direction, float penetrationStrength)
    {
        direction = direction.normalized;
        ProjectileDirection = direction;

        Vector3 velocity = direction * ProjectileSpeed;

        _rb.AddForce(velocity - _rb.velocity, ForceMode.VelocityChange);

        PenetrationStrength = penetrationStrength;
    }

    private void HandleWayPoints()
    {
        _rb.AddForce(_targetVelocity - _rb.velocity, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        CanBeHit hit = other.GetComponent<CanBeHit>();
        if (hit)
        {
            hit.Die();
        }

        ShootingMaterial mat = other.GetComponent<ShootingMaterial>();
        if (mat)
        {
            if (mat.Type == ShootingMaterial.MaterialType.BulletProof)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
