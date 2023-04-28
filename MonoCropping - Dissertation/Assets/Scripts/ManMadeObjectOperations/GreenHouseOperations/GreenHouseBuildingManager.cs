using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreenHouseBuildingManager : MonoBehaviour
{
    public void SetIsGreenhouseOpen(bool toggle) { ToggleGreenhouseView(toggle); }
    public bool GetIsGreenhouseOpen() { return _isGreenhouseOpen; }
    public bool GetIsGreenhouseViewed() { return _isGreenhouseViewed; }
    public Transform GetNextTranform() { Transform transform = _gHSpawnLocationsList[0]; _gHSpawnLocationsList.RemoveAt(0); return transform; }
    public bool CheckNoOfGreenhouse_ToReturnIfOneCanBeBought() { if (_noOfGreenhousePresent < _noOfGreenhouseAllowed) { return true; } return false; }

    public GameObject GetHUDPlantBoxContainer() { return _gHHUDPlantBoxContainer; }
    public List<Image> GetHUDPlantBoxList() { return _gHHUDPlantBoxlist; }
    public GameObject GetHUDWaterSystem() { return _gHHUDWaterSystem; }

    //To check if greenhouse is being viewed or not...
    [SerializeField] private bool _isGreenhouseOpen;
    [SerializeField] private bool _isGreenhouseViewed;

    //Handle for View Inside Green house...
    [SerializeField] private GameObject _gHHUDViewInsideGreenhouse;
    [SerializeField] private Button _gHHUDBackButton;
    //To handle Plant Box HUD Container...
    [SerializeField] private GameObject _gHHUDPlantBoxContainer;
    //List of PlantBoxHUD in a Greenhouse...
    [SerializeField] private List<Image> _gHHUDPlantBoxlist;
    //Handle for Green house Water System HUD...
    [SerializeField] private GameObject _gHHUDWaterSystem;

    [SerializeField] private List<Transform> _gHSpawnLocationsList;
    [SerializeField] private GameObject _greenhouseContainer;
    [SerializeField] private int _noOfGreenhousePresent;
    [SerializeField] private int _noOfGreenhouseAllowed;

    private void Awake()
    {
        _noOfGreenhousePresent = 0;
        _noOfGreenhouseAllowed = 2; //1
    }

    // Start is called before the first frame update
    void Start()
    {
        _gHHUDViewInsideGreenhouse.SetActive(false);
        _gHHUDBackButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _noOfGreenhousePresent = _noOfGreenhouseAllowed - _gHSpawnLocationsList.Count;
    }

    public void ToggleGreenhouseView(bool toggle)
    {
        _isGreenhouseOpen = toggle;
        _isGreenhouseViewed = toggle;
        _gHHUDBackButton.gameObject.SetActive(toggle);
        _gHHUDViewInsideGreenhouse.SetActive(toggle);
    }
}