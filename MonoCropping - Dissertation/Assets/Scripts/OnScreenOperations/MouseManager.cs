using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour, iHUDInfo
{
    //Need to set if mouse pointer is required...
    public void ToggleMouseLock(bool toggle)
    {
        _isMouseLockedInGame = toggle; //if this is true
        _isMouseVisible = !toggle; //this will be false
    }

    public bool GetIsMouseLocked() { return _isMouseLockedInGame; }

    [SerializeField] bool _isMouseLockedInGame;
    [SerializeField] bool _isMouseVisible;
    [SerializeField] bool _isAppFocused;

    // Start is called before the first frame update
    void Start()
    {
        HUDManager.Instance.AddAsIHUDListener(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        #region Moved this mouse lock code to Update
        //if (_isAppFocused)//When application is focused
        //{
        //    if (_isMouseLockedInGame)
        //    {
        //        Cursor.lockState = CursorLockMode.Locked;
        //        Cursor.visible = _isMouseVisible;
        //    }
        //    else
        //    {
        //        Cursor.lockState = CursorLockMode.Confined;
        //        Cursor.visible = _isMouseVisible;
        //    }
        //    //Debug.Log("App In Focus");
        //}
        //else//When not in focus
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = _isMouseVisible;
        //    //Debug.Log("App Not In Focus");
        //}
        #endregion

        //FixedUpdate may or may not update every single frame of game.
        //That's why mouse lock wasn't working as expected when paused because the timescale will be set to 0 on pause.
    }

    /*
    //private void LateUpdate()
    //{
    //    if (_isAppFocused)//When application is focused
    //    {
    //        if (_isMouseLockedInGame)
    //        {
    //            Cursor.lockState = CursorLockMode.Locked;
    //            Cursor.visible = _isMouseVisible;
    //        }
    //        else
    //        {
    //            Cursor.lockState = CursorLockMode.Confined;
    //            Cursor.visible = _isMouseVisible;
    //        }
    //        //Debug.Log("App In Focus");
    //    }
    //    else//When not in focus
    //    {
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = _isMouseVisible;
    //        //Debug.Log("App Not In Focus");
    //    }
    //}
    */


    private void OnApplicationFocus(bool focus) //Changing the Cursor Lock Mode when the application is focussed...
                                                //Doesn't update everyframe. Updates once per change.
                                                //If app out of focus updates once. If app comes again focus, updates once.
    {
        _isAppFocused = focus;
    }

    //This function is not used at the moment...
    void UpdateMouseLockStatus()
    {
        if (_isAppFocused)//When application is focused
        {
            if (_isMouseLockedInGame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = _isMouseVisible;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = /*_isMouseVisible*/true;
            }
            //Debug.Log("App In Focus");
        }
        else//When not in focus
        {
            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = _isMouseVisible;
            //Debug.Log("App Not In Focus");
        }
    }

    public void HUDInfoUpdate(bool isInteracting, bool isBarnOpen, bool isGrainsAndSeedsStorageOpen, 
        bool isGreenhouseOpen, bool isLandDetailsViewed, bool isShopOpen, bool isInventoryOpen, 
        bool isModalScreenOpen, bool isFeedbackActive, bool isPaused)
    {
        if(!isInteracting && !isBarnOpen && !isGrainsAndSeedsStorageOpen && !isGreenhouseOpen && !isLandDetailsViewed &&
            !isShopOpen && !isInventoryOpen && !isModalScreenOpen && !isFeedbackActive && !isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _isMouseLockedInGame = true;

            Cursor.visible = false;
            _isMouseVisible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            _isMouseLockedInGame = false;

            Cursor.visible = true;
            _isMouseVisible = true;
        }
    }
}
