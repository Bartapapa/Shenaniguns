using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletTimer : MonoBehaviour
{
    public float BulletTimeFactor = .1f;
    public float BulletTimeTransition = .5f;
    private float _bulletTimeTransitionTimer = float.MinValue;
    public bool IsInBulletTime = false;

    public void ToggleBulletTime()
    {
        if (IsInBulletTime)
        {
            StopBulletTime();
        }
        else
        {
            StartBulletTime();
        }
    }

    private void Update()
    {
        HandleBulletTimeTransition();
    }

    private void HandleBulletTimeTransition()
    {
        if (IsInBulletTime)
        {
            if(Time.timeScale != BulletTimeFactor && _bulletTimeTransitionTimer > 0)
            {
                _bulletTimeTransitionTimer -= Time.unscaledDeltaTime;
                float lerpdTimeScale = Mathf.Lerp(1f, BulletTimeFactor, 1-(_bulletTimeTransitionTimer / BulletTimeTransition));
                Time.timeScale = lerpdTimeScale;
            }
            else
            {
                Time.timeScale = BulletTimeFactor;
            }
        }
        else
        {
            if (Time.timeScale != 1f && _bulletTimeTransitionTimer > 0)
            {
                _bulletTimeTransitionTimer -= Time.unscaledDeltaTime;
                float lerpdTimeScale = Mathf.Lerp(BulletTimeFactor, 1f, 1 - (_bulletTimeTransitionTimer / BulletTimeTransition));
                Time.timeScale = lerpdTimeScale;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }


    }

    private void StartBulletTime()
    {
        IsInBulletTime = true;
        _bulletTimeTransitionTimer = BulletTimeTransition;
        Debug.Log("Bullet time!");
    }

    private void StopBulletTime()
    {
        IsInBulletTime = false;
        _bulletTimeTransitionTimer = BulletTimeTransition;
        Debug.Log("Stop bullet time.");
    }
}
