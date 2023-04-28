using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //To access the UI elements like Image, etc,.
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

//THIS SCRIPT RUNS FOR EACH SLOTS IN THE INVENTORY, SO ANYTHING THAT HAPPENS IN THIS SCRIPT IS GONNA HAPPEN INDIVIDUALLY FOR THOSE SLOTS OTHER THAN THINGS THAT HAPPEN IN START.

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected GameItemsData _gameItemToDisplay;
    protected int _quantity;
    private Image _itemImage;
    private int _index;
    private TextMeshProUGUI _quantityText;
    private TextMeshProUGUI _reqWaterInfoText;
    private TextMeshProUGUI _itemTypeText;
    private TextMeshProUGUI _isEdibleText;
    private GameItemsData _draggedItem;
    [SerializeField] private bool _hovering = false;

    public enum InventorySlotType
    {
        CS, TE
    }
    public InventorySlotType _slotType;

    public void setIndex(int i) { this._index = i; }

    //*If this script is attached to an UI object whose both parent and child(ren) are UI objects,
    //then all 3 (this object, its parent and its child(ren)) needs to be checked true for raycast target.
    //*Only then will the callbacks like IPointerEnterHandler & IPointerExitHandler(which will check for hovering feature) work properly.
    //*Because eventData.fullyExited checks for hovering state on the object and its children.
    //*Reason why this object's parent is asked to check raycast target option true is that the raycast
    //will know the difference easily when parent is also a raycast target than compared to not having any other raycast target apart from this object.
    public void OnPointerEnter(PointerEventData eventData)
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

        //if (!eventData.fullyExited) -> Used as a workaround now as version 2021.3 has a known issue related
        //to this scenario where it doesn't work when the pointer enters child object.
        if (!eventData.fullyExited)
        {
            _hovering = true;
            InventoryDisplayManager.Instance.RenderItemInfoNow(_gameItemToDisplay);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
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

        if (eventData.fullyExited)
        {
            _hovering = false;
            InventoryDisplayManager.Instance.RenderItemInfoNow(null);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
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

        if (eventData.button == PointerEventData.InputButton.Left && this._gameItemToDisplay != null)
        {
            InventoryManager.Instance.MoveInventoryItemToEquipedSlot(this._index, this._slotType);
        }
    }

    public void DisplayItemInSlot(GameItemsSlotData gameItemsSlotData = null, Sprite defaultImageToDisplay = null, bool isItEquipedSlot = false)
    {
        //Had to move this from start to here because for each slot,
        //we have different ItemDisplay object, so we want to get ItemDisplay for each of the slot and not just once.
        _itemImage = gameObject.transform.Find("ItemDisplay").GetComponent<Image>();
        _quantityText = gameObject.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();

        if (_itemImage == null || _quantityText == null)
        {
            Debug.LogError("_itemImage - NULL! or _quantityText - NULL! in Inventory Slot");
        }

        if (!isItEquipedSlot)
        {
            _reqWaterInfoText = gameObject.transform.Find("ReqWaterText").GetComponent<TextMeshProUGUI>();
            _itemTypeText = gameObject.transform.Find("ItemTypeText").GetComponent<TextMeshProUGUI>();
            _isEdibleText = gameObject.transform.Find("IsEdibleText").GetComponent<TextMeshProUGUI>();

            if (_reqWaterInfoText == null || _itemTypeText == null || _isEdibleText == null)
            {
                Debug.LogError("_itemTypeText -NULL! or _otherInfoText -NULL! or _isEdibleText -NULL! in Inventory Slot");
            }
        }

        if (gameItemsSlotData.GetGameItemsData() != null)
        {
            _gameItemToDisplay = gameItemsSlotData.GetGameItemsData();
            _quantity = gameItemsSlotData.GetQuantity();

            if (_gameItemToDisplay._thumbnail != null)
            { 
                _itemImage.sprite = _gameItemToDisplay._thumbnail;

                if (!isItEquipedSlot)
                {
                    _reqWaterInfoText.text = "";
                    _itemTypeText.text = "";

                    if (gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Seed)
                    {
                        SeedItem item = gameItemsSlotData.GetGameItemsData() as SeedItem;
                        _reqWaterInfoText.text = item._reqWaterToGrow.ToString();

                        _itemTypeText.text = GameItemsData.GameItemsDataType.Seed.ToString();
                    }
                    else if (gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Crop)
                    {
                        _itemTypeText.text = GameItemsData.GameItemsDataType.Crop.ToString();
                    }
                    else if (gameItemsSlotData.GetGameItemsData()._gameItemsDataType == GameItemsData.GameItemsDataType.Tool)
                    {
                        _itemTypeText.text = GameItemsData.GameItemsDataType.Tool.ToString();
                    }
                    else
                    {
                        _itemTypeText.text = GameItemsData.GameItemsDataType.Equipment.ToString();
                    }

                    _isEdibleText.text = "";
                    _isEdibleText.text = gameItemsSlotData.GetGameItemsData()._isEdible.ToString();
                }

                _quantityText.text = "";
                if(_quantity > 0) { _quantityText.text = _quantity.ToString(); }
            }
            else
            { 
                Debug.LogError("Error - Inventory Slot. _gameItemToDisplay._thumbnail missing! or Quantity is 0!");
            }

            return;
        }

        _itemImage.sprite = defaultImageToDisplay;
        if (!isItEquipedSlot)
        {
            _reqWaterInfoText.text = "";
            _itemTypeText.text = "";
            _isEdibleText.text = "";
        }
        _quantityText.text = "";
        _gameItemToDisplay = null;
        _quantity = 0;
    }
}