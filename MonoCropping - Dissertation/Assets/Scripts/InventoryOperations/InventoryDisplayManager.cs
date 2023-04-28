using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //To access the UI elements like Image, etc,.
using UnityEditor; //To load default sprite from the directory through code.

public class InventoryDisplayManager : MonoBehaviour
{
    #region Class Member Varibales
    //Equiped data...
    [Header("Equiped Slots")]
    [SerializeField]
    private EquipedSlot _equipedCSSlot;
    [SerializeField]
    private EquipedSlot _equipedTESlot;

    //Slot display storage for the inventory - Crops & Seeds
    [Header("Crops & Seeds Slots")]
    [SerializeField]
    private InventorySlot[] _inventoryCropsSeedsSlot;

    //Slot display storage for the inventory - Tools & Equipments
    [Header("Tools & Equipments Slots")]
    [SerializeField]
    private InventorySlot[] _inventoryToolsEquipmentsSlot;

    //To get the Object that displays the info of inventory items.
    private Text _inventoryObjectName;
    private Text _inventoryObjectDescription;

    //Default sprite to render
    [SerializeField] private Sprite _defaultSpriteToDisplay;
    private bool isSpriteAssetsLoaded = false;
    public bool getStatusOfSprites() { return isSpriteAssetsLoaded; }
    public void setStatusOfSprites(bool a) { isSpriteAssetsLoaded = a; }
    
    //To build inventory and its related data
    public void RenderInventoryNow() { RenderInventory(); }
    public void RenderItemInfoNow(GameItemsData d) { RenderItemInfo(d); }
    public void AssignInventoryIndexes() { BuildIndex(); }

    //Player handle...
    [SerializeField] PlayerController _playerController;

    //Observer pattern...
    private List<InventoryInfo> _inventoryInfolisternersList;
    public void AddAsInventoryInfoListener(InventoryInfo listener) { RegisterAsInventoryInfoListener(listener); }
    public void RemoveFromInventoryInfoListener(InventoryInfo listener) { UnregisterFromInventoryInfoListeners(listener); }

    //To check if inventory is being rendered...
    [SerializeField] private bool _isInventoryRendering;

    //To hold info of inventory rendering...
    GameItemsSlotData _equipedItemSlotDataCS;
    GameItemsSlotData _equipedItemSlotDataTE;
    GameItemsSlotData[] _inventoryCropsSeedsSlotData;
    GameItemsSlotData[] _inventoryToolsEquipmentsSlotData;

    #endregion

    #region Awake - Singleton Monobehaviour
    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static InventoryDisplayManager Instance { get; private set; }

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

        _inventoryInfolisternersList = new List<InventoryInfo>();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _inventoryObjectName = GameObject.Find("ItemNameTag").GetComponent<Text>();
        _inventoryObjectDescription = GameObject.Find("ItemDescriptionTag").GetComponent<Text>();

        //This is the default sprite that will display in the empty slots
        //_defaultSpriteToDisplay = AssetDatabase.LoadAssetAtPath("Assets/Textures/Sprites/In-Game/EmptySlot.png", typeof(Sprite)) as Sprite;// OR -> (Image)AssetDatabase.LoadAssetAtPath("Assets/Textures/Sprites/EmptySlot.png", typeof(Image));

        if (_defaultSpriteToDisplay == null)
        {
            Debug.LogError("Error - Inventory Display Manager. _defaultSpriteToDisplay missing!");
        }
        else
        {
            isSpriteAssetsLoaded = true;
        }

        _isInventoryRendering = false;

        StartCoroutine(StartSendingEquipedInfoForListeners());
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Inventory Rendering Operations
    private void BuildIndex()
    {
        //using single for loop as both the inventory slot arrays are of same length.
        //Here we build index for mapping purpose...
        for (int i = 0; i < _inventoryCropsSeedsSlot.Length; i++)
        {
            _inventoryCropsSeedsSlot[i].setIndex(i);
            _inventoryToolsEquipmentsSlot[i].setIndex(i);
        }
    }

    //This is to make the equiped info sent to all the listeners frequently rather than just when updating inventory.
    //(Cuz its the way needed for greenhouse)
    private IEnumerator StartSendingEquipedInfoForListeners()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (_equipedItemSlotDataCS == null || _equipedItemSlotDataTE == null || 
                _inventoryCropsSeedsSlotData == null || _inventoryToolsEquipmentsSlotData == null)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                if (!_isInventoryRendering)
                {
                    SendEquipedItemInfoForListeners(_equipedItemSlotDataCS, _equipedItemSlotDataTE, 
                        _inventoryCropsSeedsSlotData, _inventoryToolsEquipmentsSlotData);
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    private void RenderInventory()
    {
        _isInventoryRendering = true;

        //GameItemsData _equipedCSData = InventoryManager.Instance.GetEquipedCSData();
        //GameItemsData _equipedTEData = InventoryManager.Instance.GetEquipedTEData();

        /*GameItemsSlotData */_equipedItemSlotDataCS = InventoryManager.Instance.GetEquippedGameItemsSlot(InventorySlot.InventorySlotType.CS);
        /*GameItemsSlotData */_equipedItemSlotDataTE = InventoryManager.Instance.GetEquippedGameItemsSlot(InventorySlot.InventorySlotType.TE);
        /*GameItemsSlotData[] */_inventoryCropsSeedsSlotData = InventoryManager.Instance.GetInventoryGameItemsSlot(InventorySlot.InventorySlotType.CS);
        /*GameItemsSlotData[] */_inventoryToolsEquipmentsSlotData = InventoryManager.Instance.GetInventoryGameItemsSlot(InventorySlot.InventorySlotType.TE);

        if(_equipedItemSlotDataCS == null || _equipedItemSlotDataTE == null || _inventoryCropsSeedsSlotData == null || _inventoryToolsEquipmentsSlotData == null)
        {
            Debug.LogError("Error found in RenderInventory in Inventory Display Manager!");
        }

        _equipedCSSlot.DisplayItemInSlot(_equipedItemSlotDataCS, _defaultSpriteToDisplay, true);
        _equipedTESlot.DisplayItemInSlot(_equipedItemSlotDataTE, _defaultSpriteToDisplay, true);

        //_inventoryCropsSeedsData = InventoryManager.Instance.GetCropsSeedsData();
        //_inventoryToolsEquipmentsData = InventoryManager.Instance.GetToolsEquipmentsData();

        //using single for loop as both the inventory slot arrays are of same length.
        //This for loop is responsible for mapping of slots array with item data arrays...
        for (int i = 0; i < _inventoryCropsSeedsSlot.Length; i++)
        {
            _inventoryCropsSeedsSlot[i].DisplayItemInSlot(_inventoryCropsSeedsSlotData[i], _defaultSpriteToDisplay, false);
            _inventoryToolsEquipmentsSlot[i].DisplayItemInSlot(_inventoryToolsEquipmentsSlotData[i], _defaultSpriteToDisplay, false);
        }

        SendEquipedItemInfoForListeners(_equipedItemSlotDataCS, _equipedItemSlotDataTE, _inventoryCropsSeedsSlotData, _inventoryToolsEquipmentsSlotData);

        InventoryManager.Instance.CountItemsInInventory();

        //RenderPlayerHandWithEquipedItem();

        _isInventoryRendering = false;
    }

    private void RenderItemInfo(GameItemsData _gameItemData)
    {
        if (_gameItemData != null)
        {
            _inventoryObjectName.fontSize = 15;
            _inventoryObjectName.fontStyle = FontStyle.Bold;
            _inventoryObjectName.color = Color.white;
            _inventoryObjectName.text = _gameItemData._itemName;

            _inventoryObjectDescription.fontSize = 12;
            _inventoryObjectDescription.fontStyle = FontStyle.Normal;
            _inventoryObjectDescription.color = Color.white;
            _inventoryObjectDescription.text = _gameItemData._itemDescription;
        }
        else
        {
            _inventoryObjectName.text = "How To Use Inventory";
            _inventoryObjectDescription.text = "Use Left Mouse Button On Slots to Swap/Stack.\nUse Right Mouse Button On Equiped Slots to Remove Item Permanently.";
        }
    }

    private void SendEquipedItemInfoForListeners(GameItemsSlotData equipedItemSlotDataCS, GameItemsSlotData equipedItemSlotDataTE, 
        GameItemsSlotData[] inventoryCropsSeedsData, GameItemsSlotData[] inventoryToolsEquipmentsData)
    {
        //Sending out equiped item data for listeners...
        foreach (InventoryInfo II in _inventoryInfolisternersList)
        {
            II.InventoryInfoUpdate(equipedItemSlotDataCS, equipedItemSlotDataTE, inventoryCropsSeedsData, inventoryToolsEquipmentsData);
        }
    }

    private void RenderPlayerHandWithEquipedItem()
    {
        _playerController.RenderPlayersHandNow();
    }
    #endregion

    #region Listeners Register/Unregister Functions
    //Handling registering as listener.
    void RegisterAsInventoryInfoListener(InventoryInfo listener)
    {
        _inventoryInfolisternersList.Add(listener);
    }

    //Handling Unregistering from listeners.
    void UnregisterFromInventoryInfoListeners(InventoryInfo listener)
    {
        _inventoryInfolisternersList.Remove(listener);
    }
    #endregion
}