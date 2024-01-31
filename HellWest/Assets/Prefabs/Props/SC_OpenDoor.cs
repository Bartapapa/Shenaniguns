using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_OpenDoor : MonoBehaviour
{
    [SerializeField] private GameObject selfCadena;
    [SerializeField] private GameObject door;

    public void OpenDoor()
    {
        Destroy(selfCadena);
        Destroy(door);
    }
}