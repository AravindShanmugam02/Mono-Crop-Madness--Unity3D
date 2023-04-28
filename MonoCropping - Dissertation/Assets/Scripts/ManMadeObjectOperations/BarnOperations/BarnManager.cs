using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.Collections.Concurrent;
using Unity.VisualScripting;

[System.Serializable]
public class BarnManager : MonoBehaviour, iInteractInfo, InventoryInfo/*ISerializationCallbackReceiver*/
{
    public Dictionary<GameItemsData, int> GetBarnItemsAndCounts() { Dictionary<GameItemsData, int> temp = new Dictionary<GameItemsData, int>();
        foreach(var item in _barnItemsAndCount)
        {
            temp.Add(item.Key, item.Value);
        }
        return temp;
    }
    public int GetStorageItemsCount() { return _storageItemsCount; }
    public int GetStorageFoodItemsCount() { return _storageFoodItemsCount; }
    public int GetStorageFoodItemsTargetCount() { return _targetFoodCount; }
    public int GetStorageFoodItemsSafeCount() { return _safeFoodCount; }
    public bool GetBarnOpenStatus() { return _isBarnOpen; }



    //Barn Commands...
    public enum PlayerInteractionOptionsForBarn
    {
        StoreAll, StoreSelected, ViewInsideBarn
    }
    //Barn Commands List...
    [SerializeField] private List<string> _listOfCommandsForBarn;

    //To store the equiped crop seed item information...
    private GameItemsSlotData _equipedItemSlotDataCS;

    //Dictionary varibales...
    [SerializeField] private List<GameItemsData> _keys;
    [SerializeField] private List<int> _values;
    private Dictionary<GameItemsData, int> _barnItemsAndCount;

    //Barn Slots...
    [SerializeField] private Image[] _barnSlots = new Image[10];
    private GameObject _viewInsideBarn;

    //Is Barn Open variable...
    private bool _isBarnOpen = false;

    //Back button...
    private Button _backButton;

    //Default Sprite...
    [SerializeField] private Sprite _defaultSpriteToDisplay;

    [Header("Other Variables")]
    //Count of unique items in the barn...
    [SerializeField] private int _countOfUniqueBarnItems;
    //Storage variables for barn...
    [SerializeField] private int _storageItemsCount;
    [SerializeField] private int _previousStorageItemsCount;
    [SerializeField] private int _storageFoodItemsCount;
    [SerializeField] private int _targetFoodCount;
    [SerializeField] private int _currentFoodCount;
    [SerializeField] private int _safeFoodCount;
    [SerializeField] private int _twentyPercentageOfTarget;
    //Variables related to population...
    [SerializeField] private bool _hasTheCommunityBeenFedToday;
    //Variables for day and night...
    [SerializeField] private bool _isNight8 = false;
    [SerializeField] private bool _isMorning7 = false;

    #region Awake - Singleton Monobehaviour
    public static BarnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _equipedItemSlotDataCS = new GameItemsSlotData();

        _listOfCommandsForBarn = System.Enum.GetNames(typeof(PlayerInteractionOptionsForBarn)).ToList();

        _viewInsideBarn = GameObject.Find("ViewInsideBarn").gameObject;
        _backButton = GameObject.Find("BarnBackButton").GetComponent<Button>();

        if (_viewInsideBarn == null || _backButton == null)
        {
            Debug.LogError("_viewInsideBarn is null or _backButton is null in BarnManager!");
        }

        _viewInsideBarn.SetActive(false);
        _backButton.gameObject.SetActive(false);

        //_cropItemsInventorySlotList = new List<GameItemsSlotData>();

        _keys = new List<GameItemsData>();
        _values = new List<int>();

        _barnItemsAndCount = new Dictionary<GameItemsData, int>();

        //_defaultSpriteToDisplay = AssetDatabase.LoadAssetAtPath("Assets/Textures/Sprites/EmptySlot.png", typeof(Sprite)) as Sprite;

        _storageItemsCount = 0;
        _previousStorageItemsCount = _storageItemsCount;

        _targetFoodCount = 50;
        _safeFoodCount = 150;
        _currentFoodCount = _storageFoodItemsCount;
        _twentyPercentageOfTarget = (_targetFoodCount * 20) / 100;

        InteractOptionsManager.Instance.AddAsInteractInfoListener(this);
        InventoryDisplayManager.Instance.AddAsInventoryInfoListener(this);

        StartCoroutine(PopulationConsumeProcess());
    }

    // Update is called once per frame
    void Update()
    {
        _countOfUniqueBarnItems = _barnItemsAndCount.Count;
        _currentFoodCount = _storageFoodItemsCount;
    }

    //void OnGUI()
    //{
    //    foreach (var kvp in _barnItemsAndCount)
    //        GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    //}

    IEnumerator PopulationConsumeProcess()
    {
        while (true)
        {
            _hasTheCommunityBeenFedToday = false;

            //Debug.Log("Entered!");
            yield return new WaitUntil(isNight8);
            //Debug.Log("Working!");

            List<GameItemsData> tempKeyList = new List<GameItemsData>();
            List<int> tempValueList = new List<int>();

            tempKeyList.AddRange(_barnItemsAndCount.Keys);
            tempValueList.AddRange(_barnItemsAndCount.Values);

            //Debug.Log("Keys Size" + tempKeyList.Count);
            //Debug.Log("values size" + tempValueList.Count);

            //yield return new WaitForSeconds(30f);

            if (_currentFoodCount > _targetFoodCount)
            {
                //Debug.Log("Check for items");

                yield return new WaitUntil(() => { if(_barnItemsAndCount.Count != 0) { return true; } else { return false; }});

                //Debug.Log("Has items in it");

                int amountDeducted = 0;

                while(_targetFoodCount - amountDeducted > 0)
                {
                    //Debug.Log("Under while");
                    //foreach (var item in _barnItemsAndCount)
                    for (int i = 0; i < Mathf.Min(tempKeyList.Count, tempValueList.Count); i++)
                    {
                        //Debug.Log("Under for");
                        if (tempKeyList[i]._isEdible == GameItemsData.IsEdible.Edible &&
                            tempKeyList[i]._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
                        {
                            if (tempValueList[i] >= _twentyPercentageOfTarget)
                            {
                                int remaining = _targetFoodCount - amountDeducted;

                                if (remaining >= _twentyPercentageOfTarget)
                                {
                                    //_barnItemsAndCount[item.Key] = item.Value - _twentyPercentageOfTarget;
                                    tempValueList[i] = tempValueList[i] - _twentyPercentageOfTarget;

                                    amountDeducted += _twentyPercentageOfTarget;
                                }
                                else/* if (remaining < _twentyPercentageOfTarget)*/
                                {
                                    //_barnItemsAndCount[item.Key] = item.Value - remaining;
                                    tempValueList[i] = tempValueList[i] - remaining;

                                    amountDeducted += remaining;
                                }
                            }
                            else
                            {
                                amountDeducted += tempValueList[i];

                                //_barnItemsAndCount[item.Key] = item.Value - item.Value;
                                tempValueList[i] = tempValueList[i] - tempValueList[i];
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (_targetFoodCount - amountDeducted <= 0)
                        {
                            break;
                        }

                        //Debug.Log("For Iteration" + i);
                    }

                    //Debug.Log("While Iteration");
                }

                for(int i = 0; i < Mathf.Min(tempKeyList.Count, tempValueList.Count); i++)
                {
                    _barnItemsAndCount[tempKeyList[i]] = tempValueList[i];
                }

                _hasTheCommunityBeenFedToday = true;
                TipManager.Instance.ConstructATip("Community was fed today!", true);
            }
            else
            {
                _hasTheCommunityBeenFedToday = false;
                //Debug.Log("You have not reached the target for tonight! This results in starving!");
                TipManager.Instance.ConstructATip("You have not reached the target food count for tonight! This results in community starving!", true);
            }

            if (!_hasTheCommunityBeenFedToday)
            {
                CommunityManager.Instance.SetIsCommunityFedToday(false);
            }
            else
            {
                CommunityManager.Instance.SetIsCommunityFedToday(true);
            }

            UpdateBarnCounts();

            yield return new WaitUntil(isMorning7);
        }
    }

    private bool isNight8()
    {
        if (TimeManager.Instance.GetCurrentPartOfTheDay() == "Night" && 
            TimeManager.Instance.GetCurrentHour() == 20 && 
            TimeManager.Instance.GetCurrentMinute() == 0)
        {
            //Debug.Log("Its Night 8");
            _isNight8 = true;
        }
        else
        {
            _isNight8 = false;
        }

        return _isNight8;
    }

    private bool isMorning7()
    {
        if (TimeManager.Instance.GetCurrentPartOfTheDay() == "Morning" && 
            TimeManager.Instance.GetCurrentHour() == 7 && 
            TimeManager.Instance.GetCurrentMinute() == 0)
        {
            //Debug.Log("Its Morning 7");
            _isMorning7 = true;
        }
        else
        {
            _isMorning7 = false;
        }

        return _isMorning7;
    }

    public void PlayerInteraction(string action = "")
    {
        PlayerInteract(action);
    }

    private void PlayerInteract(string action)
    {
        //Debug.Log("Button Click in " + this + " " + action);

        if (action == PlayerInteractionOptionsForBarn.StoreAll.ToString())
        {
            StartCoroutine(StoreAllFromInventoryToBarn());
        }
        else if (action == PlayerInteractionOptionsForBarn.StoreSelected.ToString())
        {
            StartCoroutine(StoreSelectedFromInventoryToBarn());
        }
        else if (action == PlayerInteractionOptionsForBarn.ViewInsideBarn.ToString())
        {
            ViewInsideBarn();
        }
    }

    IEnumerator StoreAllFromInventoryToBarn()
    {
        GameItemsSlotData[] _cropItemsInventorySlotArray = InventoryManager.Instance.GetInventoryGameItemsSlot(InventorySlot.InventorySlotType.CS);

        List<int> indexes = new List<int>();

        yield return new WaitForSeconds(0f);

        if (_cropItemsInventorySlotArray.Length != 0)
        {
            for (int i = 0; i < _cropItemsInventorySlotArray.Length; i++)
            {
                if (!_cropItemsInventorySlotArray[i].CheckIsEmpty() &&
                    _cropItemsInventorySlotArray[i].GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
                {
                    if (_barnItemsAndCount.Count == 0)
                    {
                        _barnItemsAndCount.TryAdd(_cropItemsInventorySlotArray[i].GetGameItemsData(), _cropItemsInventorySlotArray[i].GetQuantity());
                        //Debug.Log("New:: " + _cropItemsInventorySlotArray[i].GetGameItemsData());
                    }
                    else
                    {
                        bool sameItemFound = false;

                        foreach (var item1 in _barnItemsAndCount)
                        {
                            if (item1.Key == _cropItemsInventorySlotArray[i].GetGameItemsData())
                            {
                                _barnItemsAndCount[item1.Key] = item1.Value + _cropItemsInventorySlotArray[i].GetQuantity();
                                //Debug.Log("Same:: " + _cropItemsInventorySlotArray[i].GetGameItemsData());
                                sameItemFound = true;
                                break;
                            }
                            else
                            {
                                sameItemFound = false;
                            }
                        }

                        if (!sameItemFound)
                        {
                            _barnItemsAndCount.TryAdd(_cropItemsInventorySlotArray[i].GetGameItemsData(), _cropItemsInventorySlotArray[i].GetQuantity());
                           //Debug.Log("NotSame:: " + _cropItemsInventorySlotArray[i].GetGameItemsData());
                        }
                    }

                    indexes.Add(i);
                }
                else
                {
                    //Debug.Log("item in inventory is null in BarnManager!");
                }
            }

            if(indexes.Count <= 0)
            {
                TipManager.Instance.ConstructATip("There are no crops in your inventory!", true);
            }

            InventoryManager.Instance.RemoveObjectsFromInventorySlots(indexes, InventorySlot.InventorySlotType.CS);
        }
        else
        {
            //Debug.Log("_cropItemsInventorySlot is empty in BarnManager!");
            TipManager.Instance.ConstructATip("There are no crops in your inventory!", true);
        }

        UpdateBarnCounts();

        yield return new WaitForSeconds(1f);
    }

    IEnumerator StoreSelectedFromInventoryToBarn()
    {
        if (!_equipedItemSlotDataCS.CheckIsEmpty() && _equipedItemSlotDataCS.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
        {
            bool sameItemFound = false;

            foreach (var item in _barnItemsAndCount)
            {
                if (_equipedItemSlotDataCS.GetGameItemsData() == item.Key)
                {
                    _barnItemsAndCount[item.Key] = item.Value + _equipedItemSlotDataCS.GetQuantity();
                    //Debug.Log("Same:: " + _equipedItemSlotDataCS.GetGameItemsData());
                    sameItemFound = true;
                    break;
                }
                else
                {
                    sameItemFound = false;
                }
            }

            if (!sameItemFound)
            {
                _barnItemsAndCount.TryAdd(_equipedItemSlotDataCS.GetGameItemsData(), _equipedItemSlotDataCS.GetQuantity());
                //Debug.Log("NotSame:: " + _equipedItemSlotDataCS.GetGameItemsData());
            }
        }
        else
        {
            //Debug.Log("There is nothing equiped Or equiped item is not a crop!");
            TipManager.Instance.ConstructATip("There is nothing equiped Or equiped item is not a crop!", true);
        }

        //_equipedItemSlotDataCS.EmptyIt(true);

        InventoryManager.Instance.RemoveObjectFromEquipedSlot(InventorySlot.InventorySlotType.CS);

        UpdateBarnCounts();

        yield return new WaitForSeconds(1f);
    }

    private void ViewInsideBarn()
    {
        _isBarnOpen = true;
        _viewInsideBarn.SetActive(true);
        _backButton.gameObject.SetActive(true);
        RenderBarnView();
    }

    private void RenderBarnView()
    {
        if (_barnSlots[0] != null)
        {
            for (int i = 0; i < _barnSlots.Length; i++)
            {
                Image img = _barnSlots[i].transform.Find("BarnItemDisplay").transform.GetChild(0).GetComponent<Image>();
                TextMeshProUGUI txt = _barnSlots[i].transform.Find("QuantityDisplay").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                if (img.sprite == null)
                {
                    img.sprite = _defaultSpriteToDisplay;
                    txt.text = "0";
                }
            }

            foreach (var item in _barnItemsAndCount)
            {
                for (int i = 0; i < _barnSlots.Length; i++)
                {
                    Image img = _barnSlots[i].transform.Find("BarnItemDisplay").transform.GetChild(0).GetComponent<Image>();
                    TextMeshProUGUI txt = _barnSlots[i].transform.Find("QuantityDisplay").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                    if (/*img.sprite == null || */img.sprite == _defaultSpriteToDisplay)
                    {
                        img.sprite = item.Key._thumbnail;
                        txt.text = item.Value.ToString();
                        break;
                    }
                    else if (/*img.sprite != null && */img.sprite != _defaultSpriteToDisplay)
                    {
                        if (img.sprite == item.Key._thumbnail)
                        {
                            txt.text = item.Value.ToString();
                            break;
                        }
                        continue;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("The BarnSlots are null!");
        }
    }

    private void UpdateBarnCounts()
    {
        if (_barnItemsAndCount.Count > 0)
        {
            int count = 0;
            int foodCount = 0;

            foreach (var item in _barnItemsAndCount)
            {
                count += item.Value;

                if (item.Key._isEdible == GameItemsData.IsEdible.Edible)
                {
                    foodCount += item.Value;
                }
            }

            if (_storageItemsCount != count)
            {
                _storageItemsCount = count;

                int diff = _storageItemsCount - _previousStorageItemsCount;

                CreditManager.Instance.AddAmountToCredits(diff);

                _previousStorageItemsCount = _storageItemsCount;
            }

            if (_currentFoodCount != foodCount)
            {
                _storageFoodItemsCount = foodCount;
            }
        }
    }

    //This function is called when back button is clicked. It is referenced in the inspector.
    public void OnBackButtonClick()
    {
        _backButton.gameObject.SetActive(false);
        _viewInsideBarn.SetActive(false);
        _isBarnOpen = false;
    }

    public void InteractInfoUpdate(RaycastHit outHit)
    {

        //SIMILAR TO GREENHOUSE, BARN TOO DOESN'T HAVE A COLLIDER. ONE OF ITS CHILDREN HAS A COLLIDER.
        //THUS WE hAVE TO ROUTE TO ITS PARENT UPON RAYCAST HIT INTERACTION.
        //UNLIKE GREENSHOUSE, THERE IS ONE MORE CONDITION FOR BARN. THERE ARE SOME SUB CHILDREN UNDER DIRECT CHILDREN.
        //THERE FOR outHit.collider.transform.parent WON'T WORK. BECAUSE IF SUB CHILDREN GET HIT BY RAYCAST, IT SHOULD BE PARENT OF PARENT.
        //outHit.collider.GetComponentInParent WOULD DO A RECURRSIVE SEARCH TO GET BarnManager COMPONENT.

        if (outHit.collider.tag == "Barn" && outHit.collider.GetComponentInParent<BarnManager>().transform == this.transform) //Routing to parent transform Or parent transform of parent...
        {
            //Debug.Log("outHit entered in BarnManager!");
            InteractOptionsManager.Instance.SetInteractOptionsList(this._listOfCommandsForBarn);
            //Debug.Log("outHit Data loaded from BarnManager!");
        }
        else if (outHit.collider.transform == null)
        {
            //Debug.Log("outHit entered in BarnManager!");
            //Debug.LogError("_outHit is null in BarnManager");
        }
    }

    //public void OnBeforeSerialize()
    //{
    //    _keys.Clear();
    //    _values.Clear();

    //    foreach (var kvp in _barnItemsAndCount)
    //    {
    //        _keys.Add(kvp.Key);
    //        _values.Add(kvp.Value);
    //    }
    //}

    //public void OnAfterDeserialize()
    //{
    //    _barnItemsAndCount = new ConcurrentDictionary<GameItemsData, int>();

    //    for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
    //    {
    //        _barnItemsAndCount.TryAdd(_keys[i], _values[i]);
    //    }
    //}

    public void InventoryInfoUpdate(GameItemsSlotData equipedItemSlotDataCS, GameItemsSlotData equipedItemSlotDataTE, GameItemsSlotData[] cropsSeedsSlotsData, GameItemsSlotData[] toolsEquipmentsSlotsData)
    {
        if (equipedItemSlotDataCS.GetGameItemsData() != null)
        {
            this._equipedItemSlotDataCS = new GameItemsSlotData(equipedItemSlotDataCS);
        }
        else
        {
            this._equipedItemSlotDataCS.EmptyIt(true);
        }
    }
}