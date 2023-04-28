using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Brackeys sys it enebales the values in class to be saved in a file.
                      //Whereas others say it is to display this class properties in the inspector.
public class PlayerData
{
    public bool _isSlot1Complete;
    public bool _isSlot2Complete;
    public float _playerCredits = 20f;
    public int _populationCount = 100;

    //Constructor
    public PlayerData(GameManager gameManager)
    {
        _isSlot1Complete = gameManager.IsSlot1Complete();
        _isSlot2Complete = gameManager.IsSlot2Complete();
        _playerCredits = gameManager.GetPlayerCreditsToSave();
        _populationCount = gameManager.GetPopulationCountToSave();
    }
}