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

    private void Start()
    {

        HP = MaxHealth;
        isplayer = GetComponent<MovementController>() != null;
        soundManager = SoundManager.instance;
        spawner = EnemySpawner.instance;
        if (isplayer)
        {
            movementController = GetComponent<MovementController>();
            UpdateTexts();
        }
        else
        {
            enemyNavigation = GetComponent<EnemyNavigation>();
        }

    }

    private void Update()
    {
        if (invframecounter > 0)
        {
            invframecounter--;
        }

        if (isplayer)
        {
            if (Time.time > timetillregenstarts && HP < MaxHealth)
            {
                HP += regenpersecond * Time.deltaTime;
                UpdateTexts();
                ManageHurtPostProcessing();
            }
        }

    }

    public void UpdateTexts()
    {
        movementController.HPTMP.text = "HP : " + (int)HP;
        if (currentarmor > 0)
        {
            movementController.HPTMP.text += "\nArmor : " + (int)(currentarmor);
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
            }
            else
            {


                enemyNavigation.engagedPlayer = true;

                if (HP <= 0 && !enemyNavigation.ded)
                {
                    if (enemyNavigation.DeathSound.Count > 0)
                    {
                        soundManager.PlaySFXFromList(enemyNavigation.DeathSound, 0.5f, enemyNavigation.transform);
                    }
                    enemyNavigation.PlayDeathAnim();
                    enemyNavigation.TriggerDrop();
                    spawner.KillEnemy();
                }
                else
                {
                    if (enemyNavigation.DamageSound.Count > 0)
                    {
                        soundManager.PlaySFXFromList(enemyNavigation.DamageSound, 0.5f, enemyNavigation.transform);
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
}
