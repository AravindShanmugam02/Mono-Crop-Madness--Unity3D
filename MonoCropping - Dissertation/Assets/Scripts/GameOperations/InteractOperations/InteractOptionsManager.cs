using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.EventSystems;

public class InteractOptionsManager : MonoBehaviour
{
    public void AddAsInteractInfoListener(iInteractInfo listener) { RegisterAsInteractInfoListener(listener); }
    public void RemoveFromInteractInfoListener(iInteractInfo listener) { UnregisterFromInteractInfoListeners(listener); }
    public bool GetInteractionStatus() { return _isInteracting; }
    public void SetInteractOptionsList(List<string> _interactOptions)
    {
        //_interactOptionsList = _interactOptions; //Works first time of assignment. second time doesn't work.

        //Can't directly assign one list to another (ateast after using list.clear()), thus ignoring above and doing below:
        foreach (string s in _interactOptions)
        {
            _interactOptionsList.Add(s);
        }
    }

    public void SetPlayerSleepingStatus(bool toggle) { _isPlayerSleeping = toggle; }

    private CrosshairManager _crosshairManager;
    private GameObject _initialInteractPanel;
    private GameObject _interactOptionsPanel;
    private GameObject _interactOptionsContentBox;
    private TextMeshProUGUI _initialInteractText;
    [SerializeField] private bool _isInteracting = false;
    [SerializeField] private bool _canInstantiateButtons = false;

    //Interact Info Listeners List
    [SerializeField] private List<iInteractInfo> _interactInfolistenersList;

    //Different interact options list
    [SerializeField] private List<string> _interactOptionsList;

    //Button prefab...
    [SerializeField] private GameObject _buttonPrefab;

    //Button prefab holder...
    private GameObject _btnPref;

    //Button Prefab List...
    private List<GameObject> _buttonPrefabList;

    //Interact Options Name...
    private string _interactOptionsName;

    //To handle interaction when player is sleeping...
    private bool _isPlayerSleeping;

    private string _isNight_Midnight;

    #region Awake - Singleton Monobehaviour
    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static InteractOptionsManager Instance { get; private set; }

    //Making this class to be a singleton monobehaviour class - only one instace will be created for this class.
    private void Awake()
    {
        //if there is more than one instance, destroy the extras.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static Instance to this Instance.
            Instance = this;
        }

        _interactInfolistenersList = new List<iInteractInfo>();
        _interactOptionsList = new List<string>();
        _buttonPrefabList = new List<GameObject>();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _crosshairManager = CrosshairManager.Instance;
        
        _initialInteractPanel = GameObject.Find("InitialInteractPanel");
        _initialInteractText = _initialInteractPanel?.GetComponentInChildren<TextMeshProUGUI>();

        _interactOptionsPanel = GameObject.Find("InteractOptionsPanel");
        _interactOptionsContentBox = GameObject.Find("InteractOptionsContentBox");

        //_buttonPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/2D/UI/OptionButton.prefab", typeof(GameObject)) as GameObject;

        _initialInteractPanel?.SetActive(false);
        _interactOptionsPanel?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(_crosshairManager.GetHitInfo().transform != null)
        {
            UpdateInteractObjects(true, _crosshairManager.GetHitInfo());
        }
        else
        {
            UpdateInteractObjects(false, _crosshairManager.GetHitInfo());
        }


        _isNight_Midnight = TimeManager.Instance.GetCurrentPartOfTheDay().ToString();
    }

    //Handling registering as listener.
    void RegisterAsInteractInfoListener(iInteractInfo listener)
    {
        _interactInfolistenersList.Add(listener);
    }

    //Handling Unregistering from listeners.
    void UnregisterFromInteractInfoListeners(iInteractInfo listener)
    {
        _interactInfolistenersList.Remove(listener);
    }

    void UpdateInteractObjects(bool isHit, RaycastHit hitObj)
    {
        if (isHit && (hitObj.collider.tag == "Land" 
            || hitObj.collider.tag == "Greenhouse" 
            || hitObj.collider.tag == "WaterSource" 
            || hitObj.collider.tag == "Barn" 
            || hitObj.collider.tag == "GrainsAndSeedsStorage"
            || hitObj.collider.tag == "PlayerHome") && !_isPlayerSleeping /*&& *//*( !_isInteracting && _isNight_Midnight != "Night" && _isNight_Midnight != "MidNight")*/)
        {
            if (_initialInteractPanel.activeSelf == false)
            {
                _initialInteractPanel?.SetActive(true);
            }

            //Debug.Log("Last Cropped :: " + hitObj.collider.GetComponent<LandManager>().GetLastCultivated());

            _initialInteractText.text = "Press E To Interact";

            //Need to check if this runs only once when pressed E or does it run every frame after pressing E.
            if (_initialInteractPanel.activeSelf == true && Input.GetKeyDown(KeyCode.E))
            {
                _isInteracting = true;

                //_interactOptionsList = new List<string>(); //Since using clear at bottom...

                _interactOptionsPanel?.SetActive(true);

                foreach (iInteractInfo IO in _interactInfolistenersList)
                {
                    IO.InteractInfoUpdate(hitObj); //_crosshairManager.GetHitInfo()
                }

                //Debug.Log("Interact Options List :: " + _interactOptionsList.Count);
                //Debug.Log("CanInstantiateButtons :: " + _canInstantiateButtons);

                //list.count gives the number of elements present in that list. list.capacity gives the size of container, i.e. list.
                if (_interactOptionsList.Count != 0 && _canInstantiateButtons == true)
                {
                    _buttonPrefabList = new List<GameObject>(); //Since using clear at bottom...

                    for(int i = 0; i < _interactOptionsList.Count; i++)
                    {
                        _btnPref = Instantiate(_buttonPrefab, _interactOptionsContentBox.transform);
                        _btnPref.transform.name = _interactOptionsList[i];
                        _btnPref.GetComponentInChildren<TextMeshProUGUI>().text = _interactOptionsList[i];
                        //_interactOptionsName = _btnPref.transform.name;// _interactOptionsList[i];
                        //_btnPref.GetComponent<Button>().onClick.AddListener(()
                        //    => _hitObj.collider.GetComponent<LandManager>().Interaction(_btnPref.transform.name));
                        _buttonPrefabList.Add(_btnPref);
                    }

                    if(hitObj.collider.tag == "Land")
                    {
                        LandManager hitLandManager = hitObj.collider.GetComponent<LandManager>();

                        foreach (GameObject g in _buttonPrefabList)
                        {
                            if (hitLandManager.GetLandOwnership() == LandManager.LandOwnership.Player)
                            {
                                g.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
                                    ////To Prevent elements behind the current UI element to be interacted with mouse...
                                    ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
                                    //if (EventSystem.current.IsPointerOverGameObject())
                                    //{
                                    //    return;
                                    //}

                                    //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

                                    /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
                                     * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
                                     * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
                                     * meaning, this method, which I've seen repeated... everywhere... 
                                     * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
                                    #endregion

                                    if (hitObj.collider.tag == "Land" && _isInteracting && (_isNight_Midnight == "Night" || _isNight_Midnight == "MidNight"))
                                    {
                                        TipManager.Instance.ConstructATip("You cannot interact with farm lands during Night/MidNight!", true);

                                        _interactOptionsList.Clear();
                                        _interactOptionsPanel?.SetActive(false);
                                        _canInstantiateButtons = true;
                                        _isInteracting = false;

                                        if (_buttonPrefabList.Count != 0)
                                        {
                                            foreach (GameObject g in _buttonPrefabList)
                                            {
                                                Destroy(g);
                                            }
                                        }

                                        _buttonPrefabList.Clear();
                                    }
                                    else
                                    {
                                        hitLandManager.PlayerInteraction(g.transform.name);
                                    }
                                });
                            }
                            else if (hitLandManager.GetLandOwnership() == LandManager.LandOwnership.AI)
                            {
                                g.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
                                    ////To Prevent elements behind the current UI element to be interacted with mouse...
                                    ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
                                    //if (EventSystem.current.IsPointerOverGameObject())
                                    //{
                                    //    return;
                                    //}

                                    //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

                                    /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
                                     * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
                                     * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
                                     * meaning, this method, which I've seen repeated... everywhere... 
                                     * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
                                    #endregion

                                    if (hitObj.collider.tag == "Land" && _isInteracting && (_isNight_Midnight == "Night" || _isNight_Midnight == "MidNight"))
                                    {
                                        TipManager.Instance.ConstructATip("You cannot interact with farm lands during Night/MidNight!", true);

                                        _interactOptionsList.Clear();
                                        _interactOptionsPanel?.SetActive(false);
                                        _canInstantiateButtons = true;
                                        _isInteracting = false;

                                        if (_buttonPrefabList.Count != 0)
                                        {
                                            foreach (GameObject g in _buttonPrefabList)
                                            {
                                                Destroy(g);
                                            }
                                        }

                                        _buttonPrefabList.Clear();
                                    }
                                    else
                                    {
                                        hitLandManager.AIInteraction(g.transform.name, "Player");
                                    }
                                });
                            }
                            else
                            {
                                //Debug.Log("Land Ownership Didn't Match In Interact Options Manager!");
                            }
                        }
                    }
                    else if(hitObj.collider.tag == "Greenhouse")
                    {
                        #region COMMENTS ON WHY HITOBJ.COLLIDER IS ROUTING TO ITS PARENT
                        //Since there is a special case with Green House object, the outHit.collider will not be the GreenHouse object itself but its children.
                        //Its direct children have tag "GreenHouse". That is the reason why ray hits and detects the GreenHouse.
                        //Due to which the below condition would fail all the time if we treat outHit as the actual GreenHouse object.
                        //It is because of the fact that the GreenHouse object doesn't have any mesh or collider of its own. Its children make up the meshes and colliders we see on the scene.
                        //Therefore what we understood here is that, when outHit detects a collider with tag "GreenHouse" it is actually the children objects.
                        //So we got to route to the parent GreenHouse object that has this script to avoid interaction errors and to interact with the GreenHouse object properly.
                        #endregion

                        GreenHouseManager hitGreenHouseManager = hitObj.collider.transform.parent.GetComponent<GreenHouseManager>();

                        foreach (GameObject g in _buttonPrefabList)
                        {
                            if (hitGreenHouseManager.GetGreenHouseOwnership() == GreenHouseManager.GreenHouseOwnership.Player)
                            {
                                g.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
                                    ////To Prevent elements behind the current UI element to be interacted with mouse...
                                    ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
                                    //if (EventSystem.current.IsPointerOverGameObject())
                                    //{
                                    //    return;
                                    //}

                                    //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

                                    /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
                                     * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
                                     * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
                                     * meaning, this method, which I've seen repeated... everywhere... 
                                     * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
                                    #endregion

                                    hitGreenHouseManager.PlayerInteraction(g.transform.name);
                                });
                            }
                            else
                            {
                                //Debug.Log("Greenhouse Ownership Didn't Match In Interact Options Manager!");
                            }
                        }
                    }
                    else if(hitObj.collider.tag == "WaterSource")
                    {
                        WaterSourceManager hitWaterSourceManager = hitObj.collider.GetComponent<WaterSourceManager>();

                        foreach (GameObject g in _buttonPrefabList)
                        {
                            g.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
                                ////To Prevent elements behind the current UI element to be interacted with mouse...
                                ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
                                //if (EventSystem.current.IsPointerOverGameObject())
                                //{
                                //    return;
                                //}

                                //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

                                /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
                                 * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
                                 * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
                                 * meaning, this method, which I've seen repeated... everywhere... 
                                 * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
                                #endregion

                                hitWaterSourceManager.PlayerInteraction(g.transform.name);
                            });
                        }
                    }
                    else if (hitObj.collider.tag == "Barn")
                    {
                        //SIMILAR TO GREENHOUSE, BARN TOO DOESN'T HAVE A COLLIDER. ONE OF ITS CHILDREN HAS A COLLIDER.
                        //THUS WE hAVE TO ROUTE TO ITS PARENT UPON RAYCAST HIT INTERACTION.
                        //UNLIKE GREENSHOUSE, THERE IS ONE MORE CONDITION FOR BARN. THERE ARE SOME SUB CHILDREN UNDER DIRECT CHILDREN.
                        //THERE FOR hitObj.collider.transform.parent WON'T WORK. BECAUSE IF SUB CHILDREN GET HIT BY RAYCAST, IT SHOULD BE PARENT OF PARENT.
                        //hitObj.collider.GetComponentInParent WOULD DO A RECURRSIVE SEARCH TO GET BarnManager COMPONENT.

                        BarnManager hitBarnManager = hitObj.collider./*transform.parent.*/GetComponentInParent<BarnManager>();

                        foreach (GameObject g in _buttonPrefabList)
                        {
                            g.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
                                ////To Prevent elements behind the current UI element to be interacted with mouse...
                                ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
                                //if (EventSystem.current.IsPointerOverGameObject())
                                //{
                                //    return;
                                //}

                                //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

                                /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
                                 * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
                                 * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
                                 * meaning, this method, which I've seen repeated... everywhere... 
                                 * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
                                #endregion

                                hitBarnManager.PlayerInteraction(g.transform.name);
                            });
                        }
                    }
                    else if (hitObj.collider.tag == "GrainsAndSeedsStorage")
                    {
                        GrainsAndSeedsStorageManager hitGrainsAndSeedsStorageManager = hitObj.collider.GetComponent<GrainsAndSeedsStorageManager>();

                        foreach (GameObject g in _buttonPrefabList)
                        {
                            g.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
                                ////To Prevent elements behind the current UI element to be interacted with mouse...
                                ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
                                //if (EventSystem.current.IsPointerOverGameObject())
                                //{
                                //    return;
                                //}

                                //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

                                /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
                                 * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
                                 * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
                                 * meaning, this method, which I've seen repeated... everywhere... 
                                 * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
                                #endregion

                                hitGrainsAndSeedsStorageManager.PlayerInteraction(g.transform.name);
                            });
                        }
                    }
                    else if (hitObj.collider.tag == "PlayerHome")
                    {
                        PlayerHome hitPlayerHome = hitObj.collider.GetComponent<PlayerHome>();

                        foreach (GameObject g in _buttonPrefabList)
                        {
                            g.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
                                ////To Prevent elements behind the current UI element to be interacted with mouse...
                                ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
                                //if (EventSystem.current.IsPointerOverGameObject())
                                //{
                                //    return;
                                //}

                                //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

                                /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
                                 * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
                                 * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
                                 * meaning, this method, which I've seen repeated... everywhere... 
                                 * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
                                #endregion

                                //Special condition for Sleep alone as a workaround.
                                if (g.transform.name == "Sleep")//This is because once sleep is selected the interact options just stay as it is and gives a bug.
                                                                //If you wanna try it out, comment this if condition fully and place the
                                                                //hitPlayerHome.PlayerInteraction("g.transform.name"); after this if condition.
                                {
                                    string temp = g.transform.name;

                                    if (hitPlayerHome.GetIsSleepTime()) //Menu won't destroy if it isn't sleep time.
                                    {
                                        foreach (GameObject g in _buttonPrefabList)
                                        {
                                            Destroy(g);
                                        }

                                        _buttonPrefabList.Clear();
                                        _interactOptionsList.Clear();
                                        _interactOptionsPanel?.SetActive(false);
                                        _canInstantiateButtons = true;
                                        _isInteracting = false;
                                    }

                                    hitPlayerHome.PlayerInteraction(temp);
                                }
                            });
                        }
                    }
                    else
                    {
                        //Debug.LogError("hitObj.collider.tag didn't match in Interact Options Manager! Check the tags of collider!");
                    }

                    _canInstantiateButtons = false;
                }
                else if(_interactOptionsList.Count == 0)
                {
                    //Debug.LogError("_interactOptionsList is empty in Interact Options Manager");
                }
            }

            if (Input.GetKeyDown(KeyCode.Q)) //this is to smoothly 
            {
                foreach(GameObject g in _buttonPrefabList)
                {
                    Destroy(g);
                }

                _buttonPrefabList.Clear();
                _interactOptionsList.Clear();
                _interactOptionsPanel?.SetActive(false);
                _canInstantiateButtons = true;
                _isInteracting = false;
            }

            if(hitObj.collider.tag == "Land" && _isInteracting && (_isNight_Midnight == "Night" || _isNight_Midnight == "MidNight"))
            {
                TipManager.Instance.ConstructATip("You cannot interact with farm lands during Night/MidNight!", true);

                if(_buttonPrefabList.Count != 0)
                {
                    foreach (GameObject g in _buttonPrefabList)
                    {
                        Destroy(g);
                    }
                }

                _buttonPrefabList.Clear();
                _interactOptionsList.Clear();
                _interactOptionsPanel?.SetActive(false);
                _canInstantiateButtons = true;
                _isInteracting = false;
            }
        }
        else
        {
            //Clearing does cause the count to go 0, but doesn't change the capacity.

            _interactOptionsList.Clear();   //We can't assign one list to another directly, atleast after using list.clear().

            //foreach(string s in _interactOptionsList)
            //{
            //    s.Replace(s, "");
            //}

            if (_initialInteractPanel.activeSelf == true)
            {
                _initialInteractPanel?.SetActive(false);
            }

            _canInstantiateButtons = true;
            _isInteracting = false;
        }
    }
}