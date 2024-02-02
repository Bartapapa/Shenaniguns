using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Tono_Fall : MonoBehaviour
{
    [SerializeField] private Animator feurAnimator;
    [SerializeField] private GameObject explosion;

    private bool alreadyFall = false;
    public void FallAnimation()
    {
        if (alreadyFall == false)
        {
            alreadyFall = true;
            feurAnimator.Play("A_Tono_Fall", 0, 0);
            Invoke("SpawnExplosion", 2.3f);

        }
    }

    public void SpawnExplosion()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
    }

}
