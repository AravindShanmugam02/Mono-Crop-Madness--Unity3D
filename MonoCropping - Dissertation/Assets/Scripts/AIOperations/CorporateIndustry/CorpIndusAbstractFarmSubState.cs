using System.Collections.Generic;
using UnityEngine;

public abstract class CorpIndusAbstractFarmSubState
{
    public abstract void EnterSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusFarmState corpIndusFarmState, Dictionary<LandManager, Vector3> dictionary);

    public abstract void SubStateCollisionEnter(CorpIndusFarmState corpIndusFarmState, Collision collision);

    public abstract void UpdateSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusFarmState corpIndusFarmState);
}
