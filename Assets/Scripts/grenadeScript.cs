using System.Collections.Generic;
using UnityEngine;

public class grenadeScript : MonoBehaviour
{

    private SphereCollider SphereCollider;

    public float Damage;

    public float timebeforeExplosion;

    private float timewhenexplodes;

    private List<GameObject> objectshitbyExplosion = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timewhenexplodes = Time.time + timebeforeExplosion;
        SphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!SphereCollider.enabled && Time.time > timewhenexplodes)
        {
            SphereCollider.enabled = true;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<EnemyNavigation>() && !objectshitbyExplosion.Contains(other.gameObject))
        {
            other.GetComponent<HealthScript>().TakeDamage(Damage);
            objectshitbyExplosion.Add(other.gameObject);
        }
    }
}
