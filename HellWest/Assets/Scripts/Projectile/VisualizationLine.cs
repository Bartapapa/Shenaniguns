using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationLine : MonoBehaviour
{
    public LineRenderer Line;
    public ProjectileHitData HitData;
    public bool IsLocked = false;

    public VisualizationLine(ProjectileHitData hitData)
    {
        HitData = hitData;
    }

    private void Update()
    {
        Line.startColor = HandleColor(CanConfirmShot());
        Line.endColor = HandleColor(CanConfirmShot());
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
        if (IsLocked) return Color.yellow;

        if (canShoot)
        {
            return Color.white;
        }
        else
        {
            return Color.red;
        }
    }

    public bool CanConfirmShot()
    {
        if (HitData.HitMaterial == null)
        {
            return true;
        }
        else
        {
            Vector3 toDirection = OriginPoint() - DestinationPoint();
            toDirection = toDirection.normalized;
            return HitData.CanRicochet(toDirection);
        }
    }
}
