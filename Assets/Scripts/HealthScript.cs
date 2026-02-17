using UnityEngine;

public class HealthScript : MonoBehaviour
{

    public float HP;

    public float MaxHealth;

    public float invframes;
    private int invframecounter;

    bool isplayer;

    private EnemyNavigation enemyNavigation;

    private MovementController movementController;

    private SoundManager soundManager;

    private void Start()
    {
        HP = MaxHealth;
        isplayer = GetComponent<MovementController>() != null;
        soundManager = SoundManager.instance;
        if (isplayer)
        {
            movementController = GetComponent<MovementController>();
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
    }
    public void TakeDamage(float damage)
    {
        if (invframecounter == 0)
        {
            HP -= damage;
            if (isplayer)
            {
                movementController.HPTMP.text = "HP : " + HP;
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
