using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{

    public float Damage;

    private List<GameObject> whowasdealtdamage;

    private UpgradeScript UpgradeScript;
    public AudioSource Sound;

    private void Update()
    {
        if (UpgradeScript == null)
        {
            UpgradeScript = UpgradeScript.instance;
        }

        if (UpgradeScript.gettingbonus)
        {
            if (Sound.isPlaying)
            {
                Sound.Stop();
            }
        }
        else
        {
            if (!Sound.isPlaying)
            {
                Sound.Play();
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.GetComponent<BulletScript>() != null)
        {
            return;
        }
        if (other.gameObject.GetComponent<EnemyNavigation>() != null && !whowasdealtdamage.Contains(other.gameObject))
        {
            whowasdealtdamage.Add(other.gameObject);
            if (other.gameObject.GetComponent<HealthScript>())
            {
                other.gameObject.GetComponent<HealthScript>().TakeDamage(Damage);
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponent<BulletScript>() != null)
        {
            return;
        }
        if (other.gameObject.GetComponent<EnemyNavigation>() != null && !whowasdealtdamage.Contains(other.gameObject))
        {
            whowasdealtdamage.Add(other.gameObject);
            if (other.gameObject.GetComponent<HealthScript>())
            {
                other.gameObject.GetComponent<HealthScript>().TakeDamage(Damage);
            }
        }

    }

    public void ResetList()
    {
        whowasdealtdamage = new List<GameObject>();
    }
}
