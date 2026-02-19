using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static ShootScript;

public class EnemyNavigation : MonoBehaviour
{

    public bool engagedPlayer;

    public float AgroRange;

    public float movingspeed;

    private int delaybetweendistancerecalculation;
    public float delaybetweendestinatinationchecks;

    private Transform player;

    public float mindistbeforefiring;

    public bool debug;

    private Animation Animation;
    public AnimationClip Idle;
    public AnimationClip Run;
    public AnimationClip Attack;
    public AnimationClip Die;

    private Vector3 lastlegaldestination;

    public Transform Canvas;
    public UnityEngine.UI.Image Lifebar;

    public bool ded;

    [Header("ShotVariables")]

    public bool Shoots;
    public int GunRange;
    private int guncooldown;
    public GunClass Gun;
    public float freezetimeaftershooting;
    private int freezetimeaftershootingcounter;

    [Header("Melee Variables")]

    public float minrangeformelee;
    private bool isinmelee;
    public float meleedamage;


    [Header("dispawn")]

    public float durationbeforedespawn;

    private int durationbeforedespawncounter;

    private NavMeshAgent agent;
    private float nextDestinationUpdateTime;
    private float nextShootTime;
    private float nextDespawnCheckTime;
    private float sqrAgroRange;
    private float sqrGunRange;
    private List<Material> enemymat = new List<Material>();


    [Header("drop")]
    public int globaldroprate;
    public int ammodropratepercent;
    public int armordroprate;
    public int healtkitdroprate;
    public GameObject AmmoBoxPrefab;
    public GameObject HealthKitPrefab;
    public GameObject ArmorKitPrefab;
    private UpgradeScript upgradeScript;

    [Header("Sound")]

    public List<AudioClip> AttackSound;

    public List<AudioClip> Idlesound;

    public List<AudioClip> DamageSound;

    public List<AudioClip> DeathSound;

    private float nextIdleSoundUpdateTime;
    public float timebetweenidlesound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeScript = UpgradeScript.instance;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movingspeed;
        sqrAgroRange = AgroRange * AgroRange;
        sqrGunRange = GunRange * GunRange;
        delaybetweendistancerecalculation = Random.Range(0, 60);
        player = MovementController.instance.transform;
        Animation = GetComponentInChildren<Animation>();
        Animation.clip = Idle;
        Animation.Play();
        foreach (MeshRenderer MR in GetComponentsInChildren<MeshRenderer>())
        {
            if (MR != null && MR.material != null)
            {
                enemymat.Add(MR.material);

            }

        }
    }

    // Update is called once per frame
    void Update()
    {

        //bonus
        if (upgradeScript.gettingbonus)
        {
            return;
        }

        if (ded)
        {
            if (agent.enabled)
            {
                agent.enabled = false;
            }
            Vector3 directionToPlayer = (player.transform.position - transform.position - new Vector3(0f, agent.height / 2f, 0f)).normalized;

            float newratio = enemymat[0].GetFloat("_DissolvePercent") - Time.deltaTime / 3f;

            foreach (Material mat in enemymat)
            {
                mat.SetFloat("_DissolvePercent", newratio);
            }


            if (newratio <= 0)
            {
                RecycleEnemy();
            }

            return;
        }


        if (EnemySpawner.instance.totalenemyonthemap <= 2)
        {
            engagedPlayer = true;
        }

        float sqrDistToPlayer = (player.position - transform.position).sqrMagnitude;

        if (Time.time >= nextDestinationUpdateTime)
        {
            nextDestinationUpdateTime = Time.time + 0.3f;

            if (!engagedPlayer)
            {
                if (sqrDistToPlayer <= sqrAgroRange)
                {
                    if (HasLineOfSight())
                    {
                        engagedPlayer = true;
                    }
                }
            }

            if (engagedPlayer)
            {
                agent.SetDestination(player.position);
            }
        }



        if (agent.isStopped != !checkifnaviationisavailable())
        {
            agent.isStopped = !checkifnaviationisavailable();
        }

        if (engagedPlayer)
        {
            if (Time.time >= nextShootTime)
            {
                nextShootTime = Time.time + Gun.GunCD;
                if (Shoots)
                {
                    if (sqrDistToPlayer <= sqrGunRange && HasLineOfSight())
                    {
                        Shoot();
                        nextShootTime = Time.time + Gun.GunCD;
                    }
                }

            }


            if (!Shoots && sqrDistToPlayer <= minrangeformelee * minrangeformelee && HasLineOfSight() && !isinmelee)
            {
                isinmelee = true;
                Animation.clip = Attack;
                Animation.Play();
            }

            if (isinmelee && (!Animation.isPlaying || Animation.clip != Attack))
            {
                isinmelee = false;
                if (sqrDistToPlayer <= minrangeformelee * minrangeformelee)
                {
                    player.GetComponent<HealthScript>().TakeDamage(meleedamage);
                }
            }
        }

        // Idle Sounds

        if (Time.time >= nextIdleSoundUpdateTime && Idlesound.Count > 0)
        {
            nextDestinationUpdateTime = Time.time + timebetweenidlesound;

            SoundManager.instance.PlaySFXFromList(Idlesound, 0.05f, transform);
        }


        // lifebarorientation

        if (Canvas.gameObject.activeSelf)
        {
            Canvas.LookAt(player);
        }

        ManageAnimation();

    }

    public void RecycleEnemy()
    {
        EnemySpawner.instance.SpawnedEnemylist.Remove(gameObject);
        gameObject.SetActive(false);
        ded = false;
        foreach (Material mat in enemymat)
        {
            mat.SetFloat("_DissolvePercent", 1f);
        }

        EnemySpawner.instance.EnemiesToRecycle.Add(gameObject);
    }

    private bool HasLineOfSight()
    {
        Vector3 eyePos = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 targetPos = player.position;

        Vector3 dir = targetPos - eyePos;
        float distance = dir.magnitude;
        dir.Normalize();

        Debug.DrawRay(eyePos, dir * distance, Color.red, 0.1f);

        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, distance))
        {
            //Debug.Log("Enemy hit: " + hit.transform.name);

            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                return true;
            }
        }

        return false;
    }

    private void LateUpdate()
    {
        //bonus
        if (upgradeScript.gettingbonus)
        {
            return;
        }
        if (engagedPlayer)
        {
            if (!ded)
            {
                var lookPos = player.position - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                {
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
                }

            }



            durationbeforedespawncounter = 0;
        }
        else if (!ded)
        {
            durationbeforedespawncounter++;
            if (durationbeforedespawncounter > durationbeforedespawn / Time.deltaTime)
            {
                durationbeforedespawncounter = 0;
                PlayDeathAnim(true);
                return;
            }
        }
        if (Vector2.Distance(transform.position, player.transform.position) > 25)
        {
            PlayDeathAnim(true);
        }
    }

    private void Shoot()
    {
        guncooldown = (int)(Gun.GunCD / Time.deltaTime);

        if (Animation.clip != Attack)
        {
            Animation.clip = Attack;
        }
        Animation.Play();

        Vector3 direction = player.transform.position - transform.position - Gun.wheretospawnbullet;

        GameObject newbullet = Instantiate(Gun.Bulletprefab, transform.position + Gun.wheretospawnbullet, Quaternion.identity);
        newbullet.transform.forward = transform.forward;
        BulletScript bulletscript = newbullet.GetComponentInChildren<BulletScript>();

        bulletscript.InitializeBullet(direction, Gun.bulletspeed, 1, Gun.damage, Gun.recoil);


        if (AttackSound.Count > 0)
        {
            SoundManager.instance.PlaySFXFromList(AttackSound, 0.05f, transform);
        }
    }


    public void TriggerDrop()
    {


        int freeupgradeRandomValue = UnityEngine.Random.Range(0, 100);

        if (freeupgradeRandomValue < 100f * Mathf.Max(0f, upgradeScript.FreeUpgradeChanceLevel * upgradeScript.FreeUpgradePerLevel) * Mathf.Pow(1f + upgradeScript.FreeUpgradePerLevel, upgradeScript.FreeUpgradeChanceLevel))
        {
            upgradeScript.InitializeNewBonuses();
        }



        int randomvalue = UnityEngine.Random.Range(0, 100);

        float truedroprate = globaldroprate * Mathf.Pow(1f + upgradeScript.DropRatePerLevel, upgradeScript.DropRateLevel);

        if (truedroprate > 100f)
        {

            int droptablerandomvalue = UnityEngine.Random.Range(0, ammodropratepercent + armordroprate + healtkitdroprate);
            if (droptablerandomvalue <= ammodropratepercent)
            {
                GameObject newammobox = Instantiate(AmmoBoxPrefab);
                newammobox.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                newammobox.GetComponent<AmmoBoxScript>().InitializeAmmoBox();
            }
            else if (droptablerandomvalue <= armordroprate)
            {
                GameObject newarmor = Instantiate(ArmorKitPrefab);
                newarmor.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            }
            else
            {
                GameObject newhealthkit = Instantiate(HealthKitPrefab);
                newhealthkit.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            }


            if (randomvalue <= truedroprate - 100)
            {

                droptablerandomvalue = UnityEngine.Random.Range(0, ammodropratepercent + armordroprate + healtkitdroprate);
                if (droptablerandomvalue <= ammodropratepercent)
                {
                    GameObject newammobox = Instantiate(AmmoBoxPrefab);
                    newammobox.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    newammobox.GetComponent<AmmoBoxScript>().InitializeAmmoBox();
                }
                else if (droptablerandomvalue <= armordroprate)
                {
                    GameObject newarmor = Instantiate(ArmorKitPrefab);
                    newarmor.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                }
                else
                {
                    GameObject newhealthkit = Instantiate(HealthKitPrefab);
                    newhealthkit.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                }

            }

        }
        else
        {
            if (randomvalue <= truedroprate)
            {

                int droptablerandomvalue = UnityEngine.Random.Range(0, ammodropratepercent + armordroprate + healtkitdroprate);
                if (droptablerandomvalue <= ammodropratepercent)
                {
                    GameObject newammobox = Instantiate(AmmoBoxPrefab);
                    newammobox.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    newammobox.GetComponent<AmmoBoxScript>().InitializeAmmoBox();
                }
                else if (droptablerandomvalue <= armordroprate)
                {
                    GameObject newarmor = Instantiate(ArmorKitPrefab);
                    newarmor.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                }
                else
                {
                    GameObject newhealthkit = Instantiate(HealthKitPrefab);
                    newhealthkit.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                }

            }
        }


    }
    public void PlayDeathAnim(bool immediate = false)
    {
        EnemySpawner.instance.SpawnedEnemylist.Remove(gameObject);
        if (immediate)
        {
            RecycleEnemy();
            return;
        }

        Canvas.gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        if (Animation.clip != Die)
        {
            Animation.clip = Die;
        }
        Animation.Play();
        ded = true;
    }
    private void ManageAnimation()
    {
        if (Animation.clip == Die)
        {
            return;
        }
        else if (Animation.clip == Attack && Animation.isPlaying)
        {
            return;
        }
        else if (Mathf.Abs(agent.velocity.magnitude) >= 0f)
        {
            if (Animation.clip != Run)
            {
                Animation.clip = Run;
            }
            if (!Animation.isPlaying)
            {
                Animation.Play();
            }
        }
        else
        {
            if (Animation.clip != Idle)
            {
                Animation.clip = Idle;
            }
            if (!Animation.isPlaying)
            {
                Animation.Play();
            }
        }
    }

    private bool checkifnaviationisavailable()
    {
        if (Animation.isPlaying && (Animation.clip == Attack || Animation.clip == Die))
        {
            return false;
        }

        if (!Shoots && Vector3.Distance(transform.position, player.position) <= minrangeformelee * 0.9f)
        {
            return false;
        }

        if (Shoots && Vector3.Distance(transform.position, player.position) <= mindistbeforefiring && HasLineOfSight())
        {
            return false;
        }
        if (freezetimeaftershootingcounter > 0)
        {
            return false;
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (!debug) return;




        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AgroRange);

        // Draw detection radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GunRange);

        // Eye position
        Vector3 eyePos = transform.position + new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f);


        // Draw ray to player (if exists)
        if (player != null)
        {
            Transform player = MovementController.instance.transform;
            Vector3 toPlayer = (player.position - eyePos).normalized;
            float dist = Vector3.Distance(eyePos, player.position);

            if (dist <= AgroRange)
            {
                bool hitPlayer = false;

                if (Physics.Raycast(eyePos, toPlayer, out RaycastHit hit, AgroRange, LayerMask.NameToLayer("Default") | LayerMask.NameToLayer("Ground") | LayerMask.NameToLayer("Player")))
                {
                    hitPlayer = hit.transform.gameObject.layer == LayerMask.NameToLayer("Player");
                }

                Gizmos.color = hitPlayer ? Color.green : Color.red;
                Gizmos.DrawLine(eyePos, eyePos + toPlayer * Mathf.Min(dist, AgroRange));
            }
        }
    }
}
