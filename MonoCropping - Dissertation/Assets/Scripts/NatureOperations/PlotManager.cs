using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent; //For Concurrent Dictionaries
using UnityEngine;

public class PlotManager : MonoBehaviour
{
    public int GetPlotOwnershipAsInt() { return (int)_plotOwnership; } //Since the enum order is same in both Land and Plot, we can get value by int...
    public float GetOverallPlotHealth() { return _overallPlotHealth; }
    public float GetOverallPlotNutritionLevel() { return _overallPlotNutritionLevel; }
    public string GetAveragePatternOfChildrenLand() { return _averagePatternOfChildrenLand; }
    public string GetMostOfLastCultivatedCropFromChildrenLand() { return _mostOfLastCultivatedCropInChildrenLand; }
    public string GetFarmingPatternOfPlot() { return _plotFarmingPattern.ToString(); }

    public enum PlotFarmingPatterns
    {
        NoPattern, SingleCropping, MixedCropping, InterCropping
    }
    [SerializeField] private PlotFarmingPatterns _plotFarmingPattern;
    private List<PlotFarmingPatterns> _plotFarmingPatternRecord;

    public enum PlotOwnership
    {
        Player, AI
    }
    [SerializeField] private PlotOwnership _plotOwnership;

    [SerializeField] private List<LandManager> _childrenLandManagersList;
    [SerializeField] private List<string> _lastCultivatedCropsList;
    [SerializeField] private List<string> _landFarmingPatternsList;

    [SerializeField] private int _childrenLandCount;

    [SerializeField] private int _maxNoOfPatternInLand;
    [SerializeField] private string _averagePatternOfChildrenLand;

    [SerializeField] private int _maxCountOfLastCultivatedCrop;
    [SerializeField] private string _mostOfLastCultivatedCropInChildrenLand;

    [SerializeField] private float _overallPlotHealth;
    [SerializeField] private float _overallPlotNutritionLevel;

    //private Dictionary<string, int> _numberOfEachLandPattern;

    //ConcurrentDictionary --> Represents a thread-safe collection of key/value pairs that can be accessed by multiple threads concurrently.
    [SerializeField] private ConcurrentDictionary<string, int> _countOfSameLandPatternsFromChildren;
    [SerializeField] private ConcurrentDictionary<string, int> _countOfSameLastCultivatedCropsFromChildren;

    void Awake()
    {
        _childrenLandCount = this.transform.childCount;
        _maxNoOfPatternInLand = 0;

        _childrenLandManagersList = new List<LandManager>();
        _plotFarmingPatternRecord = new List<PlotFarmingPatterns>();

        _lastCultivatedCropsList = new List<string>(_childrenLandCount); //Since each Plot has 9 Lands, we initialize with 9 or to set dynamically we can pass in children count...
        _landFarmingPatternsList = new List<string>(_childrenLandCount); //Since each Plot has 9 Lands, we initialize with 9 or to set dynamically we can pass in children count...

        _countOfSameLandPatternsFromChildren = new ConcurrentDictionary<string, int>();
        _countOfSameLastCultivatedCropsFromChildren = new ConcurrentDictionary<string, int>();

        _overallPlotHealth = 0.0f;
        _overallPlotNutritionLevel = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _childrenLandCount; i++)
        {
            _childrenLandManagersList.Add(this.transform.GetChild(i).GetComponent<LandManager>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlotPatternAndHealth()
    {
        _overallPlotHealth = 0.0f;
        _overallPlotNutritionLevel = 0.0f;

        PlotFarmingPatternDetector();
        EvaluatePlotHealth();
        EvaluatePlotNutritionLevel();
    }

    //Call this function each time when we cultivate a land...
    void PlotFarmingPatternDetector()
    {
        _maxNoOfPatternInLand = int.MinValue;
        _maxCountOfLastCultivatedCrop = int.MinValue;

        _lastCultivatedCropsList.Clear(); //List.Clear() -> count becomes 0; capacity is unchanged.
        _landFarmingPatternsList.Clear(); //List.Clear() -> count becomes 0; capacity is unchanged.

        _countOfSameLandPatternsFromChildren.Clear(); //Dictionary.Clear() -> count becomes 0; capacity is unchanged.
        _countOfSameLastCultivatedCropsFromChildren.Clear(); //Dictionary.Clear() -> count becomes 0; capacity is unchanged.

        //Getting the details of latest farming pattern to find the cultivation pattern of plot... //Mono Cropping or Crop Rotation
        for (int i = 0; i < _childrenLandCount; i++)
        {
            //_landFarmingPatternsList[i] = _childrenLandManagersList[i].GetLandFarmingPattern();
            _landFarmingPatternsList.Add(_childrenLandManagersList[i].GetLandFarmingPattern());
        }

        //Counting the number of Farming Patterns from each child using ConcurrentDictionary...
        for (int i = 0; i < _landFarmingPatternsList.Count; i++)
        {
            //Dictionary might have issues when we perform such operations when multi threaded environment comes in place. So we can use ConcurrentDictionary which is thread safe.
            //https://stackoverflow.com/a/7132978

            //_numberOfEachLandPattern.TryGetValue(_landFarmingPatternsList[k], out int count);
            //_numberOfEachLandPattern[_landFarmingPatternsList[k]] = count + 1;

            //Above 2 lines of code --> we try to get the value of a key using an out varibale and then 
            //try to update its value using the same varibale.
            //If the key is not found, it will be created and the value will be set to the datatypes default
            //value (here set to 0 as it is int type).

            //https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.addorupdate?view=net-6.0#system-collections-concurrent-concurrentdictionary-2-addorupdate(-0-system-func((-0-1))-system-func((-0-1-1)))
            //https://stackoverflow.com/a/7132913

            _countOfSameLandPatternsFromChildren.AddOrUpdate(_landFarmingPatternsList[i], 1, (key, value) => value + 1);
            //Concurrent Dictionary --> here we try to add or update it. if the key isn't found, we try to add it newly with default value of 1. If found we add 1 with the existing value.
            //key, value are part of lambda expression and we have not declared it anywhere else.
        }

        //Finding the children land patterns... Trying to find by average whether the average children land pattern is Monocropping or Crop Rotation or Potential Monocropping.
        foreach(var item in _countOfSameLandPatternsFromChildren)
        {
            if(item.Value >= _maxNoOfPatternInLand)
            {
                _maxNoOfPatternInLand = item.Value;
                _averagePatternOfChildrenLand = item.Key;
            }
        }

        //Getting the details of latest cultivated crop to find the cultivation pattern of plot... //Mixed Cropping or Inter Cropping
        for (int i = 0; i < _childrenLandCount; i++)
        {
            //_lastCultivatedCropsList[i] = _childrenLandManagersList[i].GetLastCultivated();
            _lastCultivatedCropsList.Add(_childrenLandManagersList[i].GetLastCultivated());
        }

        //Counting the number of crops cultivated from each child using ConcurrentDictionary...
        for (int i = 0; i < _lastCultivatedCropsList.Count; i++)
        {
            _countOfSameLastCultivatedCropsFromChildren.AddOrUpdate(_lastCultivatedCropsList[i], 1, (key, value) => value + 1);
            //Concurrent Dictionary --> here we try to add or update it. if the key isn't found, we try to add it newly with default value of 1. If found we add 1 with the existing value.
            //key, value are part of lambda expression and we have not declared it anywhere else.
        }

        //Finding the most cultivated crop (during the last cultivation) in children lands...
        foreach (var item in _countOfSameLastCultivatedCropsFromChildren)
        {
            if (item.Value >= _maxCountOfLastCultivatedCrop)
            {
                _maxCountOfLastCultivatedCrop = item.Value;
                _mostOfLastCultivatedCropInChildrenLand = item.Key;
            }
        }

        //CHECKING ROWS...
        //Finding the plot patterns... Trying to find if it is an InterCropping Pattern... Condition: Atleast 2 Rows are equal.
        if (_lastCultivatedCropsList[0] == _lastCultivatedCropsList[1] && _lastCultivatedCropsList[0] == _lastCultivatedCropsList[2]) //1st Row
        {
            if (_lastCultivatedCropsList[3] == _lastCultivatedCropsList[4] && _lastCultivatedCropsList[3] == _lastCultivatedCropsList[5]) //2nd Row -> 1st Row: Same
            {
                if (_lastCultivatedCropsList[6] == _lastCultivatedCropsList[7] && _lastCultivatedCropsList[6] == _lastCultivatedCropsList[8]) //3rd Row -> 1st & 2nd Row: Same
                {
                    if(_lastCultivatedCropsList[0] == _lastCultivatedCropsList[3] && _lastCultivatedCropsList[0] == _lastCultivatedCropsList[6]) //1st Column -> 1st, 2nd & 3rd Row: Same
                    {
                        //1st, 2nd & 3rd Row: Same && 1st Column: Same

                        //SingleCropping (Not Monocropping)
                        _plotFarmingPattern = PlotFarmingPatterns.SingleCropping;

                        ////When all lands of plot have same cultivated crops, we check if they are empty.
                        ////Starting of game all will be empty. If so, then we chnage it to NoPattern.
                        //if (_lastCultivatedCropsList[0] == "")
                        //{
                        //    _plotFarmingPattern = PlotFarmingPatterns.NoPattern;
                        //}

                    }
                    else //1st, 2nd & 3rd Row: Same && 1st Column: Diff
                    {
                        //InterCropping
                        _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
                    }
                }
                else //1st & 2nd Row: Same && 3rd Row: Diff
                {
                    //InterCropping
                    _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
                }
            }
            else if(_lastCultivatedCropsList[6] == _lastCultivatedCropsList[7] && _lastCultivatedCropsList[6] == _lastCultivatedCropsList[8]) //3rd Row -> 1st Row: Same && 2nd Row: Diff
            {
                //1st & 3rd Row: Same && 2nd Row: Diff

                //InterCropping
                _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
            }
            else //1st Row: Same && 2nd & 3rd Row: Diff
            {
                //Not InterCropping - Can be No Pattern
                _plotFarmingPattern = PlotFarmingPatterns.NoPattern;
            }
        }
        else if(_lastCultivatedCropsList[3] == _lastCultivatedCropsList[4] && _lastCultivatedCropsList[3] == _lastCultivatedCropsList[5]) //2nd Row -> 1st Row: Diff
        {
            if (_lastCultivatedCropsList[6] == _lastCultivatedCropsList[7] && _lastCultivatedCropsList[6] == _lastCultivatedCropsList[8]) //3rd Row -> 1st Row: Diff && 2nd Row: Same
            {
                //1st Row: Diff && 2nd & 3rd Row: Same

                //InterCropping
                _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
            }
            else //1st & 3rd Row: Diff && 2nd Row: Same
            {
                //Not InterCropping - Can be No Pattern
                _plotFarmingPattern = PlotFarmingPatterns.NoPattern;
            }
        }

        //CHECKING COLUMNS...
        //Finding the plot patterns... Trying to find if it is a MixedCropping Pattern... Condition: Not InterCropping.
        if (_plotFarmingPattern == PlotFarmingPatterns.NoPattern)
        {
            if (_lastCultivatedCropsList[0] == _lastCultivatedCropsList[3] && _lastCultivatedCropsList[0] == _lastCultivatedCropsList[6]) //1st Column
            {
                if (_lastCultivatedCropsList[1] == _lastCultivatedCropsList[4] && _lastCultivatedCropsList[1] == _lastCultivatedCropsList[7]) //2nd Column -> 1st Column: Same
                {
                    if (_lastCultivatedCropsList[2] == _lastCultivatedCropsList[5] && _lastCultivatedCropsList[2] == _lastCultivatedCropsList[8]) //3rd Column -> 1st & 2nd Column: Same
                    {
                        //1st, 2nd & 3rd Column: Same

                        //InterCropping
                        _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
                    }
                    else //1st, 2nd Column: Same && 3rd Column: Diff
                    {
                        //InterCropping
                        _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
                    }
                }
                else if (_lastCultivatedCropsList[2] == _lastCultivatedCropsList[5] && _lastCultivatedCropsList[2] == _lastCultivatedCropsList[8]) //3rd Column -> 1st Column: Same
                {
                    //1st, 3rd Column: Same && 2nd Column: Diff

                    //InterCropping
                    _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
                }
                else //1st Column: Same && 2nd, 3rd Column: Diff
                {
                    //Not InterCropping - Can be Mixed Cropping
                    _plotFarmingPattern = PlotFarmingPatterns.MixedCropping;
                }
            }
            else if (_lastCultivatedCropsList[1] == _lastCultivatedCropsList[4] && _lastCultivatedCropsList[1] == _lastCultivatedCropsList[7]) //2nd Column -> 1st Column: Diff
            {
                if (_lastCultivatedCropsList[2] == _lastCultivatedCropsList[5] && _lastCultivatedCropsList[2] == _lastCultivatedCropsList[8]) //3rd Column -> 1st Column: Diff && 2nd Column: Same
                {
                    //1st Column: Diff && 2nd & 3rd Column: Same

                    //InterCropping
                    _plotFarmingPattern = PlotFarmingPatterns.InterCropping;
                }
                else //1st & 3rd Column: Diff && 2nd Column: Same
                {
                    //Not InterCropping - Can be Mixed Cropping
                    _plotFarmingPattern = PlotFarmingPatterns.MixedCropping;
                }
            }
            else //1st & 2nd Column: Diff
            {
                //Not InterCropping - Can be Mixed Cropping
                _plotFarmingPattern = PlotFarmingPatterns.MixedCropping;
            }
        }
    }

    void EvaluatePlotHealth()
    {
        for(int i = 0; i < _childrenLandCount; i++)
        {
            _overallPlotHealth += _childrenLandManagersList[i].GetOverallLandHealth();
        }

        _overallPlotHealth /= _childrenLandCount;
    }

    void EvaluatePlotNutritionLevel()
    {
        for (int i = 0; i < _childrenLandCount; i++)
        {
            _overallPlotNutritionLevel += _childrenLandManagersList[i].GetLandNutritionLevel();
        }

        _overallPlotNutritionLevel /= _childrenLandCount;
    }
}