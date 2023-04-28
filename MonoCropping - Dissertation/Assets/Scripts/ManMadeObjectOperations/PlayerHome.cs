using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using UnityEngine;
using static WaterSourceManager;

public class PlayerHome : MonoBehaviour, iInteractInfo
{
    public bool GetIsSleepTime() { return _isItSleepTime; }

    public enum PlayerHomeCommands
    {
        Sleep
    }

    [SerializeField] private bool _isItSleepTime;
    [SerializeField] private bool _isPlayerSleeping;
    [SerializeField] private List<string> _listOfPlayerHomeCommands;
    [SerializeField] PlayerController _playerController;
    [SerializeField] CameraController _cameraController;

    // Start is called before the first frame update
    void Start()
    {
        _listOfPlayerHomeCommands = new List<string>();
        _listOfPlayerHomeCommands = System.Enum.GetNames(typeof(PlayerHomeCommands)).ToList();

        InteractOptionsManager.Instance.AddAsInteractInfoListener(this);

        TimeManager.Instance.SetPlayerSleepingStatus(false);
        InteractOptionsManager.Instance.SetPlayerSleepingStatus(false);
    }

    // Update is called once per frame
    void Update()
    {
        ValidateTimeOfTheDay();
    }

    void ValidateTimeOfTheDay()
    {
        if (TimeManager.Instance.GetCurrentHour() >= 22 && 
            TimeManager.Instance.GetCurrentMinute() >= 0 && 
            TimeManager.Instance.GetCurrentPartOfTheDay() == "Night")
        {
            _isItSleepTime = true;
        }
        else if (TimeManager.Instance.GetCurrentHour() < 2 && 
            TimeManager.Instance.GetCurrentMinute() >= 0 && 
            TimeManager.Instance.GetCurrentPartOfTheDay() == "MidNight")
        {
            _isItSleepTime = true;
        }
        else
        {
            _isItSleepTime = false;
        }
    }

    public void PlayerInteraction(string action = "")
    {
        PlayerInteract(action);
    }

    private void PlayerInteract(string action)
    {
        //Debug.Log("Button Click in " + this + " " + action);

        if (action == PlayerHomeCommands.Sleep.ToString())
        {
            StartCoroutine(Sleep());
        }
    }

    IEnumerator Sleep()
    {
        if(_isItSleepTime && !_isPlayerSleeping)
        {
            //Set Player Status to Sleep.
            _isPlayerSleeping = true;

            TipManager.Instance.ConstructATip("Player Sleeping!", true);

            //Get rid of the current interaction with Player Home by setting player sleeping status true in Interact Options Manager.
            //(This will break out of ongoing interactions safely. Also will not allow new interactions until player is sleeping)
            InteractOptionsManager.Instance.SetPlayerSleepingStatus(true);

            //Wait while we get false for player interaction status.
            yield return new WaitWhile(InteractOptionsManager.Instance.GetInteractionStatus);

            //Setting Player sleeping status to true in Player controller to restrict any inputs.
            _playerController.SetPlayerSleepingStatus(true);

            //Move Player to Sleep Location.
            _playerController.transform.position = new Vector3(_playerController.GetPlayerSleepPoint().x, 
                _playerController.transform.position.y, _playerController.GetPlayerSleepPoint().z);

            //Setting Player sleeping status to true in Camera controller to restrict any inputs.
            _cameraController.SetPlayerSleepingStatus(true);

            //Move Camera to its Spawn position.
            _cameraController.transform.position = new Vector3(_cameraController.GetCameraSpawnLocation().x,
                _cameraController.GetCameraSpawnLocation().y, _cameraController.GetCameraSpawnLocation().z);

            //Setting bool in time maanager to true indicating player is sleeping, so that time forwards.
            TimeManager.Instance.SetPlayerSleepingStatus(true);

            //Waiting while the sleep time is over. (becomes false)
            yield return new WaitWhile(() => _isItSleepTime);
            //Waiting until morning 3, this is the wakeup time.
            yield return new WaitUntil(CheckIfItIsMorning3);

            //Setting bool in time maanager to false indicating player is awake, so that time resume to normal.
            TimeManager.Instance.SetPlayerSleepingStatus(false);

            //Setting Player sleeping status to false in Camera controller to resume any inputs.
            _cameraController.SetPlayerSleepingStatus(false);

            //Setting Player sleeping status to false in Player controller to resume any inputs.
            _playerController.SetPlayerSleepingStatus(false);

            //Getting back interaction with any objects by setting player sleeping status false in Interact Options Manager.
            //(This will break out of ongoing interactions safely. Also will not allow new interactions until player is sleeping)
            InteractOptionsManager.Instance.SetPlayerSleepingStatus(false);
            TipManager.Instance.ConstructATip("Player Awake!", true);
            //Player is awake.
            _isPlayerSleeping = false;
        }
        else
        {
            //Debug.Log("It is not sleeping time!");
            TipManager.Instance.ConstructATip("It is not sleeping time! Come back at 10.00PM", true);
        }
        yield return null;
    }

    bool CheckIfItIsMorning3()
    {
        if(TimeManager.Instance.GetCurrentHour() == 3 && 
            TimeManager.Instance.GetCurrentMinute() >=0 && 
            TimeManager.Instance.GetCurrentPartOfTheDay() == "EarlyMorning")
        {
            return true;
        }
        return false;
    }

    public void InteractInfoUpdate(RaycastHit outHit)
    {
        if (outHit.collider.tag == "PlayerHome" && outHit.collider.transform == this.transform)
        {
            InteractOptionsManager.Instance.SetInteractOptionsList(this._listOfPlayerHomeCommands);
        }
        else if (outHit.collider.transform == null)
        {
            //Debug.Log("outHit entered in WaterSourceManager!");
            //Debug.LogError("_outHit is null in WaterSourceManager!");
        }
    }
}
