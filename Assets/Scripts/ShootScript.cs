using System;
using System.Collections;
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
        public string name;
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
        public float clipsize;
        public int reserveammo;
        public Vector3 wheretospawnbullet;
        public AnimationClip ShootAnim;
        public AnimationClip ReloadAnim;
        public List<AudioClip> ShootSFX;
        public List<AudioClip> ReloadSFX;
        public List<AudioClip> EmptyClipSFX;
        public bool unlocked;
        public TextMeshProUGUI ReserveAmmoTMP;
        public TextMeshProUGUI CurrentClipTMP;


    }

    private float previousshoot;
    public Transform MainCamera;

    [Header("GunVariables")]
    public List<GunClass> GunList;

    private int currentgun;
    public GameObject currentGunGO;
    private bool GunCoolDown;
    private float previousscroll;
    private InputAction WeaponChangeAction;
    private InputAction ReloadAction;
    private float previousReloadInput;
    public List<Image> SelectedSprite;
    private GameObject RayGunLaser;

    [Header("Melee")]
    public GameObject MeleeGO;
    private InputAction MeleeAction;
    public Animation MeleeAnim;
    public float meleedamage;
    public float meleerange;
    private float previousmeleevalue;

    private UpgradeScript upgradeScript;

    private bool awaitfirstweapon = true;

    private void Awake()
    {
        instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeScript = GetComponent<UpgradeScript>();
        shootAction = InputSystem.actions.FindAction("Shoot");
        WeaponChangeAction = InputSystem.actions.FindAction("ChangeWeapon");
        ReloadAction = InputSystem.actions.FindAction("Reload");
        MeleeAction = InputSystem.actions.FindAction("Melee");
        InitializeAmmoText();
    }



    // Update is called once per frame
    void Update()
    {
        //bonus
        if (upgradeScript.gettingbonus)
        {
            return;
        }

        if (awaitfirstweapon)
        {
            Getfirstgun();
            awaitfirstweapon = false;
        }

        //shoot
        if (shootAction.ReadValue<float>() != 0)
        {
            if (!GunCoolDown && (currentGunGO.GetComponentInChildren<Animation>().clip != GunList[currentgun].ReloadAnim || !currentGunGO.GetComponentInChildren<Animation>().isPlaying))
            {
                if (GunList[currentgun].currentclip > 0)
                {
                    if (currentgun == 2)
                    {
                        ShootShotgun();
                    }
                    else if (currentgun == 3)
                    {
                        ShootRayGun();
                    }
                    else
                    {
                        Shoot();
                    }
                    int randomchancetosavebullet = UnityEngine.Random.Range(0, 100);
                    if (randomchancetosavebullet > upgradeScript.AmmoSaveChanceLevel * upgradeScript.AmmoSavePerLevel)
                    {
                        GunList[currentgun].currentclip--;
                    }
                    GunList[currentgun].CurrentClipTMP.text = GunList[currentgun].currentclip + "/" + (int)GunList[currentgun].clipsize;
                }
                else if (GunList[currentgun].reserveammo > 0)
                {
                    Reload();
                }
                else if (previousshoot == 0)
                {
                    StartCoroutine(GunCD(GunList[currentgun]));
                    previousshoot = 1;
                    if (GunList[currentgun].EmptyClipSFX.Count > 0)
                    {

                        SoundManager.instance.PlaySFXFromList(GunList[currentgun].EmptyClipSFX, 0.05f, transform);
                    }
                }
                if (GunList[currentgun].reserveammo == 0 && GunList[currentgun].currentclip == 0)
                {
                    if (currentGunGO != null && currentGunGO.GetComponent<AudioSource>() != null && currentGunGO.GetComponent<AudioSource>().isPlaying)
                    {
                        currentGunGO.GetComponent<AudioSource>().Stop();
                    }
                }

            }
        }
        else
        {
            previousshoot = 0;
            if (currentGunGO != null && currentGunGO.GetComponent<AudioSource>() != null && currentGunGO.GetComponent<AudioSource>().isPlaying)
            {
                currentGunGO.GetComponent<AudioSource>().Stop();
            }
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
                int newvalue = -1;
                for (int i = currentgun + 1; i < GunList.Count; i++)
                {
                    if (GunList[i].unlocked)
                    {
                        newvalue = i;
                        break;
                    }
                }
                if (newvalue < 0)
                {
                    for (int i = 0; i < currentgun + 1; i++)
                    {
                        if (GunList[i].unlocked)
                        {
                            newvalue = i;
                            break;
                        }
                    }
                }
                currentgun = newvalue;
            }
            else
            {
                int newvalue = -1;
                for (int i = currentgun - 1; i >= 0; i--)
                {
                    if (GunList[i].unlocked)
                    {
                        newvalue = i;
                        break;
                    }
                }
                if (newvalue < 0)
                {
                    for (int i = GunList.Count - 1; i >= currentgun; i--)
                    {
                        if (GunList[i].unlocked)
                        {
                            newvalue = i;
                            break;
                        }
                    }
                }
                currentgun = newvalue;
            }
            ChangeGun(currentgun);
        }
        previousscroll = weaponchangeval;


        //manualReload

        float reloadinput = ReloadAction.ReadValue<float>();

        if (reloadinput != 0 && !currentGunGO.GetComponentInChildren<Animation>().isPlaying && reloadinput != previousReloadInput && GunList[currentgun].currentclip < (int)GunList[currentgun].clipsize && GunList[currentgun].reserveammo > 0)
        {
            Reload();
        }

        previousReloadInput = reloadinput;


        //melee
        float meleevalue = MeleeAction.ReadValue<float>();
        if (meleevalue != 0 && meleevalue != previousmeleevalue && !MeleeAnim.isPlaying)
        {
            MeleeAnim.Play();
            List<GameObject> enemylist = new List<GameObject>();
            foreach (GameObject enemy in EnemySpawner.instance.SpawnedEnemylist)
            {
                enemylist.Add(enemy);
            }
            foreach (GameObject enemy in enemylist)
            {
                if (enemy != null && enemy.activeSelf && Vector3.Distance(transform.position, enemy.transform.position) <= meleerange)
                {
                    enemy.GetComponent<HealthScript>().TakeDamage(meleedamage);
                }
            }
        }
        previousmeleevalue = meleevalue;
    }


    private IEnumerator GunCD(GunClass Gun)
    {
        GunCoolDown = true;
        yield return new WaitForSeconds(Gun.GunCD);
        GunCoolDown = false;
        if (currentGunGO.GetComponentInChildren<LaserScript>() != null)
        {
            currentGunGO.GetComponentInChildren<LaserScript>().gameObject.SetActive(false);
        }
    }

    public void Getfirstgun()
    {
        for (int i = 0; i < GunList.Count; i++)
        {
            if (GunList[i].unlocked)
            {
                ChangeGun(i);
                break;
            }
        }
    }

    public void UnlockWeapon(int i)
    {
        GunList[i].unlocked = true;
        InitializeAmmoText();

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
        currentGunGO.transform.localPosition = activegunclass.GunPosition + new Vector3(0, -1, 0);
        currentGunGO.transform.localScale = activegunclass.GunScale;
        currentGunGO.transform.localRotation = Quaternion.Euler(activegunclass.GunRotation);
        SetLayerAllChildren(currentGunGO.transform, LayerMask.NameToLayer("Weapons"));
        UpdateSelectedSprite();
        if (currentgun == 3)
        {
            RayGunLaser = currentGunGO.transform.GetChild(1).gameObject;
        }
    }
    private void Reload()
    {
        if (currentGunGO != null && currentGunGO.GetComponent<AudioSource>() != null && currentGunGO.GetComponent<AudioSource>().isPlaying)
        {
            currentGunGO.GetComponent<AudioSource>().Stop();
        }
        currentGunGO.GetComponentInChildren<Animation>().clip = GunList[currentgun].ReloadAnim;
        currentGunGO.GetComponentInChildren<Animation>().Play();
        int bulletneeded = (int)GunList[currentgun].clipsize - GunList[currentgun].currentclip;
        if (GunList[currentgun].reserveammo >= bulletneeded)
        {
            GunList[currentgun].currentclip = (int)GunList[currentgun].clipsize;
            GunList[currentgun].reserveammo -= bulletneeded;
        }
        else
        {
            GunList[currentgun].currentclip = GunList[currentgun].reserveammo;
            GunList[currentgun].reserveammo = 0;
        }
        GunList[currentgun].CurrentClipTMP.text = GunList[currentgun].currentclip + "/" + (int)GunList[currentgun].clipsize;
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
            if (!gunClass.unlocked)
            {
                if (gunClass.CurrentClipTMP.transform.parent.gameObject.activeSelf)
                {
                    gunClass.CurrentClipTMP.transform.parent.gameObject.SetActive(false);
                }

            }
            else
            {
                if (!gunClass.CurrentClipTMP.transform.parent.gameObject.activeSelf)
                {
                    gunClass.CurrentClipTMP.transform.parent.gameObject.SetActive(true);
                }
            }
        }
        foreach (GunClass gunClass in GunList)
        {
            gunClass.CurrentClipTMP.text = gunClass.currentclip + "/" + gunClass.clipsize;
            gunClass.ReserveAmmoTMP.text = gunClass.reserveammo + "";
        }
    }

    private void Shoot()
    {
        StartCoroutine(GunCD(GunList[currentgun]));
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

        Vector3 direction = (projectileDestination - currentGunGO.transform.position).normalized;

        GameObject newbullet = Instantiate(GunList[currentgun].Bulletprefab);
        newbullet.transform.parent = currentGunGO.transform;
        newbullet.transform.localPosition = GunList[currentgun].wheretospawnbullet;
        newbullet.transform.parent = null;
        newbullet.transform.forward = currentGunGO.transform.forward;
        BulletScript bulletscript = newbullet.GetComponentInChildren<BulletScript>();

        bulletscript.InitializeBullet(direction, GunList[currentgun].bulletspeed + GetComponent<Rigidbody>().linearVelocity.magnitude, 0, GunList[currentgun].damage, GunList[currentgun].recoil);


        if (GunList[currentgun].ShootSFX.Count > 0)
        {
            SoundManager.instance.PlaySFXFromList(GunList[currentgun].ShootSFX, 0.05f, transform);
        }


    }

    private void ShootRayGun()
    {
        StartCoroutine(GunCD(GunList[currentgun]));
        currentGunGO.GetComponentInChildren<Animation>().clip = GunList[currentgun].ShootAnim;
        currentGunGO.GetComponentInChildren<Animation>().Play();
        if (!RayGunLaser.GetComponent<LaserScript>().gameObject.activeSelf)
        {
            RayGunLaser.GetComponent<LaserScript>().gameObject.SetActive(true);
        }
        RayGunLaser.GetComponent<LaserScript>().ResetList();
        RayGunLaser.GetComponent<LaserScript>().Damage = GunList[currentgun].damage;

        if (!currentGunGO.GetComponent<AudioSource>().isPlaying)
        {
            currentGunGO.GetComponent<AudioSource>().Play();
        }


    }

    private void ShootShotgun()
    {

        int pelletCount = 9;
        float spreadAngle = 10f;

        // Set gun cooldown
        StartCoroutine(GunCD(GunList[currentgun]));

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
        Vector3 baseDirection = (projectileDestination - currentGunGO.transform.position).normalized;

        // Fire multiple pellets
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDirection = GetSpreadDirection(baseDirection, spreadAngle);

            GameObject newBullet = Instantiate(GunList[currentgun].Bulletprefab, Vector3.zero, Quaternion.identity);
            newBullet.transform.parent = currentGunGO.transform;
            //newBullet.transform.localPosition = GunList[currentgun].wheretospawnbullet;
            newBullet.transform.localPosition = GunList[currentgun].wheretospawnbullet;
            newBullet.transform.forward = spreadDirection;

            BulletScript bulletScript = newBullet.GetComponentInChildren<BulletScript>();

            bulletScript.InitializeBullet(spreadDirection, GunList[currentgun].bulletspeed + GetComponent<Rigidbody>().linearVelocity.magnitude, 0, GunList[currentgun].damage, GunList[currentgun].recoil);
            newBullet.transform.parent = null;

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
