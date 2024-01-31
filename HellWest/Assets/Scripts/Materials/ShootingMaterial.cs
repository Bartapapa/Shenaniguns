using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingMaterial : MonoBehaviour
{
    public struct MaterialStats
    {
        public float MaterialResistance;
        public Vector2 MinMaxDotProductReflection;

        public MaterialStats(float materialResistance, Vector2 minMaxDotProductReflection)
        {
            MaterialResistance = materialResistance;

            MinMaxDotProductReflection = minMaxDotProductReflection;
        }
    }

    public enum MaterialType
    {
        None,
        ThinWood,
        ThickWood,
        ThinBone,
        ThickBone,
        ThinMetal,
        ThickMetal,
        Glass,
        Fabric,
        BulletProof,
        Custom,
    }

    public MaterialType Type = MaterialType.Custom;
    public MaterialStats Stats;
    public float CustomStatResistance = 0f;
    public Vector2 CustomStatReflection = new Vector2(0, 1);

    private void Awake()
    {
        InitializeStats(Type);
    }
    private void InitializeStats(MaterialType type)
    {
        switch (type)
        {
            case MaterialType.None:
                break;
            case MaterialType.ThinWood:
                Stats = new MaterialStats(10f, new Vector2(.3f, .3f));
                break;
            case MaterialType.ThickWood:
                Stats = new MaterialStats(45f, new Vector2(.3f, .3f));
                break;
            case MaterialType.ThinBone:
                Stats = new MaterialStats(10f, new Vector2(.3f, .5f));
                break;
            case MaterialType.ThickBone:
                Stats = new MaterialStats(45f, new Vector2(.3f, .5f));
                break;
            case MaterialType.ThinMetal:
                Stats = new MaterialStats(30f, new Vector2(.15f, .7f));
                break;
            case MaterialType.ThickMetal:
                Stats = new MaterialStats(70f, new Vector2(.15f, .7f));
                break;
            case MaterialType.Glass:
                Stats = new MaterialStats(0f, new Vector2(0, 0));
                break;
            case MaterialType.Fabric:
                Stats = new MaterialStats(0f, new Vector2(0, 0));
                break;
            case MaterialType.BulletProof:
                Stats = new MaterialStats(999f, new Vector2(0, 0));
                break;
            case MaterialType.Custom:
                Stats = new MaterialStats(CustomStatResistance, CustomStatReflection);
                break;
            default:
                Stats = new MaterialStats(10f, new Vector2(.3f, .75f));
                break;
        }
    }
}
