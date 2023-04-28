using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameItemsSlotData
{
    public GameItemsData GetGameItemsData() { return _gameItemsData; }
    public int GetQuantity() { return _quantity; }
    public void SetQuantity(int quantity) { _quantity = quantity; }
    public void PlusOneQuantity() { AddOneQuantity(); }
    public void PlusQuantity(int quantity) { AddQuantity(quantity); }
    public void MinusOneQuantity() { RemoveOneQuantity(); }
    public void MinusQuantity(int quantity) { RemoveQuantity(quantity); }
    public void SetMaxQuantity(int maxQuantity) { _maxQuantity = maxQuantity; }
    public void SetMinQuantity(int minQuantity) { _minQuantity = minQuantity; }
    public bool CheckStackable(GameItemsSlotData dataToCompare) { return isStackable(dataToCompare); }
    public int CheckQuantityToStack(GameItemsSlotData dataToCompare) { return QuantityToStack(dataToCompare); }
    public int CheckQuantityThatCanBeStacked() { return QuantityThatCanBeStacked(); }

    [SerializeField] private GameItemsData _gameItemsData;
    [SerializeField] private int _quantity;
    [SerializeField] private int _maxQuantity = 10; //by default..
    [SerializeField] private int _minQuantity = 1; //by default..

    //Constructor - This constructor can construct a class with null gameItemData and 0 quantity, thus we do checks...
    public GameItemsSlotData(GameItemsData gameItemsData = null, int quantity = 0, float presentToolQuantity = 0f)
    {
        _gameItemsData = gameItemsData;
        _quantity = quantity;

        if (gameItemsData != null)
        {
            if (gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Crop || gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Seed)
            {
                _maxQuantity = 10;
            }
            else if (gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Tool || gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Equipment)
            {
                //I will have one tool and it is the max quantity player can carry in each type of tool...
                _maxQuantity = 1;

                //Only in the case of Watering Can we see the _minQuantity to be 0 and max to 7;
                if (gameItemsData._itemName == "Watering Can")
                {
                    _minQuantity = 0;
                    _maxQuantity = 25;
                }
            }
        }
        
        QuantityAndGameItemsDataChecks();
    }

    //Overloaded Constructor - For keeping the quantity of some items automatically to be 1 by default...
    public GameItemsSlotData(GameItemsData gameItemsData)
    {
        _gameItemsData = gameItemsData;
        _quantity = 1;

        if (gameItemsData != null)
        {
            if (gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Crop || gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Seed)
            {
                _maxQuantity = 10;
            }
            else if (gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Tool || gameItemsData._gameItemsDataType == GameItemsData.GameItemsDataType.Equipment)
            {
                _maxQuantity = 1;

                //Only in the case of Watering Can we see the _minQuantity to be 0 and max to 7;
                if (gameItemsData._itemName == "Watering Can")
                {
                    _minQuantity = 0;
                    _maxQuantity = 18;
                }
            }
        }
    }

    //Overload Constructor - For creating clone instances for inventory swapping, stacking operations...
    public GameItemsSlotData(GameItemsSlotData gameItemsSlotData)
    {
        _gameItemsData = gameItemsSlotData._gameItemsData;
        _quantity = gameItemsSlotData._quantity;
        _maxQuantity = gameItemsSlotData._maxQuantity;
        _minQuantity = gameItemsSlotData._minQuantity;
    }

    #region Stacking System

    #region CROPS & SEEDS - TOOLS & EQUIPMENTS

    //Functions to handle the stacking system for items...
    private void AddOneQuantity() //To add one quantity
    {
        _quantity++;

        ValidateWaterCanQuantity();
    }

    private void AddQuantity(int quanity) //To add specified quantity
    {
        _quantity += quanity;
        
        ValidateWaterCanQuantity();
    }

    private void RemoveOneQuantity() //To remove one quantity
    {
        _quantity--;

        ValidateWaterCanQuantity();

        QuantityAndGameItemsDataChecks();
    }

    private void RemoveQuantity(int quantity) //To add specified quantity
    {
        _quantity -= quantity;

        ValidateWaterCanQuantity();

        QuantityAndGameItemsDataChecks();
    }

    private bool isStackable(GameItemsSlotData dataToCompare) //To compare if items can be stacked, returns true if equal
    {
        return (dataToCompare.GetGameItemsData() == this.GetGameItemsData()); //Condition is scalable with different test conditions.
    }

    //Accessed by Equiped Slot to check Inventory Slot - from Move Inventory to Equiped
    private int QuantityToStack(GameItemsSlotData dataToCompare)
    {
        if(_maxQuantity >= GetQuantity() + dataToCompare.GetQuantity()) //If maxQuantity is greater or equal, return the full quantity as it can be added fully.
        {
            return dataToCompare.GetQuantity();
        }
        else if(_maxQuantity < GetQuantity() + dataToCompare.GetQuantity()) //If maxQuantity is lesser, return the quantity that can be added upto the maxQuantity.
        {
            return _maxQuantity - GetQuantity();
        }

        return 0;
    }

    private int QuantityThatCanBeStacked()
    {
        return _maxQuantity - _quantity;
    }

    //Function to return bool after checking if the instance of Game Items Slot Data is considered empty(that is, GameItemsData = Null and Quantity = Null)
    public bool CheckIsEmpty() //Note that the GameItemsSlotData will never be null even if the GameItemsData is null. In such case, GameItemsSlotData will hold an instance for empty slot...
    {
        if(_gameItemsData != null && _gameItemsData._itemName == "Watering Can" && _quantity >= 0)
        {
            return false;
        }

        if(_gameItemsData == null || _quantity <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //To clear the instances of the empty slots...
    public void EmptyIt(bool confirmation)
    {
        if (confirmation) { Empty(); }
    }

    private void ValidateWaterCanQuantity()
    {
        if(_gameItemsData != null && _gameItemsData._itemName == "Watering Can")
        {
            if (_quantity <= _minQuantity)
            {
                _quantity = _minQuantity;
            }
            else if (_quantity >= _maxQuantity)
            {
                _quantity = _maxQuantity;
            }
        }
    }


    //For checking the instances for null and 0, so that we can get rid of the old empty instances...
    private void QuantityAndGameItemsDataChecks() //To check if the quantity is 0 OR the Game Items Data is null
    {
        if (_gameItemsData != null && _gameItemsData._itemName == "Watering Can" && _quantity == 0)
        {
            //Do nothing for watering can!!!
        }
        else if ( _quantity <= 0 || _gameItemsData == null)
        {
            Empty();
        }
    }

    //Fucntion to perform the emptying task and clearing the old instances...
    private void Empty() //To clear the Game Items Slot
    {
        _gameItemsData = null;
        _quantity = 0;
        _maxQuantity = 0;
        _minQuantity = 0;
    }

    #endregion

    #endregion
}
