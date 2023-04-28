using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreetingMessageManager : MonoBehaviour
{
    public bool IsGreetingComplete() { return _isGreetingComplete; }

    IntroductionManager _introductionManager;
    IntroductionManager.IntroModal _greetingMessage;

    HistoryMessageManager _historyMessageManager;
    private bool _isGreetingComplete;

    // Start is called before the first frame update
    void Start()
    {
        _introductionManager = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();
        _historyMessageManager = GameObject.Find("HistoryMessageManager").GetComponent<HistoryMessageManager>();

        if (_introductionManager == null)
        {
            Debug.LogError("_introductionManager is null in Greeting Message Manager!");
        }

        if (_historyMessageManager == null)
        {
            Debug.LogError("_historyMessageManager is null in Greeting Message Manager!");
        }

        _isGreetingComplete = false;

        //_greetingMessage = new IntroductionManager.IntroModal();
        _greetingMessage._introModalType = IntroductionManager.IntroModalType.SinglePage;
        _greetingMessage._heading = "Welcome To Monocrop Madness!";
        _greetingMessage._needBodyImg = false;
        _greetingMessage._bodyImg = null;
        _greetingMessage._bodyTxt = File.ReadAllText("Assets/InputFiles/GreetingsMessage.txt");
        _greetingMessage._negativeButtonTxt = "";
        _greetingMessage._negativeAction = null;
        _greetingMessage._alternateButtonTxt = "";
        _greetingMessage._alternateAction = null;
        _greetingMessage._positiveButtonTxt = "Next";
        _greetingMessage._positiveAction = () => {

            _introductionManager.DestroyModalScreen();

            _isGreetingComplete = true;

            _historyMessageManager.SetGreetingMessageComplete(_isGreetingComplete);
        };

        PushGreetingMsgToIntroModalScreen(_greetingMessage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PushGreetingMsgToIntroModalScreen(IntroductionManager.IntroModal modal)
    {
        _introductionManager.BuildIntroModalScreenWithMsg(modal);
    }
}