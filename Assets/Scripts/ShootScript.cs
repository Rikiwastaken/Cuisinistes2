using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour
{

    public static ShootScript instance;

    private InputAction shootAction;

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

    private float previousshoot;
    public Transform MainCamera;

    [Header("GunVariables")]
    public List<GunClass> GunList;
    private int currentgun;
    private GameObject currentGunGO;
    private int GunCoolDown;
    public List<AudioClip> EmptyCliPSFX;
    private float previousscroll;
    private InputAction WeaponChangeAction;
    private InputAction ReloadAction;
    private float previousReloadInput;
    public List<Image> SelectedSprite;


    private void Awake()
    {
        instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
        WeaponChangeAction = InputSystem.actions.FindAction("ChangeWeapon");
        ReloadAction = InputSystem.actions.FindAction("Reload");
        ChangeGun(0);
        InitializeAmmoText();
    }



    // Update is called once per frame
    void Update()
    {
        //shoot
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
                    if (currentgun == 2)
                    {
                        ShootShotgun();
                    }
                    else
                    {
                        Shoot();
                    }

                    GunList[currentgun].currentclip--;
                    GunList[currentgun].CurrentClipTMP.text = GunList[currentgun].currentclip + "/" + GunList[currentgun].clipsize;
                }
                else if (GunList[currentgun].reserveammo > 0)
                {
                    Reload();
                }
                else if (previousshoot == 0)
                {
                    GunCoolDown = (int)(Mathf.Max(GunList[currentgun].GunCD / Time.deltaTime, GunList[currentgun].ShootAnim.length / Time.deltaTime));
                    previousshoot = 1;
                    if (EmptyCliPSFX.Count > 0)
                    {

                        SoundManager.instance.PlaySFXFromList(EmptyCliPSFX, 0.05f, transform);
                    }
                }


            }
        }
        else
        {
            previousshoot = 0;
        }

        //make gun appear
        if (currentGunGO != null && currentGunGO.transform.localPosition.y < GunList[currentgun].GunPosition.y)
        {
            currentGunGO.transform.localPosition += new Vector3(0f, 5 * Time.deltaTime, 0f);
        }

        //Change gun
        float weaponchangeval = WeaponChangeAction.ReadValue<float>();

        if (weaponchangeval != 0 && previousscroll != weaponchangeval && !currentGunGO.GetComponentInChildren<Animation>().isPlaying)
        {
            if (weaponchangeval > 1)
            {
                if (currentgun < GunList.Count - 1)
                {
                    currentgun++;
                }
                else
                {
                    currentgun = 0;
                }
            }
            else
            {
                if (currentgun > 0)
                {
                    currentgun--;
                }
                else
                {
                    currentgun = GunList.Count - 1;
                }
            }
            ChangeGun(currentgun);
        }
        previousscroll = weaponchangeval;


        //manualReload

        float reloadinput = ReloadAction.ReadValue<float>();

        if (reloadinput != 0 && !currentGunGO.GetComponentInChildren<Animation>().isPlaying && reloadinput != previousReloadInput && GunList[currentgun].currentclip < GunList[currentgun].clipsize && GunList[currentgun].reserveammo > 0)
        {
            Reload();
        }

        previousReloadInput = reloadinput;

    }

    private void UpdateSelectedSprite()
    {
        foreach (Image SelectedSprite in SelectedSprite)
        {
            SelectedSprite.gameObject.SetActive(false);
        }

        SelectedSprite[currentgun].gameObject.SetActive(true);

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
        currentGunGO.transform.localPosition = activegunclass.GunPosition - new Vector3(0, 2, 0);
        currentGunGO.transform.localScale = activegunclass.GunScale;
        currentGunGO.transform.localRotation = Quaternion.Euler(activegunclass.GunRotation);
        SetLayerAllChildren(currentGunGO.transform, LayerMask.NameToLayer("Weapons"));
        UpdateSelectedSprite();
    }
    private void Reload()
    {

        currentGunGO.GetComponentInChildren<Animation>().clip = GunList[currentgun].ReloadAnim;
        currentGunGO.GetComponentInChildren<Animation>().Play();
        int bulletneeded = GunList[currentgun].clipsize - GunList[currentgun].currentclip;
        if (GunList[currentgun].reserveammo >= bulletneeded)
        {
            GunList[currentgun].currentclip = GunList[currentgun].clipsize;
            GunList[currentgun].reserveammo -= bulletneeded;
        }
        else
        {
            GunList[currentgun].currentclip = GunList[currentgun].reserveammo;
            GunList[currentgun].reserveammo = 0;
        }
        GunList[currentgun].CurrentClipTMP.text = GunList[currentgun].currentclip + "/" + GunList[currentgun].clipsize;
        GunList[currentgun].ReserveAmmoTMP.text = GunList[currentgun].reserveammo + "";
        if (GunList[currentgun].ReloadSFX.Count > 0)
        {
            SoundManager.instance.PlaySFXFromList(GunList[currentgun].ReloadSFX, 0.05f, transform);
        }
    }

    public void InitializeAmmoText()
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
        newbullet.transform.parent = currentGunGO.transform;
        newbullet.transform.localPosition = GunList[currentgun].wheretospawnbullet;
        newbullet.transform.forward = currentGunGO.transform.forward;
        BulletScript bulletscript = newbullet.GetComponentInChildren<BulletScript>();

        bulletscript.InitializeBullet(direction, GunList[currentgun].bulletspeed, 0, GunList[currentgun].damage, GunList[currentgun].recoil);


        if (GunList[currentgun].ShootSFX.Count > 0)
        {
            SoundManager.instance.PlaySFXFromList(GunList[currentgun].ShootSFX, 0.05f, transform);
        }


    }

    private void ShootShotgun()
    {

        int pelletCount = 9;
        float spreadAngle = 10f;

        // Set gun cooldown
        GunCoolDown = (int)(GunList[currentgun].GunCD / Time.deltaTime);

        // Play shooting animation
        Animation gunAnimation = currentGunGO.GetComponentInChildren<Animation>();
        gunAnimation.clip = GunList[currentgun].ShootAnim;
        gunAnimation.Play();

        // Calculate projectile destination from center of screen
        Vector3 screenCentre = new Vector3(0.5f, 0.5f, 0f);
        Vector3 projectileDestination;
        Ray ray = MainCamera.GetComponent<Camera>().ViewportPointToRay(screenCentre);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            projectileDestination = hit.point;
        }
        else
        {
            projectileDestination = ray.GetPoint(100); // fallback distance
        }

        // Spawn position for bullets
        Vector3 worldSpawn = currentGunGO.transform.position + GunList[currentgun].wheretospawnbullet;
        Vector3 baseDirection = (projectileDestination - worldSpawn).normalized;

        // Fire multiple pellets
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDirection = GetSpreadDirection(baseDirection, spreadAngle);

            GameObject newBullet = Instantiate(GunList[currentgun].Bulletprefab, worldSpawn, Quaternion.identity);
            newBullet.transform.parent = currentGunGO.transform;
            newBullet.transform.localPosition = GunList[currentgun].wheretospawnbullet;
            newBullet.transform.forward = spreadDirection;

            BulletScript bulletScript = newBullet.GetComponentInChildren<BulletScript>();

            bulletScript.InitializeBullet(spreadDirection, GunList[currentgun].bulletspeed, 0, GunList[currentgun].damage, GunList[currentgun].recoil);
        }

        // Play shooting sound
        if (GunList[currentgun].ShootSFX.Count > 0)
        {
            SoundManager.instance.PlaySFXFromList(GunList[currentgun].ShootSFX, 0.05f, transform);
        }
    }

    /// <summary>
    /// Returns a random direction within a cone around the base direction
    /// </summary>
    private Vector3 GetSpreadDirection(Vector3 direction, float angle)
    {
        // Random circular spread
        float spreadRadius = Mathf.Tan(angle * Mathf.Deg2Rad);
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spreadRadius;

        Vector3 spread = direction + currentGunGO.transform.right * randomCircle.x + currentGunGO.transform.up * randomCircle.y;

        return spread.normalized;
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
