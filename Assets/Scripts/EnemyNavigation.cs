using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{

    public bool engagedPlayer;

    public float AgroRange;

    private int delaybetweendistancerecalculation;
    public float delaybetweendestinatinationchecks;

    private Transform player;

    public float mindistbeforefiring;

    public bool debug;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        delaybetweendistancerecalculation = Random.Range(0, 60);
        player = MovementController.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (delaybetweendistancerecalculation > 0)
        {
            delaybetweendistancerecalculation--;
        }
        else
        {
            delaybetweendistancerecalculation = (int)(delaybetweendestinatinationchecks / Time.deltaTime);
            if (engagedPlayer)
            {
                GetComponent<NavMeshAgent>().SetDestination(player.position);
            }
            else
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= AgroRange)
                {
                    Debug.Log("inrange");
                    Vector3 directionToPlayer = (player.transform.position - transform.position - new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f)).normalized;

                    if (Physics.Raycast(transform.position + new Vector3(0f, GetComponent<NavMeshAgent>().height / 2f, 0f), directionToPlayer, out RaycastHit hit))
                    {
                        Debug.Log("hitsomthing : " + hit.transform.gameObject.name);
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            engagedPlayer = true;
                            GetComponent<NavMeshAgent>().SetDestination(player.position);
                        }
                    }
                }
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        if (!debug) return;




        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AgroRange);

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
