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

    private SoundManager soundManager;

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
        soundManager.PlaySFX(FirstTextClip, 0.05f, transform);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "ThE";
        soundManager.PlaySFX(SecondTextClip, 0.05f, transform);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "BackdoomS";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform);
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
        soundManager.PlaySFX(FirstTextClip, 0.05f, transform);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "SurviveD";
        soundManager.PlaySFX(SecondTextClip, 0.05f, transform);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "ThE";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform);
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "BackdoomS";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform);
        yield return new WaitForSeconds(timeforfinaltext * 2);
        textMeshPro.text = "ThankS FoR PlayinG";
        soundManager.PlaySFX(MainTextClip, 0.05f, transform);
        yield return new WaitForSeconds(timeforfinaltext * 4);
        SceneManager.LoadScene("MainMenu");
    }


}
