using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "GameItems/Crop")]
public class CropItem : GameItemsData
{
    public enum Item
    {
        Cabbage, Carrot, Coffee, Corn, Cotton, Potato, Sugarcane, Wheat
    }
    public enum CropType
    {
        CerealGrain, Fiber, Fruit, Grass, Vegetable
    }

    [Header("Crop Properties")]
    public Item _item;
    public CropType _cropType;
}