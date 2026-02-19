
using System;
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

    [Serializable]
    public class WaveClass
    {
        public int numberofenemies;
        public float damagemultiplier;
        public float healthmultiplier;
        public int bonusperwave;
    }

    [Header("Wave Variables")]
    public List<WaveClass> waves;

    public int currentwave;
    private Transform player;
    public float waveenemymultiplier;
    public int waveenemyadding;
    public int maxwaves;
    public float difficultyincreaseperround;

    private UpgradeScript upgradeScript;

    private int remainingbonustogive;

    public GameObject Arena;

    public GameObject Boss;

    private GameObject BossInstance;
    public bool won;
    private bool showedvictory;
    public bool endless;
    private WaveClass lastwave;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        upgradeScript = UpgradeScript.instance;
        player = MovementController.instance.transform;
        currentwave = 0;
        totalenemyonthemap = waves[currentwave].numberofenemies;
        player.GetComponent<MovementController>().WaveTMP.text = "Wave " + (currentwave + 1) + "\nRemaining: " + totalenemyonthemap;
        lastwave = waves[0];
    }
    // Update is called once per frame
    void Update()
    {
        if (won)
        {
            if (!showedvictory)
            {
                showedvictory = true;
                TitleText.instance.StartVictoryText();
            }
        }
        else
        {
            //bonus
            if (upgradeScript.gettingbonus)
            {
                return;
            }



            if (remainingbonustogive > 0)
            {
                Debug.Log("spawning additionnal bonus : " + currentwave);
                remainingbonustogive--;
                upgradeScript.InitializeNewBonuses();
            }

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

    }

    public void InitializeNewEndlessWave()
    {
        WaveClass newwave = new WaveClass();
        newwave.bonusperwave = 1;
        newwave.numberofenemies = (int)((lastwave.numberofenemies + waveenemyadding) * waveenemymultiplier);
        newwave.damagemultiplier = lastwave.damagemultiplier * difficultyincreaseperround * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
        newwave.healthmultiplier = lastwave.healthmultiplier * difficultyincreaseperround * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
        lastwave = newwave;
        won = false;
        BossInstance = null;
    }

    public void KillEnemy()
    {
        if (BossInstance != null)
        {
            if (BossInstance.GetComponent<HealthScript>().HP <= 0)
            {
                won = true;
            }

        }
        else
        {
            totalenemyonthemap--;
            if (totalenemyonthemap <= 0)
            {
                if (endless)
                {
                    InitializeNewEndlessWave();
                    totalenemyonthemap = lastwave.numberofenemies;
                    currentwave++;
                    player.GetComponent<MovementController>().WaveTMP.text = "Wave " + (currentwave + 1) + "\nRemaining: " + totalenemyonthemap;
                }
                else if (currentwave < waves.Count - 1)
                {
                    remainingbonustogive = waves[currentwave].bonusperwave - 1;
                    currentwave++;
                    upgradeScript.InitializeNewBonuses();
                    Debug.Log("now wave : " + currentwave);
                    totalenemyonthemap = waves[currentwave].numberofenemies;
                    lastwave = waves[currentwave];
                    player.GetComponent<MovementController>().WaveTMP.text = "Wave " + (currentwave + 1) + "\nRemaining: " + totalenemyonthemap;
                }
                else
                {
                    player.transform.position = Arena.transform.position + new Vector3(-10, 3, 0);
                    BossInstance = Instantiate(Boss);
                    NavMeshAgent agent = BossInstance.GetComponent<NavMeshAgent>();
                    BossInstance.transform.parent = EnemyHolder;
                    BossInstance.GetComponentInChildren<Animation>().transform.localRotation = Quaternion.identity;


                    NavMesh.SamplePosition(Arena.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas);
                    Vector3 spawnPos = hit.position;
                    agent.enabled = false;
                    BossInstance.transform.position = spawnPos;
                    agent.enabled = true;
                    agent.Warp(spawnPos);
                    player.GetComponent<MovementController>().WaveTMP.text = "Last Wave\n Purify it.";
                }
            }
            else
            {
                player.GetComponent<MovementController>().WaveTMP.text = "Wave " + (currentwave + 1) + "\nRemaining: " + totalenemyonthemap;
            }

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


                        newenemy.GetComponent<EnemyNavigation>().engagedPlayer = false;
                        newenemy.GetComponent<EnemyNavigation>().ded = false;
                        newenemy.GetComponent<BoxCollider>().enabled = true;
                        newenemy.GetComponent<NavMeshAgent>().enabled = true;
                        newenemy.GetComponent<EnemyNavigation>().Lifebar.fillAmount = 1f;
                        newenemy.GetComponent<EnemyNavigation>().Canvas.gameObject.SetActive(false);
                    }
                    else
                    {
                        newenemy = Instantiate(enemyprefabList[randomID]);
                    }



                    newenemy.GetComponentInChildren<Animation>().clip = newenemy.GetComponent<EnemyNavigation>().Idle;
                    newenemy.GetComponentInChildren<Animation>().Play();
                    if (endless)
                    {
                        newenemy.GetComponent<HealthScript>().scaledMaxHealth = newenemy.GetComponent<HealthScript>().MaxHealth * lastwave.healthmultiplier * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
                        newenemy.GetComponent<EnemyNavigation>().scaledgundamage = newenemy.GetComponent<EnemyNavigation>().Gun.damage * lastwave.damagemultiplier * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
                        newenemy.GetComponent<EnemyNavigation>().scaledmeleedamage = newenemy.GetComponent<EnemyNavigation>().meleedamage * lastwave.damagemultiplier * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
                    }
                    else
                    {
                        if (waves[currentwave].damagemultiplier <= 1)
                        {
                            waves[currentwave].damagemultiplier = 1;
                        }

                        if (waves[currentwave].healthmultiplier <= 1)
                        {
                            waves[currentwave].healthmultiplier = 1;
                        }
                        newenemy.GetComponent<HealthScript>().scaledMaxHealth = newenemy.GetComponent<HealthScript>().MaxHealth * waves[currentwave].healthmultiplier * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
                        newenemy.GetComponent<EnemyNavigation>().scaledgundamage = newenemy.GetComponent<EnemyNavigation>().Gun.damage * waves[currentwave].damagemultiplier * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
                        newenemy.GetComponent<EnemyNavigation>().scaledmeleedamage = newenemy.GetComponent<EnemyNavigation>().meleedamage * waves[currentwave].damagemultiplier * (float)Math.Pow(1f + upgradeScript.DifficultyPerLevel, upgradeScript.DifficultyLevel);
                    }


                    newenemy.GetComponent<HealthScript>().HP = newenemy.GetComponent<HealthScript>().scaledMaxHealth;
                    SpawnedEnemylist.Add(newenemy);
                    NavMeshAgent agent = newenemy.GetComponent<NavMeshAgent>();
                    newenemy.transform.parent = EnemyHolder;
                    newenemy.GetComponentInChildren<Animation>().transform.localRotation = Quaternion.identity;

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
            float distance = UnityEngine.Random.Range(minrange, maxrange);
            float angle = UnityEngine.Random.Range(0f, 360f);

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


    private float CalculateNumberofEnemies(int iterations)
    {
        if (iterations <= 0)
        {
            return 1f;
        }
        else
        {
            return waveenemyadding + CalculateNumberofEnemies(iterations - 1) * waveenemymultiplier;
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Calculate Waves")]
    void CalculateWaves()
    {
        List<WaveClass> newwaves = new List<WaveClass>();
        for (int i = 0; i < maxwaves; i++)
        {
            WaveClass wave = new WaveClass();
            wave.healthmultiplier = Mathf.Pow(1f + difficultyincreaseperround, i);
            wave.damagemultiplier = Mathf.Pow(1f + difficultyincreaseperround, i);
            if (i % 2 == 0)
            {
                wave.bonusperwave = 2;
            }
            else if (i == 0)
            {
                wave.bonusperwave = 3;
            }
            else
            {
                wave.bonusperwave = 1;
            }
            wave.numberofenemies = (int)CalculateNumberofEnemies(i);
            newwaves.Add(wave);
        }
        waves = newwaves;
    }



#endif

}
