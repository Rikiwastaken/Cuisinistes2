using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxScript : MonoBehaviour
{
    [Header("GlobalSettings")]
    public float rangefordrag;

    public float rangeforpickup;

    private Transform player;

    public float dragspeed;

    public float rotationpersecond;

    public float clipratio;
    public List<GameObject> ammoboxmodel;

    public float healthRestoredByMedkit;

    public float armorgained;
    public AudioClip SFXToPlay;

    private UpgradeScript upgradeScript;

    [Header("LocalSettings")]
    public int ammotype;
    public bool isMedKit;
    public bool isArmor;

    private void Start()
    {
        upgradeScript = UpgradeScript.instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (player == null)
        {
            player = MovementController.instance.transform;
        }

        if (Vector3.Distance(transform.position, player.position) < rangeforpickup)
        {
            if (isMedKit)
            {
                healthRestoredByMedkit *= Mathf.Pow(1f + upgradeScript.DropPowerPerLevel, upgradeScript.DropPowerLevel);
                player.GetComponent<HealthScript>().HP = (int)Mathf.Min(player.GetComponent<HealthScript>().MaxHealth, player.GetComponent<HealthScript>().HP + healthRestoredByMedkit);
                player.GetComponent<HealthScript>().UpdateTexts();
            }
            else if (isArmor)
            {
                armorgained *= Mathf.Pow(1f + upgradeScript.DropPowerPerLevel, upgradeScript.DropPowerLevel);
                player.GetComponent<HealthScript>().currentarmor = (int)Mathf.Min(player.GetComponent<HealthScript>().maxarmor, player.GetComponent<HealthScript>().currentarmor + armorgained);
                player.GetComponent<HealthScript>().UpdateTexts();
            }
            else
            {
                clipratio *= Mathf.Pow(1f + upgradeScript.DropPowerPerLevel, upgradeScript.DropPowerLevel);
                player.GetComponent<ShootScript>().GunList[ammotype].reserveammo = (int)(player.GetComponent<ShootScript>().GunList[ammotype].reserveammo + player.GetComponent<ShootScript>().GunList[ammotype].clipsize * clipratio);

            }
            SoundManager.instance.PlaySFX(SFXToPlay, 0.05f, player);

            Destroy(gameObject);
            ShootScript.instance.InitializeAmmoText();
        }
        else if (Vector3.Distance(transform.position, player.position) < rangefordrag)
        {
            transform.position += (player.position - transform.position).normalized * Time.fixedDeltaTime * dragspeed;
        }

        transform.Rotate(0, rotationpersecond * Time.fixedDeltaTime, 0);
    }

    public void InitializeAmmoBox()
    {
        player = MovementController.instance.transform;

        ammotype = UnityEngine.Random.Range(0, player.GetComponent<ShootScript>().GunList.Count);

        ammoboxmodel[ammotype].SetActive(true);
    }
}
