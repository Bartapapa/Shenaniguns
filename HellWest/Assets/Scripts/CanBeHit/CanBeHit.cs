using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanBeHit : MonoBehaviour
{
    public UnityEvent OnDeath;

    public void Die()
    {
        OnDeath.Invoke();
    }
}
