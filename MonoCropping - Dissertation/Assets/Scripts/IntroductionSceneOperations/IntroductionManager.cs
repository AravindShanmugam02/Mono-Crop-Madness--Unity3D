using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroductionManager : MonoBehaviour
{
    public void BuildIntroModalScreenWithMsg(IntroModal modal) { ConstructAModalScreen(modal); }
    public void DestroyModalScreen() { DestroyActiveModalScreen(); }

    public enum IntroModalType
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

    public struct IntroModal
    {
        public IntroModalType _introModalType;
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

    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _testText;

    // Start is called before the first frame update
    void Start()
    {
        //This is incase if the mouse cursor is set to lock mode in previous screen. (It should have updated to unlocked, but just for safety)
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IntroModalScreenManager.Instance.IsIntroModalScreenActive())
        {
            _background.color = Color.black;
            _testText.color = Color.white;
            _testText.text = "Active";
        }
        else
        {
            _background.color = Color.white;
            _testText.color = Color.black;
            _testText.text = "Not Active";
        }
    }

    void ConstructAModalScreen(IntroModal modal)
    {
        //Debug.Log("Contructing A Modal!");
        if(modal._introModalType == IntroModalType.Book)
        {
            IntroModalScreenManager.Instance.ActivateModalScreenDisplay(true, false, false, false, false, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        if(modal._introModalType == IntroModalType.BookWithAlternateOption)
        {
            IntroModalScreenManager.Instance.ActivateModalScreenDisplay(false, true, false, false, false, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        else if(modal._introModalType == IntroModalType.SinglePage)
        {
            IntroModalScreenManager.Instance.ActivateModalScreenDisplay(false, false, true, false, false, modal._needBodyImg, modal._heading, modal._bodyImg, 
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt, 
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        else if(modal._introModalType == IntroModalType.SinglePageWithNegativeOption)
        {
            IntroModalScreenManager.Instance.ActivateModalScreenDisplay(false, false, false, true, false, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        else if(modal._introModalType == IntroModalType.SinglePageWithNegativeAndAlternateOption)
        {
            IntroModalScreenManager.Instance.ActivateModalScreenDisplay(false, false, false, false, true, modal._needBodyImg, modal._heading, modal._bodyImg,
                modal._bodyTxt, modal._negativeButtonTxt, modal._alternateButtonTxt, modal._positiveButtonTxt,
                modal._negativeAction, modal._alternateAction, modal._positiveAction);
        }
        //Debug.Log("A Modal Constructed!");
    }

    void DestroyActiveModalScreen()
    {
        if (IntroModalScreenManager.Instance.IsIntroModalScreenActive())
        {
            IntroModalScreenManager.Instance.DeactivateModalScreenDisplay();
            //Debug.Log("Modal Destroyed!");
        }
    }
}