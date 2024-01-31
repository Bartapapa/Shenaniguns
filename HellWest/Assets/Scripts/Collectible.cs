using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacterController player = other.GetComponent<PlayerCharacterController>();
        if (player)
        {
            Player.Instance.AcquireBullets(6);
            Destroy(this.gameObject);
        }
    }
}
