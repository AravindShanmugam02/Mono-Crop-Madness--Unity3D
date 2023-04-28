using UnityEngine;

public abstract class CorpIndusAbstractPatrolSubState
{
    public abstract void EnterSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusPatrolState corpIndusPatrolState);

    public abstract void SubStateCollisionEnter(CorpIndusPatrolState corpIndusPatrolState, Collision collision);

    public abstract void UpdateSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusPatrolState corpIndusPatrolState);
}
