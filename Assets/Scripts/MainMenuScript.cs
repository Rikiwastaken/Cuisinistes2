using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataScript;

public class MainMenuScript : MonoBehaviour
{

    private DataScript dataScript;

    private OptionsClass options;

    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SFXSlider;
    public Slider SensitivitySlider;
    public TMP_Dropdown resolutionDropdown;
    public bool allowchange;
    public void QuitApp()
    {
        Application.Quit();
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Level");
    }

    public void ChangeRes()
    {
        if (allowchange)
        {
            options.resolution = resolutionDropdown.value;
            dataScript.SaveOptions();
        }

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        if (DataScript.instance == null)
        {
            SceneManager.LoadScene("FirstScene");
        }
        else
        {
            dataScript = DataScript.instance;
            options = dataScript.Options;
        }
    }

    public void InitializeSliders()
    {
        allowchange = false;
        MasterSlider.value = options.Mastervol;
        MusicSlider.value = options.MusicVol;
        SFXSlider.value = options.SFXVol;
        SensitivitySlider.value = options.sensibility;
        resolutionDropdown.value = options.resolution;
        allowchange = true;
    }

    public void SaveSliders()
    {
        if (allowchange)
        {
            options.Mastervol = MasterSlider.value;
            options.MusicVol = MusicSlider.value;
            options.SFXVol = SFXSlider.value;
            options.sensibility = SensitivitySlider.value;
            dataScript.SaveOptions();
        }

    }

}
