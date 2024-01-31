using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public struct ProjectileHitData
{
    public ShootingMaterial HitMaterial;
    public Vector3 ProjectileDirection;
    public Vector3 HitNormal;
    public Vector3 HitPoint;

    public ProjectileHitData(ShootingMaterial hitMaterial, Vector3 projectileDirection, Vector3 hitNormal, Vector3 hitPoint)
    {
        HitMaterial = hitMaterial;
        ProjectileDirection = projectileDirection;
        HitNormal = hitNormal;
        HitPoint = hitPoint;
    }

    public bool CanRicochet(Vector3 toProjectileDirection)
    {
        if (HitMaterial)
        {
            if (-Vector3.Dot(toProjectileDirection, HitNormal) >= HitMaterial.Stats.MinMaxDotProductReflection.x &&
                -Vector3.Dot(toProjectileDirection, HitNormal) <= HitMaterial.Stats.MinMaxDotProductReflection.y)
            {

                return true;
            }

        }
        return false;

    }

    //public bool CanPenetrate(float projectilePenetrationStrength)
    //{
    //    if (!CanRicochet)
    //    {
    //        if (projectilePenetrationStrength >= HitMaterial.Stats.MaterialResistance)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public Vector3 ExitPoint(float projectilePenetrationStrength)
    //{
    //    if (!CanPenetrate(projectilePenetrationStrength))
    //    {
    //        return Vector3.zero;
    //    }

    //    Vector3 entryPoint = HitPoint;
    //    Vector3 inverseRaycastOriginPoint = HitPoint + (ProjectileDirection * 50f);
    //    RaycastHit exitPointHit;
    //    if (Physics.Raycast(inverseRaycastOriginPoint, -ProjectileDirection, out exitPointHit, 500f))
    //    {
    //        return exitPointHit.point;
    //    }
    //    else
    //    {
    //        return Vector3.zero;
    //    }
    //}
}
//public class PlayerShoot2 : MonoBehaviour
//{
//    [Header("Object refs")]
//    public Projectile PlayerBullet;
//    public VisualizationLine VisualizationLine;
//    private VisualizationLine _currentVisualizationLine;
//    private List<VisualizationLine> _shootingLines = new List<VisualizationLine>();

//    [Header("Shoot")]
//    public Transform RayCastOrigin;
//    public Transform ShootPoint;
//    public LayerMask ShootingMask;
//    public float PenetrationStrength = 100f;
//    public int MaxNumberOfRicochets = 3;
//    public float ShootCooldown = .25f;
//    private int _currentNumberOfRicochets = 0;
//    private float _shootCooldownTimer = float.MinValue;
//    private bool _canShoot = true;

//    private RaycastHit _hit;
//    private RaycastHit _screenHit;
//    private bool _hasReachedMaxNumberOfRicochets = false;
//    private float _visualizedPenetrationStrength = 100f;
//    private ProjectileHitData _currentHitData;
//    private bool _canCreateVisualizationLine = false;
//    private List<ProjectileHitData> _hitData = new List<ProjectileHitData>();
//    private Vector3 _cachedOriginPoint;

//    private void Update()
//    {
//        if(Player.Instance.CurrentState != Player.PlayerState.Dead)
//        {
//            HandleShootCooldown();
//        }

//        if (Player.Instance.CurrentState == Player.PlayerState.Character)
//        {
//            Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
//            RaycastHit rayHit;
//            Vector3 endPoint = Vector3.zero;
//            if (Physics.Raycast(ray, out rayHit))
//            {
//                endPoint = rayHit.point;
//            }
//            else
//            {
//                endPoint = ray.origin + (ray.direction * 500f);
//            }

//            Vector3 direction = endPoint - RayCastOrigin.position;

//            _canCreateVisualizationLine = RaycastAllBullet(RayCastOrigin.position, direction, out _hitData, 500f);
//        }

//        if (Player.Instance.CurrentState == Player.PlayerState.ConfirmingFire)
//        {
//            if(_currentVisualizationLine != null && !_hasReachedMaxNumberOfRicochets)
//            {
//                Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
//                RaycastHit rayHit;
//                Vector3 endPoint = Vector3.zero;
//                if (Physics.Raycast(ray, out rayHit))
//                {
//                    endPoint = rayHit.point;
//                }
//                else
//                {
//                    endPoint = ray.origin + (ray.direction * 500f);
//                }

//                Vector3 direction = endPoint - _cachedOriginPoint;
//                RaycastAllBullet(_cachedOriginPoint, direction, out _hitData, 500f);
//                Vector3 visualizationEndPoint = GetFinalHitData(_hitData, _visualizedPenetrationStrength).HitPoint;
//                _currentVisualizationLine.SetPosition(1, visualizationEndPoint);
//            }
//        }
//    }
//    private void HandleShootCooldown()
//    {
//        if (_shootCooldownTimer > 0)
//        {
//            _shootCooldownTimer -= Time.deltaTime;
//        }
//        else
//        {
//            if (!_canShoot)
//            {
//                _canShoot = true;
//            }
//        }
//    }

//    public void Shoot()
//    {
//        if (!_canShoot) return;

//        _canShoot = false;
//        _shootCooldownTimer = ShootCooldown;

//        Projectile newBullet = Instantiate<Projectile>(PlayerBullet, ShootPoint.position, ShootPoint.rotation);
//        newBullet.InitializeProjectile(ShootPoint.forward, PenetrationStrength);
//    }

//    private bool RaycastAllBullet(Vector3 origin, Vector3 direction, out List<ProjectileHitData> hitDataList, float maxDistance)
//    {
//        hitDataList = new List<ProjectileHitData>();
//        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance, ShootingMask);

//        if (hits.Length > 0)
//        {
//            for (int i = 0; i < hits.Length; i++)
//            {
//                ShootingMaterial shotMat = hits[i].collider.GetComponent<ShootingMaterial>();
//                if (shotMat)
//                {
//                    ProjectileHitData hitData = new ProjectileHitData(shotMat, direction, _hit.normal, _hit.point);
//                    hitDataList.Add(hitData);            
//                }
//            }
//            return true;
//        }
//        return false;
//    }

//    public void CreateVisualizationLine(Vector3 from, Vector3 to, out VisualizationLine createdLine, ProjectileHitData hitData)
//    {
//        createdLine = Instantiate<VisualizationLine>(VisualizationLine, from, Quaternion.identity);
//        createdLine.HitData = hitData;
//        createdLine.SetPosition(0, from);
//        createdLine.SetPosition(1, to);

//        _shootingLines.Add(createdLine);
//    }

//    public void InitiateBulletTrajectoryVisualization()
//    {
//        if (_canCreateVisualizationLine)
//        {
//            //float currentPenetrationStrength = _visualizedPenetrationStrength;

//            //bool canRicochet = false;
//            ProjectileHitData finalHitData = GetFinalHitData(_hitData, _visualizedPenetrationStrength);
//            Vector3 endPoint = finalHitData.HitPoint;
//            //for (int i = 0; i < _hitData.Count; i++)
//            //{
//            //    if (_hitData[i].CanRicochet)
//            //    {
//            //        endPoint = _hitData[i].HitPoint;
//            //        canRicochet = true;
//            //        finalHitData = _hitData[i];
//            //    }
//            //    else
//            //    {
//            //        canRicochet = false;
//            //        if (_hitData[i].CanPenetrate(currentPenetrationStrength))
//            //        {
//            //            currentPenetrationStrength -= _hitData[i].HitMaterial.Stats.MaterialResistance;
//            //        }
//            //    }
//            //}

//            //if (finalHitData.CanRicochet)
//            //{
//            //    CreateVisualizationLine(ShootPoint.position, endPoint, out _currentVisualizationLine, finalHitData);
//            //    _currentNumberOfRicochets++;
//            //    _hasReachedMaxNumberOfRicochets = false;
//            //    CreateVisualizationLine(endPoint, endPoint, out _currentVisualizationLine, finalHitData);
//            //    _cachedOriginPoint = endPoint;
//            //    Player.Instance.TransitionToState(Player.PlayerState.ConfirmingFire);
//            //}
//            //else
//            //{
//            //    Shoot();
//            //}
//        }
//        else
//        {
//            Shoot();
//        }
//    }

//    private ProjectileHitData GetFinalHitData(List<ProjectileHitData> hitData, float visualizedPenetrationStrength)
//    {
//        ProjectileHitData finalHitData = new ProjectileHitData();
//        float currentPenetrationStrength = visualizedPenetrationStrength;

//        for (int i = 0; i < _hitData.Count; i++)
//        {
//            if (_hitData[i].CanRicochet)
//            {
//                finalHitData = _hitData[i];
//                break;
//            }
//            else
//            {
//                if (_hitData[i].CanPenetrate(currentPenetrationStrength))
//                {
//                    currentPenetrationStrength -= _hitData[i].HitMaterial.Stats.MaterialResistance;
//                }
//                else
//                {
//                    finalHitData = _hitData[i];
//                    break;
//                }
//            }
//        }
//        return finalHitData;
//    }

//    public void ConfirmVisualizationLine()
//    {
//        if (_currentNumberOfRicochets < MaxNumberOfRicochets)
//        {
//            _currentVisualizationLine.HitData = GetFinalHitData(_hitData, _visualizedPenetrationStrength);
//            int currentIndex = _currentNumberOfRicochets;
//            CreateVisualizationLine(_currentVisualizationLine.DestinationPoint(), _currentVisualizationLine.DestinationPoint(), out _currentVisualizationLine, GetFinalHitData(_hitData, _visualizedPenetrationStrength));
//            _currentNumberOfRicochets++;
//            _hasReachedMaxNumberOfRicochets = false;
//        }
//        else
//        {
//            _currentVisualizationLine.HitData = GetFinalHitData(_hitData, _visualizedPenetrationStrength);
//            //_currentVisualizationLine = null;
//            _hasReachedMaxNumberOfRicochets = true;
//        }
//    }
//}
