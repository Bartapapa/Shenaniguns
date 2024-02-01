using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PipeFall : MonoBehaviour
{
    [SerializeField] private Animator animationTor;
    [SerializeField] private List<GameObject> enemy = new List<GameObject>();

    private bool alreadyFall = false;
    public void FallAnimation()
    {
        if (alreadyFall == false)
        {
            alreadyFall = true;
            animationTor.Play("A_Pipe_Fall", 0, 0);
            Invoke("KillDemon", 0.3f);
            Debug.Log("debug");
        }
    }

    private void KillDemon()
    {
        foreach (GameObject currentEnemy in enemy)
        {
            if (currentEnemy != null)
            {
                currentEnemy.GetComponent<Demon>().Die();
            }

        }
    }
}
