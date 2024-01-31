using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public Projectile Projectile;
    public int ProjectileNumber;
    public Transform DestructibleCenterOfMass;
    public float ProjectileSphereRadius = 5f;
    public void Explode()
    {
        for (int i = 0; i < ProjectileNumber; i++)
        {
            Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * ProjectileSphereRadius;
            Projectile newProjectile = Instantiate<Projectile>(Projectile, DestructibleCenterOfMass.position+randomPoint, Quaternion.identity);
            Vector3 direction = newProjectile.transform.position - DestructibleCenterOfMass.position;
            direction = direction.normalized;
            newProjectile.InitializeProjectile(direction, 100f);
            
        }

        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (!DestructibleCenterOfMass) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(DestructibleCenterOfMass.position, ProjectileSphereRadius);
    }
}
