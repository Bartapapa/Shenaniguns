using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletTimer : MonoBehaviour
{
    public float BulletTimeFactor = .1f;
    public float MegaBulletTime = .01f;
    public float BulletTimeTransition = .33f;
    private float _bulletTimeTransitionTimer = float.MinValue;
    public bool IsInBulletTime = false;
    private float _toTimeScale = 0f;

    public void ToggleBulletTime()
    {
        if (IsInBulletTime)
        {
            StopBulletTime();
        }
        else
        {
            MoveToTimeScaleBulletTime(BulletTimeFactor);
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
                float lerpdTimeScale = Mathf.Lerp(1f, _toTimeScale, 1-(_bulletTimeTransitionTimer / BulletTimeTransition));
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
                float lerpdTimeScale = Mathf.Lerp(_toTimeScale, 1f, 1 - (_bulletTimeTransitionTimer / BulletTimeTransition));
                Time.timeScale = lerpdTimeScale;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }


    }

    public void MoveToTimeScaleBulletTime(float toTimeScale)
    {
        IsInBulletTime = true;
        _bulletTimeTransitionTimer = BulletTimeTransition;
        _toTimeScale = BulletTimeFactor;
    }

    public void StopBulletTime()
    {
        IsInBulletTime = false;
        _bulletTimeTransitionTimer = BulletTimeTransition;
        Debug.Log("Stop bullet time.");
    }
}
