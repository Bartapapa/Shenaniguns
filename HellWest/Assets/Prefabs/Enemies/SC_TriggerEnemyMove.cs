using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TriggerEnemyMove : MonoBehaviour
{
    private bool isActivated = false;
    [SerializeField] private Animator runningAwayAnimator;

    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacterController player = other.GetComponent<PlayerCharacterController>();
        if (player != null && isActivated == false)
        {
            isActivated = true;
            runningAwayAnimator.Play("A_EnemyEscape", 0, 0);
        }
    }
}
