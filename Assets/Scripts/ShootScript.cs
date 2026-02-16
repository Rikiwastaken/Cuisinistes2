using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{

    public InputAction shootAction;

    [Serializable]
    public class GunClass
    {
        public GameObject GunModel;
        public Vector3 GunPosition;
        public Vector3 GunRotation;
        public Vector3 GunScale = Vector3.one;
        public GameObject Bulletprefab;
        public float damage;
        public float recoil;
        public float bulletspeed;
        public float GunCD;
        public int currentclip;
        public int clipsize;
        public int reserveammo;
        public Vector3 wheretospawnbullet;
        public AnimationClip ShootAnim;
        public AnimationClip ReloadAnim;
        public List<AudioClip> ShootSFX;
        public List<AudioClip> ReloadSFX;
        public TextMeshProUGUI ReserveAmmoTMP;
        public TextMeshProUGUI CurrentClipTMP;

    }

    public Transform MainCamera;

    [Header("GunVariables")]
    public List<GunClass> GunList;
    private int currentgun;
    private GameObject currentGunGO;
    private int GunCoolDown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
        ChangeGun(0);
        InitializeAmmoText();
    }

    private void ChangeGun(int newGunID)
    {
        currentgun = newGunID;

        GunClass activegunclass = GunList[currentgun];
        if (currentGunGO != null)
        {
            DestroyImmediate(currentGunGO);
        }
        currentGunGO = Instantiate(activegunclass.GunModel);
        currentGunGO.transform.parent = MainCamera;
        currentGunGO.transform.localPosition = activegunclass.GunPosition;
        currentGunGO.transform.localScale = activegunclass.GunScale;
        currentGunGO.transform.localRotation = Quaternion.Euler(activegunclass.GunRotation);
        SetLayerAllChildren(currentGunGO.transform, LayerMask.NameToLayer("Weapons"));

    }

    // Update is called once per frame
    void Update()
    {
        if (GunCoolDown > 0)
        {
            GunCoolDown--;
        }
        if (shootAction.ReadValue<float>() != 0)
        {
            if (GunCoolDown == 0 && (currentGunGO.GetComponentInChildren<Animation>().clip != GunList[currentgun].ReloadAnim) || !currentGunGO.GetComponentInChildren<Animation>().isPlaying)
            {
                if (GunList[currentgun].currentclip > 0)
                {
                    Shoot();
                    GunList[currentgun].currentclip--;
                    GunList[currentgun].CurrentClipTMP.text = GunList[currentgun].currentclip + "/" + GunList[currentgun].clipsize;
                }
                else if (GunList[currentgun].reserveammo > 0)
                {
                    Reload();
                }



            }
        }

    }
    private void Reload()
    {

        currentGunGO.GetComponentInChildren<Animation>().clip = GunList[currentgun].ReloadAnim;
        currentGunGO.GetComponentInChildren<Animation>().Play();
        if (GunList[currentgun].reserveammo >= GunList[currentgun].clipsize)
        {
            GunList[currentgun].currentclip = GunList[currentgun].clipsize;
            GunList[currentgun].reserveammo -= GunList[currentgun].clipsize;
        }
        else
        {
            GunList[currentgun].currentclip = GunList[currentgun].reserveammo;
            GunList[currentgun].reserveammo = 0;
        }
        GunList[currentgun].CurrentClipTMP.text = GunList[currentgun].currentclip + "/" + GunList[currentgun].clipsize;
        GunList[currentgun].ReserveAmmoTMP.text = GunList[currentgun].reserveammo + "";

    }

    private void InitializeAmmoText()
    {
        foreach (GunClass gunClass in GunList)
        {
            gunClass.CurrentClipTMP.text = gunClass.currentclip + "/" + gunClass.clipsize;
            gunClass.ReserveAmmoTMP.text = gunClass.reserveammo + "";
        }
    }

    private void Shoot()
    {
        GunCoolDown = (int)(GunList[currentgun].GunCD / Time.deltaTime);
        currentGunGO.GetComponentInChildren<Animation>().clip = GunList[currentgun].ShootAnim;
        currentGunGO.GetComponentInChildren<Animation>().Play();
        Vector3 ScreenCentreCoordinates = new Vector3(0.5f, 0.5f, 0f);
        Vector3 projectileDestination = new Vector3();
        Ray ray = MainCamera.GetComponent<Camera>().ViewportPointToRay(ScreenCentreCoordinates);
        RaycastHit hit;

        // if the raycast collides with an object, then make that our projectile target
        if (Physics.Raycast(ray, out hit))
        {
            projectileDestination = hit.point;
        }
        // if it doesn't hit anything, make our projectile target 1000 away from us (adjust this accordingly)
        else
        {
            projectileDestination = ray.GetPoint(100);
        }

        Vector3 Worldspawn = currentGunGO.transform.position + GunList[currentgun].wheretospawnbullet;

        Vector3 direction = projectileDestination - Worldspawn;

        GameObject newbullet = Instantiate(GunList[currentgun].Bulletprefab, Worldspawn, Quaternion.identity);
        newbullet.transform.forward = currentGunGO.transform.forward;
        BulletScript bulletscript = newbullet.GetComponentInChildren<BulletScript>();

        bulletscript.InitializeBullet(direction, GunList[currentgun].bulletspeed, 0, GunList[currentgun].damage, GunList[currentgun].recoil);


        if (GunList[currentgun].ShootSFX.Count > 0)
        {
            SoundManager.instance.PlaySFXFromList(GunList[currentgun].ShootSFX, 0.05f, transform);
        }


    }

    void SetLayerAllChildren(Transform root, int layer)
    {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            //            Debug.Log(child.name);
            child.gameObject.layer = layer;
        }
    }

}
