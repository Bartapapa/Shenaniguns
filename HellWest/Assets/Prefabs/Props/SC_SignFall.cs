using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SignFall : MonoBehaviour
{
    [SerializeField] private Animator animationTor;

    private bool alreadyFall = false;
    public void FallAnimation()
    {
        Debug.Log("aaaad");
        if (alreadyFall == false)
        {
            Debug.Log("Feur fEUR FEUR feur");
            alreadyFall = true;
            animationTor.Play("A_Sign_Fall", 0,0);
        }
    }
}
