using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public AudioMixer AudioMixer;

    public static SoundManager instance;

    public float SFXVol;

    public float maxdistancetonullify;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeVolume()
    {
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(DataScript.instance.Options.MusicVol) * 20f);
        AudioMixer.SetFloat("SoundVolume", Mathf.Log10(DataScript.instance.Options.SFXVol) * 20f);
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(DataScript.instance.Options.Mastervol) * 20f);
    }

    public void PlaySFXFromList(List<AudioClip> cliplist, float pitchrandomness, Transform Emiter, float volume = -1)
    {
        int randomSFXID = Random.Range(0, cliplist.Count);
        StartCoroutine(PlaySFXCoroutine(cliplist[randomSFXID], pitchrandomness, Emiter, volume));
    }

    public void PlaySFX(AudioClip clip, float pitchrandomness, Transform Emiter, float volume = -1)
    {
        StartCoroutine(PlaySFXCoroutine(clip, pitchrandomness, Emiter, volume));
    }

    private IEnumerator PlaySFXCoroutine(AudioClip clip, float pitchrandomness, Transform Emiter, float newvolume)
    {

        float vol = newvolume;
        if (vol < 0)
        {
            vol = SFXVol;
        }

        GameObject newAudioSource = new GameObject();
        newAudioSource.name = clip.name;
        newAudioSource.transform.parent = transform;
        if (Emiter == null)
        {
            newAudioSource.transform.localPosition = Vector3.zero;
        }
        else
        {
            newAudioSource.transform.position = Emiter.position;
        }

        IndividualSoundScript ISS = newAudioSource.AddComponent<IndividualSoundScript>();
        ISS.basevolume = vol;
        ISS.Emiter = Emiter;
        if (MovementController.instance != null)
        {
            ISS.PlayerTransform = MovementController.instance.transform;
        }
        ISS.maxdistancetonullify = maxdistancetonullify;
        AudioSource AS = newAudioSource.AddComponent<AudioSource>();
        AS.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("SFX")[0];
        AS.clip = clip;

        float volume = 1;
        if (MovementController.instance)
        {
            Vector3 playerposition = MovementController.instance.transform.position;

            float distance = Vector3.Distance(playerposition, Emiter.position);

            volume = 0;

            if (distance < maxdistancetonullify)
            {
                volume = (maxdistancetonullify - distance) / maxdistancetonullify;
            }
        }


        AS.volume = vol * volume;
        AS.pitch = 1f + UnityEngine.Random.Range(-pitchrandomness, pitchrandomness);
        AS.Play();
        yield return new WaitForSeconds(AS.clip.length);
        Destroy(newAudioSource);
    }
}
