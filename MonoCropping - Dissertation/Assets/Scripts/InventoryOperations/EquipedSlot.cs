using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipedSlot : InventorySlot
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        #region THE CODE INSIDE THIS REGION DOESN"T WORK PROPERLY AS EXPECTED. NEED TO FIX IT LATER
        ////To Prevent elements behind the current UI element to be interacted with mouse...
        ////https://www.youtube.com/watch?v=rATAnkClkWU&ab_channel=JasonWeimann
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}

        //BELOW IS A COMMENT FROM THAT YOUTUBE VIDEO EXPLAINING WHY EVERY UI ELEMENT IS BLOCKED FROM CLICKING...

        /*IsPointerOverGameObject DOES NOT MEAN "over UI object" 
         * it means "OVER ANYTHING THAT HAS EVENTSYSTEM INTERFACES implemented" meaning, if you implement anything 
         * from the IPointer*Handler in your MonoBehavior, the function will return true, even if it's an in-game, non-ui object.
         * meaning, this method, which I've seen repeated... everywhere... 
         * only works when you mix the old and new method of detecting mouse events, just like you do here.*/
        #endregion

        if (/*eventData.pointerClick && */eventData.button == PointerEventData.InputButton.Left && this._gameItemToDisplay != null)
        {
            InventoryManager.Instance.MoveEquipedItemToInventorySlot(this._slotType);
        }

        #region EVENT DATA - RIGHT CLICK DETECTION - FORUM ANSWER
        /*
            So there is an even simpler method. If anyone has a similar issue to mine (4.6 buttons that need to be right clicked), then you're in luck! Basically it involved augmenting your class (or creating a new one) with 'IPointerClickHandler'. Example below:

                 using UnityEngine;
                 using UnityEngine.EventSystems;
     
     
                 public class MyRightClickClass : MonoBehaviour, IPointerClickHandler {
     
                     public void OnPointerClick (PointerEventData eventData) {
                         if (eventData.button == PointerEventData.InputButton.Right) {
                             Debug.Log ("Right Mouse Button Clicked on: " + name);
                         }
                     }
     
                 }

            So there it is; a simple alternative to the other examples on this page for those that are seeking a clean (and short) solution.
         */
        #endregion //http://answers.unity.com/answers/1397085/view.html

        else if (eventData.button == PointerEventData.InputButton.Right && this._gameItemToDisplay != null)
        {
            InventoryManager.Instance.DropItemFromEquipedSlot(this._slotType);
        }
    }
}
