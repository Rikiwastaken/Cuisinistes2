using System.Collections.Generic;
using UnityEngine;

public class grenadeScript : MonoBehaviour
{

    private SphereCollider SphereCollider;

    public float Damage;

    public float timebeforeExplosion;

    public float timebeforedestruction;

    private float timewhendestroys;

    private float timewhenexplodes;

    public List<AudioClip> explosionSFX;

    private List<GameObject> objectshitbyExplosion = new List<GameObject>();

    public GameObject ExplosionSphere;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timewhenexplodes = Time.time + timebeforeExplosion;
        SphereCollider = GetComponent<SphereCollider>();
        ExplosionSphere.transform.localScale = Vector3.one * SphereCollider.radius * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!SphereCollider.enabled && Time.time > timewhenexplodes)
        {
            SphereCollider.enabled = true;
            timewhendestroys = Time.time + timebeforedestruction;
            SoundManager.instance.PlaySFXFromList(explosionSFX, 0.05f, transform);
            ExplosionSphere.SetActive(true);
        }
        if (timewhendestroys != 0 && Time.time > timewhendestroys)
        {
            Destroy(gameObject);
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
