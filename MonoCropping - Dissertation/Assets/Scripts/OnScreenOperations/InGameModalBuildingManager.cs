using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameModalBuildingManager : MonoBehaviour
{
    public void BuildInGameModalScreenWithMsg(InGameModal modal) { ConstructAModalScreen(modal); }
    public void DestroyModalScreen() { DestroyActiveModalScreen(); }

    public enum InGameModalType
    {
        //Book - has front and back navigation options
        Book,
        //BookWithAlternateOption - Book with an addition of alternate option. (Eg: Skip button)
        BookWithAlternateOption,
        //SinglePage - has one button (positive option - Eg: Continue, start, etc. buttons)
        SinglePage,
        //SinglePageWithNegativeOption - Single page with an addition of negative option (Eg: Confirmations)
        SinglePageWithNegativeOption,
        //SinglePageWithNegativeAndAlternateOption - Single page with negative option and an aditional alternate button (Eg: 3rd option in any choice making scenes)
        SinglePageWithNegativeAndAlternateOption
    }

    public struct InGameModal
    {
        public InGameModalType _inGameModalType;
        public string _heading;
        public bool _needBodyImg;
        public Sprite _bodyImg;
        public string _bodyTxt;
        public string _negativeButtonTxt;
        public Action _negativeAction;
        public string _alternateButtonTxt;
        public Action _alternateAction;
        public string _positiveButtonTxt;
        public Action _positiveAction;
    }

    //Will be assigned in the inspector...
    [SerializeField] private InGameModalScreenManager _inGameModalScreenManager;

    public static InGameModalBuildingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DestroyActiveModalScreen(); //Moved to Start so that InGameModalScreenManager will identify itself to be active or not on awake.
                                    //So that here we can set to false to hide it while game starts.
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ConstructAModalScreen(InGameModal modal)
    {
        //Debug.Log("Contructing A In Game Modal!");
        if (modal._inGameModalType == InGameModalType.Book)
        {
            _inGameModalScreenManager.ActivateModalScreenDisplay(true, false, false, false, false, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        if (modal._inGameModalType == InGameModalType.BookWithAlternateOption)
        {
            _inGameModalScreenManager.ActivateModalScreenDisplay(false, true, false, false, false, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        else if (modal._inGameModalType == InGameModalType.SinglePage)
        {
            _inGameModalScreenManager.ActivateModalScreenDisplay(false, false, true, false, false, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        else if (modal._inGameModalType == InGameModalType.SinglePageWithNegativeOption)
        {
            _inGameModalScreenManager.ActivateModalScreenDisplay(false, false, false, true, false, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        else if (modal._inGameModalType == InGameModalType.SinglePageWithNegativeAndAlternateOption)
        {
            _inGameModalScreenManager.ActivateModalScreenDisplay(false, false, false, false, true, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        //Debug.Log("A In Game Modal Constructed!");
    }

    void DestroyActiveModalScreen()
    {
        if (_inGameModalScreenManager.IsInGameModalScreenActive())
        {
            _inGameModalScreenManager.DeactivateModalScreenDisplay();
            //Debug.Log("In Game Modal Destroyed!");
        }
    }
}
