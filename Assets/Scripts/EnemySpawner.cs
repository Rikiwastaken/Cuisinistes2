
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{

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

    private void Awake()
    {
        instance = this;
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

            if (TryGetSpawnPosition(out Vector3 newpos))
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


    bool TryGetSpawnPosition(out Vector3 result)
    {
        Vector3 playerPos = MovementController.instance.transform.position;
        Camera cam = Camera.main;

        for (int i = 0; i < 10; i++) // max 10 attempts per frame
        {
            float distance = Random.Range(minplayerrangewheretospawn, maxplayerrangewheretospawn);
            float angle = Random.Range(0f, 360f);

            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 candidate = playerPos + MovementController.instance.transform.forward * 3f + dir * distance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                Vector3 spawnPos = hit.position;

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

    private bool checkifspawnposisvalid(Vector3 position)
    {

        //if (position.y > -1f)
        //{
        //    return false;
        //}

        Vector3 playerpos = MovementController.instance.transform.position;

        //if (Vector3.Distance(position, playerpos) > playerrangewheretospawn)
        //{
        //    return false;
        //}



        Vector3 direction = (playerpos - position - new Vector3(0, 0.5f, 0)).normalized;

        if (Physics.Raycast(position, direction, out RaycastHit hit, maxplayerrangewheretospawn, LayerMask.GetMask("Player", "Ground")))
        {
            return false;
        }

        return true;
    }

}
