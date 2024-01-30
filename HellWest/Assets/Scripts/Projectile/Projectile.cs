using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool PhysicsBasedProjectile = false;
    public float ProjectileSpeed = 10f;
    public Vector3 ProjectileDirection = Vector3.zero;
    public float PenetrationStrength = 100f;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (!_rb) Destroy(this.gameObject);
    }

    public void InitializeProjectile(Vector3 direction, float penetrationStrength)
    {
        direction = direction.normalized;
        ProjectileDirection = direction;

        Vector3 velocity = direction * ProjectileSpeed;

        _rb.velocity = velocity;

        PenetrationStrength = penetrationStrength;
    }
}
