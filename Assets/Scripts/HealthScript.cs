using UnityEngine;

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

    public float timebeforeregen;

    private float timetillregenstarts;

    public float regenpersecond;

    private void Start()
    {

        HP = MaxHealth;
        isplayer = GetComponent<MovementController>() != null;
        soundManager = SoundManager.instance;
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
            }
        }

    }

    private void UpdateTexts()
    {
        movementController.HPTMP.text = "HP : " + (int)HP;
        if (currentarmor > 0)
        {
            movementController.HPTMP.text += "\nArmor : " + (int)(currentarmor);
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
