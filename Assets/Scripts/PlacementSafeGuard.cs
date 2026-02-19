using UnityEngine;

public class PlacementSafeGuard : MonoBehaviour
{

    private int inthewallcounter;

    private EnemySpawner spawner;
    private void Update()
    {
        if (spawner == null)
        {
            spawner = EnemySpawner.instance;
        }

        if (spawner.totalenemyonthemap == 1 && GetComponentInParent<HealthScript>() != null && GetComponentInParent<HealthScript>().HP > 0)
        {
            if (!transform.GetChild(0).gameObject.activeSelf)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            inthewallcounter++;
            if (inthewallcounter > +300)
            {
                transform.parent.GetComponent<EnemyNavigation>().RecycleEnemy();
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            inthewallcounter = 0;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            inthewallcounter++;
            if (inthewallcounter > +300)
            {
                transform.parent.GetComponent<EnemyNavigation>().RecycleEnemy();
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            inthewallcounter = 0;
        }
    }
}
