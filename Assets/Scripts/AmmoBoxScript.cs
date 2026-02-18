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

    [Header("LocalSettings")]
    public int ammotype;
    public bool isMedKit;
    public bool isArmor;


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
                player.GetComponent<HealthScript>().HP = Mathf.Min(player.GetComponent<HealthScript>().MaxHealth, player.GetComponent<HealthScript>().HP + healthRestoredByMedkit);
                player.GetComponent<HealthScript>().UpdateTexts();
            }
            else if (isArmor)
            {
                player.GetComponent<HealthScript>().currentarmor = Mathf.Min(player.GetComponent<HealthScript>().maxarmor, player.GetComponent<HealthScript>().currentarmor + armorgained);
                player.GetComponent<HealthScript>().UpdateTexts();
            }
            else
            {
                player.GetComponent<ShootScript>().GunList[ammotype].reserveammo += (int)(player.GetComponent<ShootScript>().GunList[ammotype].clipsize * clipratio);

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
