using System;
using System.Collections.Generic;
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
        public GameObject Bulletprefab;
        public float damage;
        public float recoil;
        public float bulletspeed;
        public float GunCD;
        public Vector3 wheretospawnbullet;
        public List<AudioClip> ShootSFX;
        public List<AudioClip> ReloadSFX;
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
            if (GunCoolDown == 0)
            {

                Shoot();


            }
        }

    }

    private void Shoot()
    {
        GunCoolDown = (int)(GunList[currentgun].GunCD / Time.deltaTime);
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

        bulletscript.InitializeBullet(direction, GunList[currentgun].bulletspeed, gameObject, GunList[currentgun].damage, GunList[currentgun].recoil);



        SoundManager.instance.PlaySFXFromList(GunList[currentgun].ShootSFX, 0.05f, transform);

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
