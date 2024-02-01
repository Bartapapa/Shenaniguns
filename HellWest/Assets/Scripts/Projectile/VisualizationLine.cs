using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ExtensionMethods
{
    public static float Remap (this float value, float inputFrom, float inputTo, float outputFrom, float outputTo)
    {
        return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;
    }
}

public class VisualizationLine : MonoBehaviour
{
    public LineRenderer Line;
    public ProjectileHitData HitData;
    public bool IsLocked = false;
    private float _currentDP = 0f;
    public LayerMask ShootingMask;

    public VisualizationLine(ProjectileHitData hitData)
    {
        HitData = hitData;
    }

    private void Update()
    {
        //Line.startColor = HandleColor(CanConfirmShot());
        //Line.endColor = HandleColor(CanConfirmShot());

        if (IsLocked)
        {
            Line.startColor = Color.white;
            Line.endColor = Color.white;
        }
        else
        {
            Vector3 toDirection = OriginPoint() - DestinationPoint();
            toDirection = toDirection.normalized;

            Line.startColor = GetColor(HitData.HitMaterial.Stats.MinMaxDotProductReflection.x,
                                        HitData.HitMaterial.Stats.MinMaxDotProductReflection.y,
                                        -Vector3.Dot(toDirection, HitData.HitNormal));
            Line.endColor = GetColor(HitData.HitMaterial.Stats.MinMaxDotProductReflection.x,
                                        HitData.HitMaterial.Stats.MinMaxDotProductReflection.y,
                                        -Vector3.Dot(toDirection, HitData.HitNormal));
        }

    }

    public Vector3 OriginPoint()
    {
        return Line.GetPosition(0);
    }

    public Vector3 DestinationPoint()
    {
        return Line.GetPosition(1);
    }

    public void SetPosition(int index, Vector3 to)
    {
        Line.SetPosition(index, to);
    }

    public Color HandleColor(bool canShoot)
    {
        if (canShoot)
        {
            return Color.white;
        }
        else
        {
            return Color.red;
        }
    }

    public Color GetColor(float min, float max, float current)
    {
        if (IsLocked) return Color.white;

        if (CanConfirmShot)
        {
            if (current > max || current < min)
            {
                return Color.red;
            }
            else
            {
                //Find 'half' DP, and use that as a basis for the Color.Lerp?
                float ALPH = current;
                ALPH = ALPH.Remap(min, max, -1f, 1f);
                return Color.Lerp(Color.red, Color.yellow, 1 - (Mathf.Abs(ALPH)));
            }
        }
        else
        {
            return Color.red;
        }
    }

    public bool CanConfirmShot
    {
        get
        {
            if (HitData.HitMaterial == null)
            {
                return false;
            }
            else
            {
                Vector3 toDirection = OriginPoint() - DestinationPoint();
                toDirection = toDirection.normalized;

                Vector3 checkDirection = DestinationPoint() - OriginPoint();
                checkDirection = checkDirection.normalized;

                if (Physics.Raycast(OriginPoint() + checkDirection, checkDirection, Vector3.Distance(OriginPoint(), DestinationPoint()) - 1f, ShootingMask))
                {
                    return false;
                }
                return HitData.CanRicochet(toDirection);
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Vector3 from = OriginPoint();
        //Vector3 toDirection = DestinationPoint() - OriginPoint();
        //toDirection = toDirection.normalized;
        //Vector3 to = OriginPoint() + (toDirection * (Vector3.Distance(OriginPoint(), DestinationPoint()) - 1f));
        //Gizmos.DrawLine(from + toDirection, to);
    }
}
