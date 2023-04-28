using UnityEngine;

//https://www.youtube.com/watch?v=Vt8aZDPzRjI&ab_channel=iHeartGameDev

public abstract class CorpIndusAbstractBaseState
{
    public abstract void EnterState(CorpIndusStateManager corpIndusStateManager);

    public abstract void CollisionEnter(CorpIndusStateManager corpIndusStateManager, Collision collision);

    public abstract void UpdateState(CorpIndusStateManager corpIndusStateManager);
}