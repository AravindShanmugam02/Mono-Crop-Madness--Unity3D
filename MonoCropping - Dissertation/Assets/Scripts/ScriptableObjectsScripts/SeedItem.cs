using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSeed", menuName = "GameItems/Seed")]
public class SeedItem : GameItemsData
{
    [Header("Seed Properties")]
    public float _daysToGrow = 0f;
    public int _reqWaterToGrow = 0;
    public CropItem _cropToYield;
    public GameObject _seedlingModel;
}
