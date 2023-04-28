using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public bool GetShopOpenStatus() { return _isShopOpen; }

    //Bool for Is Shop Open or Close...
    [SerializeField] private bool _isShopOpen;

    //Array to hold the Shop Item Slots...
    [SerializeField] private Image[] _shopItemSlots;

    //Shop Item Image And Price...
    [SerializeField] private List<GameItemsData> _key;
    [SerializeField] private List<float> _value;
    private Dictionary<GameItemsData, float> _shopItemGameItemsDataAndPrice;

    //List of Items that needs to be in shop...
    [SerializeField] private string[] _assetsPaths;
    [SerializeField] private List<GameItemsData> _toBeInTheShopItems;

    //Getting the GameObject of Shop UI Panel...
    private GameObject _shopPanel;
    private GameObject _shopItemBoxContainer;

    //Greenhouse game object...
    [SerializeField] private GameObject _greenhouse;
    private GreenHouseBuildingManager _greenhouseBuildingManager;

    //default sprite

    [SerializeField] private Sprite _emptySlotSprite;

    //Quantity of items per purchase...
    [SerializeField] private int _shopItemQuantityPerPurchase;

    //Shop Item Description Text handle...
    private TextMeshProUGUI _shopItemDescriptionText;

    public static ShopManager Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        _shopPanel = GameObject.Find("ShopPanel");
        _shopItemBoxContainer = GameObject.Find("ShopItemBoxContainer");
        _shopItemDescriptionText = GameObject.Find("ShopItemDescriptionText").GetComponent<TextMeshProUGUI>();

        if (_shopPanel == null)
        {
            Debug.LogError("_shopPanel is null in Shop Manager!");
        }

        if(_shopItemBoxContainer == null)
        {
            Debug.LogError("_shopItemBoxContainer is null in Shop Manager!");
        }

        if (_shopItemDescriptionText == null)
        {
            Debug.LogError("_shopItemDescriptionText is null in Shop Manager!");
        }

        _shopItemSlots = new Image[_shopItemBoxContainer.transform.childCount];

        for(int i = 0; i < _shopItemSlots.Length; i++)
        {
            _shopItemSlots[i] = _shopItemBoxContainer.transform.GetChild(i).GetComponent<Image>();
        }

        if (_shopItemSlots.Length <= 0)
        {
            Debug.LogError("_shopItemSlots is null in Shop Manager!");
        }

        ////https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        ////https://stackoverflow.com/questions/53968958/how-can-i-get-all-prefabs-from-a-assets-folder-getting-not-valid-cast-exception
        ////This doesn't return a string path, rather returns GUIDs for the paths. We got to chnage the GUIDs to actual string paths.
        //_assetsPaths = AssetDatabase.FindAssets("t:GameItemsData", new string[] 
        //{"Assets/Scriptable Objects/GameItems/Crops_Seeds/Crops/", 
        //    "Assets/Scriptable Objects/GameItems/Crops_Seeds/Seeds/", 
        //    "Assets/Scriptable Objects/GameItems/Tools_Equipments/Equipments/", 
        //    "Assets/Scriptable Objects/GameItems/Tools_Equipments/Tools/" });

        //if(_assetsPaths.Length <= 0)
        //{
        //    Debug.LogError("_assetsPaths is null in Shop Manager!");
        //}

        //_toBeInTheShopItems = new List<GameItemsData>();

        //foreach (var item in _assetsPaths)
        //{
        //    var path = AssetDatabase.GUIDToAssetPath(item); //Here we chnage the GUIDs to actual string paths one by one.

        //    GameItemsData g = AssetDatabase.LoadAssetAtPath(path, typeof(GameItemsData)) as GameItemsData; //Here we use the converted actual string path.

        //    _toBeInTheShopItems.Add(g);
        //}

        if(_toBeInTheShopItems.Count <= 0)
        {
            Debug.LogError("_toBeInTheShopItems is null in Shop Manager!");
        }

        //_greenhouse = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/3D/Buildings/greenhouse.prefab", typeof(GameObject));

        if(_greenhouse == null)
        {
            Debug.LogError("_greenhouse is null in Shop Manager!");
        }

        _greenhouseBuildingManager = GameObject.Find("GreenHouseManager").GetComponent<GreenHouseBuildingManager>();

        if (_greenhouseBuildingManager == null)
        {
            Debug.LogError("_greenhouseBuildingManager is null in Shop Manager!");
        }

        _key = new List<GameItemsData>();
        _value = new List<float>();
        _shopItemGameItemsDataAndPrice = new Dictionary<GameItemsData, float>();

        _shopItemQuantityPerPurchase = 1;

        PopulateTheShop();

        _shopPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        #region #Only for Windows and WebGl Builds...
        //#Only for Windows and WebGl Builds...
        //Shop can be opened only with keyboard bindings and can be closed only with mouse cursor...
        if (!_shopPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _shopPanel.SetActive(true);
                _isShopOpen = true;
            }
        }
        //#Only for Windows and WebGl Builds...
        #endregion
    }

    private void PopulateTheShop()
    {
        //Gonna populate the shop and Dictionary with items, images and prices.
        //For that I need to set up the all items now as scriptable objects.

        //Sprite _emptySlotSprite = AssetDatabase.LoadAssetAtPath("Assets/Textures/Sprites/In-Game/EmptySlot.png", typeof(Sprite)) as Sprite;

        foreach (var item in _toBeInTheShopItems)
        {
            _shopItemGameItemsDataAndPrice.Add(item, item._buyingCost);
        }

        //Just to keep the empty slots with "Empty Slot" Image.
        foreach(var item in _shopItemSlots)
        {
            Image img = item.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            TextMeshProUGUI text = item.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (img.sprite == null)
            {
                img.sprite = _emptySlotSprite;
                text.text = "Cred: 0";
            }
        }

        foreach(var item in _shopItemGameItemsDataAndPrice)
        {
            foreach (var item1 in _shopItemSlots)
            {
                Image img = item1.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                TextMeshProUGUI text = item1.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                ShopItemSlot shopItemSlot = item1.transform.GetComponent<ShopItemSlot>();

                if (img.sprite != null && img.sprite != item.Key._thumbnail && img.sprite == _emptySlotSprite)
                {
                    img.sprite = item.Key._thumbnail;
                    text.text = "Cred: " + item.Value;

                    //Setting each ShopItemSlot script with its GameItemsData..
                    shopItemSlot.SetThisSlotWithShopItem(item.Key);

                    break;
                }
                else
                {
                    //text.text = "Cred: " + item.Value;
                    continue;
                }
            }
        }
    }

    public void OnBuyButtonClick(Button button)
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

        if (button != null)
        {
            Image img = button.transform.parent.GetChild(0).GetChild(0).GetComponent<Image>();

            if(img != null && img.sprite != null)
            {
                foreach(var item in _shopItemGameItemsDataAndPrice)
                {
                    if(item.Key._thumbnail == img.sprite)
                    {
                        //Check if enough credits are available.
                        //Get the cost of credits from the GameItemsData from the dictionary/directly from the dictionary.
                        //Check if enough space is available in the inventory.
                        //Remove the amount of credits it costs to buy the item.
                        //Add the item to the inventory.
                        //if added, break the loop.

                        float currentCredits = CreditManager.Instance.GetCurrentCredits();

                        if(item.Value <= currentCredits)
                        {
                            if(item.Key._itemName == "Greenhouse") //Special case for greenhouse.
                            {
                                //if(TimeManager.Instance.GetCurrentSeason() == "Wet") //Removed season condition for now, as it is gonna be small game.
                                if(_greenhouseBuildingManager.CheckNoOfGreenhouse_ToReturnIfOneCanBeBought())
                                {
                                    Transform temp = _greenhouseBuildingManager.GetNextTranform();

                                    //Will instantiate under greenhouse container object...
                                    GameObject gHTemp = new GameObject();
                                    gHTemp = _greenhouse;
                                    Instantiate(gHTemp, temp.position, temp.rotation, _greenhouseBuildingManager.transform.GetChild(1));
                                    CreditManager.Instance.RemoveAmountFromCredits(item.Value);
                                    TipManager.Instance.ConstructATip("Item bought!", true);
                                }
                                else
                                {
                                    //Debug.Log("Item cannot be bought as it is not a Wet season yet!");
                                    //Debug.Log("Item cannot be bought as you already own max number of greenhouses!");
                                    TipManager.Instance.ConstructATip("Item cannot be bought as you already own max number of greenhouses!", true);
                                }
                            }
                            else
                            {
                                if (InventoryManager.Instance.AddObjectToInventorySlot(item.Key, _shopItemQuantityPerPurchase) == 0)
                                {
                                    CreditManager.Instance.RemoveAmountFromCredits(item.Value);
                                    //Debug.Log("Item bought!");
                                    TipManager.Instance.ConstructATip("Item bought!", true);
                                }
                                else
                                {
                                    //Debug.Log("Item cannot be bought as there is no space in inventory! (Or) The maximum quantity that can be owned by player is reached!");
                                    TipManager.Instance.ConstructATip("Item cannot be bought as there is no space in inventory! (Or) The maximum quantity that can be owned by player is reached!", true);
                                }
                            }
                        }
                        else
                        {
                            //Debug.Log("Item cannot be bought as there is not enough credits!");
                            TipManager.Instance.ConstructATip("Item cannot be bought as there is not enough credits!", true);
                        }
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                //Debug.LogError("Image / Image component is missing!");
            }
        }
        else
        {
            //Debug.LogError("Button component is missing!");
        }
    }

    //This function is used by the shop open/close button...
    public void OnShopOpenButtonClick()
    {
        #region #Only for Windows and WebGl Builds...
        //#Only for Windows and WebGl Builds...
        //Shop can be closed only with mouse cursor and can be opened only with keyboard bindings...
        if (_shopPanel.activeSelf)
        {
            _shopPanel.SetActive(false);
            _isShopOpen = false;
        }
        else if (!_shopPanel.activeSelf)
        {
            //_shopPanel.SetActive(true);
            //_isShopOpen = true;
        }
        //#Only for Windows and WebGl Builds...
        #endregion
    }

    public void RenderItemInfoInShopItemDescriptionTextBox(GameItemsData gameItemsData)
    {
        if (gameItemsData != null)
        {
            _shopItemDescriptionText.text = gameItemsData._itemName;
        }
        else
        {
            _shopItemDescriptionText.text = "SHOP";
        }
    }

    public void OnBeforeSerialize()
    {
        _key.Clear();
        _value.Clear();

        foreach (var item in _shopItemGameItemsDataAndPrice)
        {
            _key.Add(item.Key);
            _value.Add(item.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _shopItemGameItemsDataAndPrice = new Dictionary<GameItemsData, float>();

        for (int i = 0; i < Mathf.Min(_key.Count, _value.Count); i++)
        {
            _shopItemGameItemsDataAndPrice.Add(_key[i], _value[i]);
        }
    }
}