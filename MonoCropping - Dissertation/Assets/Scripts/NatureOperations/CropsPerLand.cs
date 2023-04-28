using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropsPerLand : MonoBehaviour
{
    #region Public Getters & Setters
    public bool GetReadyToHarvest() { return _readyToHarvest; }
    public void SetCropsPerLandDestroyTrigger(bool trigger) { _destroyTrigger = trigger; }
    public void SetDestroyTriggerByPlayer(bool trigger) { _isDestroyedByPlayer = trigger; }
    public void RemoveQuantity(int quantity) { _quantityOfItems -= quantity; }
    public int GetQuantityOfItems() { return _quantityOfItems; }
    #endregion

    #region Class Member Variables

    private CropBehaviour _cropBehaviour;
    private List<CropBehaviour> _cropBehaviourList;  
    private bool _readyToHarvest = false;
    private bool _destroyTrigger = false;
    private bool _isDestroyedByPlayer = false;
    private int _quantityOfItems = 10;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Get an handle to a random child component...
        //This is only used to check the status of the child components. Only one is taken here as all the child components follow the same code and behaviour...
        _cropBehaviour = GetComponentInChildren<CropBehaviour>();

        //Get handles for all the child components...
        _cropBehaviourList = new List<CropBehaviour>();

        for(int i = 0; i < this.transform.childCount; i++)
        {
            _cropBehaviourList.Add(this.transform.GetChild(i).GetComponent<CropBehaviour>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Nothing...
        if (_destroyTrigger == true && _quantityOfItems <= 0)
        {
            ObjectDestroy();
        }
        else if(_isDestroyedByPlayer == true)
        {
            ObjectDestroy(_isDestroyedByPlayer);
        }
        else
        {
            _destroyTrigger = false;
        }
    }

    public void TryHarvest()
    {
        //Debug.Log(" :: " + _cropBehaviour.GetCropDestroyStatus() + " :: " + _cropBehaviour.GetCropGrowthStage());

        if (_cropBehaviour.GetCropDestroyStatus() == true
            && _cropBehaviour.GetCropGrowthStage() == CropBehaviour.CropGrowthStage.Yield
            && _quantityOfItems > 0)
        {
            _readyToHarvest = true;
            //Debug.Log("Crops are ready to be harvested!");
        }
        else
        {
            _readyToHarvest = false;
            //Debug.Log("Crops are not ready to be harvested. Give it some time!");
        }
    }

    void ObjectDestroy()
    {
        Destroy(this.gameObject);
    }

    void ObjectDestroy(bool value)
    {
        //_cropBehaviour.SetCropDestroyStatus(value); //This is for only one of the child and doesn't apply for all the children.
        
        //Thus we use the foreach loop below to set for every children...

        foreach(var item in _cropBehaviourList)
        {
            //item.transform.parent = null;
            //Destroy(item.gameObject);

            item.SetCropDestroyStatus(value);
        }

        Destroy(this.gameObject);
    }
}
