
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{

    [Header("Enemy Spawn")]

    public static EnemySpawner instance;

    public List<GameObject> enemyprefabList;

    public List<GameObject> SpawnedEnemylist;

    public List<GameObject> EnemiesToRecycle;


    public float durationBetweenEnemySpawn;
    private int durationBetweenEnemySpawncnt;

    public int totalenemyonthemap;

    public float minplayerrangewheretospawn;
    public float maxplayerrangewheretospawn;

    public Transform EnemyHolder;


    [Header("Loot Spawn")]

    public List<GameObject> pickups;
    private List<GameObject> spawnedpickups = new List<GameObject>();
    public Transform lootholder;
    public int maxpickuptospawn;



    private Transform player;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = MovementController.instance.transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (durationBetweenEnemySpawncnt > 0)
        {
            durationBetweenEnemySpawncnt--;
        }
        else
        {
            durationBetweenEnemySpawncnt = (int)(durationBetweenEnemySpawn / Time.deltaTime);
            StartCoroutine(spawnEnemies());
            StartCoroutine(SpawnPickups());
        }
    }


    private IEnumerator spawnEnemies()
    {
        List<GameObject> enemiescopy = new List<GameObject>();

        foreach (GameObject enemy in SpawnedEnemylist)
        {
            enemiescopy.Add(enemy);
        }

        foreach (GameObject enemy in enemiescopy)
        {
            if (!enemy.activeSelf)
            {
                SpawnedEnemylist.Remove(enemy);
                if (!EnemiesToRecycle.Contains(enemy))
                {
                    EnemiesToRecycle.Add(enemy);
                }
            }
        }



        int safeguard = 0;
        int spawnenemylistsize = SpawnedEnemylist.Count;


        while (safeguard < 100 && spawnenemylistsize < totalenemyonthemap)
        {
            safeguard++;

            if (TryGetSpawnPosition(out Vector3 newpos, minplayerrangewheretospawn, maxplayerrangewheretospawn))
            {
                if (newpos != Vector3.zero)
                {

                    int randomID = UnityEngine.Random.Range(0, enemyprefabList.Count);
                    GameObject newenemy = null;
                    if (EnemiesToRecycle.Count > 0)
                    {
                        newenemy = EnemiesToRecycle[0];
                        EnemiesToRecycle.Remove(newenemy);
                        newenemy.SetActive(true);
                        newenemy.GetComponent<HealthScript>().HP = newenemy.GetComponent<HealthScript>().MaxHealth;
                        newenemy.GetComponent<EnemyNavigation>().engagedPlayer = false;
                        newenemy.GetComponent<BoxCollider>().enabled = true;
                        newenemy.GetComponent<NavMeshAgent>().enabled = true;
                        newenemy.GetComponent<EnemyNavigation>().Lifebar.fillAmount = 1f;
                        newenemy.GetComponent<EnemyNavigation>().Canvas.gameObject.SetActive(false);
                    }
                    else
                    {
                        newenemy = Instantiate(enemyprefabList[randomID]);
                    }

                    SpawnedEnemylist.Add(newenemy);
                    NavMeshAgent agent = newenemy.GetComponent<NavMeshAgent>();
                    newenemy.transform.parent = EnemyHolder;

                    agent.enabled = false;
                    newenemy.transform.position = newpos;
                    agent.enabled = true;
                    agent.Warp(newpos);

                }
            }







            spawnenemylistsize = SpawnedEnemylist.Count;
            if (spawnenemylistsize >= totalenemyonthemap)
            {
                safeguard = 100;
            }
        }

        yield return null;

    }

    private IEnumerator SpawnPickups()
    {


        int safeguard = 0;
        int lootsizelist = spawnedpickups.Count;


        while (safeguard < 100 && lootsizelist < maxpickuptospawn)
        {
            safeguard++;

            if (TryGetSpawnPosition(out Vector3 newpos, maxplayerrangewheretospawn, maxplayerrangewheretospawn * 2))
            {
                if (newpos != Vector3.zero)
                {

                    int randomID = UnityEngine.Random.Range(0, pickups.Count + player.GetComponent<ShootScript>().GunList.Count);
                    GameObject newloot = null;
                    if (randomID < player.GetComponent<ShootScript>().GunList.Count)
                    {
                        newloot = Instantiate(pickups[0]);
                        newloot.GetComponent<AmmoBoxScript>().ammotype = randomID;
                    }
                    else
                    {
                        Debug.Log("spawning ID : " + (randomID - player.GetComponent<ShootScript>().GunList.Count));
                        newloot = Instantiate(pickups[randomID - player.GetComponent<ShootScript>().GunList.Count]);
                    }


                    spawnedpickups.Add(newloot);
                    newloot.transform.parent = lootholder;

                    newloot.transform.position = newpos;

                }
            }







            lootsizelist = spawnedpickups.Count;
            if (lootsizelist >= maxpickuptospawn)
            {
                safeguard = 100;
            }
        }

        yield return null;

    }


    bool TryGetSpawnPosition(out Vector3 result, float minrange, float maxrange)
    {
        Vector3 playerPos = MovementController.instance.transform.position;
        Camera cam = Camera.main;


        float checkRadius = 0.5f;
        LayerMask wallMask = LayerMask.GetMask("Ground");

        for (int i = 0; i < 10; i++) // max 10 attempts per frame
        {
            float distance = Random.Range(minrange, maxrange);
            float angle = Random.Range(0f, 360f);

            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 candidate = playerPos + MovementController.instance.transform.forward * 10f + dir * distance;


            if (Vector3.Distance(candidate, playerPos) < minrange)
                continue;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                Vector3 spawnPos = hit.position;

                //checkifinsidewall
                bool insideWall = Physics.CheckSphere(spawnPos + new Vector3(0, checkRadius + 0.1f, 0), checkRadius, wallMask);

                if (insideWall)
                    continue; // Try another position

                // Check if behind camera
                Vector3 viewportPoint = cam.WorldToViewportPoint(spawnPos);

                bool behindCamera = viewportPoint.z < 0;
                bool outsideView =
                    viewportPoint.x < 0 || viewportPoint.x > 1 ||
                    viewportPoint.y < 0 || viewportPoint.y > 1;

                if (!behindCamera && outsideView)
                {
                    result = spawnPos;
                    return true;
                }
            }
        }

        result = Vector3.zero;
        return false;
    }

}
