using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public void SetPausedStatus(bool toggle) { ToggleIsGamePaused(toggle); }
    public bool GetPausedStatus() { return _isGamePaused; }

    private GameObject _pauseMenuPanel;
    private Animator _pauseMenuBoxAnimator;
    private bool _isGamePaused;

    //To handle Mouse movement and hiding...
    private MouseManager _mouseManager;

    //To handle HUD info...
    [SerializeField] private GameObject _onScreenInfoDisplayPanel;

    //To handle Audio...
    [SerializeField] private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _pauseMenuPanel = GameObject.Find("PauseMenuPanel").gameObject;
        _pauseMenuBoxAnimator = GameObject.Find("PauseMenuBox").GetComponent<Animator>();
        _mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();

        if (_pauseMenuPanel == null || _pauseMenuBoxAnimator == null || _mouseManager == null)
        {
            Debug.LogError("Error in PauseManager - PauseMenuPanel / PauseMenuBox / MouseManager is null");
        }

        //Setting the bools related to game pausing to false.
        ToggleIsGamePaused(false);
        _pauseMenuPanel?.SetActive(false);
        _pauseMenuBoxAnimator?.SetBool("isPaused", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnGamePause();
            }
        }
    }

    public void OnGamePause()
    {
        if (!_isGamePaused)  //Just a double check...
        {
            //ToggleMouseLockFromPause(false);

            _pauseMenuPanel?.SetActive(true);
            _pauseMenuBoxAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _pauseMenuBoxAnimator?.SetBool("isPaused", true);

            ToggleIsGamePaused(true); //This will stop the timescale...
        }
    }

    public void OnGameResume()
    {
        //ToggleMouseLockFromPause(true);

        _pauseMenuBoxAnimator?.SetBool("isPaused", false);
        _pauseMenuPanel?.SetActive(false);

        ToggleIsGamePaused(false); //This will resume the timescale...
    }

    public void GoToMainMenu()
    {
        //ToggleMouseLockFromPause(false);

        _pauseMenuBoxAnimator?.SetBool("isPaused", false);
        _pauseMenuPanel?.SetActive(false);

        ToggleIsGamePaused(false, "MainMenu"); //This will resume the timescale...

        SceneManager.UnloadSceneAsync("Scene02_Game");
        SceneManager.LoadScene("Scene00_MainMenu");
    }

    public void OnClickSave()
    {
        //Nothing For Now!
    }

    public void OnClickLoad()
    {
        //Nothing For Now!
    }

    public void OnInfoButtonClick()
    {
        _onScreenInfoDisplayPanel.SetActive(!_onScreenInfoDisplayPanel.activeSelf);
    }

    public void OnAudioButtonClick()
    {
        _audioSource.mute = !_audioSource.mute;
    }

    void ToggleIsGamePaused(bool toggle, string action = "") //This strng is for special case,
                                                             //where I don't want the mouse to lock when clikcing main menu option...
    {
        _isGamePaused = toggle;

        //Even though ToggleMouseLockFromPause() is called from the above fucntions that receive button call back,
        //This is a double check if the mouse lock needs to be true or false.
        //Because this function will be accessed by other functions like Feedback Manager and Modal Screen Manager...
        //if (_isGamePaused)
        //{
        //    ToggleMouseLockFromPause(false);
        //}
        //else
        //{
        //    if (action == "MainMenu") //Only when Menu Menu is selected, because we set the timescale back to 1 if main menu is selcted...
        //    {
        //        ToggleMouseLockFromPause(false);
        //    }
        //    else
        //    {
        //        ToggleMouseLockFromPause(true);
        //    }
        //}

        UpdateTimeScale();
    }

    void ToggleMouseLockFromPause(bool toggle)
    {
        _mouseManager.ToggleMouseLock(toggle);
    }

    void UpdateTimeScale()
    {
        if (_isGamePaused)
        {
            GameManager.Instance.PauseTimeScale(_isGamePaused);
        }
        else
        {
            GameManager.Instance.PauseTimeScale(_isGamePaused);
        }
    }
}