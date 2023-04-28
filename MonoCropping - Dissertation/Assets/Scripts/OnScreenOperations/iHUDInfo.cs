using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iHUDInfo
{
    void HUDInfoUpdate(bool isInteracting, bool isBarnOpen,
        bool isGrainsAndSeedsStorageOpen, bool isGreenhouseOpen, bool isLandDetailsViewed, bool isShopOpen,
        bool isInventoryOpen, bool isModalScreenOpen, bool isFeedbackActive, bool isPaused); //Had problem with mouse cursor when timescale set to 0,
                                                                                             //because mouse lock was updated in FixedUpdate in Mouse Manager.
                                                                                             //Now it is fixed by moveing to Update() of Mouse Manager.
}
