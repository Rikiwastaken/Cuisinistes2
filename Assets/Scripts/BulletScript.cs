using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public GameObject Emiter;
    public Vector3 Direction;
    public float Speed;
    public float Damage;
    public float Recoil;


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != Emiter.gameObject.layer)
        {
            bullethitlogic(other.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != Emiter.gameObject.layer)
        {
            bullethitlogic(other.gameObject);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Direction * Speed * Time.fixedDeltaTime;
    }

    void bullethitlogic(GameObject collision)
    {

        if (collision.GetComponent<HealthScript>())
        {
            collision.GetComponent<HealthScript>().TakeDamage(Damage);
            //collision.GetComponent<Rigidbody>().AddForce(Direction * Recoil, ForceMode.Impulse);
        }


        Destroy(transform.parent.gameObject);
    }

    public void InitializeBullet(Vector3 direction, float speed, GameObject emiter, float damage, float recoil)
    {
        Emiter = emiter;
        Direction = direction;
        Speed = speed;
        Damage = damage;
        Recoil = recoil;
    }

}
