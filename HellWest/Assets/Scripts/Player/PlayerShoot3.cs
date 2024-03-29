using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerShoot3 : MonoBehaviour
{
    [Header("Object refs")]
    public Projectile PlayerBullet;
    public VisualizationLine VisualizationLine;
    public VisualizationLine CurrentVisualizationLine;
    [HideInInspector] public List<VisualizationLine> ShootingLines = new List<VisualizationLine>();
    public ParticleSystem GunSmoke;

    [Header("Shoot")]
    public Transform RayCastOrigin;
    public LayerMask ShootingMask;
    public float PenetrationStrength = 100f;
    public int MaxNumberOfRicochets = 3;
    public int CurrentNumberOfRicochets = 0;
    public Transform ShootPoint;
    public Transform DetachedShootPoint;
    public float ShootCooldown = .25f;
    private float _shootCooldownTimer = float.MinValue;
    private bool _canShoot = true;
    private RaycastHit _hit;
    private RaycastHit _screenHit;
    private bool _hasReachedMaxNumberOfRicochets = false;
    private float _visualizedPenetrationStrength = 100f;
    private ProjectileHitData _cachedHitData;
    private bool _canCreateVisualizationLine = false;
    private List<ProjectileHitData> _hitData = new List<ProjectileHitData>();

    private void Update()
    {
        HandleShootCooldown();

        if (Player.Instance.CurrentState == Player.PlayerState.ConfirmingFire)
        {
            if (CurrentVisualizationLine != null && !_hasReachedMaxNumberOfRicochets)
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
                Vector3 endPoint = Vector3.zero;
                if (Physics.Raycast(ray, out _screenHit, 500f, ShootingMask))
                {
                    endPoint = _screenHit.point;
                    ShootPoint.LookAt(endPoint);
                    DetachedShootPoint.LookAt(ShootingLines[0].DestinationPoint());
                }
                else if (Physics.Raycast(ray, out _screenHit, 500f))
                {
                    endPoint = _screenHit.point;
                    ShootPoint.LookAt(endPoint);
                    DetachedShootPoint.LookAt(ShootingLines[0].DestinationPoint());
                }
                else
                {
                    endPoint = ray.origin + (ray.direction * 500f);
                    ShootPoint.LookAt(endPoint);
                    DetachedShootPoint.LookAt(ShootingLines[0].DestinationPoint());
                }

                Vector3 direction = endPoint - ShootingLines[CurrentNumberOfRicochets - 1].DestinationPoint();

                if (Physics.Raycast(ShootingLines[CurrentNumberOfRicochets-1].DestinationPoint(), direction, out _hit, 500f, ShootingMask))
                {
                    ShootingMaterial shotMat = _hit.collider.GetComponent<ShootingMaterial>();
                    if (shotMat)
                    {
                        CurrentVisualizationLine.SetPosition(1, endPoint);
                        _cachedHitData = new ProjectileHitData(shotMat, direction, _hit.normal, _hit.point);
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
        if (!_canShoot || Player.Instance.CurrentBullets <= 0) return;

        _canShoot = false;
        _shootCooldownTimer = ShootCooldown;
        Player.Instance.CurrentBullets--;

        Projectile newBullet = Instantiate<Projectile>(PlayerBullet, DetachedShootPoint.position, DetachedShootPoint.rotation);
        newBullet.InitializeProjectile(DetachedShootPoint.forward, PenetrationStrength);
        ParticleSystem newGunSmoke = Instantiate<ParticleSystem>(GunSmoke, DetachedShootPoint.position, Quaternion.identity);
    }

    public void VisualizeProjectileTrajectory()
    {
        if (!_canShoot) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        Vector3 endPoint = Vector3.zero;
        if (Physics.Raycast(ray, out _screenHit, 500f, ShootingMask))
        {
            endPoint = _screenHit.point;
            ShootPoint.LookAt(endPoint);
            DetachedShootPoint.LookAt(endPoint);
        }
        else if (Physics.Raycast(ray, out _screenHit, 500f))
        {
            endPoint = _screenHit.point;
            ShootPoint.LookAt(endPoint);
            DetachedShootPoint.LookAt(endPoint);
        }
        else
        {
            endPoint = ray.origin + (ray.direction * 500f);
            ShootPoint.LookAt(endPoint);
            DetachedShootPoint.LookAt(endPoint);
        }

        Vector3 direction = endPoint - ray.origin;

        if (Physics.Raycast(ray.origin, direction, out _hit, 500f, ShootingMask))
        {
            ShootingMaterial shotMat = _hit.collider.GetComponent<ShootingMaterial>();
            if (shotMat)
            {
                Player.Instance.TransitionToState(Player.PlayerState.ConfirmingFire);
                CreateVisualizationLine(ray.origin, _hit.point, out CurrentVisualizationLine);
                CurrentNumberOfRicochets++;
                _hasReachedMaxNumberOfRicochets = false;
                CurrentVisualizationLine.IsLocked = true;
                CreateVisualizationLine(_hit.point, endPoint, out CurrentVisualizationLine);
                CurrentVisualizationLine.HitData = new ProjectileHitData(shotMat, direction, _hit.normal, _hit.point);
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
        createdLine.SetPosition(0, from);
        createdLine.SetPosition(1, to);

        ShootingLines.Add(createdLine);
    }

    public void RemoveVisualizationLine(int index)
    {
        Destroy(ShootingLines[index].gameObject);
        ShootingLines.RemoveAt(index);
    }

    public void ClearVisualizationLines()
    {
        foreach (VisualizationLine line in ShootingLines)
        {
            Destroy(line.gameObject);
        }
        ShootingLines.Clear();
    }

    public void ReturnFire()
    {
        if (_hasReachedMaxNumberOfRicochets)
        {
            _hasReachedMaxNumberOfRicochets = false;
            CurrentVisualizationLine.IsLocked = false;
        }
        else
        {
            RemoveVisualizationLine(CurrentNumberOfRicochets);
            CurrentNumberOfRicochets--;
            _hasReachedMaxNumberOfRicochets = false;
            if (CurrentNumberOfRicochets <= 0)
            {
                Player.Instance.TransitionToState(Player.PlayerState.Character);
                ClearVisualizationLines();
                CurrentNumberOfRicochets = 0;
            }
            else
            {
                CurrentVisualizationLine = ShootingLines[CurrentNumberOfRicochets];
                CurrentVisualizationLine.IsLocked = false;
            }
        }
    }

    public void SetNewTrajectoryPoint()
    {
        if (!CurrentVisualizationLine.CanConfirmShot) return;

        if (CurrentNumberOfRicochets < MaxNumberOfRicochets)
        {
            //_currentVisualizationLine.HitData = _cachedHitData;
            CurrentVisualizationLine.IsLocked = true;

            int currentIndex = CurrentNumberOfRicochets;
            CreateVisualizationLine(ShootingLines[currentIndex].DestinationPoint(), _screenHit.point, out CurrentVisualizationLine);
            CurrentVisualizationLine.HitData = _cachedHitData;
            CurrentNumberOfRicochets++;
            _hasReachedMaxNumberOfRicochets = false;
        }
        else
        {
            CurrentVisualizationLine.IsLocked = true;
            _hasReachedMaxNumberOfRicochets = true;
        }
    }

    public void ConfirmFire(List<Vector3> wayPoints)
    {
        if (!_canShoot || Player.Instance.CurrentBullets <= 0) return;

        _canShoot = false;
        _shootCooldownTimer = ShootCooldown;
        Player.Instance.CurrentBullets--;

        Projectile newBullet = Instantiate<Projectile>(PlayerBullet, DetachedShootPoint.position, DetachedShootPoint.rotation);
        newBullet.InitializeProjectile(DetachedShootPoint.forward, PenetrationStrength);
        newBullet.WayPoints = wayPoints;
        ParticleSystem newGunSmoke = Instantiate<ParticleSystem>(GunSmoke, DetachedShootPoint.position, Quaternion.identity);

        Player.Instance.TransitionToState(Player.PlayerState.Character);
        ClearVisualizationLines();
        CurrentNumberOfRicochets = 0;
    }
}
