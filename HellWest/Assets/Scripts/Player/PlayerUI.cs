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
    public GameObject Reticle;
    public GameObject NoReticle;
    public GameObject YOUDIEDEDED;

    public Player Player;
    public GameObject BulletMode;

    private void Start()
    {
        if (Player == null)
        {
            Player = Player.Instance;
        }

        YOUDIEDEDED.SetActive(false);
    }

    private void Update()
    {
        if (BulletMode.activeSelf) HandleBulletMode();
        else
        {
            if (!Reticle.active)
            {
                Debug.Log("can shoot");
                Reticle.SetActive(true);
            }
            if (NoReticle.active)
            {
                Debug.Log("can shoot");
                NoReticle.SetActive(false);
            }
        }
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

        if (Player.PlayerShoot.CurrentVisualizationLine.CanConfirmShot)
        {          
            if (!Reticle.active)
            {
                Debug.Log("can shoot");
                Reticle.SetActive(true);
            }
            if (NoReticle.active)
            {
                Debug.Log("can shoot");
                NoReticle.SetActive(false);
            }
        }
        else
        {           
            if (Reticle.active)
            {
                Debug.Log("cannot shoot");
                Reticle.SetActive(false);
            }
            if (!NoReticle.active)
            {
                Debug.Log("cannot shoot");
                NoReticle.SetActive(true);
            }
        }
    }

    public void DIE()
    {
        YOUDIEDEDED.SetActive(true);
    }
}
