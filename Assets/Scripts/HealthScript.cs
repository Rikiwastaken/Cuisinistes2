using UnityEngine;

public class HealthScript : MonoBehaviour
{

    public float HP;

    public float MaxHealth;

    private void Start()
    {
        HP = MaxHealth;
    }
    public void TakeDamage(float damage)
    {
        HP -= damage;
        if (GetComponent<EnemyNavigation>() != null)
        {
            GetComponent<EnemyNavigation>().engagedPlayer = true;

            if (HP <= 0 && !GetComponent<EnemyNavigation>().ded)
            {
                GetComponent<EnemyNavigation>().PlayDeathAnim();
            }
            else
            {
                if (!GetComponent<EnemyNavigation>().Canvas.gameObject.activeSelf)
                {
                    GetComponent<EnemyNavigation>().Canvas.gameObject.SetActive(true);
                }
                GetComponent<EnemyNavigation>().Lifebar.fillAmount = HP / MaxHealth;
            }
        }
        if (GetComponent<MovementController>() != null)
        {
            GetComponent<MovementController>().HPTMP.text = "HP : " + HP;
        }
    }
}
