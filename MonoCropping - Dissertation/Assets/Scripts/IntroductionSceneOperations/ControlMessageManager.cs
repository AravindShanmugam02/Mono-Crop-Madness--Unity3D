using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlMessageManager : MonoBehaviour
{
    public void SetPlotMessageComplete(bool toggle)
    {
        _isPlotMessageComplete = toggle;
        var msg = _listOfControlMessages[0];
        PushControlMsgToIntroModalScreen(msg);
    }

    [SerializeField] private bool _isPlotMessageComplete;
    [SerializeField] private bool _hasControlMessageLoaded;
    [SerializeField] private bool _isControlMessageComplete;

    private IntroductionManager _introductionManager;

    IntroductionManager.IntroModal _controlMessage00;
    IntroductionManager.IntroModal _controlMessage01;

    private List<IntroductionManager.IntroModal> _listOfControlMessages;

    private void Awake()
    {
        //_introductionManager = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();

        //if (_introductionManager == null)
        //{
        //    Debug.LogError("_introductionManager is null in Control Message Manager!");
        //}

        //_listOfControlMessages = new List<IntroductionManager.IntroModal>();

        //_isPlotMessageComplete = false;
        //_hasControlMessageLoaded = false;
        //_isControlMessageComplete = false;

        //_listOfControlMessages.Add(_controlMessage00);
        ////_listOfControlMessages.Add(_controlMessage01);

        //LoadControlMessages();
    }

    // Start is called before the first frame update
    void Start()
    {
        _introductionManager = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();

        if (_introductionManager == null)
        {
            Debug.LogError("_introductionManager is null in Control Message Manager!");
        }

        _listOfControlMessages = new List<IntroductionManager.IntroModal>();

        _isPlotMessageComplete = false;
        _hasControlMessageLoaded = false;
        _isControlMessageComplete = false;

        _listOfControlMessages.Add(_controlMessage00);
        //_listOfControlMessages.Add(_controlMessage01);

        LoadControlMessages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadControlMessages()
    {
        for (int i = 0; i < _listOfControlMessages.Count; i++)
        {
            var msg = _listOfControlMessages[i];

            msg._introModalType = IntroductionManager.IntroModalType.Book;
            msg._heading = "Basic Instructions To Play...";
            msg._needBodyImg = false;
            msg._bodyImg = null;
            msg._bodyTxt = File.ReadAllText("Assets/InputFiles/ControlMessage0" + i + ".txt");
            //msg._bodyTxt = i.ToString();

            _listOfControlMessages[i] = msg;
        }

        var msg00 = _listOfControlMessages[0];
        //var msg01 = _listOfControlMessages[1];

        msg00._negativeButtonTxt = "Can't Go Back!";
        msg00._negativeAction = () =>
        {
            //Nothing!
        };
        msg00._positiveButtonTxt = "Next";
        msg00._positiveAction = () =>
        {
            _introductionManager.DestroyModalScreen();

            _isControlMessageComplete = true;

            SceneManager.UnloadSceneAsync("Scene01_Introduction");
            SceneManager.LoadScene("Scene02_Game");
        };

        //msg01._negativeButtonTxt = "Back";
        //msg01._negativeAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushControlMsgToIntroModalScreen(msg00);
        //};
        //msg01._positiveButtonTxt = "Next";
        //msg01._positiveAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();

        //    _isControlMessageComplete = true;

        //    //_plotMessageManager.SetHistoryMessageComplete(_isHistoryMessageComplete);

        //    SceneManager.UnloadSceneAsync("Scene01_Introduction");
        //    SceneManager.LoadScene("Scene02_Game");
        //};

        _listOfControlMessages[0] = msg00;
        //_listOfControlMessages[1] = msg01;

        _hasControlMessageLoaded = true;
    }

    void PushControlMsgToIntroModalScreen(IntroductionManager.IntroModal modal)
    {
        _introductionManager.BuildIntroModalScreenWithMsg(modal);
    }
}