using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static ShootScript;

public class UpgradeScript : MonoBehaviour
{

    public static UpgradeScript instance;
    [Serializable]

    public class UpgradeClass
    {
        public int upgradeID;
        public string upgradeName;
        public Sprite UpgradeSprite;
    }

    [Serializable]

    public class UpgradeCardClass
    {
        public GameObject UpgradeCard;
        public TextMeshProUGUI upgradeName;
        public TextMeshProUGUI upgradeLevel;
        public Image UpgradeImage;
        public Image CompletionCircle;
    }

    [Header("upgrades")]

    public List<UpgradeClass> upgradelist;

    public int HandGunUpgradeLevel;
    public int ARUpgradeLevel;
    public int ShotgunUpgradeLevel;
    public int RaygunUpgradeLevel;
    public int DamageReductionLevel;
    public int GlobalDamageLevel;
    public int RunSpeedLevel;
    public int MagSizeLevel;
    public int DropRateLevel;
    public int JumpHeightLevel;
    public int MaxHPLevel;
    public int RegenLevel;
    public int LifeStealLevel;
    public int CritChanceLevel;
    public int CritDamageLevel;
    public int FreeUpgradeChanceLevel;
    public int AmmoSaveChanceLevel;
    public int MeleeDamageLevel;
    public int DropPowerLevel;
    public int DifficultyLevel;

    [Header("UpgradeDetails")]
    public float GunIncreasePerLevel;
    public float DamageReductionPerLevel;
    public float GlobalDamagePerLevel;
    public float RunSpeedPerLevel;
    public float MagSizePerLevel;
    public float DropRatePerLevel;
    public float JumpHeightPerLevel;
    public float MaxHPPerLevel;
    public float RegenPerLevel;
    public float LifeStealPerLevel;
    public float CritChancePerLevel;
    public float CritDamagePerLevel;
    public float FreeUpgradePerLevel;
    public float AmmoSavePerLevel;
    public float MeleeDamagePerLevel;
    public float DropPowerPerLevel;
    public float DifficultyPerLevel;

    public float basecritchance;
    public float basecritmultiplier;

    private ShootScript ShootScript;

    [Header("BonusCard")]

    public UpgradeCardClass NorthCard;
    private int maintainingNorth;
    private int currentNorthUpgrade;
    public UpgradeCardClass SouthCard;
    private int maintainingSouth;
    private int currentSouthUpgrade;
    public UpgradeCardClass EastCard;
    private int maintainingEast;
    private int currentEastUpgrade;
    public UpgradeCardClass WestCard;
    private int maintainingWest;
    private int currentWestUpgrade;

    public GameObject CardHolder;

    public bool gettingbonus;
    private InputAction MoveAction;

    public float timebeforeconfirm;
    private int framestoconfirm;

    private void Awake()
    {
        instance = this;
    }

    public int distancetotarget;
    public int basepos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShootScript = GetComponent<ShootScript>();
        MoveAction = InputSystem.actions.FindAction("Move");
        framestoconfirm = (int)(timebeforeconfirm * 60);
        InitializeWeaponChoice();

    }

    private void Update()
    {
        if (gettingbonus)
        {
            if (Time.timeScale > 0)
            {
                float newtimescale = Time.timeScale - 1f / 60f;
                if (newtimescale < 0)
                {
                    newtimescale = 0;
                }
                Time.timeScale = newtimescale;
            }
            if (NorthCard.UpgradeCard.transform.localPosition.y > basepos)
            {
                NorthCard.UpgradeCard.transform.localPosition += new Vector3(0f, -distancetotarget / 30, 0f);
            }



            Vector2 moveactionvalue = MoveAction.ReadValue<Vector2>();
            if (moveactionvalue.y > 0f)
            {
                maintainingNorth++;
            }
            else
            {
                maintainingNorth = 0;
            }
            NorthCard.CompletionCircle.fillAmount = (float)maintainingNorth / (float)framestoconfirm;
            if (maintainingNorth > framestoconfirm)
            {
                GetUpgrade(currentNorthUpgrade);
                gettingbonus = false;
                Time.timeScale = 1f;
                CardHolder.SetActive(false);
            }


            if (SouthCard.UpgradeCard.transform.localPosition.y < -basepos)
            {
                SouthCard.UpgradeCard.transform.localPosition += new Vector3(0f, distancetotarget / 30, 0f);
            }

            if (moveactionvalue.y < 0f)
            {
                maintainingSouth++;
            }
            else
            {
                maintainingSouth = 0;
            }
            SouthCard.CompletionCircle.fillAmount = (float)maintainingSouth / (float)framestoconfirm;
            if (maintainingSouth > framestoconfirm)
            {
                GetUpgrade(currentSouthUpgrade);
                gettingbonus = false;
                Time.timeScale = 1f;
                CardHolder.SetActive(false);
            }


            if (EastCard.UpgradeCard.transform.localPosition.x > basepos)
            {
                EastCard.UpgradeCard.transform.localPosition += new Vector3(-distancetotarget / 30, 0f, 0f);
            }

            if (moveactionvalue.x > 0f)
            {
                maintainingEast++;
            }
            else
            {
                maintainingEast = 0;
            }
            EastCard.CompletionCircle.fillAmount = (float)maintainingEast / (float)framestoconfirm;
            if (maintainingEast > framestoconfirm)
            {
                GetUpgrade(currentEastUpgrade);
                gettingbonus = false;
                Time.timeScale = 1f;
                CardHolder.SetActive(false);
            }

            if (WestCard.UpgradeCard.transform.localPosition.x < -basepos)
            {
                WestCard.UpgradeCard.transform.localPosition += new Vector3(distancetotarget / 30, 0f, 0f);
            }


            if (moveactionvalue.x < 0f)
            {
                maintainingWest++;
            }
            else
            {
                maintainingWest = 0;
            }
            WestCard.CompletionCircle.fillAmount = (float)maintainingWest / (float)framestoconfirm;
            if (maintainingWest > framestoconfirm)
            {
                GetUpgrade(currentWestUpgrade);
                gettingbonus = false;
                Time.timeScale = 1f;
                CardHolder.SetActive(false);
            }
        }

    }

    public void GetUpgrade(int upgradeID)
    {
        switch (upgradeID)
        {
            case 0: //handgun
                HandGunUpgradeLevel++;
                if (HandGunUpgradeLevel == 1)
                {
                    ShootScript.GunList[0].unlocked = true;
                    ShootScript.InitializeAmmoText();
                }
                ShootScript.GunList[0].damage *= (1f + GunIncreasePerLevel);
                ShootScript.GunList[0].GunCD /= (1f + GunIncreasePerLevel);
                break;
            case 1: //ar
                ARUpgradeLevel++;
                if (ARUpgradeLevel == 1)
                {
                    ShootScript.GunList[1].unlocked = true;
                    ShootScript.InitializeAmmoText();
                }
                ShootScript.GunList[1].damage *= (1f + GunIncreasePerLevel);
                ShootScript.GunList[1].GunCD /= (1f + GunIncreasePerLevel);
                break;
            case 2: //shotgun
                ShotgunUpgradeLevel++;
                if (ShotgunUpgradeLevel == 1)
                {
                    ShootScript.GunList[2].unlocked = true;
                    ShootScript.InitializeAmmoText();
                }
                ShootScript.GunList[2].damage *= (1f + GunIncreasePerLevel);
                ShootScript.GunList[2].GunCD /= (1f + GunIncreasePerLevel);
                break;
            case 3: //raygun
                RaygunUpgradeLevel++;
                if (RaygunUpgradeLevel == 1)
                {
                    ShootScript.GunList[3].unlocked = true;
                    ShootScript.InitializeAmmoText();
                }
                ShootScript.GunList[3].damage *= (1f + GunIncreasePerLevel);
                ShootScript.GunList[3].GunCD /= (1f + GunIncreasePerLevel);
                break;
            case 4: //damage reduction
                DamageReductionLevel++;

                break;
            case 5: //global damage
                GlobalDamageLevel++;
                foreach (GunClass gun in ShootScript.GunList)
                {
                    gun.damage *= (1f + GlobalDamagePerLevel);
                }
                ShootScript.meleedamage *= (1f + GlobalDamagePerLevel);
                break;
            case 6://speed
                RunSpeedLevel++;
                GetComponent<MovementController>().speed *= (1f + RunSpeedPerLevel);
                break;
            case 7: //mag size
                MagSizeLevel++;
                foreach (GunClass gun in ShootScript.GunList)
                {
                    gun.clipsize *= (1f + MagSizeLevel);
                }
                ShootScript.InitializeAmmoText();
                break;
            case 8: //drop rate
                DropRateLevel++;
                break;
            case 9: //jump height
                JumpHeightLevel++;
                GetComponent<MovementController>().JumpVerticalSpeed *= (1f + JumpHeightPerLevel);
                break;
            case 10: //max hp
                MaxHPLevel++;
                GetComponent<HealthScript>().MaxHealth *= MaxHPPerLevel;
                break;
            case 11: //regen
                RegenLevel++;
                GetComponent<HealthScript>().regenpersecond *= MaxHPPerLevel;
                GetComponent<HealthScript>().regenpersecond /= MaxHPPerLevel;
                break;
            case 12: //life steal
                LifeStealLevel++;
                break;
            case 13: //crit chance
                CritChanceLevel++;
                break;
            case 14: //crit damage
                CritDamageLevel++;
                break;
            case 15: //Free upgrade chance
                FreeUpgradeChanceLevel++;
                break;
            case 16: //ammo save chance
                AmmoSaveChanceLevel++;
                break;
            case 17: //melee damage
                MeleeDamageLevel++;
                ShootScript.meleedamage *= MeleeDamagePerLevel;
                break;
            case 18: //drop power
                DropPowerLevel++;
                break;
            case 19: //difficulty
                DifficultyLevel++;
                break;

        }
    }

    public void InitializeWeaponChoice()
    {
        int northID = 0;
        currentNorthUpgrade = northID;
        InitializeUpgradeCard(NorthCard, currentNorthUpgrade);
        NorthCard.UpgradeCard.transform.localPosition = new Vector3(0f, basepos + distancetotarget, 0f);


        int southID = 1;
        currentSouthUpgrade = southID;
        InitializeUpgradeCard(SouthCard, currentSouthUpgrade);
        SouthCard.UpgradeCard.transform.localPosition = new Vector3(0f, -basepos - distancetotarget, 0f);

        int eastID = 2;
        currentEastUpgrade = eastID;
        InitializeUpgradeCard(EastCard, currentEastUpgrade);
        EastCard.UpgradeCard.transform.localPosition = new Vector3(basepos + distancetotarget, 0f, 0f);

        int westID = 3;
        currentWestUpgrade = westID;
        InitializeUpgradeCard(WestCard, currentWestUpgrade);
        WestCard.UpgradeCard.transform.localPosition = new Vector3(-basepos - distancetotarget, 0f, 0f);


        gettingbonus = true;
        CardHolder.SetActive(true);
    }

    public void InitializeNewBonuses()
    {
        List<int> potentialupgrades = new List<int>();
        foreach (UpgradeClass upgrade in upgradelist)
        {
            potentialupgrades.Add(upgrade.upgradeID);
        }

        int northID = UnityEngine.Random.Range(0, potentialupgrades.Count);
        currentNorthUpgrade = potentialupgrades[northID];
        maintainingNorth = 0;
        potentialupgrades.RemoveAt(northID);
        InitializeUpgradeCard(NorthCard, currentNorthUpgrade);
        NorthCard.UpgradeCard.transform.localPosition = new Vector3(0f, basepos + distancetotarget, 0f);

        int southID = UnityEngine.Random.Range(0, potentialupgrades.Count);
        currentSouthUpgrade = potentialupgrades[southID];
        potentialupgrades.RemoveAt(southID);
        maintainingSouth = 0;
        InitializeUpgradeCard(SouthCard, currentSouthUpgrade);
        SouthCard.UpgradeCard.transform.localPosition = new Vector3(0f, -basepos - distancetotarget, 0f);

        int eastID = UnityEngine.Random.Range(0, potentialupgrades.Count);
        currentEastUpgrade = potentialupgrades[eastID];
        potentialupgrades.RemoveAt(eastID);
        maintainingEast = 0;
        InitializeUpgradeCard(EastCard, currentEastUpgrade);
        EastCard.UpgradeCard.transform.localPosition = new Vector3(basepos + distancetotarget, 0f, 0f);

        int westID = UnityEngine.Random.Range(0, potentialupgrades.Count);
        currentWestUpgrade = potentialupgrades[westID];
        potentialupgrades.RemoveAt(westID);
        maintainingWest = 0;
        InitializeUpgradeCard(WestCard, currentWestUpgrade);
        WestCard.UpgradeCard.transform.localPosition = new Vector3(-basepos - distancetotarget, 0f, 0f);

        gettingbonus = true;
        CardHolder.SetActive(true);
    }

    private void InitializeUpgradeCard(UpgradeCardClass card, int upgradeID)
    {
        UpgradeClass upgrade = upgradelist[upgradeID];
        card.upgradeName.text = upgrade.upgradeName;
        card.upgradeLevel.text = "Level " + (GetUpgradeLevel(upgradeID) + 1);
        card.UpgradeImage.sprite = upgrade.UpgradeSprite;
        card.CompletionCircle.fillAmount = 0;
    }

    public int GetUpgradeLevel(int upgradeID)
    {
        switch (upgradeID)
        {
            case 0: return HandGunUpgradeLevel;
            case 1: return ARUpgradeLevel;
            case 2: return ShotgunUpgradeLevel;
            case 3: return RaygunUpgradeLevel;
            case 4: return DamageReductionLevel;
            case 5: return GlobalDamageLevel;
            case 6: return RunSpeedLevel;
            case 7: return MagSizeLevel;
            case 8: return DropRateLevel;
            case 9: return JumpHeightLevel;
            case 10: return MaxHPLevel;
            case 11: return RegenLevel;
            case 12: return LifeStealLevel;
            case 13: return CritChanceLevel;
            case 14: return CritDamageLevel;
            case 15: return FreeUpgradeChanceLevel;
            case 16: return AmmoSaveChanceLevel;
            case 17: return MeleeDamageLevel;
            case 18: return DropPowerLevel;
            case 19: return DifficultyLevel;
            default:
                Debug.Log("Invalid upgrade ID");
                return -1; // Return -1 for invalid upgrade ID
        }
    }

}
