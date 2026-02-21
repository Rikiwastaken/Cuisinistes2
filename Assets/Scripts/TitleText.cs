using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleText : MonoBehaviour
{

    public static TitleText instance;

    public TextMeshProUGUI textMeshPro;

    public float timeforeachtext;

    public float timeforfinaltext;

    public AudioClip FirstTextClip;
    public AudioClip SecondTextClip;

    public AudioClip MainTextClip;
    public AudioClip DeathClip;

    public float smallsoundvolume = 1f;
    public float smallsound2volume = 1f;
    public float bigsoundvolume = 1f;
    public float deathsoundvolume = 1f;

    private SoundManager soundManager;

    private bool playinggameover;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        soundManager = SoundManager.instance;
    }

    public void StartTitleText()
    {
        StartCoroutine(StartText());
    }

    IEnumerator StartText()
    {
        textMeshPro.gameObject.SetActive(true);
        textMeshPro.text = "Survive";
        soundManager.PlaySFX(FirstTextClip, 0.05f, transform, smallsoundvolume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "ThE";
        soundManager.PlaySFX(SecondTextClip, 0.05f, transform, smallsound2volume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "BackdoomS";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform, bigsoundvolume);
        yield return new WaitForSeconds(timeforfinaltext);
        textMeshPro.gameObject.SetActive(false);


    }
    public void StartVictoryText()
    {
        StartCoroutine(VictoryText());
    }

    IEnumerator VictoryText()
    {
        textMeshPro.gameObject.SetActive(true);
        textMeshPro.text = "YoU";
        soundManager.PlaySFX(FirstTextClip, 0.05f, transform, smallsoundvolume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "SurviveD";
        soundManager.PlaySFX(SecondTextClip, 0.05f, transform, smallsound2volume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "ThE";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform, bigsoundvolume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "BackdoomS";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform, bigsoundvolume);
        yield return new WaitForSeconds(timeforfinaltext * 1.5f);
        textMeshPro.text = "ThankS FoR PlayinG";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform, bigsoundvolume);
        yield return new WaitForSeconds(timeforfinaltext * 2);
        textMeshPro.text = "BuT it NeveR EndS";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform, bigsoundvolume);
        yield return new WaitForSeconds(timeforfinaltext * 2);
        transform.position = MovementController.instance.basepos;
        textMeshPro.gameObject.SetActive(false);
        EnemySpawner.instance.endless = true;
        EnemySpawner.instance.InitializeNewEndlessWave();
    }

    public void StartGameOverText()
    {
        if (playinggameover)
        {
            return;
        }
        StartCoroutine(GameOverText());
    }

    IEnumerator GameOverText()
    {
        playinggameover = true;
        textMeshPro.gameObject.SetActive(true);
        textMeshPro.text = "YoU";
        soundManager.PlaySFX(DeathClip, 0.05f, transform, deathsoundvolume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "Died";
        soundManager.PlaySFX(SecondTextClip, 0.05f, transform, smallsound2volume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "IN";
        soundManager.PlaySFX(SecondTextClip, 0.05f, transform, smallsound2volume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "ThE";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform, bigsoundvolume);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "BackdoomS";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform, bigsoundvolume);
        yield return new WaitForSeconds(timeforfinaltext * 2);
        SceneManager.LoadScene("MainMenu");
    }

}
