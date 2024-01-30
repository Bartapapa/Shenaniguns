using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Object refs")]
    public Projectile PlayerBullet;
    public VisualizationLine VisualizationLine;
    private VisualizationLine _currentVisualizationLine;
    private List<VisualizationLine> _shootingLines = new List<VisualizationLine>();

    [Header("Shoot")]
    public float PenetrationStrength = 100f;
    public int MaxNumberOfRicochets = 3;
    private int _currentNumberOfRicochets = 0;
    public Transform ShootPoint;
    public float ShootCooldown = .25f;
    private float _shootCooldownTimer = float.MinValue;
    private bool _canShoot = true;
    private RaycastHit _hit;
    private RaycastHit _screenHit;

    private void Update()
    {
        HandleShootCooldown();

        if(Player.Instance.CurrentState == Player.PlayerState.ConfirmingFire)
        {
            if (_currentVisualizationLine != null)
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
                Vector3 endPoint = Vector3.zero;
                if (Physics.Raycast(ray, out _screenHit))
                {
                    endPoint = _screenHit.point;
                }
                else
                {
                    endPoint = ray.origin + (ray.direction * 500f);
                }

                _currentVisualizationLine.SetPosition(1, endPoint);
            }
        }
    }

    private void HandleShootCooldown()
    {
        if (_shootCooldownTimer > 0)
        {
            _shootCooldownTimer -= Time.deltaTime;
        }
        else
        {
            if (!_canShoot)
            {
                _canShoot = true;
            }
        }
    }

    public void Shoot()
    {
        if (!_canShoot) return;

        _canShoot = false;
        _shootCooldownTimer = ShootCooldown;

        Projectile newBullet = Instantiate<Projectile>(PlayerBullet, ShootPoint.position, ShootPoint.rotation);
        newBullet.InitializeProjectile(ShootPoint.forward, PenetrationStrength);
    }

    public void VisualizeProjectileTrajectory()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        RaycastHit rayHit;
        Vector3 endPoint = Vector3.zero;
        if (Physics.Raycast(ray, out rayHit))
        {
            endPoint = rayHit.point;
        }
        else
        {
            endPoint = ray.origin + (ray.direction * 500f);
        }

        Vector3 direction =  endPoint - ShootPoint.position; 
        if (Physics.Raycast(ShootPoint.position, direction, out _hit, 500f))
        {
            Player.Instance.TransitionToState(Player.PlayerState.ConfirmingFire);
            CreateVisualizationLine(ShootPoint.position, _hit.point);
            _currentNumberOfRicochets++;
            CreateVisualizationLine(_hit.point, endPoint, out _currentVisualizationLine);
        }
    }

    public void CreateVisualizationLine(Vector3 from, Vector3 to)
    {
        VisualizationLine newVisualizationLine = Instantiate<VisualizationLine>(VisualizationLine, from, Quaternion.identity);
        newVisualizationLine.SetPosition(0, from);
        newVisualizationLine.SetPosition(1, to);

        _shootingLines.Add(newVisualizationLine);
    }

    public void CreateVisualizationLine(Vector3 from, Vector3 to, out VisualizationLine createdLine)
    {
        createdLine = Instantiate<VisualizationLine>(VisualizationLine, from, Quaternion.identity);
        createdLine.SetPosition(0, from);
        createdLine.SetPosition(1, to);

        _shootingLines.Add(createdLine);
    }

    public void RemoveVisualizationLine(int index)
    {
        Destroy(_shootingLines[index].gameObject);
        _shootingLines.RemoveAt(index);
    }

    public void ClearVisualizationLines()
    {
        foreach(VisualizationLine line in _shootingLines)
        {
            Destroy(line.gameObject);
        }
        _shootingLines.Clear();
    }

    public void ReturnFire()
    {
        RemoveVisualizationLine(_currentNumberOfRicochets);
        _currentNumberOfRicochets--;
        if (_currentNumberOfRicochets <= 0)
        {
            Player.Instance.TransitionToState(Player.PlayerState.Character);
            _currentNumberOfRicochets = 0;
        }
        else
        {
            _currentVisualizationLine = _shootingLines[_currentNumberOfRicochets];
        }
    }

    public void SetNewTrajectoryPoint()
    {
        //Check if can set
        int currentIndex = _currentNumberOfRicochets;
        CreateVisualizationLine(_shootingLines[currentIndex].DestinationPoint(), _screenHit.point, out _currentVisualizationLine);
        _currentNumberOfRicochets++;
    }
}
