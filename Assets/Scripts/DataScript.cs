
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataScript : MonoBehaviour
{

    [Serializable]
    public class OptionsClass
    {
        public float Mastervol;
        public float MusicVol;
        public float SFXVol;
        public float sensibility;
        public int resolution;
        public bool fullscreen;
    }

    public OptionsClass Options;

    public static DataScript instance;


    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "FirstScene")
        {
            SceneManager.activeSceneChanged += SceneChange;
            SceneManager.LoadScene("MainMenu");
        }

        LoadOptions();
    }

    private void setResolution()
    {
        switch (Options.resolution)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Options.fullscreen);
                break;
            case 1:
                Screen.SetResolution(1600, 900, Options.fullscreen);
                break;
            case 2:
                Screen.SetResolution(1280, 720, Options.fullscreen);
                break;
            case 3:
                Screen.SetResolution(960, 540, Options.fullscreen);
                break;
        }
    }

    private void Update()
    {


        if (Options.sensibility == 0)
        {
            Options = new OptionsClass();
            Options.MusicVol = 1.000001f;
            Options.SFXVol = 1.000001f;
            Options.Mastervol = 1.000001f;
            Options.sensibility = 0.000001f;
            Options.resolution = 0;
            Options.fullscreen = true;
        }

    }

    private void LoadOptions()
    {
        string path = Application.persistentDataPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string fileName = "options.json";
        string fullPath = Path.Combine(path, fileName);

        try
        {
            string json = File.ReadAllText(fullPath);
            if (json != null)
            {
                Options = JsonUtility.FromJson<OptionsClass>(json);
            }
        }
        catch
        {
            Debug.Log("creating new options data");
            Options = new OptionsClass();
            Options.MusicVol = 1.000001f;
            Options.SFXVol = 1.000001f;
            Options.Mastervol = 1.000001f;
            Options.sensibility = 1.000001f;
            Options.resolution = 0;
            Options.fullscreen = true;
        }
        SoundManager.instance.ChangeVolume();
        setResolution();
    }

    public void SaveOptions()
    {
        if (Options == null)
        {
            Options = new OptionsClass();
        }
        string path = Application.persistentDataPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string fileName = "options.json";
        string fullPath = Path.Combine(path, fileName);
        string json = JsonUtility.ToJson(Options, true);

        try
        {
            File.WriteAllText(fullPath, json);
            Debug.Log($"Options Saved : {fullPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error when saving options : {e.Message}");
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.ChangeVolume();
            setResolution();
        }

    }


    void SceneChange(Scene arg0, Scene arg1)
    {

    }
}
