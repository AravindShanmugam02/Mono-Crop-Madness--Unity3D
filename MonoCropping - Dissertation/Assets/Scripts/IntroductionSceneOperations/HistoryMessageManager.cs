using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HistoryMessageManager : MonoBehaviour
{
    public void SetGreetingMessageComplete(bool toggle)
    {
        _isGreetingMessageComplete = toggle;
        var msg = _listOfHistoryMessages[0];
        PushHistoryMsgToIntroModalScreen(msg);
    }

    [SerializeField] private bool _isGreetingMessageComplete;
    [SerializeField] private bool _hasHistoryMessageLoaded;
    [SerializeField] private bool _isHistoryMessageComplete;

    private IntroductionManager _introductionManager;
    private PlotMessageManager _plotMessageManager;

    private IntroductionManager.IntroModal _historyMessage00;
    private IntroductionManager.IntroModal _historyMessage01;
    private IntroductionManager.IntroModal _historyMessage02;
    //private IntroductionManager.IntroModal _historyMessage03;
    //private IntroductionManager.IntroModal _historyMessage04;

    [SerializeField] private List<IntroductionManager.IntroModal> _listOfHistoryMessages;

    private void Awake()
    {
        //_introductionManager = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();
        //_plotMessageManager = GameObject.Find("PlotMessageManager").GetComponent<PlotMessageManager>();

        //if (_introductionManager == null)
        //{
        //    Debug.LogError("_introductionManager is null in History Message Manager!");
        //}

        //if (_plotMessageManager == null)
        //{
        //    Debug.LogError("_plotMessageManager is null in History Message Manager!");
        //}

        //_listOfHistoryMessages = new List<IntroductionManager.IntroModal>();

        //_isGreetingMessageComplete = false;
        //_hasHistoryMessageLoaded = false;
        //_isHistoryMessageComplete = false;

        //#region COMMENTED PART - NEDD TO RESOLVE LATER - KEPT FOR FUTURE UPGRADE...
        ////### HERE I WAS TRYING TO GET ALL THE SAME TYPE VARIBALES PRESENT IN THIS CLASS INTO A LIST OR ARRAY. BUT FAILED, HENCE COMMENTED.
        ////### WILL TRY AGAIN LATER!
        //////https://stackoverflow.com/a/7495999/18762063
        //////https://stackoverflow.com/a/7495977/18762063
        ////var currentMethod = System.Reflection.MethodBase.GetCurrentMethod()/*.GetCustomAttributes(typeof(IntroductionManager.IntroModal), false)*/;

        ////foreach (var item in currentMethod.GetParameters())
        ////{
        ////    if(item.ParameterType == typeof(IntroductionManager.IntroModal))
        ////    {
        ////        _listOfHistoryMessages.Add();
        ////    }
        ////}

        ////if (_listOfHistoryMessages.Count <= 0)
        ////{
        ////    Debug.LogError("_listOfHistoryMessages is null or its count is 0 in History Message Manager!");
        ////}
        ////###
        ////###
        //#endregion

        //_listOfHistoryMessages.Add(_historyMessage00);
        //_listOfHistoryMessages.Add(_historyMessage01);
        //_listOfHistoryMessages.Add(_historyMessage02);
        ////_listOfHistoryMessages.Add(_historyMessage03);
        ////_listOfHistoryMessages.Add(_historyMessage04);

        //LoadHistoryMessages();
    }

    // Start is called before the first frame update
    void Start()
    {
        _introductionManager = GameObject.Find("IntroductionManager").GetComponent<IntroductionManager>();
        _plotMessageManager = GameObject.Find("PlotMessageManager").GetComponent<PlotMessageManager>();

        if (_introductionManager == null)
        {
            Debug.LogError("_introductionManager is null in History Message Manager!");
        }

        if (_plotMessageManager == null)
        {
            Debug.LogError("_plotMessageManager is null in History Message Manager!");
        }

        _listOfHistoryMessages = new List<IntroductionManager.IntroModal>();

        _isGreetingMessageComplete = false;
        _hasHistoryMessageLoaded = false;
        _isHistoryMessageComplete = false;

        #region COMMENTED PART - NEDD TO RESOLVE LATER - KEPT FOR FUTURE UPGRADE...
        //### HERE I WAS TRYING TO GET ALL THE SAME TYPE VARIBALES PRESENT IN THIS CLASS INTO A LIST OR ARRAY. BUT FAILED, HENCE COMMENTED.
        //### WILL TRY AGAIN LATER!
        ////https://stackoverflow.com/a/7495999/18762063
        ////https://stackoverflow.com/a/7495977/18762063
        //var currentMethod = System.Reflection.MethodBase.GetCurrentMethod()/*.GetCustomAttributes(typeof(IntroductionManager.IntroModal), false)*/;

        //foreach (var item in currentMethod.GetParameters())
        //{
        //    if(item.ParameterType == typeof(IntroductionManager.IntroModal))
        //    {
        //        _listOfHistoryMessages.Add();
        //    }
        //}

        //if (_listOfHistoryMessages.Count <= 0)
        //{
        //    Debug.LogError("_listOfHistoryMessages is null or its count is 0 in History Message Manager!");
        //}
        //###
        //###
        #endregion

        _listOfHistoryMessages.Add(_historyMessage00);
        _listOfHistoryMessages.Add(_historyMessage01);
        _listOfHistoryMessages.Add(_historyMessage02);
        //_listOfHistoryMessages.Add(_historyMessage03);
        //_listOfHistoryMessages.Add(_historyMessage04);

        LoadHistoryMessages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadHistoryMessages()
    {
        ////Constant variable values
        //const int n0 = 0;
        //const int n1 = 1;
        //const int n2 = 2;
        //const int n3 = 3;
        //const int n4 = 4;

        for (int i = 0; i < _listOfHistoryMessages.Count; i++)
        {
            //var msg = new IntroductionManager.IntroModal(); //This is because I couldn't change values of the IntroductionManager.IntroModal struct
            var msg = _listOfHistoryMessages[i];                //handle variables directly using foreach (var item) or by for (_listOfHistoryMessages[i]).

            msg._introModalType = IntroductionManager.IntroModalType.Book;
            msg._heading = "A quick look into the past...";
            msg._needBodyImg = false;
            msg._bodyImg = null;
            msg._bodyTxt = File.ReadAllText("Assets/InputFiles/HistoryMessage0" + i + ".txt");
            //msg._bodyTxt = i.ToString();

            #region COMMENTED PART - NEDD TO RESOLVE LATER - KEPT FOR FUTURE UPGRADE...
            //if (i == 0)
            //{
            //    msg._negativeButtonTxt = "";
            //    msg._negativeAction = null;
            //    Debug.Log(i + "Null");
            //}
            //else
            //{
            //    msg._negativeButtonTxt = "Back";
            //    msg._negativeAction = () =>
            //    {
            //        _introductionManager.DestroyModalScreen();
            //        var msg0 = _listOfHistoryMessages[i - 1];
            //        PushHistoryMsgToIntroModalScreen(msg0);
            //    };
            //    Debug.Log(i + "Back");
            //}
            //
            //msg._alternateButtonTxt = "";
            //msg._alternateAction = null;
            //
            //if (i < _listOfHistoryMessages.Count - 1)
            //{
            //    msg._positiveButtonTxt = "Continue";
            //    msg._positiveAction = () =>
            //    {
            //        _introductionManager.DestroyModalScreen();
            //        var msg1 = _listOfHistoryMessages[i + 1];
            //        PushHistoryMsgToIntroModalScreen(msg1);
            //    };
            //    Debug.Log(i + "Continue");
            //}
            //else
            //{
            //    msg._positiveButtonTxt = "Continue";
            //    msg._positiveAction = () =>
            //    {
            //        _introductionManager.DestroyModalScreen();
            //        PushHistoryMsgToIntroModalScreen(_listOfHistoryMessages[i + 1]);
            //
            //        SceneManager.UnloadSceneAsync("Scene01_Introduction");
            //        SceneManager.LoadScene("Scene02_Game");
            //    };
            //    Debug.Log(i + "Load Scene");
            //}
            //...
            #endregion

            _listOfHistoryMessages[i] = msg;
        }
        //### For some reason using for loop to assign button call backs in list is giving out of range error...
        //### Thus, it needs to be done manually...

        //### Found the reason after some debugging. It's that when we give var msg1 = _listOfHistoryMessages[i + 1]; in a button click call back, 
        //It tracks back to the value of variables. Unfortunately, i's latest value becomes 4 during the loop cycle and hence when ever we click button, 
        //it shows out of range exception as i + 1 will be 4 + 1 which is 5, and there is nothing in 5th position of the list. only 0 - 4 is filled in list.
        //Therefore, I will be trying to make a way to remember the positions of the list values, despite of loop cycle.
        //Maybe manually assign 5 varibales separately out of loop which can hold numbers 0 to 4 as constants.
        //Then use them to do _listOfHistoryMessages[constant value].
        //### Gonna try this on the top commented first loop part.
        //### --> RESULT - FAILED TO IMPLEMENT - I tried with constant variables, but eventually if i have to use them to locate the correct value of, 
        //the list, I need to know which item is it looping with. Then to match it with constant I have to check all the 5 constants, which is again a 
        //manual work. Thus, commented it now and lets keep this and improve for future upgrades.
        //### Fow now let's use the manual one done below for the button callbacks.

        //var msg00 = new IntroductionManager.IntroModal();
        //var msg01 = new IntroductionManager.IntroModal();
        //var msg02 = new IntroductionManager.IntroModal();
        //var msg03 = new IntroductionManager.IntroModal();
        //var msg04 = new IntroductionManager.IntroModal();
        var msg00 = _listOfHistoryMessages[0];
        var msg01 = _listOfHistoryMessages[1];
        var msg02 = _listOfHistoryMessages[2];
        //var msg03 = _listOfHistoryMessages[3];
        //var msg04 = _listOfHistoryMessages[4];

        msg00._negativeButtonTxt = "Can't Go Back!";
        msg00._negativeAction = () =>
        {
            //Nothing!
        };
        msg00._positiveButtonTxt = "Continue";
        msg00._positiveAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            PushHistoryMsgToIntroModalScreen(msg01);
        };

        msg01._negativeButtonTxt = "Back";
        msg01._negativeAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            PushHistoryMsgToIntroModalScreen(msg00);
        };
        msg01._positiveButtonTxt = "Continue";
        msg01._positiveAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            PushHistoryMsgToIntroModalScreen(msg02);
        };

        msg02._negativeButtonTxt = "Back";
        msg02._negativeAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            PushHistoryMsgToIntroModalScreen(msg01);
        };
        msg02._positiveButtonTxt = "Next";
        msg02._positiveAction = () =>
        {
            _introductionManager.DestroyModalScreen();
            //PushHistoryMsgToIntroModalScreen(msg03);

            _isHistoryMessageComplete = true;

            _plotMessageManager.SetHistoryMessageComplete(_isHistoryMessageComplete);
        };

        //msg03._negativeButtonTxt = "Back";
        //msg03._negativeAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushHistoryMsgToIntroModalScreen(msg02);
        //};
        //msg03._positiveButtonTxt = "Continue";
        //msg03._positiveAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushHistoryMsgToIntroModalScreen(msg04);
        //};

        //msg04._negativeButtonTxt = "Back";
        //msg04._negativeAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();
        //    PushHistoryMsgToIntroModalScreen(msg03);
        //};
        //msg04._positiveButtonTxt = "Next";
        //msg04._positiveAction = () =>
        //{
        //    _introductionManager.DestroyModalScreen();

        //    _isHistoryMessageComplete = true;

        //    _plotMessageManager.SetHistoryMessageComplete(_isHistoryMessageComplete);
        //};


        _listOfHistoryMessages[0] = msg00;
        _listOfHistoryMessages[1] = msg01;
        _listOfHistoryMessages[2] = msg02;
        //_listOfHistoryMessages[3] = msg03;
        //_listOfHistoryMessages[4] = msg04;

        _hasHistoryMessageLoaded = true;
    }

    void PushHistoryMsgToIntroModalScreen(IntroductionManager.IntroModal modal)
    {
        _introductionManager.BuildIntroModalScreenWithMsg(modal);
    }
}