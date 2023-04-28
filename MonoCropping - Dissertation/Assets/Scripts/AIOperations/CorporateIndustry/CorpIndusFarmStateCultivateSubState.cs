using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CorpIndusFarmStateCultivateSubState : CorpIndusAbstractFarmSubState
{
    private Dictionary<LandManager, Vector3> _dictionary;
    private NavMeshAgent _navMeshAgent;
    private LandManager _landToHandle = null;
    
    [SerializeField] private bool _navAgentReachedDestination = false;
    [SerializeField] private bool _landProcessing = false;

    //private SeedItem _seedItem1;
    //private SeedItem _seedItem2;

    private Dictionary<PlotManager, SeedItem> _plotManagerPairedWithRespectiveCrop;

    public override void EnterSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusFarmState corpIndusFarmState, Dictionary<LandManager, Vector3> dictionary)
    {
        _dictionary = dictionary;
        _navMeshAgent = corpIndusStateManager.GetNavMeshAgent();

        //_seedItem1 = corpIndusFarmState.GetSeetItem1();
        //_seedItem2 = corpIndusFarmState.GetSeetItem2();

        _plotManagerPairedWithRespectiveCrop = new Dictionary<PlotManager, SeedItem>();
        _plotManagerPairedWithRespectiveCrop = corpIndusFarmState.GetAIPlotsWithRespectiveCrops();

        corpIndusStateManager.StartCoroutine(StartCultivate(corpIndusFarmState));
    }

    public override void SubStateCollisionEnter(CorpIndusFarmState corpIndusFarmState, Collision collision)
    {

    }

    public override void UpdateSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusFarmState corpIndusFarmState)
    {

    }

    IEnumerator StartCultivate(CorpIndusFarmState corpIndusFarmState)
    {
        foreach (var item in _dictionary)
        {
            _landToHandle = item.Key;
            _navMeshAgent.SetDestination(item.Value);

            _landProcessing = true;
            _navAgentReachedDestination = false;

            //https://forum.unity.com/threads/coroutine-wait-until-a-condition.999046/#post-6484981
            yield return new WaitUntil(WhetherNavMeshAgentReachedDestination);

            while(_landProcessing == true)
            {
                if (_navAgentReachedDestination == true && _landToHandle != null)
                {
                    CropsPerLand cropsPerLand = _landToHandle.GetComponentInChildren<CropsPerLand>();

                    if(_landToHandle.GetLandState() == LandManager.LandState.Dry && cropsPerLand != null && _landToHandle.GetIsLandCultivated())
                    {
                        _landToHandle.AIInteraction("Destroy", "AI");

                        yield return new WaitWhile(CheckWeatherLandIsCultivated);
                    }

                    if (_landToHandle.GetLandState() == LandManager.LandState.Dry)
                    {
                        _landToHandle.AIInteraction("Till", "AI");

                        //https://forum.unity.com/threads/coroutine-wait-until-a-condition.999046/#post-6484981
                        yield return new WaitUntil(CheckWhetherLandIsTilled);
                    }

                    if (_landToHandle.GetLandState() == LandManager.LandState.Muddy)
                    {
                        _landToHandle.AIInteraction("Water", "AI");

                        //https://forum.unity.com/threads/coroutine-wait-until-a-condition.999046/#post-6484981
                        yield return new WaitUntil(CheckWhetherLandIsWatered);
                    }

                    if (_landToHandle.GetLandState() == LandManager.LandState.Farm)
                    {
                        foreach(var plotCropItem in _plotManagerPairedWithRespectiveCrop)
                        {
                            if (plotCropItem.Key == _landToHandle.transform.parent.GetComponent<PlotManager>())
                            {
                                _landToHandle.AIInteraction("Cultivate", "AI", plotCropItem.Value._cropToYield._itemName, plotCropItem.Value);
                                break;
                            }
                        }

                        //https://forum.unity.com/threads/coroutine-wait-until-a-condition.999046/#post-6484981
                        yield return new WaitUntil(CheckWeatherLandIsCultivated);
                    }

                    if (_landToHandle.GetIsLandCultivated())
                    {
                        _landProcessing = false;
                        //Debug.Log("Land cultivation successfull!");
                    }
                    else
                    {
                        //Debug.Log("Land cultivation failed!");
                    }
                }
                else
                {
                    //Debug.Log("Agent didn't reach the destination yet!");
                    //Debug.Log("LandToHandle is null in CultivateSubState");
                    break;
                }
            }
        }

        yield return new WaitForSeconds(1f);

        corpIndusFarmState.SetCultivationStatus(true);
    }

    bool WhetherNavMeshAgentReachedDestination()
    {
        //http://answers.unity.com/answers/746157/view.html
        //if no path pending
        if (!_navMeshAgent.pathPending)
        {
            //if remaining distance is less than or equal to stoping distance (i.e (0 <= 0))
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                //if agent has no path or if agent's velocity in magnitude is 0 (i.e not moving)
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    _navAgentReachedDestination = true;
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckWhetherLandIsTilled()
    {
        if (_landToHandle.GetLandState() == LandManager.LandState.Muddy)
        {
            return true;
        }
        return false;
    }

    bool CheckWhetherLandIsWatered()
    {
        if (_landToHandle.GetLandState() == LandManager.LandState.Farm)
        {
            return true;
        }
        return false;
    }

    bool CheckWeatherLandIsCultivated()
    {
        if (_landToHandle.GetIsLandCultivated())
        {
            return true;
        }
        return false;
    }
}