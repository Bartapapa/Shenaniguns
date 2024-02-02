using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAKRAKOOMEXPLODE : MonoBehaviour
{
    public ParticleSystem EXPLOSION;
    public float EXPLOSIONRADIUS = 5f;

    private void Start()
    {
        SphereCollider coll = GetComponent<SphereCollider>();
        coll.radius = EXPLOSIONRADIUS;
        coll.enabled = true;
        Explode();

    }
    public void Explode()
    {
        ParticleSystem boom = Instantiate<ParticleSystem>(EXPLOSION, transform.position, Quaternion.identity);
        Invoke("DestroyMe", .1f);
    }

    private void DestroyMe()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CanBeHit hit = other.GetComponent<CanBeHit>();
        if (hit)
        {
            hit.Die();
        }
    }
}
