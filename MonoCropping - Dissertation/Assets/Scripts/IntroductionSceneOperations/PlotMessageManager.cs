using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlotMessageManager : MonoBehaviour
{
    public void SetHistoryMessageComplete(bool toggle)
    {
        _isHistoryMessageComplete = toggle;
        var msg = _listOfPlotMessages[0];
        PushPlotMsgToIntroModalScreen(msg);
    }

    [SerializeField] private bool _isHistoryMessageComplete;
    [SerializeField] private bool _hasPlotMessageLoaded;
    [SerializeField] private bool _isPlotMessageComplete;

    private IntroductionManager _introductionManager;
    private ControlMessageManager _controlMessageManager;

    IntroductionManager.IntroModal _gamePlotMessage00;
    IntroductionManager.IntroModal _gamePlotMessage01;
    //IntroductionManager.IntroModal _gamePlotMessage02;
    //IntroductionManager.IntroModal _gamePlotMessage03;
    //IntroductionManager.IntroModal _gamePlotMessage04;

    private List<IntroductionManager.IntroModal> _listOfPlotMessages;

    private void Awake()
    {
        //_introductionManager = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();
        //_controlMessageManager = GameObject.Find("ControlMessageManager").GetComponent<ControlMessageManager>();

        //if (_introductionManager == null)
        //{
        //    Debug.LogError("_introductionManager is null in Plot Message Manager!");
        //}

        //if(_controlMessageManager == null)
        //{
        //    Debug.LogError("_controlMessageManager is null in Plot Message Manager!");
        //}

        //_listOfPlotMessages = new List<IntroductionManager.IntroModal>();

        //_isHistoryMessageComplete = false;
        //_hasPlotMessageLoaded = false;
        //_isPlotMessageComplete = false;

        //_listOfPlotMessages.Add(_gamePlotMessage00);
        //_listOfPlotMessages.Add(_gamePlotMessage01);
        ////_listOfPlotMessages.Add(_gamePlotMessage02);
        ////_listOfPlotMessages.Add(_gamePlotMessage03);
        ////_listOfPlotMessages.Add(_gamePlotMessage04);

        //LoadPlotMessages();
    }

    // Start is called before the first frame update
    void Start()
    {
        _introductionManager = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();
        _controlMessageManager = GameObject.Find("ControlMessageManager").GetComponent<ControlMessageManager>();

        if (_introductionManager == null)
        {
            Debug.LogError("_introductionManager is null in Plot Message Manager!");
        }

        if (_controlMessageManager == null)
        {
            Debug.LogError("_controlMessageManager is null in Plot Message Manager!");
        }

        _listOfPlotMessages = new List<IntroductionManager.IntroModal>();

        _isHistoryMessageComplete = false;
        _hasPlotMessageLoaded = false;
        _isPlotMessageComplete = false;

        _listOfPlotMessages.Add(_gamePlotMessage00);
        _listOfPlotMessages.Add(_gamePlotMessage01);
        //_listOfPlotMessages.Add(_gamePlotMessage02);
        //_listOfPlotMessages.Add(_gamePlotMessage03);
        //_listOfPlotMessages.Add(_gamePlotMessage04);

        LoadPlotMessages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Took this function from History Message Manager and changed variable names & removed comments...
    void LoadPlotMessages()
    {
        for (int i = 0; i < _listOfPlotMessages.Count; i++)
        {
            var msg = _listOfPlotMessages[i];

            msg._introModalType = IntroductionManager.IntroModalType.Book;
            msg._heading = "What are you doing in this game...";
            msg._needBodyImg = false;
            msg._bodyImg = null;
            msg._bodyTxt = File.ReadAllText("Assets/InputFiles/PlotMessage0" + i + ".txt");
            //msg._bodyTxt = i.ToString();

            _listOfPlotMessages[i] = msg;
        }

        var msg00 = _listOfPlotMessages[0];
        var msg01 = _listOfPlotMessages[1];
        //var msg02 = _listOfPlotMessages[2];
        //var msg03 = _listOfPlotMessages[3];
        //var msg04 = _listOfPlotMessages[4];

        msg00._negativeButtonTxt = "Can't Go Back!";
        msg00._negativeAction = () =>
        {
            //Nothing!
        };
        msg00._positiveButtonTxt = "Continue";
        msg00._positiveAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            PushPlotMsgToIntroModalScreen(msg01);
        };

        msg01._negativeButtonTxt = "Back";
        msg01._negativeAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            PushPlotMsgToIntroModalScreen(msg00);
        };
        msg01._positiveButtonTxt = "Next";
        msg01._positiveAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            //PushPlotMsgToIntroModalScreen(msg02);

            _isPlotMessageComplete = true;

            _controlMessageManager.SetPlotMessageComplete(_isPlotMessageComplete);
        };

        //msg02._negativeButtonTxt = "Back";
        //msg02._negativeAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushPlotMsgToIntroModalScreen(msg01);
        //};
        //msg02._positiveButtonTxt = "Continue";
        //msg02._positiveAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushPlotMsgToIntroModalScreen(msg03);
        //};

        //msg03._negativeButtonTxt = "Back";
        //msg03._negativeAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushPlotMsgToIntroModalScreen(msg02);
        //};
        //msg03._positiveButtonTxt = "Continue";
        //msg03._positiveAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushPlotMsgToIntroModalScreen(msg04);
        //};

        //msg04._negativeButtonTxt = "Back";
        //msg04._negativeAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushPlotMsgToIntroModalScreen(msg03);
        //};
        //msg04._positiveButtonTxt = "Next";
        //msg04._positiveAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();

        //    _isPlotMessageComplete = true;

        //    _controlMessageManager.SetPlotMessageComplete(_isPlotMessageComplete);
        //};


        _listOfPlotMessages[0] = msg00;
        _listOfPlotMessages[1] = msg01;
        //_listOfPlotMessages[2] = msg02;
        //_listOfPlotMessages[3] = msg03;
        //_listOfPlotMessages[4] = msg04;

        _hasPlotMessageLoaded = true;
    }

    void PushPlotMsgToIntroModalScreen(IntroductionManager.IntroModal modal)
    {
        _introductionManager.BuildIntroModalScreenWithMsg(modal);
    }
}
