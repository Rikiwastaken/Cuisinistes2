using UnityEngine;

public class PlacementSafeGuard : MonoBehaviour
{

    private int inthewallcounter;

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
