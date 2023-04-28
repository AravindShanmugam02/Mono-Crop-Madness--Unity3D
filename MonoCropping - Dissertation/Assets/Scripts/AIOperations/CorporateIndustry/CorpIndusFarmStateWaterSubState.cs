using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CorpIndusFarmStateWaterSubState : CorpIndusAbstractFarmSubState
{
    private Dictionary<LandManager, Vector3> _dictionary;
    private NavMeshAgent _navMeshAgent;
    private LandManager _landToHandle = null;

    [SerializeField] private bool _navAgentReachedDestination = false;
    [SerializeField] private bool _landProcessing = false;

    public override void EnterSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusFarmState corpIndusFarmState, Dictionary<LandManager, Vector3> dictionary)
    {
        _dictionary = dictionary;
        _navMeshAgent = corpIndusStateManager.GetNavMeshAgent();

        corpIndusStateManager.StartCoroutine(StartWatering(corpIndusFarmState));
    }

    public override void SubStateCollisionEnter(CorpIndusFarmState corpIndusFarmState, Collision collision)
    {

    }

    public override void UpdateSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusFarmState corpIndusFarmState)
    {

    }

    IEnumerator StartWatering(CorpIndusFarmState corpIndusFarmState)
    {
        foreach (var item in _dictionary)
        {
            _landToHandle = item.Key;
            _navMeshAgent.SetDestination(item.Value);

            _landProcessing = true;
            _navAgentReachedDestination = false;

            yield return new WaitUntil(WhetherNavMeshAgentReachedDestination);

            while (_landProcessing == true)
            {
                if (_navAgentReachedDestination == true && _landToHandle != null)
                {
                    if (_landToHandle.GetIsLandCultivated() && _landToHandle.GetLandState() == LandManager.LandState.Muddy)
                    {
                        _landToHandle.AIInteraction("Water", "AI");

                        //https://forum.unity.com/threads/coroutine-wait-until-a-condition.999046/#post-6484981
                        yield return new WaitUntil(CheckWhetherLandIsWatered);
                    }

                    if (_landToHandle.GetLandState() == LandManager.LandState.Farm)
                    {
                        _landProcessing = false;
                        //Debug.Log("Land watering successfull!");
                    }
                    else
                    {
                        //Debug.Log("Land watering failed!");
                    }
                }
                else
                {
                    //Debug.Log("Agent didn't reach the destination yet!");
                   // Debug.Log("LandToHandle is null in WateringSubState");
                    break;
                }
            }
        }

        yield return new WaitForSeconds(1f);

        corpIndusFarmState.SetWateringStatus(true);
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

    bool CheckWhetherLandIsWatered()
    {
        if (_landToHandle.GetLandState() == LandManager.LandState.Farm)
        {
            return true;
        }
        return false;
    }
}