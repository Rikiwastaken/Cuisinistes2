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
    public Toggle FullScreenTOggle;
    public bool allowchange;


    public AudioClip ButtonSound;

    public AudioClip BackSound;

    private SoundManager soundManager;
    public void PlayButtonSound()
    {
        soundManager.PlaySFX(ButtonSound, 0.05f, transform);
    }

    public void PlayBackSound()
    {
        soundManager.PlaySFX(BackSound, 0.05f, transform);
    }

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
            soundManager = SoundManager.instance;
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
        FullScreenTOggle.enabled = options.fullscreen;
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
            options.fullscreen = FullScreenTOggle.enabled;
            dataScript.SaveOptions();
        }

    }

}
