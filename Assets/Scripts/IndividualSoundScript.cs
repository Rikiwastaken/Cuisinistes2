using UnityEngine;

public class IndividualSoundScript : MonoBehaviour
{

    public Transform Emiter;

    public Transform PlayerTransform;

    public float maxdistancetonullify;

    public float basevolume;

    private Vector3 previousposEmiterPos;

    private Vector3 playerpos;

    // Update is called once per frame
    void Update()
    {

        if (PlayerTransform != null)
        {
            playerpos = PlayerTransform.position;
        }

        float volume = 0;

        if (Emiter != null)
        {
            float distance = Vector3.Distance(playerpos, Emiter.position);


            if (distance < maxdistancetonullify)
            {
                volume = (maxdistancetonullify - distance) / maxdistancetonullify;
            }
        }
        else if (MovementController.instance != null)
        {
            float distance = Vector3.Distance(playerpos, previousposEmiterPos);


            if (distance < maxdistancetonullify)
            {
                volume = (maxdistancetonullify - distance) / maxdistancetonullify;
            }
        }
        else
        {
            volume = 1f;
        }



        GetComponent<AudioSource>().volume = basevolume * volume;

        if (Emiter != null)
        {
            previousposEmiterPos = Emiter.position;
        }

    }
}
