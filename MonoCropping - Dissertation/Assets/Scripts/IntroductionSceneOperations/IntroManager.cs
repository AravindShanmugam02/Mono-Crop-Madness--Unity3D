using System;
using System.Resources;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _greetingHeadText;
    [SerializeField] private TextMeshProUGUI _greetingBodyText;
    [SerializeField] private TextMeshProUGUI _historyHeadText;
    [SerializeField] private TextMeshProUGUI _historyBodyText;
    [SerializeField] private TextMeshProUGUI _plotHeadText;
    [SerializeField] private TextMeshProUGUI _plotBodyText;
    [SerializeField] private TextMeshProUGUI _controlHeadText;
    [SerializeField] private TextMeshProUGUI _controlBodyText;

    [SerializeField] private Button _backButton;
    [SerializeField] private Button _proceedButton;

    // Start is called before the first frame update
    void Start()
    {
        _greetingHeadText.text = "Welcome To Monocrop Madness!";
        _historyHeadText.text = "A quick look into the past...";
        _plotHeadText.text = "What are you doing in this game...";
        _controlHeadText.text = "Basic Instructions To Play...";

        _greetingBodyText.text = "";
        _historyBodyText.text = "";
        _plotBodyText.text = "";
        _controlBodyText.text = "";

        //_greetingBodyText.text += "\n" + File.ReadAllText("Assets/Resources/InputFiles/GreetingsMessage.txt") + "\n"; --> DOESN'T WORK IN BUILD.

        //_greetingBodyText.text = "\n" + Resources.Load("/InputFiles/GreetingsMessage.txt") + "\n"; --> WRONG IMPLEMENTATION.
        //TextAsset txt = Resources.Load("/InputFiles/GreetingsMessage.txt") as TextAsset; --> WRONG IMPLEMENTATION.

        //https://stackoverflow.com/questions/36116367/resources-load-returning-null
        //https://docs.unity3d.com/ScriptReference/Resources.Load.html
        //Resources.Load(string path) --> the path shouldn't have extentions to the targetted file.
        //Also there should not be any slash at the beginning of the path as it is understood by the system that the path is already inside Resources folder.
        TextAsset txt = Resources.Load("InputFiles/GreetingsMessage") as TextAsset;

        if (txt != null)
        {
            _greetingBodyText.text = "\n" + txt.ToString() + "\n";
        }
        else
        {
            Debug.Log("txt is null for GreetingsMessage!");
        }

        for (int i = 0; i < 3; i++)
        {
            //_historyBodyText.text += "\n" + File.ReadAllText("Assets/Resources/InputFiles/HistoryMessage0" + i + ".txt") + "\n"; --> DOESN'T WORK IN BUILD.

            _historyBodyText.text += "\n";
            txt = Resources.Load("InputFiles/HistoryMessage0" + i) as TextAsset;

            if(txt != null)
            {
                _historyBodyText.text += txt.ToString();
            }
            else
            {
                Debug.Log("txt is null for HistoryMessage0" + i);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            //_plotBodyText.text += "\n" + File.ReadAllText("Assets/Resources/InputFiles/PlotMessage0" + i + ".txt") + "\n"; --> DOESN'T WORK IN BUILD.

            _plotBodyText.text += "\n";
            txt = Resources.Load("InputFiles/PlotMessage0" + i) as TextAsset;

            if (txt != null)
            {
                _plotBodyText.text += txt.ToString();
            }
            else
            {
                Debug.Log("txt is null for PlotMessage0" + i);
            }
        }

        for (int i = 0; i < 1; i++)
        {
            //_controlBodyText.text += "\n" + File.ReadAllText("Assets/Resources/InputFiles/ControlMessage0" + i + ".txt") + "\n"; --> DOESN'T WORK IN BUILD.

            _controlBodyText.text += "\n";
            txt = Resources.Load("InputFiles/ControlMessage0" + i) as TextAsset;

            if (txt != null)
            {
                _controlBodyText.text += txt.ToString();
            }
            else
            {
                Debug.Log("txt is null for ControlMessage0" + i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBackButtonClick()
    {
        SceneManager.UnloadSceneAsync("Scene01_Intro");
        SceneManager.LoadScene("Scene00_MainMenu");
    }

    public void OnProceedButtonClick()
    {
        SceneManager.UnloadSceneAsync("Scene01_Intro");
        SceneManager.LoadScene("Scene02_Game");
    }
}
