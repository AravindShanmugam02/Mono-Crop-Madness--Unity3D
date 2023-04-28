using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class MainMenuManager : MonoBehaviour
{
    static string savedPath;
    BinaryFormatter formatter = new BinaryFormatter();

    [SerializeField] private Button _startNewGameButton;
    [SerializeField] private Button _loadSaveButton;
    [SerializeField] private Button _deleteSaveButton;
    [SerializeField] private Button _exitGameButton;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _mainMenuBGM;
    [SerializeField] private Toggle _muteAudioToggle;

    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private bool _toggleCredits;
    [SerializeField] private GameObject _howToLoadPanel;
    [SerializeField] private bool _toggleHowToLoad;

    [SerializeField] private Image _underPanel;

    private void Awake()
    {
        //Using Application.persistentDataPath here in Awake / Start because - UnityException: get_persistentDataPath is not
        //allowed to be called from a MonoBehaviour constructor (or instance field initializer),
        //call it in Awake or Start instead. Called from MonoBehaviour 'MainMenuManager' on game object 'MainMenuManager'.
        savedPath = Application.persistentDataPath + "/MCMSD.mcm"; //MonoCropMadnessSaveData.monocropmadness

        //_loadSaveButton.gameObject.SetActive(CheckForSavedData()/*false*/);
        //_deleteSaveButton.gameObject.SetActive(CheckForSavedData()/*false*/);

        UpdateLoadSaveAndDeleteSaveButtons();
    }

    // Start is called before the first frame update
    void Start()
    {
        //This is incase if the mouse cursor is set to lock mode in previous screen. (It should have updated to unlocked, but just for safety)
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        _toggleCredits = false;
        _toggleHowToLoad = false;

        _creditsPanel.SetActive(_toggleCredits);
        _howToLoadPanel.SetActive(_toggleHowToLoad);

        _audioSource.clip = _mainMenuBGM;
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (_muteAudioToggle.isOn)
        {
            _audioSource.mute = false;
        }
        else
        {
            _audioSource.mute = true;
        }

        Color color = Color.Lerp(Color.red, Color.blue, Mathf.Lerp(0, 2, Mathf.Cos(Time.time))); //Mathf.PingPong(Time.time, 2)
        color.a = 0.6f;
        _underPanel.color = color;
    }

    void UpdateLoadSaveAndDeleteSaveButtons()
    {
        _loadSaveButton.gameObject.SetActive(CheckForSavedData()/*false*/);
        _deleteSaveButton.gameObject.SetActive(CheckForSavedData()/*false*/);
    }

    bool CheckForSavedData()
    {
        if (File.Exists(savedPath))
        {
            //Debug.Log("Saved Data Found!");
            return true;
        }
        else
        {
            //Debug.Log("No Saved Data Found!");
            return false;
        }
    }

    //Starts a new game all over...
    public void StartNewGame()
    {
        StaticCarrier._newGameOrLoadGame = "NewGame";

        SceneManager.UnloadSceneAsync("Scene00_MainMenu");
        SceneManager.LoadScene("Scene01_Intro");
    }

    public void LoadSavedGame()
    {
        if (CheckForSavedData())
        {
            StaticCarrier._newGameOrLoadGame = "LoadGame";

            SceneManager.UnloadSceneAsync("Scene00_MainMenu");
            SceneManager.LoadScene("Scene02_Game");
        }
        else
        {
            //Debug.Log("Data Not Loaded!");
        }
    }

    //private bool LoadSavedFile()
    //{
    //    if (File.Exists(savedPath))
    //    {
    //        //FileStream fStream = new FileStream(savedPath, FileMode.Open, FileAccess.Read);
    //        ////We have to tell it which type of data we would be needing it as - hence casting is done in the end.
    //        //PlayerData playerData = formatter.Deserialize(fStream) as PlayerData;
    //        //fStream.Close();

    //        //StaticCarrier._isSlot1Complete = playerData._isSlot1Complete;
    //        //StaticCarrier._isSlot2Complete = playerData._isSlot2Complete;
    //        //StaticCarrier._playerCredits = playerData._playerCredits;
    //        //StaticCarrier._populationCount = playerData._populationCount;

    //        //Debug.Log("Data Loaded!");

    //        return true;
    //    }
    //    else
    //    {
    //        Debug.Log("No Saved Data Found!");

    //        return false;
    //    }
    //}

    public void DeleteSavedFile()
    {
        if (CheckForSavedData())
        {
            SaveDataToSystem.DeleteData();
        }
        else
        {
            //Debug.Log("No Save File Found!");
        }

        if (CheckForSavedData())
        {
            //Debug.Log("File Not Deleted!");
        }
        else
        {
            UpdateLoadSaveAndDeleteSaveButtons();
        }
    }

    //Exits the game...
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnCreditsButtonClick()
    {
        _toggleCredits = !_toggleCredits;
        _creditsPanel.SetActive(_toggleCredits);
        _howToLoadPanel.SetActive(false);
        _toggleHowToLoad = false;
    }

    public void OnHowToLoadButtonClick()
    {
        _toggleHowToLoad = !_toggleHowToLoad;
        _howToLoadPanel.SetActive(_toggleHowToLoad);
        _creditsPanel.SetActive(false);
        _toggleCredits = false;
    }
}