using System.Collections;
using TMPro;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    public float timeforeachtext;

    public float timeforfinaltext;


    public void StartTitleText()
    {
        StartCoroutine(StartText());
    }

    IEnumerator StartText()
    {
        textMeshPro.gameObject.SetActive(true);
        textMeshPro.text = "Survive";
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "ThE";
        yield return new WaitForSeconds(timeforeachtext);
        textMeshPro.text = "BackdoomS";
        yield return new WaitForSeconds(timeforfinaltext);
        textMeshPro.gameObject.SetActive(false);


    }

}
