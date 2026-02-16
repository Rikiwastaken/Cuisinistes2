using UnityEngine;
using UnityEngine.AI;
using static ShootScript;

public class EnemyNavigation : MonoBehaviour
{

    public bool engagedPlayer;

    public float AgroRange;

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








    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        delaybetweendistancerecalculation = Random.Range(0, 60);
        player = MovementController.instance.transform;
        Animation = GetComponentInChildren<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ded)
        {
            if (GetComponent<NavMeshAgent>().enabled)
            {
                GetComponent<NavMeshAgent>().enabled = false;
            }
            Vector3 directionToPlayer = (player.transform.position - transform.position - new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f)).normalized;

            if (Physics.Raycast(transform.position + new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f), directionToPlayer, out RaycastHit hit))
            {
                if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    Destroy(gameObject);

                }
            }
            return;
        }

        if (delaybetweendistancerecalculation > 0)
        {
            delaybetweendistancerecalculation--;
        }
        else
        {
            delaybetweendistancerecalculation = (int)(delaybetweendestinatinationchecks / Time.deltaTime);
            if (engagedPlayer)
            {
                NavMeshPath path = new NavMeshPath();
                GetComponent<NavMeshAgent>().CalculatePath(player.position, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    engagedPlayer = true;
                    GetComponent<NavMeshAgent>().SetDestination(player.position);
                }
                else
                {
                    engagedPlayer = false;
                }
            }
            else
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= AgroRange)
                {
                    Vector3 directionToPlayer = (player.transform.position - transform.position - new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f)).normalized;

                    if (Physics.Raycast(transform.position + new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f), directionToPlayer, out RaycastHit hit))
                    {
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            NavMeshPath path = new NavMeshPath();
                            GetComponent<NavMeshAgent>().CalculatePath(player.position, path);

                            if (path.status == NavMeshPathStatus.PathComplete)
                            {
                                engagedPlayer = true;
                                GetComponent<NavMeshAgent>().SetDestination(player.position);
                            }

                        }
                    }
                }
            }
        }
        ManageAnimation();

        if (engagedPlayer)
        {
            transform.forward = (player.transform.position - transform.position).normalized;
        }


        if (freezetimeaftershootingcounter > 0)
        {
            freezetimeaftershootingcounter--;
            if (!GetComponent<NavMeshAgent>().isStopped)
            {
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
        else
        {
            if (GetComponent<NavMeshAgent>().isStopped)
            {
                GetComponent<NavMeshAgent>().isStopped = false;
            }
        }

        if (guncooldown > 0)
        {
            guncooldown--;

        }
        else
        {

            freezetimeaftershootingcounter = (int)(freezetimeaftershooting / Time.timeScale);

            if (Shoots)
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= GunRange)
                {
                    Vector3 directionToPlayer = (player.transform.position - transform.position - new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f)).normalized;

                    if (Physics.Raycast(transform.position + new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f), directionToPlayer, out RaycastHit hit))
                    {
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {

                            Shoot();
                        }
                    }
                }
            }
        }

        Canvas.forward = player.transform.forward;
    }

    private void Shoot()
    {
        guncooldown = (int)(Gun.GunCD / Time.deltaTime);

        if (Animation.clip != Attack)
        {
            Animation.clip = Attack;
        }
        Animation.Play();

        Vector3 direction = MovementController.instance.transform.position - transform.position - Gun.wheretospawnbullet;

        GameObject newbullet = Instantiate(Gun.Bulletprefab, transform.position + Gun.wheretospawnbullet, Quaternion.identity);
        newbullet.transform.forward = transform.forward;
        BulletScript bulletscript = newbullet.GetComponentInChildren<BulletScript>();

        bulletscript.InitializeBullet(direction, Gun.bulletspeed, gameObject, Gun.damage, Gun.recoil);


        if (Gun.ShootSFX.Count > 0)
        {
            SoundManager.instance.PlaySFXFromList(Gun.ShootSFX, 0.05f, transform);
        }
    }

    public void PlayDeathAnim()
    {
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
        else if (Mathf.Abs(GetComponent<NavMeshAgent>().velocity.magnitude) >= 0.05f)
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
        if (MovementController.instance != null)
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
