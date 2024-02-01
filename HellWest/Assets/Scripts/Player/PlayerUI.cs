using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI MinAngle;
    public TextMeshProUGUI MaxAngle;
    public TextMeshProUGUI CurrentAngle;
    public TextMeshProUGUI BulletsLeft;
    public TextMeshProUGUI RicochetsLeft;

    public Player Player;
    public GameObject BulletMode;

    private void Start()
    {
        if (Player == null)
        {
            Player = Player.Instance;
        }
    }

    private void Update()
    {
        if (BulletMode.activeSelf) HandleBulletMode();
    }

    private void HandleBulletMode()
    {
        BulletsLeft.text = Player.CurrentBullets.ToString();
        RicochetsLeft.text = (Player.PlayerShoot.MaxNumberOfRicochets - Player.PlayerShoot.CurrentNumberOfRicochets).ToString();
        MinAngle.text = Mathf.Acos(Player.PlayerShoot.CurrentVisualizationLine.HitData.HitMaterial.Stats.MinMaxDotProductReflection.x).ToString();
        MaxAngle.text = Mathf.Acos(Player.PlayerShoot.CurrentVisualizationLine.HitData.HitMaterial.Stats.MinMaxDotProductReflection.y).ToString();
        Vector3 toDirection = Player.PlayerShoot.CurrentVisualizationLine.OriginPoint() - Player.PlayerShoot.CurrentVisualizationLine.DestinationPoint();
        float dot = -Vector3.Dot(toDirection, Player.PlayerShoot.CurrentVisualizationLine.HitData.HitNormal);
        CurrentAngle.text = Mathf.Acos(dot).ToString();
    }
}
