using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanBeHit : MonoBehaviour
{
    public UnityEvent OnDeath;
    public bool IsDead = false;

    public void Die()
    {
        if (IsDead) return;
        OnDeath.Invoke();
        IsDead = true;
    }
}
