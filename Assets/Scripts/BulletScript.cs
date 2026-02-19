using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public int EmiterType; // 0 = player, 1=enemy
    public Vector3 Direction;
    public float Speed;
    public float Damage;
    public float Recoil;

    public bool ishandgun;
    public List<GameObject> hitbyhandgun = new List<GameObject>();

    private int duration;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<BulletScript>() != null)
        {
            return;
        }
        if (hitbyhandgun != null && hitbyhandgun.Count > 0 && hitbyhandgun.Contains(other.gameObject))
        {
            return;
        }
        if ((other.gameObject.GetComponent<MovementController>() != null && EmiterType == 1) || (other.gameObject.GetComponent<EnemyNavigation>() != null && EmiterType == 0))
        {
            bullethitlogic(other.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<BulletScript>() != null)
        {
            return;
        }
        if ((other.gameObject.GetComponent<MovementController>() != null && EmiterType == 1) || (other.gameObject.GetComponent<EnemyNavigation>() != null && EmiterType == 0))
        {
            bullethitlogic(other.gameObject);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Direction * Speed * Time.fixedDeltaTime;
        duration++;
        if (duration > 5 / Time.deltaTime)
        {
            Destroy(gameObject);
        }
    }

    void bullethitlogic(GameObject collision)
    {

        if (collision.GetComponent<HealthScript>())
        {
            collision.GetComponent<HealthScript>().TakeDamage(Damage);
            //collision.GetComponent<Rigidbody>().AddForce(Direction * Recoil, ForceMode.Impulse);
        }

        if (ishandgun)
        {
            hitbyhandgun.Add(collision);
            if (hitbyhandgun.Count > 2)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void InitializeBullet(Vector3 direction, float speed, int emiter, float damage, float recoil)
    {
        EmiterType = emiter;
        Direction = direction;
        Speed = speed;
        Damage = damage;
        Recoil = recoil;
    }

}
