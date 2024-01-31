using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerShoot3 : MonoBehaviour
{
    [Header("Object refs")]
    public Projectile PlayerBullet;
    public VisualizationLine VisualizationLine;
    private VisualizationLine _currentVisualizationLine;
    private List<VisualizationLine> _shootingLines = new List<VisualizationLine>();

    [Header("Shoot")]
    public Transform RayCastOrigin;
    public LayerMask ShootingMask;
    public float PenetrationStrength = 100f;
    public int MaxNumberOfRicochets = 3;
    private int _currentNumberOfRicochets = 0;
    public Transform ShootPoint;
    public float ShootCooldown = .25f;
    private float _shootCooldownTimer = float.MinValue;
    private bool _canShoot = true;
    private RaycastHit _hit;
    private RaycastHit _screenHit;
    private bool _hasReachedMaxNumberOfRicochets = false;
    private float _visualizedPenetrationStrength = 100f;
    private ProjectileHitData _currentHitData;
    private bool _canCreateVisualizationLine = false;
    private List<ProjectileHitData> _hitData = new List<ProjectileHitData>();

    private void Update()
    {
        HandleShootCooldown();

        if (Player.Instance.CurrentState == Player.PlayerState.ConfirmingFire)
        {
            if (_currentVisualizationLine != null && !_hasReachedMaxNumberOfRicochets)
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

                Vector3 direction = endPoint - _shootingLines[_currentNumberOfRicochets - 1].DestinationPoint();

                if (Physics.Raycast(_shootingLines[_currentNumberOfRicochets-1].DestinationPoint(), direction, out _hit, 500f, ShootingMask))
                {
                    ShootingMaterial shotMat = _hit.collider.GetComponent<ShootingMaterial>();
                    if (shotMat)
                    {
                        _currentVisualizationLine.SetPosition(1, endPoint);
                        _currentVisualizationLine.HitData = new ProjectileHitData(shotMat, direction, _hit.normal, _hit.point);
                    }
                }


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
        if (!_canShoot) return;

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

        Vector3 direction = endPoint - RayCastOrigin.position;

        if (Physics.Raycast(RayCastOrigin.position, direction, out _hit, 500f, ShootingMask))
        {
            ShootingMaterial shotMat = _hit.collider.GetComponent<ShootingMaterial>();
            if (shotMat)
            {
                Player.Instance.TransitionToState(Player.PlayerState.ConfirmingFire);
                CreateVisualizationLine(ShootPoint.position, _hit.point, out _currentVisualizationLine);
                _currentNumberOfRicochets++;
                _hasReachedMaxNumberOfRicochets = false;
                CreateVisualizationLine(_hit.point, endPoint, out _currentVisualizationLine);
                _currentVisualizationLine.HitData = new ProjectileHitData(shotMat, direction, _hit.normal, _hit.point);
            }
            else
            {
                Shoot();
            }
        }
        else
        {
            Shoot();
        }
    }

    public void CreateVisualizationLine(Vector3 from, Vector3 to, out VisualizationLine createdLine)
    {
        createdLine = Instantiate<VisualizationLine>(VisualizationLine, from, Quaternion.identity);
        createdLine.HitData = _currentHitData;
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
        foreach (VisualizationLine line in _shootingLines)
        {
            Destroy(line.gameObject);
        }
        _shootingLines.Clear();
    }

    public void ReturnFire()
    {
        if (_hasReachedMaxNumberOfRicochets)
        {
            _hasReachedMaxNumberOfRicochets = false;
        }
        else
        {
            RemoveVisualizationLine(_currentNumberOfRicochets);
            _currentNumberOfRicochets--;
            _hasReachedMaxNumberOfRicochets = false;
            if (_currentNumberOfRicochets <= 0)
            {
                Player.Instance.TransitionToState(Player.PlayerState.Character);
                ClearVisualizationLines();
                _currentNumberOfRicochets = 0;
            }
            else
            {
                _currentVisualizationLine = _shootingLines[_currentNumberOfRicochets];
            }
        }
    }

    public void SetNewTrajectoryPoint()
    {
        if (_currentNumberOfRicochets < MaxNumberOfRicochets)
        {
            int currentIndex = _currentNumberOfRicochets;
            CreateVisualizationLine(_shootingLines[currentIndex].DestinationPoint(), _screenHit.point, out _currentVisualizationLine);
            _currentNumberOfRicochets++;
            _hasReachedMaxNumberOfRicochets = false;
        }
        else
        {
            _hasReachedMaxNumberOfRicochets = true;
        }
    }

    public void ConfirmFire(List<Vector3> wayPoints)
    {
        if (!_canShoot || !_currentVisualizationLine.CanConfirmShot()) return;

        _canShoot = false;
        _shootCooldownTimer = ShootCooldown;

        Projectile newBullet = Instantiate<Projectile>(PlayerBullet, ShootPoint.position, ShootPoint.rotation);
        newBullet.InitializeProjectile(ShootPoint.forward, PenetrationStrength);
    }
}
