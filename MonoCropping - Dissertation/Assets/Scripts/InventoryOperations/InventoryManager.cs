using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, iInteractInfo
{
    #region Public Getters & Setters

    //Public GETTERs and SETTERs
    public bool GetInventoryStatus() { return _inventoryPanelToggle; }

    //public GameItemsData[] GetCropsSeedsData() { return _cropsSeedsData; }
    //public GameItemsData[] GetToolsEquipmentsData() { return _toolsEquipmentsData; }

    //public GameItemsData GetEquipedCSData() { return _equipedItemCS; }
    //public GameItemsData GetEquipedTEData() { return _equipedItemTE; }

    public GameItemsData GetEquipedGameItems(InventorySlot.InventorySlotType inventorySlotType)
    {
        if(inventorySlotType == InventorySlot.InventorySlotType.CS) { return _equipedItemSlotDataCS.GetGameItemsData(); }
        else { return _equipedItemSlotDataTE.GetGameItemsData(); }
    }

    public GameItemsSlotData GetEquippedGameItemsSlot(InventorySlot.InventorySlotType inventorySlotType)
    {
        if (inventorySlotType == InventorySlot.InventorySlotType.CS) { return _equipedItemSlotDataCS; }
        else { return _equipedItemSlotDataTE; }
    }

    public GameItemsSlotData[] GetInventoryGameItemsSlot(InventorySlot.InventorySlotType inventorySlotType)
    {
        if(inventorySlotType == InventorySlot.InventorySlotType.CS) { return _cropsSeedsSlotsData; }
        else { return _toolsEquipmentsSlotsData; }
    }

    public int GetNumberOfSeedsInInventory() { return _numberOfSeeds; }
    public int GetNumberOfCropsInInventory() { return _numberOfCrops; }
    public int GetNumberOfToolsInInventory() { return _numberOfTools; }
    public int GetNumberOfEquipmentsInInventory() { return _numberOfEquipments; }

    #endregion

    #region Awake - Singleton Monobehaviour
    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static InventoryManager Instance { get; private set; }

    //Making this class to be a singleton monobehaviour class - only one instace will be created for this class.
    private void Awake()
    {
        //if there is more than one instance, destroy the extras.
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static Instance to this Instance.
            Instance = this;
        }
    }
    #endregion

    #region Class Members

    //Storage for equiped item
    [Header("Equiped Items")]
    [SerializeField] private GameItemsSlotData _equipedItemSlotDataCS = new GameItemsSlotData(); //Changed from GameItemsData to GameItemsSlotData
    [SerializeField] private GameItemsSlotData _equipedItemSlotDataTE = new GameItemsSlotData(); //Changed from GameItemsData to GameItemsSlotData

    //Storage for the inventory items - Crops & Seeds
    [Header("Crops & Seeds")]
    [SerializeField] private GameItemsSlotData[] _cropsSeedsSlotsData = new GameItemsSlotData[5]; //Changed from GameItemsData to GameItemsSlotData

    //Storage for the inventory items - Tools & Equipments
    [Header("Tools & Equipments")]
    [SerializeField] private GameItemsSlotData[] _toolsEquipmentsSlotsData = new GameItemsSlotData[5]; //Changed from GameItemsData to GameItemsSlotData

    //Inventory Panel...
    private GameObject _inventoryPanel;
    private bool _inventoryPanelToggle = false;

    //Some Crusial variabls...
    private bool _isGameRunning = false;

    //Some varibales for keeping count...
    private int _numberOfSeeds = 0;
    private int _numberOfCrops = 0;
    private int _numberOfTools = 0;
    private int _numberOfEquipments = 0;

    #endregion

    void Start()
    {
        _inventoryPanel = transform.Find("InventoryPanel").gameObject;
        _isGameRunning = true;
        InteractOptionsManager.Instance.AddAsInteractInfoListener(this);
    }

    void Update()
    {
        #region #Only for Windows and WebGl Builds...
        if (!_inventoryPanel.activeSelf) //Check if inventory is active or not to get keybindings...
                                         //Only when it is not active player can use keyboard to toggle it.
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                _inventoryPanelToggle = true;
            }
        }
        #endregion

        //Toggles Inventory Panel...
        _inventoryPanel.SetActive(_inventoryPanelToggle);

        //Renders the inventory for the first time once the sprites required have been loaded...
        if (InventoryDisplayManager.Instance.getStatusOfSprites() == true)
        {
            InventoryDisplayManager.Instance.RenderInventoryNow();
            InventoryDisplayManager.Instance.AssignInventoryIndexes();
            InventoryDisplayManager.Instance.setStatusOfSprites(false);
        }
    }

    #region OnValidate - Inventory Values Set From Inspector
    void OnValidate() //Runs everytime when a value is changed in the inspector.
    {
        //Validating Equiped slots everytime when we change values in inspector
        ValidateValuesForIndividualSlot(_equipedItemSlotDataCS);
        ValidateValuesForIndividualSlot(_equipedItemSlotDataTE);

        //Validating Inventory slots everytime when we change values in inspector
        ValidateValuesForArrayOfSlots(_cropsSeedsSlotsData);
        ValidateValuesForArrayOfSlots(_toolsEquipmentsSlotsData);

        //Reflect all the changes on the UI screen after validation
        if (_isGameRunning)
        {
            InventoryDisplayManager.Instance.RenderInventoryNow();
        }
    }

    void ValidateValuesForIndividualSlot(GameItemsSlotData SlotData)
    {
        if(SlotData.GetGameItemsData() != null && SlotData.GetQuantity() == 0)
        {
            SlotData.SetQuantity(1);

            if(SlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Seed || 
                SlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
            {
                SlotData.SetQuantity(10);
                SlotData.SetMaxQuantity(10);
                SlotData.SetMinQuantity(1);
            }
            else
            {
                SlotData.SetMaxQuantity(1);
                SlotData.SetMinQuantity(1);

                if(SlotData.GetGameItemsData()._itemName == "Watering Can")
                {
                    SlotData.SetQuantity(6);
                    SlotData.SetMaxQuantity(12);
                    SlotData.SetMinQuantity(0);
                }
            }
        }

        if (SlotData.GetGameItemsData() == null)
        {
            SlotData.SetQuantity(0);
            SlotData.SetMaxQuantity(0);
            SlotData.SetMinQuantity(0);
        }
    }

    void ValidateValuesForArrayOfSlots(GameItemsSlotData[] inventorySlotData)
    {
        foreach (GameItemsSlotData slotData in inventorySlotData)
        {
            ValidateValuesForIndividualSlot(slotData);
        }
    }
    #endregion

    #region Functions Accessed By UI Buttons
    //Don't try to remove the below function, as this is accessd by the UI buttons.
    void ToggleInventoryPanel()
    {
        #region #Only for Windows and WebGl Builds...
        //#Only for Windows or WebGL build...
        if (_inventoryPanelToggle)
        {
            _inventoryPanelToggle = !_inventoryPanelToggle;
        }
        else
        {
            //Keyboard bindings will be used to open the inventory.
        }
        //#Only for Windows or WebGL build...
        #endregion
    }
    #endregion

    //Count Number Of Items In Inventory...
    public void CountItemsInInventory()
    {
        _numberOfSeeds = 0;
        _numberOfCrops = 0;
        _numberOfTools = 0;
        _numberOfEquipments = 0;

        foreach (var item in _cropsSeedsSlotsData)
        {
            if (item.GetGameItemsData() != null)
            {
                if (item.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
                {
                    _numberOfCrops++;
                }
                else
                {
                    _numberOfSeeds++;
                }
            }
        }

        foreach (var item in _toolsEquipmentsSlotsData)
        {
            if (item.GetGameItemsData() != null)
            {
                if (item.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Tool)
                {
                    _numberOfTools++;
                }
                else
                {
                    _numberOfEquipments++;
                }
            }
        }

        if(_equipedItemSlotDataCS.GetGameItemsData() != null)
        {
            if (_equipedItemSlotDataCS.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
            {
                _numberOfCrops++;
            }
            else
            {
                _numberOfSeeds++;
            }
        }

        if(_equipedItemSlotDataTE.GetGameItemsData() != null)
        {
            if (_equipedItemSlotDataTE.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Tool)
            {
                _numberOfTools++;
            }
            else
            {
                _numberOfEquipments++;
            }
        }
    }

    #region Inventory Moving Operations

    public void MoveInventoryItemToEquipedSlot(int slotIndex, InventorySlot.InventorySlotType slotType)
    {
        switch (slotType)
        {
            case InventorySlot.InventorySlotType.CS:

                //Creating new instances so that swap can be done without any old instance confusions... [Reason is because we don't pass by values here, instead we pass by reference]

                //The equiped slot data to alter...
                GameItemsSlotData equipSlotDataCS = _equipedItemSlotDataCS; //Here, _equipSlotDataCS acts as an instance holder to access the instance assigned to it. ----.
                //The slot data array to alter...                                                                                                                           |
                GameItemsSlotData[] inventorySlotArrayToAlterCS = _cropsSeedsSlotsData;//                                                                                  |
                //                                                                                                                                                          |
                //Checks if item can be stacked or Not...                                                                                                                   |
                if (equipSlotDataCS != null && equipSlotDataCS.CheckStackable(inventorySlotArrayToAlterCS[slotIndex])) //Can be stacked...                              |
                {//                                                                                                                                                         |
                    GameItemsSlotData inventorySlotDataToAlterCS = inventorySlotArrayToAlterCS[slotIndex];//                                                             |
                    //                                                                                                                                                      |
                    int quantityToStackCS = inventorySlotDataToAlterCS.GetQuantity();//                                                                                    |
                    //                                                                                                                                                      |
                    int quantityCanBeStacked = equipSlotDataCS.CheckQuantityToStack(inventorySlotDataToAlterCS);//                                                        |
                    //                                                                                                                                                      |
                    //Checks if full quantity can be stacked or some to Be left out...
                    //There is no lesser comaprision here...
                    if (quantityCanBeStacked > 0)                                                   //To check if anything can be stacked or no!                            |
                    {//                                                                                                                                                     |
                        if (quantityToStackCS == quantityCanBeStacked)                              //Fully stackable...                                                    |
                        {//                                                                                                                                                 |
                            equipSlotDataCS.PlusQuantity(quantityCanBeStacked);//                                                                                          |
                            inventorySlotDataToAlterCS.MinusQuantity(quantityCanBeStacked);//                                                                              |
                                                                                            //                                                                                                                                              |
                        }//                                                                                                                                                 |
                        else if (quantityToStackCS > quantityCanBeStacked)                          //Partially stackable...                                                |
                        {//                                                                                                                                                 |
                            equipSlotDataCS.PlusQuantity(quantityCanBeStacked);//                                                                                          |
                            inventorySlotDataToAlterCS.MinusQuantity(quantityCanBeStacked);//                                                                              |
                        }// 
                    }//                                                                                                                                                     |
                }//                                                                                                                                                         |
                else                                                                                                //Can not be stacked...                                 |
                {//                                                                                                                                                         |
                    //Creating new instances so that swap can be done without any old instance confusions...                                                                |
                    //                                                                                                                                                      |
                    GameItemsSlotData itemToEquipSlotDataCS = new GameItemsSlotData(inventorySlotArrayToAlterCS[slotIndex]);//                                           |
                    //                                                                                                                                                      |
                    inventorySlotArrayToAlterCS[slotIndex] = new GameItemsSlotData(equipSlotDataCS);//                                                                  \./
                    //                                                                                                                                                      V
                    equipSlotDataCS = itemToEquipSlotDataCS;  //Thus, if we assign some other instance to it, it is gonna hold te new instance and behave as usual as an instance holder.
                                                                //Even though _inventorySlotArrayToAlterCS[_slotindex] is of similar datatype, except it is an array of that datatype, when a new instance 
                                                                //is assigned to it, it changes the actual instance value in _cropsSeedsSlotsData. But _equipSlotDataCS doesn't do it.

                    //_equipSlotDataCS = new GameItemsSlotData(_inventorySlotArrayToAlterCS[_slotIndex]);

                                                                //There might be an explanation here, since _inventorySlotArrayToAlterCS[] is an array datatype, using [] in
                                                                //_inventorySlotArrayToAlterCS[_slotIndex] will access the value of _cropsSeedsSlotsData in that index (even though
                                                                //_inventorySlotArrayToAlterCS[] is just an instance holder) and that is the reason why it changed the value of
                                                                //_cropsSeedsSlotsData in its _slotindex.

                                                                //Moreover, _equipedItemSlotDataCS is used in inventory rendering and not _equipSlotDataCS. Hence, _equipSlotDataCS is just an
                                                                //instance holder.

                    _equipedItemSlotDataCS = equipSlotDataCS;  //The equiped slot changes with this line... but not with the previous line... I dunno why! But above explanation does makes sense.
                }

                break;


            case InventorySlot.InventorySlotType.TE:

                //Creating new instances so that swap can be done without any old instance confusions... [Reason is because we don't pass by values here, instead we pass by reference]

                //The equiped slot data to alter...
                GameItemsSlotData equipSlotDataTE = _equipedItemSlotDataTE;
                //The slot data array to alter...                                                                                             
                GameItemsSlotData[] inventorySlotArrayToAlterTE = _toolsEquipmentsSlotsData;

                //Checks if item can be stacked or Not...                                                                                     
                if (equipSlotDataTE != null && equipSlotDataTE.CheckStackable(inventorySlotArrayToAlterTE[slotIndex])) //Can be stacked...
                {
                    GameItemsSlotData inventorySlotDataToAlterTE = inventorySlotArrayToAlterTE[slotIndex];

                    int quantityToStackTE = inventorySlotDataToAlterTE.GetQuantity();

                    int quantityCanBeStacked = equipSlotDataTE.CheckQuantityToStack(inventorySlotDataToAlterTE);

                    //Checks if full quantity can be stacked or some to Be left out...
                    //There is no lesser comaprision here...
                    if (quantityCanBeStacked > 0)                                               //To check if anything can be stacked or no!
                    {
                        if (quantityToStackTE == quantityCanBeStacked)                          //Fully stackable...
                        {
                            equipSlotDataTE.PlusQuantity(quantityCanBeStacked);
                            inventorySlotDataToAlterTE.MinusQuantity(quantityCanBeStacked);
                        }
                        else if (quantityToStackTE > quantityCanBeStacked)                      //Partially stackable...
                        {
                            equipSlotDataTE.PlusQuantity(quantityCanBeStacked);
                            inventorySlotDataToAlterTE.MinusQuantity(quantityCanBeStacked);
                        }
                    }
                }
                else                                                                                                //Can not be stacked...
                {
                    //Creating new instances so that swap can be done without any old instance confusions...                                  

                    GameItemsSlotData itemToEquipSlotDataTE = new GameItemsSlotData(inventorySlotArrayToAlterTE[slotIndex]);

                    inventorySlotArrayToAlterTE[slotIndex] = new GameItemsSlotData(equipSlotDataTE);

                    equipSlotDataTE = itemToEquipSlotDataTE;

                    _equipedItemSlotDataTE = equipSlotDataTE;
                }

                break;
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();
    }

    public void MoveEquipedItemToInventorySlot(InventorySlot.InventorySlotType slotType)
    {
        switch (slotType)
        {
            case InventorySlot.InventorySlotType.CS:

                //Creating new instances so that swap can be done without any old instance confusions... [Reason is because we don't pass by values here, instead we pass by reference]

                //The equiped slot data to alter...
                GameItemsSlotData equipSlotDataCS = _equipedItemSlotDataCS;
                //The slot data array to alter...                                                                                             
                GameItemsSlotData[] inventorySlotArrayToAlterCS = _cropsSeedsSlotsData;

                //Trying & checking if the equiped item can be stacked in any of the inventory slots...
                if (/*_equipSlotDataCS.GetGameItemsData() != null && */!TryStackingInInventory(equipSlotDataCS, inventorySlotArrayToAlterCS))
                {
                    //if not able to stack, need to see if it can be stored in any empty slots...

                    //Can't use foreach loop because couldn't set values directly to slotData variable as it is just an iteration variable...
                    //foreach(GameItemsSlotData slotData in _inventorySlotArrayToAlterCS)
                    for (int i = 0; i < inventorySlotArrayToAlterCS.Length; i++)
                    {
                        if (inventorySlotArrayToAlterCS[i].CheckIsEmpty()) //_inventorySlotArrayToAlterCS[i].GetGameItemsData() == null & _inventorySlotArrayToAlterCS[i].GetQuantity() == 0
                        {
                            //Setting the empty slot a new instance of the equiped slot data...
                            inventorySlotArrayToAlterCS[i] = new GameItemsSlotData(equipSlotDataCS);

                            //Once it is moved entirely, setting the equiped slot to null...
                            equipSlotDataCS.EmptyIt(true);

                            _equipedItemSlotDataCS = equipSlotDataCS;

                            break;
                        }
                    }
                }

                break;


            case InventorySlot.InventorySlotType.TE:
                
                //Creating new instances so that swap can be done without any old instance confusions... [Reason is because we don't pass by values here, instead we pass by reference]

                //The equiped slot data to alter...
                GameItemsSlotData equipSlotDataTE = _equipedItemSlotDataTE;
                //The slot data array to alter...                                                                                             
                GameItemsSlotData[] inventorySlotArrayToAlterTE = _toolsEquipmentsSlotsData;

                //Trying & checking if the equiped item can be stacked in any of the inventory slots...
                if (/*_equipSlotDataTE.GetGameItemsData() != null && */!TryStackingInInventory(equipSlotDataTE, inventorySlotArrayToAlterTE))
                {
                    //if not able to stack, need to see if it can be stored in any empty slots...

                    //Can't use foreach loop because couldn't set values directly to slotData variable as it is just an iteration variable...
                    //foreach(GameItemsSlotData slotData in _inventorySlotArrayToAlterTE)

                    for(int i = 0; i < inventorySlotArrayToAlterTE.Length; i++)
                    {
                        if (inventorySlotArrayToAlterTE[i].CheckIsEmpty()) //_inventorySlotArrayToAlterTE[i].GetGameItemsData() == null & _inventorySlotArrayToAlterTE[i].GetQuantity() == 0
                        {
                            //Setting the empty slot a new instance of the equiped slot data...
                            inventorySlotArrayToAlterTE[i] = new GameItemsSlotData(equipSlotDataTE);

                            //Once it is moved entirely, setting the equiped slot to null...
                            equipSlotDataTE.EmptyIt(true);

                            _equipedItemSlotDataTE = equipSlotDataTE;

                            break;
                        }
                    }
                }

                break;
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();
    }

    //Returns the quantity added inside the inventory...
    public int AddObjectToInventorySlot(GameItemsData gameItemsData, int quantity)
    {
        GameItemsSlotData gameItemsSlotData = new GameItemsSlotData(gameItemsData, quantity);

        GameItemsSlotData[] inventoryToAlter = _toolsEquipmentsSlotsData;

        if (gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop || 
            gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Seed)
        {
            inventoryToAlter = _cropsSeedsSlotsData;
        }
        else
        {
            //I will comment out the if condition checking if its a "Watering Can".
            //Because when I buy a watering can from shop, it checks here and even if I have one already in my inventory the condition makes it to neglect 
            //it and adds one more watering can to the inventory.
            //Adding water quantity to a watering can doesn't check at this function as it has a separate one to do it.
            //Therefore I am gonna comment out the condition that checks for watering can and make it general for all tools and equipments...
            
            ////For an equipment or a tool, we check if there is no existence of it in the inventory so that only one quantity of the kind exists.
            /*if (gameItemsSlotData.GetGameItemsData()._itemName != "Watering Can")
             {*/
                //This if portion doesn't apply to Crops or Seeds or Watering Can.
                if (IsItemAlreadyInInventoryOrEquiped(gameItemsSlotData, inventoryToAlter))
                {
                    return 1;

                //This AddObjectToInventorySlot() function returns remaining quantity after adding to the inventory.
                //Hence, Returning 1 is a way of implying that the tool or equipment isn't added to the inventory.
            }
            /*}*/
        }

        //If stacking is true, this if statement won't be executed... Which means even if a partial stacking happened it won't proceed into this if.
        if(!TryStackingInInventory(gameItemsSlotData, inventoryToAlter)) //Tools and equipments wouldn't stack as their max quantity is 1 except watering can.
        {
            //Debug.Log("Couldn't Stack");

            for(int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].CheckIsEmpty())
                {
                    //Setting the empty slot with a new instance of the external GameItemsSlotData data...
                    inventoryToAlter[i] = new GameItemsSlotData(gameItemsSlotData);

                    //Once it is moved entirely, setting the equiped slot to null...
                    gameItemsSlotData.EmptyIt(true);

                    break;
                }
            }
        }
        //We have this else part to stack items when there happens a partial stacking... The remaining will be put into the empty slots...
        else if (gameItemsSlotData.GetQuantity() != 0)
        {
           // Debug.Log("Remaining Quantity 1 ::: " + gameItemsSlotData.GetQuantity());

            for (int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (inventoryToAlter[i].CheckIsEmpty())
                {
                    //Setting the empty slot with a new instance of the external GameItemsSlotData data...
                    inventoryToAlter[i] = new GameItemsSlotData(gameItemsSlotData);

                    //Once it is moved entirely, setting the equiped slot to null...
                    gameItemsSlotData.EmptyIt(true);

                    break;
                }
            }
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();

        //This is the remaining value of the item (after adding this item to inventory) that came into this function ...
        return gameItemsSlotData.GetQuantity();
    }

    public void RemoveObjectsFromInventorySlots(List<int> indexes, InventorySlot.InventorySlotType slotType)
    {
        if(indexes.Count != 0 && slotType == InventorySlot.InventorySlotType.CS)
        {
            GameItemsSlotData[] inventoryToAlter = _cropsSeedsSlotsData;

            for(int i = 0; i < inventoryToAlter.Length; i++)
            {
                if (!inventoryToAlter[i].CheckIsEmpty())
                {
                    if (inventoryToAlter[i].GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
                    {
                        inventoryToAlter[i].EmptyIt(true);
                    }
                    else
                    {
                        //Debug.Log("The item is not a crop!");
                    }
                }
                else
                {
                    //Debug.Log("The slot is empty!");
                }
            }
        }
        else
        {
            //Debug.Log("Either there are no indexes Or the slotType is not crop!");
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();
    }

    //Currently this is written to work only for Crops and Seeds...
    public void RemoveObjectFromEquipedSlot(InventorySlot.InventorySlotType slotType)
    {
        if(slotType == InventorySlot.InventorySlotType.CS)
        {
            GameItemsSlotData itemToRemove = _equipedItemSlotDataCS;

            itemToRemove.EmptyIt(true);
        }
        else
        {
            //Debug.Log("Tools Equipments Equiped Slot!");
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();
    }

    //This is used for Watering Can to add quantity of water...
    public void AddQuantityToEquiped(InventorySlot.InventorySlotType slotType, int quantity)
    {
        if (slotType == InventorySlot.InventorySlotType.CS)
        {
            GameItemsSlotData gameItemsSlotData = _equipedItemSlotDataCS;
            gameItemsSlotData.PlusQuantity(quantity);
        }

        if (slotType == InventorySlot.InventorySlotType.TE)
        {
            GameItemsSlotData gameItemsSlotData = _equipedItemSlotDataTE;
            gameItemsSlotData.PlusQuantity(quantity);
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();
    }

    //This is used for Watering Can to remove quantity of water...
    public void RemoveQuantityFromEquiped(InventorySlot.InventorySlotType slotType, int quantity)
    {
        if(slotType == InventorySlot.InventorySlotType.CS)
        {
            GameItemsSlotData gameItemsSlotData = _equipedItemSlotDataCS;
            gameItemsSlotData.MinusQuantity(quantity);
        }
        
        if(slotType == InventorySlot.InventorySlotType.TE)
        {
            GameItemsSlotData gameItemsSlotData = _equipedItemSlotDataTE;
            gameItemsSlotData.MinusQuantity(quantity);
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();
    }

    //The below function currently deletes the object completely from equiped slot...
    //It can be used to implement pick up and drop feature in future...
    public void DropItemFromEquipedSlot(InventorySlot.InventorySlotType slotType)
    {
        if (slotType == InventorySlot.InventorySlotType.CS)
        {
            GameItemsSlotData itemToRemove = _equipedItemSlotDataCS;

            //Here I have to instantiate a prefab [which is for the dropped items that'll be lying around in the world] 
            //that has a script holding GameItemsData and Quantity.
            //As soon as the dropped item prefab is instantiated, we set the variables of its script with the equiped 
            //item's details.
            //After that, we can empty it in the next line.

            itemToRemove.EmptyIt(true);
        }
        else
        {
            GameItemsSlotData itemToRemove = _equipedItemSlotDataTE;

            //Here I have to instantiate a prefab [which is for the dropped items that'll be lying around in the world] 
            //that has a script holding GameItemsData and Quantity.
            //As soon as the dropped item prefab is instantiated, we set the variables of its script with the equiped 
            //item's details.
            //After that, we can empty it in the next line.

            itemToRemove.EmptyIt(true);
        }

        InventoryDisplayManager.Instance.RenderInventoryNow();
    }

    //A reusable function for checking and trying to stack items into inventory slots...
    private bool TryStackingInInventory(GameItemsSlotData gameItemsSlotData, GameItemsSlotData[] InventoryToAlter)
    {
        for(int i = 0; i < InventoryToAlter.Length; i++)
        {
            if(gameItemsSlotData.GetGameItemsData() == InventoryToAlter[i].GetGameItemsData())
            {
                int quantityToStack = gameItemsSlotData.GetQuantity();

                int quantityCanBeStacked = InventoryToAlter[i].CheckQuantityToStack(gameItemsSlotData);

                //Debug.Log("Quantity To Stack :: " + quantityToStack);
                //Debug.Log("Quantity Can Be Stacked :: " + quantityCanBeStacked);

                //Checks if full quantity can be stacked or some to Be left out...
                //There is no lesser comaprision here...
                if (quantityCanBeStacked > 0)                                           //To check if anything can be stacked or no!
                {
                    if (quantityToStack == quantityCanBeStacked)                        //Fully stackable...
                    {
                        InventoryToAlter[i].PlusQuantity(quantityCanBeStacked);
                        gameItemsSlotData.MinusQuantity(quantityCanBeStacked);
                    }
                    else if (quantityToStack > quantityCanBeStacked)                    //Partially stackable...
                    {
                        //Debug.Log("Item :: " + InventoryToAlter[i].GetGameItemsData().name);
                        InventoryToAlter[i].PlusQuantity(quantityCanBeStacked);
                        gameItemsSlotData.MinusQuantity(quantityCanBeStacked);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    //A function that checks if the item already exists in the inventory or equiped slot and returns true or false...
    private bool IsItemAlreadyInInventoryOrEquiped(GameItemsSlotData gameItemsSlotData, GameItemsSlotData[] InventoryToAlter)
    {
        for(int i = 0; i < InventoryToAlter.Length; i++)
        {
            if(InventoryToAlter[i].GetGameItemsData() == null)
            {
                continue;
            }
            else if(gameItemsSlotData.GetGameItemsData() == InventoryToAlter[i].GetGameItemsData())
            {
                return true;
            }
            else
            {
                continue;
            }
        }

        if(gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop || 
            gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Seed)
        {
            if(_equipedItemSlotDataCS.GetGameItemsData() == null)
            {
                return false;
            }
            else if(gameItemsSlotData.GetGameItemsData() == _equipedItemSlotDataCS.GetGameItemsData())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Equipment || 
            gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Tool)
        {
            if (_equipedItemSlotDataTE.GetGameItemsData() == null)
            {
                return false;
            }
            else if (gameItemsSlotData.GetGameItemsData() == _equipedItemSlotDataTE.GetGameItemsData())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
    
    #endregion

    #region Observer Design Pattern Functions
    public void InteractInfoUpdate(RaycastHit _outHit)
    {

    }
    #endregion
}