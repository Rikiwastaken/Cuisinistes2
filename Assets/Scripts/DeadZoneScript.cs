using UnityEngine;

public class DeadZoneScript : MonoBehaviour
{

    public float PushStrength;
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<MovementController>() != null)
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, PushStrength, 0), ForceMode.VelocityChange);
            collision.transform.GetComponent<HealthScript>().TakeDamage(damage);
        }
        else if (collision.transform.GetComponent<EnemyNavigation>() != null)
        {
            collision.transform.GetComponent<EnemyNavigation>().PlayDeathAnim();
        }
    }
}
