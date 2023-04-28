using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourceManager : MonoBehaviour, iInteractInfo, InventoryInfo
{
    public enum PlayerInteractionCommandsForWaterSource
    {
        RefillWateringCan
    }

    [SerializeField] private bool _isFull;
    [SerializeField] private bool _isNotFull;
    [SerializeField] private bool _isEmpty;

    [SerializeField] private int _minimumCapacity;
    [SerializeField] private int _maximumCapacity;
    [SerializeField] private int _presentQuantity;

    [SerializeField] private List<string> _listOfWaterSourceCommandsForPlayer;

    [SerializeField] private GameItemsSlotData _playerEquipedTool;

    [SerializeField] private int _wateringCanQuantity;

    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static WaterSourceManager Instance { get; private set; }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        _listOfWaterSourceCommandsForPlayer = new List<string>();
        _listOfWaterSourceCommandsForPlayer = System.Enum.GetNames(typeof(PlayerInteractionCommandsForWaterSource)).ToList();

        _playerEquipedTool = new GameItemsSlotData();

        _minimumCapacity = 0;
        _maximumCapacity = 50;
        _presentQuantity = 0;

        _isFull = false;
        _isNotFull = false;
        _isEmpty = true;

        InteractOptionsManager.Instance.AddAsInteractInfoListener(this);
        InventoryDisplayManager.Instance.AddAsInventoryInfoListener(this);

        StartCoroutine(ProduceSourceWater());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {

    }

    public void PlayerInteraction(string action = "")
    {
        PlayerInteract(action);
    }

    private void PlayerInteract(string action)
    {
        //Debug.Log("Button Click in " + this + " " + action);

        if (action == PlayerInteractionCommandsForWaterSource.RefillWateringCan.ToString())
        {
            StartCoroutine(RefillWateringCan());
        }
    }

    IEnumerator RefillWateringCan()
    {
        if (_playerEquipedTool.GetGameItemsData() != null)
        {
            if(_playerEquipedTool.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Tool)
            {
                GameItemsSlotData waterCanTool = new GameItemsSlotData(_playerEquipedTool.GetGameItemsData(), _playerEquipedTool.GetQuantity());

                if (waterCanTool.GetGameItemsData()._itemName == "Watering Can")
                {
                    if (!_isEmpty)
                    {
                        int quantityToRefill = waterCanTool.CheckQuantityThatCanBeStacked();

                        if (_presentQuantity >= quantityToRefill)
                        {
                            InventoryManager.Instance.AddQuantityToEquiped(InventorySlot.InventorySlotType.TE, quantityToRefill);
                            TipManager.Instance.ConstructATip("Watering can refilled!", true);
                            _presentQuantity -= quantityToRefill;
                        }
                        else if (_presentQuantity < quantityToRefill)
                        {
                            InventoryManager.Instance.AddQuantityToEquiped(InventorySlot.InventorySlotType.TE, _presentQuantity);
                            TipManager.Instance.ConstructATip("Watering can refilled!", true);
                            _presentQuantity -= _presentQuantity;
                        }

                        yield return new WaitForSeconds(0.5f);

                        ValidateCapacityOfWaterSource();
                    }
                    else
                    {
                        //Debug.Log("Water source is empty at the moment! Please come back later!");
                        TipManager.Instance.ConstructATip("Water source is empty at the moment! Please come back later!", true);
                    }
                }
                else
                {
                    //Debug.Log("You haven't equiped a watering can! Equip a watering can to refil it!");
                    TipManager.Instance.ConstructATip("You haven't equiped a watering can! Equip a watering can to refil it!", true);
                }
            }
            else
            {
                //Debug.Log("The equiped item is not a tool!");
                TipManager.Instance.ConstructATip("The equiped item is not a tool!", true);
            }
        }
        else
        {
            //Debug.Log("You haven't equiped any tool!");
            TipManager.Instance.ConstructATip("You haven't equiped any tool!", true);
        }
    }

    IEnumerator ProduceSourceWater()
    {
        while (true)
        {
            _presentQuantity++;

            ValidateCapacityOfWaterSource();

            yield return new WaitUntil(() => _isNotFull);
            yield return new WaitForSeconds(2f);
        }
    }

    void ValidateCapacityOfWaterSource()
    {
        if (_presentQuantity <= _minimumCapacity)
        {
            _presentQuantity = _minimumCapacity;

            _isFull = false;
            _isNotFull = true;
            _isEmpty = true;
        }
        else if (_presentQuantity >= _maximumCapacity)
        {
            _presentQuantity = _maximumCapacity;

            _isFull = true;
            _isNotFull = false;
            _isEmpty = false;
        }
        else if (_presentQuantity > _minimumCapacity && _presentQuantity < _maximumCapacity)
        {
            _isFull = false;
            _isNotFull = true;
            _isEmpty = false;
        }
    }

    public void InteractInfoUpdate(RaycastHit outHit)
    {
        if(outHit.collider.tag == "WaterSource" && outHit.collider.transform == this.transform)
        {
            InteractOptionsManager.Instance.SetInteractOptionsList(this._listOfWaterSourceCommandsForPlayer);
        }
        else if (outHit.collider.transform == null)
        {
            //Debug.Log("outHit entered in WaterSourceManager!");
            //Debug.LogError("_outHit is null in WaterSourceManager!");
        }
    }

    public void InventoryInfoUpdate(GameItemsSlotData equipedItemSlotDataCS, GameItemsSlotData equipedItemSlotDataTE, GameItemsSlotData[] cropsSeedsSlotsData, GameItemsSlotData[] toolsEquipmentsSlotsData)
    {
        if (equipedItemSlotDataTE.GetGameItemsData() != null)
        {
            this._playerEquipedTool = new GameItemsSlotData(equipedItemSlotDataTE);
        }
        else
        {
            this._playerEquipedTool.EmptyIt(true);
        }
    }
}
