using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GrainsAndSeedsStorageManager : MonoBehaviour, iInteractInfo, ISerializationCallbackReceiver
{
    public bool GetGrainsAndSeedsStorageOpenStatus() { return _isGrainsAndSeedsStorageOpen; }

    public struct Seed
    {
        public string seedName;
        public int seedQuantity;
        public SeedItem seedItem;
    }
    Seed cabbage;
    Seed carrot;
    Seed corn;
    Seed potato;
    Seed wheat;

    //Target variables...
    private int _targetValue;
    private int _currentValue;

    [SerializeField] private int _totalCount;
    [SerializeField] private int _cabbageCount;
    [SerializeField] private int _carrotCount;
    [SerializeField] private int _cornCount;
    [SerializeField] private int _potatoCount;
    [SerializeField] private int _wheatCount;

    //Grains And Seeds Storage Commands...
    public enum PlayerInteractionOptionsForGrainsAndSeedsStorage
    {
        GetCabbageSeeds, GetCarrotSeeds, GetCornSeeds, GetPotatoSeeds, GetWheatSeeds, ViewInsideStorage
    }
    //Grains And Seeds Storage Commands List...
    [SerializeField] private List<string> _listOfCommandsForGrainsAndSeedsStorage;

    //To hold Grains and seeds storage information...
    [SerializeField] private List<GameItemsData> _keys;
    [SerializeField] private List<int> _values;
    private Dictionary<GameItemsData, int> _grainsAndSeedsStorageItems;

    //To hold Barn information...
    private Dictionary<GameItemsData, int> _barnItemsAndCount;

    //Grains And Seeds Storage Slots...
    [SerializeField] private Image[] _grainsAndSeedsStorageSlots = new Image[10];
    private GameObject _viewInsideGrainsAndSeedsStorage;

    //Seed items...
    [SerializeField] SeedItem _cabbageSeedItem;
    [SerializeField] SeedItem _carrotSeedItem;
    [SerializeField] SeedItem _cornSeedItem;
    [SerializeField] SeedItem _potatoSeedItem;
    [SerializeField] SeedItem _wheatSeedItem;

    //Is Grains and seeds storage open varibale...
    private bool _isGrainsAndSeedsStorageOpen = false;

    //Default sprite for empty slots...
    [SerializeField] private Sprite _defaultSpriteToDisplay;

    //Back button...
    private Button _backButton;

    //Bool to indicate is Grains And Seeds Storage is being viewed...
    private bool _isBeingViewed = false;

    #region Awake - Singleton Monobehaviour
    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static GrainsAndSeedsStorageManager Instance { get; private set; }

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

        //Giving Cabbage, carrot and corn alone some intial seeds quantity for the player to get started.
        cabbage = new Seed();
        cabbage.seedName = "CabbageSeed";
        cabbage.seedQuantity = 30;
        //cabbage.seedItem = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/Cabbage Seed.asset", typeof(SeedItem)) as SeedItem;
        cabbage.seedItem = _cabbageSeedItem;

        carrot = new Seed();
        carrot.seedName = "CarrotSeed";
        carrot.seedQuantity = 30;
        //carrot.seedItem = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/Sugarcane Seed.asset", typeof(SeedItem)) as SeedItem;
        carrot.seedItem = _carrotSeedItem;

        corn = new Seed();
        corn.seedName = "CornSeed";
        corn.seedQuantity = 30;
        //corn.seedItem = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/Corn Seed.asset", typeof(SeedItem)) as SeedItem;
        corn.seedItem = _cornSeedItem;

        potato = new Seed();
        potato.seedName = "PotatoSeed";
        potato.seedQuantity = 0;
        //potato.seedItem = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/Potato Seed.asset", typeof(SeedItem)) as SeedItem;
        potato.seedItem = _potatoSeedItem;

        wheat = new Seed();
        wheat.seedName = "WheatSeed";
        wheat.seedQuantity = 0;
        //wheat.seedItem = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/Wheat Seed.asset", typeof(SeedItem)) as SeedItem;
        wheat.seedItem = _wheatSeedItem;
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        _listOfCommandsForGrainsAndSeedsStorage = System.Enum.GetNames(typeof(PlayerInteractionOptionsForGrainsAndSeedsStorage)).ToList();

        //_defaultSpriteToDisplay = AssetDatabase.LoadAssetAtPath("Assets/Textures/Sprites/EmptySlot.png", typeof(Sprite)) as Sprite;

        _keys = new List<GameItemsData>();
        _values = new List<int>();
        _grainsAndSeedsStorageItems = new Dictionary<GameItemsData, int>();

        _barnItemsAndCount = new Dictionary<GameItemsData, int>();

        _viewInsideGrainsAndSeedsStorage = GameObject.Find("ViewInsideGrainsAndSeedsStorage").gameObject;
        _backButton = GameObject.Find("GrainsAndSeedsStorageBackButton").GetComponent<Button>();

        if (_viewInsideGrainsAndSeedsStorage == null || _backButton == null)
        {
            Debug.LogError("_viewInsideBarn is null or _backButton is null in BarnManager!");
        }

        _viewInsideGrainsAndSeedsStorage.SetActive(false);
        _backButton.gameObject.SetActive(false);

        InteractOptionsManager.Instance.AddAsInteractInfoListener(this);

        StartCoroutine(StorageStockUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        _cabbageCount = cabbage.seedQuantity;
        _carrotCount = carrot.seedQuantity;
        _cornCount = corn.seedQuantity;
        _potatoCount = potato.seedQuantity;
        _wheatCount = wheat.seedQuantity;

        //This is to constantly update the quantity of seed items in the storage...
        if (_cabbageCount >= 0) //CABBAGE
        {
            if (_grainsAndSeedsStorageItems.Count > 0)
            {
                bool sameCropSeed = false;

                foreach (var item in _grainsAndSeedsStorageItems)
                {
                    if (cabbage.seedItem == item.Key)
                    {
                        _grainsAndSeedsStorageItems[item.Key] = cabbage.seedQuantity;
                        sameCropSeed = true;
                        break;
                    }
                    continue;
                }

                if (!sameCropSeed)
                {
                    _grainsAndSeedsStorageItems.Add(cabbage.seedItem, cabbage.seedQuantity);
                }
            }
            else
            {
                _grainsAndSeedsStorageItems.Add(cabbage.seedItem, cabbage.seedQuantity);
            }
        }

        //This is to constantly update or add a new quantity of seed items in the storage...
        if (_carrotCount >= 0) //CARROT
        {
            if (_grainsAndSeedsStorageItems.Count > 0)
            {
                bool sameCropSeed = false;

                foreach (var item in _grainsAndSeedsStorageItems)
                {
                    if (carrot.seedItem == item.Key)
                    {
                        _grainsAndSeedsStorageItems[item.Key] = carrot.seedQuantity;
                        sameCropSeed = true;
                        break;
                    }
                    continue;
                }

                if (!sameCropSeed)
                {
                    _grainsAndSeedsStorageItems.Add(carrot.seedItem, carrot.seedQuantity);
                }
            }
            else
            {
                _grainsAndSeedsStorageItems.Add(carrot.seedItem, carrot.seedQuantity);
            }
        }

        //This is to constantly update or add a new quantity of seed items in the storage...
        if (_cornCount >= 0) //CORN
        {
            if (_grainsAndSeedsStorageItems.Count > 0)
            {
                bool sameCropSeed = false;

                foreach (var item in _grainsAndSeedsStorageItems)
                {
                    if (corn.seedItem == item.Key)
                    {
                        _grainsAndSeedsStorageItems[item.Key] = corn.seedQuantity;
                        sameCropSeed = true;
                        break;
                    }
                    continue;
                }

                if (!sameCropSeed)
                {
                    _grainsAndSeedsStorageItems.Add(corn.seedItem, corn.seedQuantity);
                }
            }
            else
            {
                _grainsAndSeedsStorageItems.Add(corn.seedItem, corn.seedQuantity);
            }
        }

        //This is to constantly update or add a new quantity of seed items in the storage...
        if (_potatoCount >= 0) //POTATO
        {
            if (_grainsAndSeedsStorageItems.Count > 0)
            {
                bool sameCropSeed = false;

                foreach (var item in _grainsAndSeedsStorageItems)
                {
                    if (potato.seedItem == item.Key)
                    {
                        _grainsAndSeedsStorageItems[item.Key] = potato.seedQuantity;
                        sameCropSeed = true;
                        break;
                    }
                    continue;
                }

                if (!sameCropSeed)
                {
                    _grainsAndSeedsStorageItems.Add(potato.seedItem, potato.seedQuantity);
                }
            }
            else
            {
                _grainsAndSeedsStorageItems.Add(potato.seedItem, potato.seedQuantity);
            }
        }

        //This is to constantly update or add a new quantity of seed items in the storage...
        if (_wheatCount >= 0) //WHEAT
        {
            if (_grainsAndSeedsStorageItems.Count > 0)
            {
                bool sameCropSeed = false;

                foreach (var item in _grainsAndSeedsStorageItems)
                {
                    if (wheat.seedItem == item.Key)
                    {
                        _grainsAndSeedsStorageItems[item.Key] = wheat.seedQuantity;
                        sameCropSeed = true;
                        break;
                    }
                    continue;
                }

                if (!sameCropSeed)
                {
                    _grainsAndSeedsStorageItems.Add(wheat.seedItem, wheat.seedQuantity);
                }
            }
            else
            {
                _grainsAndSeedsStorageItems.Add(wheat.seedItem, wheat.seedQuantity);
            }
        }

        if (_isBeingViewed)
        {
            RenderGrainsAndSeedsStorageView();
        }
    }

    //void OnGUI()
    //{
    //    foreach (var kvp in _grainsAndSeedsStorageItems)
    //        GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    //}

    //Only when a crop is in barn, its seed will start producing in storage...
    IEnumerator StorageStockUpdate()
    {
        while (true)
        {
            yield return new WaitUntil(CheckIfBarnIsNotEmpty);

            if(_barnItemsAndCount.Count != 0)
            {
                //If the particular crop type is not empty, its seed will be produced in certain interval of time...
                foreach (var item in _barnItemsAndCount)
                {
                    if (item.Key._itemName == "Cabbage" && item.Value > 0)
                    {
                        cabbage.seedQuantity++;
                    }

                    if (item.Key._itemName == "Carrot" && item.Value > 0)
                    {
                        carrot.seedQuantity++;
                    }

                    if (item.Key._itemName == "Corn" && item.Value > 0)
                    {
                        corn.seedQuantity++;
                    }

                    if (item.Key._itemName == "Potato" && item.Value > 0)
                    {
                        potato.seedQuantity++;
                    }

                    if (item.Key._itemName == "Wheat" && item.Value > 0)
                    {
                        wheat.seedQuantity++;
                    }
                }
            }

            yield return new WaitForSeconds(2f); //Seeds generate every two seconds
        }
    }

    bool CheckIfBarnIsNotEmpty()
    {
        _barnItemsAndCount = BarnManager.Instance.GetBarnItemsAndCounts();

        if(_barnItemsAndCount.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlayerInteraction(string action = "")
    {
        PlayerInteract(action);
    }

    private void PlayerInteract(string action)
    {
        //Debug.Log("Button Click in " + this + " " + action);

        if (action == PlayerInteractionOptionsForGrainsAndSeedsStorage.GetCabbageSeeds.ToString())
        {
            StartCoroutine(ExtractCabbageSeedsFromStorage());
        }

        else if (action == PlayerInteractionOptionsForGrainsAndSeedsStorage.GetCarrotSeeds.ToString())
        {
            StartCoroutine(ExtractCarrotSeedsFromStorage());
        }

        else if (action == PlayerInteractionOptionsForGrainsAndSeedsStorage.GetCornSeeds.ToString())
        {
            StartCoroutine(ExtractCornSeedsFromStorage());
        }

        else if (action == PlayerInteractionOptionsForGrainsAndSeedsStorage.GetPotatoSeeds.ToString())
        {
            StartCoroutine(ExtractPotatoSeedsFromStorage());
        }

        else if (action == PlayerInteractionOptionsForGrainsAndSeedsStorage.GetWheatSeeds.ToString())
        {
            StartCoroutine(ExtractWheatSeedsFromStorage());
        }

        else if(action == PlayerInteractionOptionsForGrainsAndSeedsStorage.ViewInsideStorage.ToString())
        {
            ViewInsideGrainsAndSeedsStorage();
        }
    }

    IEnumerator ExtractCabbageSeedsFromStorage()
    {
        if(cabbage.seedQuantity > 10)
        {
            int remainingQuantity = InventoryManager.Instance.AddObjectToInventorySlot(cabbage.seedItem, 10);

            cabbage.seedQuantity -= (10 - remainingQuantity);
        }
        else
        {
            //Debug.Log("Not enough Cabbage seeds in the storage. Please wait!");
            TipManager.Instance.ConstructATip("Not enough Cabbage seeds in the storage. Please wait!", true);
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ExtractCarrotSeedsFromStorage()
    {
        if (carrot.seedQuantity > 10)
        {
            int remainingQuantity = InventoryManager.Instance.AddObjectToInventorySlot(carrot.seedItem, 10);

            carrot.seedQuantity -= (10 - remainingQuantity);
        }
        else
        {
            //Debug.Log("Not enough Carrot seeds in the storage. Please wait!");
            TipManager.Instance.ConstructATip("Not enough Carrot seeds in the storage. Please wait!", true);
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ExtractCornSeedsFromStorage()
    {
        if (corn.seedQuantity > 10)
        {
            int remainingQuantity = InventoryManager.Instance.AddObjectToInventorySlot(corn.seedItem, 10);

            corn.seedQuantity -= (10 - remainingQuantity);
        }
        else
        {
            //Debug.Log("Not enough Corn seeds in the storage. Please wait!");
            TipManager.Instance.ConstructATip("Not enough Corn seeds in the storage. Please wait!", true);
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ExtractPotatoSeedsFromStorage()
    {
        if (potato.seedQuantity > 10)
        {
            int remainingQuantity = InventoryManager.Instance.AddObjectToInventorySlot(potato.seedItem, 10);

            potato.seedQuantity -= (10 - remainingQuantity);
        }
        else
        {
            //Debug.Log("Not enough Potato seeds in the storage. Please wait!");
            TipManager.Instance.ConstructATip("Not enough Potato seeds in the storage. Please wait!", true);
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ExtractWheatSeedsFromStorage()
    {
        if (wheat.seedQuantity > 10)
        {
            int remainingQuantity = InventoryManager.Instance.AddObjectToInventorySlot(wheat.seedItem, 10);

            wheat.seedQuantity -= (10 - remainingQuantity);
        }
        else
        {
            //Debug.Log("Not enough Wheat seeds in the storage. Please wait!");
            TipManager.Instance.ConstructATip("Not enough Wheat seeds in the storage. Please wait!", true);
        }

        yield return new WaitForSeconds(1f);
    }

    private void ViewInsideGrainsAndSeedsStorage()
    {
        _isGrainsAndSeedsStorageOpen = true;

        _viewInsideGrainsAndSeedsStorage.SetActive(true);
        _backButton.gameObject.SetActive(true);
        _isBeingViewed = true;

        RenderGrainsAndSeedsStorageView();
    }

    private void RenderGrainsAndSeedsStorageView()
    {
        if (_grainsAndSeedsStorageSlots[0] != null)
        {
            for (int i = 0; i < _grainsAndSeedsStorageSlots.Length; i++)
            {
                Image img = _grainsAndSeedsStorageSlots[i].transform.Find("GrainsAndSeedsStorageItemDisplay").transform.GetChild(0).GetComponent<Image>();
                TextMeshProUGUI txt = _grainsAndSeedsStorageSlots[i].transform.Find("QuantityDisplay").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                if (img.sprite == null)
                {
                    img.sprite = _defaultSpriteToDisplay;
                    txt.text = "0";
                }
            }

            foreach (var item in _grainsAndSeedsStorageItems)
            {
                for (int i = 0; i < _grainsAndSeedsStorageSlots.Length; i++)
                {
                    Image img = _grainsAndSeedsStorageSlots[i].transform.Find("GrainsAndSeedsStorageItemDisplay").transform.GetChild(0).GetComponent<Image>();
                    TextMeshProUGUI txt = _grainsAndSeedsStorageSlots[i].transform.Find("QuantityDisplay").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

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
            Debug.LogError("The GrainsAndSeedsStorageSlots are null!");
        }
    }

    //This function is called when back button is clicked. It is referenced in the inspector.
    public void OnBackButtonClick()
    {
        _backButton.gameObject.SetActive(false);
        _viewInsideGrainsAndSeedsStorage.SetActive(false);
        _isBeingViewed = false;

        _isGrainsAndSeedsStorageOpen = false;
    }

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in _grainsAndSeedsStorageItems)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _grainsAndSeedsStorageItems = new Dictionary<GameItemsData, int>();

        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
        {
            _grainsAndSeedsStorageItems.Add(_keys[i], _values[i]);
        }
    }

    public void InteractInfoUpdate(RaycastHit outHit)
    {
        if (outHit.collider.tag == "GrainsAndSeedsStorage" && outHit.collider.transform == this.transform)
        {
            //Debug.Log("outHit entered in GrainsAndSeedsStorageManager!");
            InteractOptionsManager.Instance.SetInteractOptionsList(this._listOfCommandsForGrainsAndSeedsStorage);
           // Debug.Log("outHit Data loaded from GrainsAndSeedsStorageManager!");
        }
        else if (outHit.collider.transform == null)
        {
            //Debug.Log("outHit entered in GrainsAndSeedsStorageManager!");
            //Debug.LogError("_outHit is null in GrainsAndSeedsStorageManager");
        }
    }
}