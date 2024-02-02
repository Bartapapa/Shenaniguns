using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public ParticleSystem Blood;
    public GameObject Outline;
    private float _outlineTimer = .1f;
    private float _currentOutlineTimer = float.MinValue;

    private void Update()
    {
        if (_currentOutlineTimer > 0)
        {
            Outline.SetActive(true);
            _currentOutlineTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            Outline.SetActive(false);
        }
    }
    public void Die()
    {
        ParticleSystem newBlood = Instantiate<ParticleSystem>(Blood, transform.position + (Vector3.up * 1.5f), Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void ShowOutline()
    {
        _currentOutlineTimer = _outlineTimer;
    }
}
