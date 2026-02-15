using UnityEngine;

public class HealthScript : MonoBehaviour
{

    public float HP;

    public void TakeDamage(float damage)
    {
        HP -= damage;
        if (GetComponent<EnemyNavigation>() != null)
        {
            GetComponent<EnemyNavigation>().engagedPlayer = true;
        }
    }
}
