using UnityEngine;
using UnityEngine.Rendering;

public class HealthScript : MonoBehaviour
{

    public float HP;

    public float MaxHealth;

    public float currentarmor;

    public float maxarmor;

    public float invframes;
    private int invframecounter;

    bool isplayer;

    private EnemyNavigation enemyNavigation;

    private MovementController movementController;

    private SoundManager soundManager;

    private EnemySpawner spawner;

    public float timebeforeregen;

    private float timetillregenstarts;

    public float regenpersecond;

    public Volume HurtPostProcessing;

    private UpgradeScript UpgradeScript;

    public float scaledMaxHealth;

    private void Start()
    {


        isplayer = GetComponent<MovementController>() != null;
        if (!isplayer)
        {
            HP = scaledMaxHealth;
        }
        else
        {
            HP = MaxHealth;
        }
        soundManager = SoundManager.instance;
        spawner = EnemySpawner.instance;
        if (isplayer)
        {
            movementController = GetComponent<MovementController>();
            UpgradeScript = GetComponent<UpgradeScript>();
            UpdateTexts();
        }
        else
        {
            UpgradeScript = UpgradeScript.instance;
            movementController = MovementController.instance;
            enemyNavigation = GetComponent<EnemyNavigation>();
        }
    }

    private void Update()
    {
        //bonus
        if (UpgradeScript.gettingbonus)
        {
            return;
        }
        if (invframecounter > 0)
        {
            invframecounter--;
        }

        if (isplayer)
        {
            if (Time.time > timetillregenstarts && HP < MaxHealth)
            {
                HP += regenpersecond * MaxHealth * Time.deltaTime;
                UpdateTexts();
                ManageHurtPostProcessing();
            }
        }

    }

    public void UpdateTexts()
    {
        movementController.HPTMP.text = "HP : " + (int)HP;
        movementController.HPBar.fillAmount = HP / MaxHealth;
        movementController.ArmorBar.fillAmount = currentarmor / maxarmor;
        if (currentarmor > 0)
        {
            movementController.HPTMP.text += "\nArmor : " + (int)(currentarmor);
            movementController.ArmorBG.fillAmount = 1;
        }
        else
        {
            movementController.ArmorBG.fillAmount = 0;
        }
    }

    private void ManageHurtPostProcessing()
    {
        if (HP < MaxHealth / 2)
        {
            HurtPostProcessing.weight = 1 - (HP - 10f) / (MaxHealth / 2f - 10f);
        }
        else
        {
            if (HurtPostProcessing.weight != 0)
            {
                HurtPostProcessing.weight = 0;
            }
        }
    }

    public void TakeDamage(float damage)
    {

        if (isplayer)
        {
            //damagereduction
            damage *= Mathf.Pow(1f - UpgradeScript.DamageReductionPerLevel, UpgradeScript.DamageReductionLevel);
        }
        else
        {
            //crit chance

            int randomvalue = UnityEngine.Random.Range(0, 100);

            if (randomvalue < 100f * (UpgradeScript.basecritchance + UpgradeScript.CritChancePerLevel * UpgradeScript.CritChanceLevel))
            {
                soundManager.PlayCritSFX(transform);
                damage *= UpgradeScript.basecritmultiplier * Mathf.Pow(1f + UpgradeScript.CritDamagePerLevel, UpgradeScript.CritDamageLevel);
            }

            //lifesteal
            movementController.GetComponent<HealthScript>().GainHP(damage * UpgradeScript.LifeStealPerLevel * UpgradeScript.LifeStealLevel);
        }


        if (invframecounter == 0)
        {
            if (currentarmor > 0)
            {
                if (damage <= currentarmor)
                {
                    currentarmor -= damage;
                }
                else
                {

                    HP -= damage - currentarmor;
                    currentarmor = 0;
                }
            }
            else
            {
                HP -= damage;
            }



            if (isplayer)
            {

                timetillregenstarts = Time.time + timebeforeregen;

                soundManager.PlaySFXFromList(movementController.playerDamageSounds, 0.05f, movementController.transform);
                ManageHurtPostProcessing();
                UpdateTexts();
                invframecounter = (int)(invframes / Time.fixedDeltaTime);
                if ((int)HP <= 0)
                {
                    GetComponent<TitleText>().StartGameOverText();
                    return;
                }
            }
            else
            {


                enemyNavigation.engagedPlayer = true;

                if (HP <= 0 && !enemyNavigation.ded)
                {
                    if (enemyNavigation.DeathSound.Count > 0)
                    {
                        soundManager.PlaySFXFromList(enemyNavigation.DeathSound, 0.5f, enemyNavigation.transform, enemyNavigation.DeathVolume);
                    }
                    enemyNavigation.PlayDeathAnim();
                    enemyNavigation.TriggerDrop();
                    spawner.KillEnemy();
                }
                else
                {
                    if (enemyNavigation.DamageSound.Count > 0)
                    {
                        soundManager.PlaySFXFromList(enemyNavigation.DamageSound, 0.5f, enemyNavigation.transform, enemyNavigation.DamageVolume);
                    }
                    if (!enemyNavigation.Canvas.gameObject.activeSelf)
                    {
                        enemyNavigation.Canvas.gameObject.SetActive(true);
                    }
                    enemyNavigation.Lifebar.fillAmount = HP / MaxHealth;
                }
            }
        }

    }

    public void GainHP(float Gain)
    {

        HP = Mathf.Min(HP + Gain, MaxHealth);
        ManageHurtPostProcessing();
        UpdateTexts();
    }
}
