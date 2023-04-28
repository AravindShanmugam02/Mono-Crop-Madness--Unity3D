using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "GameItems")]
public class GameItemsData : ScriptableObject
{
    //Crop, Seed, Yield, Tool, Equipment

    public enum GameItemsDataType
    {
        Crop, Equipment, Seed, Tool
    }

    public enum IsEdible
    {
        None, Edible, Non_Edible
    }

    [Header("Item Properties")]
    public string _itemName;
    public string _itemDescription;
    public Sprite _thumbnail;
    public GameObject _itemModel;
    public string _currency = "£";
    public float _buyingCost = 1.0f;
    public float _sellingCost = 1.0f;
    public GameItemsDataType _gameItemsDataType;
    public IsEdible _isEdible;
}