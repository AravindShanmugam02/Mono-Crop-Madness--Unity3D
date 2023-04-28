using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Public function in Game Manager to Save Player Progress...
    public bool SavePlayerProgress()
    {
        _playerCreditsToSave = CreditManager.Instance.GetCurrentCredits();
        _populationCountToSave = PopulationCountManager.Instance.GetCurrentPopulationCount();

        return (SaveDataToSystem.SaveData(this));
    }

    public void PauseTimeScale(bool toggle) { _pauseTimeScale = toggle; }

    public bool IsSlot1Complete() { return _isSlot1Completed; }
    public bool IsSlot2Complete() { return _isSlot2Completed; }
    public float GetPlayerCreditsToSave() { return _playerCreditsToSave; }
    public int GetPopulationCountToSave() { return _populationCountToSave; }
    public float GetPlayerCredits() { return _playerCredits; }
    public int GetPopulationCount() { return _populationCount; }

    public GameType GetGameTypeToLoad() { return _gameType; }

    private bool _isSlot1Completed = false;
    private bool _isSlot2Completed = false;
    private float _playerCredits = 0f;
    private int _populationCount = 0;
    private float _playerCreditsToSave = 0f;
    private int _populationCountToSave = 0;

    [SerializeField] private SlotScenarioDisplayManager _slotScenarioDisplayManager;

    [SerializeField] private int _daysPerSlot;

    public enum GameSlotsToLoad
    {
        Test, GameOver, Slot1, Slot2
    }
    private GameSlotsToLoad _gameSlotToLoad;
    [SerializeField] private string _gameSlotToLoadInString;
    [SerializeField] private string _currentGameSlotInString;

    public enum GameType
    {
        None, NewGame, LoadGame
    }
    [SerializeField] private GameType _gameType;

    private bool _pauseTimeScale;
    private bool _hasCoroutineRunOnce;
    private bool _hasSlotChangeCoroutineRan;
    private bool _ifFeedbackIsShown;

    private InGameModalBuildingManager _inGameModalBuildingManager;
    private MouseManager _mouseManager;

    private string _aboutGameTitle;
    private string _aboutGameContent;
    private string _aboutGameConclusion;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (StaticCarrier._newGameOrLoadGame == "NewGame")
        {
            _gameType = GameType.NewGame;
            _gameSlotToLoad = GameSlotsToLoad.Slot1;
        }
        else if (StaticCarrier._newGameOrLoadGame == "LoadGame")
        {
            _gameType = GameType.LoadGame;

            PlayerData playerData = SaveDataToSystem.LoadData();

            if (playerData != null)
            {
                _isSlot1Completed = playerData._isSlot1Complete;
                _isSlot2Completed = playerData._isSlot2Complete;
                _playerCredits = playerData._playerCredits;
                _populationCount = playerData._populationCount;
            }
            else
            {
                Debug.LogError("playerData is Empty");
            }

            //When Player Clicks LoadGame in main menu, this is where the slot is decided.
            //Now in a situation when player has completed both slots, the present conditions will result in GameOver Slot.
            //Which at the moment is not used anywhere. Therefore I am gonna comment the Test and GameOver Slots and fill in 
            //with Slot1 or Slot2 as suited.
            if (!_isSlot1Completed && !_isSlot2Completed)
            {
                _gameSlotToLoad = GameSlotsToLoad.Slot1;
            }
            else if(_isSlot1Completed && !_isSlot2Completed)
            {
                _gameSlotToLoad = GameSlotsToLoad.Slot2;
            }
            else if(!_isSlot1Completed && _isSlot2Completed)
            {
                //_gameSlotToLoad = GameSlotsToLoad.Test;
                
                _gameSlotToLoad = GameSlotsToLoad.Slot1; //Since in an hyphothetical scenario
                                                         //when Slot 1 is not done and Slot 2 is done.
            }
            else if(_isSlot1Completed && _isSlot2Completed)
            {
                //_gameSlotToLoad = GameSlotsToLoad.GameOver;

                _gameSlotToLoad = GameSlotsToLoad.Slot2; //Since in an hyphothetical scenario
                                                         //when Slot 1 is done and Slot 2 is done.
            }
        }
        else if (StaticCarrier._newGameOrLoadGame == "GameOver")
        {
            //Nothing as control won't flow here!
        }
        else
        {
            _gameType = GameType.NewGame;
            _gameSlotToLoad = GameSlotsToLoad.Slot1;
        }

        _gameSlotToLoadInString = _gameSlotToLoad.ToString();

        TimeManager.Instance.SetGameSlotToLoad(_gameSlotToLoad);

        //CreditManager.Instance.SetGameTypeToLoad(_gameType);

        //PopulationCountManager.Instance.SetGameTypeToLoad(_gameType);

        _daysPerSlot = 3;
        _pauseTimeScale = false;
        _hasCoroutineRunOnce = false;
        _hasSlotChangeCoroutineRan = false;
        _ifFeedbackIsShown = false;

        _inGameModalBuildingManager = GameObject.Find("InGameModalScreenManager").GetComponent<InGameModalBuildingManager>();
        _mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentGameSlotInString = _gameSlotToLoadInString;

        _slotScenarioDisplayManager.SetSlotName(_currentGameSlotInString);

        if(_inGameModalBuildingManager == null)
        {
            Debug.LogError("_inGameModalBuildingManager is null in Game Manager!");
        }

        _aboutGameTitle = "Thank You For Playing " + _currentGameSlotInString + "!";
        _aboutGameContent = "Hope You Had A Wonderful Experience! \n\n Please Continue.";
        _aboutGameConclusion = "";

        _ifFeedbackIsShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckIfEndOfDay() && !_ifFeedbackIsShown)
        {
            _ifFeedbackIsShown = true;
            FeedbackManager.Instance.ActivateFeedbackScreen(); //Feedback will be shown at the end of day.
        }

        if (_ifFeedbackIsShown)
        {
            CheckIfItIsNotNight();
        }

        if (CheckIfASlotIsComplete() && !_hasSlotChangeCoroutineRan)
        {
            StartCoroutine(ShootSlotChangeSequence());
        }

        //UpdateCurrentGameStatus();
        UpdateTimeScale();

        //if (!_hasCoroutineRunOnce)
        //{
        //    StartCoroutine(ShootAboutGameWindow());
        //}
    }

    bool CheckIfEndOfDay()
    {
        if(TimeManager.Instance.GetCurrentPartOfTheDay() == "Night" && TimeManager.Instance.GetCurrentHour() == 21 &&
            TimeManager.Instance.GetCurrentMinute() >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckIfItIsNotNight()
    {
        if(TimeManager.Instance.GetCurrentPartOfTheDay() != "Night")
        {
            _ifFeedbackIsShown = false;
        }
        else
        {

        }
    }

    bool CheckIfASlotIsComplete()
    {
        if (TimeManager.Instance.GetCurrentDayOrDate() >= _daysPerSlot &&
            (TimeManager.Instance.GetCurrentPartOfTheDay() == "Night" && TimeManager.Instance.GetCurrentHour() == 21 &&
            TimeManager.Instance.GetCurrentMinute() >= 0)) //>= just to not miss the time when fast forwarding time.!
        {
            return true;
        }

        return false;
    }

    //void UpdateCurrentGameStatus()
    //{
    //    if (!_isSlot1Completed && !_isSlot2Completed)
    //    {
    //        _currentGameSlotInString = GameSlotsToLoad.Slot1.ToString();
    //    }
    //    else if (_isSlot1Completed && !_isSlot2Completed)
    //    {
    //        _currentGameSlotInString = GameSlotsToLoad.Slot2.ToString();
    //    }
    //    else if (!_isSlot1Completed && _isSlot2Completed)
    //    {
    //        _currentGameSlotInString = GameSlotsToLoad.Test.ToString();
    //    }
    //    else if (_isSlot1Completed && _isSlot2Completed)
    //    {
    //        _currentGameSlotInString = GameSlotsToLoad.GameOver.ToString();
    //    }
    //}

    void UpdateTimeScale()
    {
        //0 -> Stops game update time. Hence the game stops functioning.
        Time.timeScale = _pauseTimeScale ? 0f : 1f;
        //1 -> Resumes game update time. Hence the game resumes the game.
    }

    //IEnumerator ShootAboutGameWindow()
    //{
    //    //_modalScreenManager.ActivateModalScreenDisplay(false, true, false, _aboutGameTitle, null, 
    //    //    _aboutGameContent + _aboutGameConclusion, "", "", "Start", null, null, 
    //    //    () => { _modalScreenManager.DeactivateModalScreenDisplay(); });
        
    //    _hasCoroutineRunOnce = true;

    //    yield return new WaitForEndOfFrame();
    //}

    //Triggers at the end of a Playing Slot...
    IEnumerator ShootSlotChangeSequence()
    {
        _hasSlotChangeCoroutineRan = true;

        yield return new WaitWhile(CheckIfFeedbackManagerIsActive);
        yield return new WaitForSeconds(2f);
        
        if(_currentGameSlotInString == "Slot1")
        {
            _isSlot1Completed = true;
            _isSlot2Completed = false;
        }
        else if(_currentGameSlotInString == "Slot2")
        {
            _isSlot2Completed = true;
            _isSlot1Completed = true;
        }

        yield return new WaitUntil(SavePlayerProgress);
        yield return new WaitForSeconds(1f);

        //Modal Screen
        if(_currentGameSlotInString == "Slot1")
        {
            InGameModalBuildingManager.InGameModal modalToConstruct = new InGameModalBuildingManager.InGameModal();


            if (HUDManager.Instance.GetOverallCommunityLevel() < 50)
            {
                _aboutGameContent = "Well player, However, Environment level is below 50, which means you should try farming more effectively Player!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }
            else if(HUDManager.Instance.GetOverallCommunityLevel() < 65)
            {
                _aboutGameContent = "Well player. The Environment level is below 65, which means you have got potential to keep it up!!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }
            else if(HUDManager.Instance.GetOverallCommunityLevel() < 80)
            {
                _aboutGameContent = "Well player. The Environment level is below 70, which means you are trying hard to keep this environment healthy! keep it up!!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }
            else if (HUDManager.Instance.GetOverallCommunityLevel() > 80)
            {
                _aboutGameContent = "Well player. The Environment level has reached 80, which means you are gave your best shot! You have won your community's heart!!!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }

            modalToConstruct._inGameModalType = InGameModalBuildingManager.InGameModalType.SinglePageWithNegativeOption;
            modalToConstruct._heading = _aboutGameTitle;
            modalToConstruct._needBodyImg = false;
            modalToConstruct._bodyImg = null;
            modalToConstruct._bodyTxt = _aboutGameContent;
            modalToConstruct._negativeButtonTxt = "Main Menu";
            modalToConstruct._negativeAction = () => { ModalScreenNegativeActionForSlotCompletion(); };
            modalToConstruct._alternateButtonTxt = "";
            modalToConstruct._alternateAction = null;
            modalToConstruct._positiveButtonTxt = "Load Slot 2 Scenario";
            modalToConstruct._positiveAction = () => { ModalScreenPositiveActionForSlotCompletion(); };

            _inGameModalBuildingManager.BuildInGameModalScreenWithMsg(modalToConstruct);
        }
        else if(_currentGameSlotInString == "Slot2")
        {
            InGameModalBuildingManager.InGameModal modalToConstruct = new InGameModalBuildingManager.InGameModal();

            if (HUDManager.Instance.GetOverallCommunityLevel() < 50)
            {
                _aboutGameContent = "You have completed the game! Well player, However, Environment level is below 50, which means you should try farming more effectively Player!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }
            else if (HUDManager.Instance.GetOverallCommunityLevel() < 65)
            {
                _aboutGameContent = "You have completed the game! Well player. The Environment level is below 65, which means you have got potential to keep it up!!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }
            else if (HUDManager.Instance.GetOverallCommunityLevel() < 80)
            {
                _aboutGameContent = "You have completed the game! Well player. The Environment level is below 70, which means you are trying hard to keep this environment healthy! keep it up!!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }
            else if (HUDManager.Instance.GetOverallCommunityLevel() > 80)
            {
                _aboutGameContent = "You have completed the game! Well player. The Environment level has reached 80, which means you are gave your best shot! You have won your community's heart!!!" +
                    "\n\nHope You Had A Wonderful Experience! \n\n Please Continue.";
            }

            modalToConstruct._inGameModalType = InGameModalBuildingManager.InGameModalType.SinglePage;
            modalToConstruct._heading = _aboutGameTitle;
            modalToConstruct._needBodyImg = false;
            modalToConstruct._bodyImg = null;
            modalToConstruct._bodyTxt = _aboutGameContent;
            modalToConstruct._negativeButtonTxt = "";
            modalToConstruct._negativeAction = null;
            modalToConstruct._alternateButtonTxt = "";
            modalToConstruct._alternateAction = null;
            modalToConstruct._positiveButtonTxt = "Continue To Main Menu";
            modalToConstruct._positiveAction = () => { ModalScreenPositiveActionForSlotCompletion(); };

            _inGameModalBuildingManager.BuildInGameModalScreenWithMsg(modalToConstruct);
        }
        else
        {
            //Debug.Log("_currentGameSlotInString is not Slot1 or Slot2 while loading modal screen in Game Manager!");
        }

        //Debug.Log("Check Mouse Locked!");

        yield return new WaitWhile(CheckIfMouseIsLocked);

        //Debug.Log("Mouse is not locked!");

        yield return null;
    }

    bool CheckIfFeedbackManagerIsActive()
    {
        return FeedbackManager.Instance.GetIsFeedbackActive();
    }

    bool CheckIfMouseIsLocked()
    {
        if (_mouseManager.GetIsMouseLocked())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ModalScreenPositiveActionForSlotCompletion()
    {
        if (_currentGameSlotInString == "Slot1" && _isSlot1Completed && !_isSlot2Completed)
        {
            StaticCarrier._newGameOrLoadGame = "LoadGame";

            _inGameModalBuildingManager.DestroyModalScreen();

            SceneManager.UnloadSceneAsync("Scene02_Game");
            SceneManager.LoadScene("Scene02_Game");
        }
        else if (_currentGameSlotInString == "Slot2" && _isSlot1Completed && _isSlot2Completed)
        {
            StaticCarrier._newGameOrLoadGame = "GameOver";//""GameOver wouldn't affect anything as when buttons clicked
                                                          //in main menu, it will either be "NewGame" Or "LoadGame".

            _inGameModalBuildingManager.DestroyModalScreen();

            SceneManager.UnloadSceneAsync("Scene02_Game");
            SceneManager.LoadScene("Scene00_MainMenu");
        }
        else
        {
            Debug.LogError("ModalScreenPositiveActionForSlotCompletion Error in Game Manager!");
        }
    }

    void ModalScreenNegativeActionForSlotCompletion()
    {
        if (_currentGameSlotInString == "Slot1" && _isSlot1Completed && !_isSlot2Completed)
        {
            //StaticCarrier._newGameOrLoadGame = "LoadGame";

            _inGameModalBuildingManager.DestroyModalScreen();

            SceneManager.UnloadSceneAsync("Scene02_Game");
            SceneManager.LoadScene("Scene00_MainMenu");
        }
        else if (_currentGameSlotInString == "Slot2" && _isSlot1Completed && _isSlot2Completed)
        {
            StaticCarrier._newGameOrLoadGame = "GameOver";//""GameOver wouldn't affect anything as when buttons clicked
                                                          //in main menu, it will either be "NewGame" Or "LoadGame".

            _inGameModalBuildingManager.DestroyModalScreen();

            SceneManager.UnloadSceneAsync("Scene02_Game");
            SceneManager.LoadScene("Scene00_MainMenu");
        }
        else
        {
            Debug.LogError("ModalScreenNegativeActionForSlotCompletion Error in Game Manager!");
        }
    }
}