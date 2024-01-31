using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public ParticleSystem Blood;
    public void Die()
    {
        ParticleSystem newBlood = Instantiate<ParticleSystem>(Blood, transform.position + (Vector3.up * 1.5f), Quaternion.identity);
        Destroy(this.gameObject);
    }
}
