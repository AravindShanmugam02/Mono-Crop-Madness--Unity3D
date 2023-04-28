using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameModalScreenManager : MonoBehaviour
{
    public bool IsInGameModalScreenActive() { return _isModalScreenActive; }

    //Bool variables...
    [SerializeField] private bool _isModalScreenActive;

    private PauseManager _pauseManager;

    //MODAL SCREEN MANAGER
    [Header("MODAL SCREEN MANAGER")]
    [SerializeField] private GameObject _modalScreenPanel;
    [SerializeField] private GameObject _modalScreenHeader;
    [SerializeField] private GameObject _modalScreenBody;
    [SerializeField] private GameObject _modalScreenFooter;

    //MODAL SCREEN PANEL
    [Header("MODAL SCREEN PANEL")]
    [SerializeField] private Image _modalScreenPanelImage;

    //HEADER
    [Header("HEADER")]
    [SerializeField] private Image _modalScreenPanelImageHeaderImage;
    [SerializeField] private TextMeshProUGUI _modalScreenPanelImageHeaderImageHeaderText;

    //BODY
    [Header("BODY")]
    [SerializeField] private Image _modalScreenPanelImageBodyImage;
    [SerializeField] private Image _modalScreenPanelImageBodyImageBodyImage;
    [SerializeField] private TextMeshProUGUI _modalScreenPanelImageBodyImageBodyText;

    //FOOTER
    [Header("FOOTER")]
    [SerializeField] private Image _modalScreenPanelImageFooterImage;
    [SerializeField] private Button _modalScreenPanelImageFooterImageFooterNegativeButton;
    [SerializeField] private TextMeshProUGUI _modalScreenPanelImageFooterImageFooterNegativeButtonText;
    [SerializeField] private Button _modalScreenPanelImageFooterImageFooterAlternateButton;
    [SerializeField] private TextMeshProUGUI _modalScreenPanelImageFooterImageFooterAlternateButtonText;
    [SerializeField] private Button _modalScreenPanelImageFooterImageFooterPositiveButton;
    [SerializeField] private TextMeshProUGUI _modalScreenPanelImageFooterImageFooterPositiveButtonText;

    private void Awake()
    {
        _pauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>();
        _isModalScreenActive = _modalScreenPanel.activeSelf; //So that we know whether panel is active on awake.
                                                             //Then we can set it to false to hide it on start.
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Tutorials, Tips, Confirmation Screen
    public void ActivateModalScreenDisplay(bool isBook, bool isBookWithAlternateOption,
        bool isSinglePage, bool isSinglePageWithNegativeOption, bool isSinglePageWithNegativeAndAlternateOption,
        bool needBodyImage,
        string headerText = "",
        Sprite bodyImageSprite = null, string bodyText = "",
        string footerNegativeButtonText = "", string footerAlternateButtonText = "", string footerPositiveButtonText = "",
        Action negativeActionOption = null, Action alternateActionOption = null, Action positiveActionOption = null)
    {
        _modalScreenPanel.SetActive(true);

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

        _isModalScreenActive = true;

        _pauseManager.SetPausedStatus(true);
    }

    public void DeactivateModalScreenDisplay()
    {
        DeactivateAndClearAll();

        _modalScreenPanel.SetActive(false);

        _isModalScreenActive = false;
        
        _pauseManager.SetPausedStatus(false);
    }

    void DeactivateAndClearAll()
    {
        DeactivateHeader();
        DeactivateBody();
        DeactivateFooter();
    }

    void ActivateHeader(string headerText)
    {
        //Only if the header text has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if (headerText != "" && headerText != null)
        {
            //HEADER
            _modalScreenHeader.SetActive(true);//Header Object

            _modalScreenPanelImageHeaderImageHeaderText.gameObject.SetActive(true);//Header Text Object
            _modalScreenPanelImageHeaderImageHeaderText.text = headerText;//Header Text
        }
    }

    void DeactivateHeader()
    {
        _modalScreenPanelImageHeaderImageHeaderText.text = "";//Header Text
        _modalScreenPanelImageHeaderImageHeaderText.gameObject.SetActive(false);//Header Text Object

        _modalScreenHeader.SetActive(false);//Header Object
    }

    void ActivateBody(bool needBodyImage, bool needBodyText, Sprite bodyImageSprite, string bodyText)
    {
        //BODY
        _modalScreenBody.SetActive(true);//Body Object

        _modalScreenPanelImageBodyImageBodyImage.gameObject.SetActive(needBodyImage);//Body Image Object
        if (_modalScreenPanelImageBodyImageBodyImage.gameObject.activeSelf)//Body Image Object Image Sprite
        { _modalScreenPanelImageBodyImageBodyImage.sprite = bodyImageSprite; }

        _modalScreenPanelImageBodyImageBodyText.gameObject.SetActive(needBodyText);//Body Text Object
        if (_modalScreenPanelImageBodyImageBodyText.gameObject.activeSelf)//Body Text Object Text
        { _modalScreenPanelImageBodyImageBodyText.text = bodyText; }
    }

    void DeactivateBody()
    {
        _modalScreenPanelImageBodyImageBodyText.text = "";
        _modalScreenPanelImageBodyImageBodyText.gameObject.SetActive(false);//Body Text Object

        _modalScreenPanelImageBodyImageBodyImage.sprite = null;
        _modalScreenPanelImageBodyImageBodyImage.gameObject.SetActive(false);//Body Image Object

        _modalScreenBody.SetActive(false);//Body Object
    }

    void ActivateFooter(bool needFooterNegativeButton, bool needFooterAlternateButton, bool needFooterPositiveButton, 
        string footerNegativeButtonText, string footerAlternateButtonText, string footerPositiveButtonText,
        Action negativeActionOption, Action alternateActionOption, Action positiveActionOption)
    {
        //FOOTER
        _modalScreenFooter.SetActive(true);//Footer Object

        //When this footer button is needed, only if the footer text and action has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if (needFooterNegativeButton && footerNegativeButtonText != null && footerNegativeButtonText != "" && negativeActionOption != null)
        {
            _modalScreenPanelImageFooterImageFooterNegativeButton.gameObject.SetActive(needFooterNegativeButton);//Footer Negative Button

            if (_modalScreenPanelImageFooterImageFooterNegativeButton.gameObject.activeSelf)
            {
                _modalScreenPanelImageFooterImageFooterNegativeButtonText.text = footerNegativeButtonText;//Footer Negative Button Text
                _modalScreenPanelImageFooterImageFooterNegativeButton.onClick.AddListener(new UnityEngine.Events.UnityAction(negativeActionOption));//Add listener to the button's OnClick
            }
        }

        //When this footer button is needed, only if the footer text and action has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if (needFooterAlternateButton && footerAlternateButtonText != null && footerAlternateButtonText != "" && alternateActionOption != null)
        {
            _modalScreenPanelImageFooterImageFooterAlternateButton.gameObject.SetActive(needFooterAlternateButton);//Footer Alternate Button

            if (_modalScreenPanelImageFooterImageFooterAlternateButton.gameObject.activeSelf)
            {
                _modalScreenPanelImageFooterImageFooterAlternateButtonText.text = footerAlternateButtonText;//Footer Alternate Button Text
                _modalScreenPanelImageFooterImageFooterAlternateButton.onClick.AddListener(new UnityEngine.Events.UnityAction(alternateActionOption));//Add listener to the button's OnClick
            }
        }

        //When this footer button is needed, only if the footer text and action has some value, this component will be active... Implemented this only for header & footer, as header & footer is used for all types of modal screen layouts.
        if (needFooterPositiveButton && footerPositiveButtonText != null && footerPositiveButtonText != "" && positiveActionOption != null)
        {
            _modalScreenPanelImageFooterImageFooterPositiveButton.gameObject.SetActive(needFooterPositiveButton);//Footer Positive Button

            if (_modalScreenPanelImageFooterImageFooterPositiveButton.gameObject.activeSelf)
            {
                _modalScreenPanelImageFooterImageFooterPositiveButtonText.text = footerPositiveButtonText;//Footer Positive Button Text
                _modalScreenPanelImageFooterImageFooterPositiveButton.onClick.AddListener(new UnityEngine.Events.UnityAction(positiveActionOption));//Add listener to the button's OnClick
            }
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

        _modalScreenPanelImageFooterImageFooterNegativeButton.onClick.RemoveAllListeners();//Remove all listeners to the button's OnClick
        //var nav = _modalScreenPanelImageFooterImageFooterNegativeButton.navigation;
        //nav.mode = Navigation.Mode.None;
        _modalScreenPanelImageFooterImageFooterNegativeButton.gameObject.SetActive(false);//Footer Negative Button

        _modalScreenPanelImageFooterImageFooterAlternateButton.onClick.RemoveAllListeners();//Remove all listeners to the button's OnClick
        //nav = _modalScreenPanelImageFooterImageFooterAlternateButton.navigation;
        //nav.mode = Navigation.Mode.None;
        _modalScreenPanelImageFooterImageFooterAlternateButton.gameObject.SetActive(false);//Footer Alternate Button

        _modalScreenPanelImageFooterImageFooterPositiveButton.onClick.RemoveAllListeners();//Remove all listeners to the button's OnClick
        //nav = _modalScreenPanelImageFooterImageFooterPositiveButton.navigation;
        //nav.mode = Navigation.Mode.None;
        _modalScreenPanelImageFooterImageFooterPositiveButton.gameObject.SetActive(false);//Footer Positive Button

        _modalScreenFooter.SetActive(false);//Footer Object
    }
}