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

    [Header("LocalSettings")]
    public int ammotype;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) < rangeforpickup)
        {
            player.GetComponent<ShootScript>().GunList[ammotype].reserveammo += (int)(player.GetComponent<ShootScript>().GunList[ammotype].clipsize * clipratio);
            SoundManager.instance.PlaySFXFromList(player.GetComponent<ShootScript>().GunList[ammotype].ReloadSFX, 0.05f, transform);
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
