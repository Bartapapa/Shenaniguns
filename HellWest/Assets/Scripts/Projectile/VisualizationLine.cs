using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationLine : MonoBehaviour
{
    public LineRenderer Line;

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
}
