using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroModalScreenManager : MonoBehaviour
{
    public bool IsIntroModalScreenActive() { return _isIntroModalScreenActive; }

    //Bool variables...
    [SerializeField] private bool _isIntroModalScreenActive;

    //INTRO MODAL SCREEN MANAGER
    [Header("INTRO MODAL SCREEN MANAGER")]
    [SerializeField] private GameObject _introModalScreenPanel;
    [SerializeField] private GameObject _introModalScreenHeader;
    [SerializeField] private GameObject _introModalScreenBody;
    [SerializeField] private GameObject _introModalScreenFooter;

    //INTRO MODAL SCREEN PANEL
    [Header("INTRO MODAL SCREEN PANEL")]
    [SerializeField] private Image _introModalScreenPanelImage;

    //HEADER
    [Header("HEADER")]
    [SerializeField] private Image _introModalScreenPanelImageHeaderImage;
    [SerializeField] private TextMeshProUGUI _introModalScreenPanelImageHeaderImageHeaderText;

    //BODY
    [Header("BODY")]
    [SerializeField] private Image _introModalScreenPanelImageBodyImage;
    [SerializeField] private Image _introModalScreenPanelImageBodyImageBodyImage;
    [SerializeField] private TextMeshProUGUI _introModalScreenPanelImageBodyImageBodyText;

    //FOOTER
    [Header("FOOTER")]
    [SerializeField] private Image _introModalScreenPanelImageFooterImage;
    [SerializeField] private Button _introModalScreenPanelImageFooterImageFooterNegativeButton;
    [SerializeField] private TextMeshProUGUI _introModalScreenPanelImageFooterImageFooterNegativeButtonText;
    [SerializeField] private Button _introModalScreenPanelImageFooterImageFooterAlternateButton;
    [SerializeField] private TextMeshProUGUI _introModalScreenPanelImageFooterImageFooterAlternateButtonText;
    [SerializeField] private Button _introModalScreenPanelImageFooterImageFooterPositiveButton;
    [SerializeField] private TextMeshProUGUI _introModalScreenPanelImageFooterImageFooterPositiveButtonText;

    public static IntroModalScreenManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DeactivateModalScreenDisplay();
    }

    //Called by Awake() in the begining and by other script to deactivate modal screen when needed
    public void DeactivateModalScreenDisplay()
    {
        DeactivateAndClearAll();

        _introModalScreenPanel.SetActive(false);

        _isIntroModalScreenActive = false;
    }

    void DeactivateAndClearAll()
    {
        DeactivateHeader();
        DeactivateBody();
        DeactivateFooter();
    }

    //Called by another script to activate modal screen when needed
    public void ActivateModalScreenDisplay(bool isBook, bool isBookWithAlternateOption, 
        bool isSinglePage, bool isSinglePageWithNegativeOption, bool isSinglePageWithNegativeAndAlternateOption, 
        bool needBodyImage, 
        string headerText = "", 
        Sprite bodyImageSprite = null, string bodyText = "", 
        string footerNegativeButtonText = "", string footerAlternateButtonText = "", string footerPositiveButtonText = "", 
        Action negativeActionOption = null, Action alternateActionOption = null, Action positiveActionOption = null)
    {
        _introModalScreenPanel.SetActive(true);

        //Book - has front and back navigation options
        if (isBook)
        {
            ActivateHeader(headerText);
            ActivateBody(needBodyImage, true, bodyImageSprite, bodyText);
            ActivateFooter(true, false, true, footerNegativeButtonText, footerAlternateButtonText, footerPositiveButtonText,
                negativeActionOption, alternateActionOption, positiveActionOption);
        }
        //BookWithAlternateOption - Book with an addition of alternate option. (Eg: Skip button)
        else if (isBookWithAlternateOption)
        {
            ActivateHeader(headerText);
            ActivateBody(needBodyImage, true, bodyImageSprite, bodyText);
            ActivateFooter(true, true, true, footerNegativeButtonText, footerAlternateButtonText, footerPositiveButtonText,
                negativeActionOption, alternateActionOption, positiveActionOption);
        }
        //SinglePage - has one button (positive option - Eg: Continue, start, etc. buttons)
        else if (isSinglePage)
        {
            ActivateHeader(headerText);
            ActivateBody(needBodyImage, true, bodyImageSprite, bodyText);
            ActivateFooter(false, false, true, footerNegativeButtonText, footerAlternateButtonText, footerPositiveButtonText, 
                negativeActionOption, alternateActionOption, positiveActionOption);
        }
        //SinglePageWithNegativeOption - Single page with an addition of negative option (Eg: Confirmations)
        else if (isSinglePageWithNegativeOption)
        {
            ActivateHeader(headerText);
            ActivateBody(needBodyImage, true, bodyImageSprite, bodyText);
            ActivateFooter(true, false, true, footerNegativeButtonText, footerAlternateButtonText, footerPositiveButtonText, 
                negativeActionOption, alternateActionOption, positiveActionOption);
        }
        //SinglePageWithNegativeAndAlternateOption - Single page with negative option and an aditional alternate button (Eg: 3rd option in any choice making scenes)
        else if (isSinglePageWithNegativeAndAlternateOption)
        {
            ActivateHeader(headerText);
            ActivateBody(needBodyImage, true, bodyImageSprite, bodyText);
            ActivateFooter(true, true, true, footerNegativeButtonText, footerAlternateButtonText, footerPositiveButtonText,
                negativeActionOption, alternateActionOption, positiveActionOption);
        }

        _isIntroModalScreenActive = true;
    }


    void ActivateHeader(string headerText)
    {
        //Only if the header text has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if(headerText != "" && headerText != null)
        {
            //HEADER
            _introModalScreenHeader.SetActive(true);//Header Object

            _introModalScreenPanelImageHeaderImageHeaderText.gameObject.SetActive(true);//Header Text Object
            _introModalScreenPanelImageHeaderImageHeaderText.text = headerText;//Header Text
        }
    }

    void DeactivateHeader()
    {
        _introModalScreenPanelImageHeaderImageHeaderText.text = "";//Header Text
        if (_introModalScreenPanelImageHeaderImageHeaderText.gameObject.activeSelf)
        {
            _introModalScreenPanelImageHeaderImageHeaderText.gameObject.SetActive(false);//Header Text Object
        }

        if (_introModalScreenHeader.activeSelf)
        {
            _introModalScreenHeader.SetActive(false);//Header Object
        }
    }

    void ActivateBody(bool needBodyImage, bool needBodyText, Sprite bodyImageSprite, string bodyText)
    {
        //BODY
        _introModalScreenBody.SetActive(true);//Body Object

        _introModalScreenPanelImageBodyImageBodyImage.gameObject.SetActive(needBodyImage);//Body Image Object
        if (_introModalScreenPanelImageBodyImageBodyImage.gameObject.activeSelf)//Body Image Object Image Sprite
        { _introModalScreenPanelImageBodyImageBodyImage.sprite = bodyImageSprite; }

        _introModalScreenPanelImageBodyImageBodyText.gameObject.SetActive(needBodyText);//Body Text Object
        if (_introModalScreenPanelImageBodyImageBodyText.gameObject.activeSelf)//Body Text Object Text
        { _introModalScreenPanelImageBodyImageBodyText.text = bodyText; }
    }

    void DeactivateBody()
    {
        _introModalScreenPanelImageBodyImageBodyText.text = "";
        if (_introModalScreenPanelImageBodyImageBodyText.gameObject.activeSelf)
        {
            _introModalScreenPanelImageBodyImageBodyText.gameObject.SetActive(false);//Body Text Object
        }

        _introModalScreenPanelImageBodyImageBodyImage.sprite = null;
        if (_introModalScreenPanelImageBodyImageBodyImage.gameObject.activeSelf)
        {
            _introModalScreenPanelImageBodyImageBodyImage.gameObject.SetActive(false);//Body Image Object
        }

        if (_introModalScreenBody.activeSelf)
        {
            _introModalScreenBody.SetActive(false);//Body Object
        }
    }

    void ActivateFooter(bool needFooterNegativeButton, bool needFooterAlternateButton, bool needFooterPositiveButton, 
        string footerNegativeButtonText, string footerAlternateButtonText, string footerPositiveButtonText,
        Action negativeActionOption, Action alternateActionOption, Action positiveActionOption)
    {
        //FOOTER
        _introModalScreenFooter.SetActive(true);//Footer Object

        //When this footer button is needed, only if the footer text and action has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if (needFooterNegativeButton && footerNegativeButtonText != null && footerNegativeButtonText != "" && negativeActionOption != null)
        {
            _introModalScreenPanelImageFooterImageFooterNegativeButton.gameObject.SetActive(needFooterNegativeButton);//Footer Negative Button

            if (_introModalScreenPanelImageFooterImageFooterNegativeButton.gameObject.activeSelf)
            {
                _introModalScreenPanelImageFooterImageFooterNegativeButtonText.text = footerNegativeButtonText;//Footer Negative Button Text
                _introModalScreenPanelImageFooterImageFooterNegativeButton.onClick.AddListener(new UnityEngine.Events.UnityAction(negativeActionOption));//Add listener to the button's OnClick
            }
        }
        else
        {
            _introModalScreenPanelImageFooterImageFooterNegativeButton.gameObject.SetActive(needFooterNegativeButton);//Footer Negative Button
        }

        //When this footer button is needed, only if the footer text and action has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if (needFooterAlternateButton && footerAlternateButtonText != null && footerAlternateButtonText != "" && alternateActionOption != null)
        {
            _introModalScreenPanelImageFooterImageFooterAlternateButton.gameObject.SetActive(needFooterAlternateButton);//Footer Alternate Button

            if (_introModalScreenPanelImageFooterImageFooterAlternateButton.gameObject.activeSelf)
            {
                _introModalScreenPanelImageFooterImageFooterAlternateButtonText.text = footerAlternateButtonText;//Footer Alternate Button Text
                _introModalScreenPanelImageFooterImageFooterAlternateButton.onClick.AddListener(new UnityEngine.Events.UnityAction(alternateActionOption));//Add listener to the button's OnClick
            }
        }
        else
        {
            _introModalScreenPanelImageFooterImageFooterAlternateButton.gameObject.SetActive(needFooterAlternateButton);//Footer Alternate Button
        }

        //When this footer button is needed, only if the footer text and action has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if (needFooterPositiveButton && footerPositiveButtonText != null && footerPositiveButtonText != "" && positiveActionOption != null)
        {
            _introModalScreenPanelImageFooterImageFooterPositiveButton.gameObject.SetActive(needFooterPositiveButton);//Footer Positive Button

            if (_introModalScreenPanelImageFooterImageFooterPositiveButton.gameObject.activeSelf)
            {
                _introModalScreenPanelImageFooterImageFooterPositiveButtonText.text = footerPositiveButtonText;//Footer Positive Button Text
                _introModalScreenPanelImageFooterImageFooterPositiveButton.onClick.AddListener(new UnityEngine.Events.UnityAction(positiveActionOption));//Add listener to the button's OnClick
            }
        }
        else
        {
            _introModalScreenPanelImageFooterImageFooterPositiveButton.gameObject.SetActive(needFooterPositiveButton);//Footer Positive Button
        }
    }

    void DeactivateFooter()
    {
        #region COMMENTED PART - NEDD TO RESOLVE LATER - KEPT FOR FUTURE UPGRADE...
        //To make the button not be in selected state after clicking once: is to set it's Navigation drop down option from inspector to none.
        //Or can do it by code as done for each button below.
        //Unfortunately, I tried, but it doesn't work that way. Maybe I did it wrong. For now lets do the inspector method.
        //https://forum.unity.com/threads/clicking-a-button-leaves-it-in-mouseover-state.285167/
        #endregion

        _introModalScreenPanelImageFooterImageFooterNegativeButton.onClick.RemoveAllListeners();//Remove all listeners to the button's OnClick
        //var nav = _introModalScreenPanelImageFooterImageFooterNegativeButton.navigation;
        //nav.mode = Navigation.Mode.None;
        if (_introModalScreenPanelImageFooterImageFooterNegativeButton.gameObject.activeSelf)
        {
            _introModalScreenPanelImageFooterImageFooterNegativeButton.gameObject.SetActive(false);//Footer Negative Button
        }

        _introModalScreenPanelImageFooterImageFooterAlternateButton.onClick.RemoveAllListeners();//Remove all listeners to the button's OnClick
        //nav = _introModalScreenPanelImageFooterImageFooterAlternateButton.navigation;
        //nav.mode = Navigation.Mode.None;
        if (_introModalScreenPanelImageFooterImageFooterAlternateButton.gameObject.activeSelf)
        {
            _introModalScreenPanelImageFooterImageFooterAlternateButton.gameObject.SetActive(false);//Footer Alternate Button
        }

        _introModalScreenPanelImageFooterImageFooterPositiveButton.onClick.RemoveAllListeners();//Remove all listeners to the button's OnClick
        //nav = _introModalScreenPanelImageFooterImageFooterPositiveButton.navigation;
        //nav.mode = Navigation.Mode.None;
        if (_introModalScreenPanelImageFooterImageFooterPositiveButton.gameObject.activeSelf)
        {
            _introModalScreenPanelImageFooterImageFooterPositiveButton.gameObject.SetActive(false);//Footer Positive Button
        }

        if (_introModalScreenFooter.activeSelf)
        {
            _introModalScreenFooter.SetActive(false);//Footer Object
        }
    }
}