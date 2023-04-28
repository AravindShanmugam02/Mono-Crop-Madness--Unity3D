using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditManager : MonoBehaviour
{
    public float GetCurrentCredits() { return _credits; }
    public void AddAmountToCredits(float amount) { AddAmountToCurrentCredits(amount); }
    public void RemoveAmountFromCredits(float amount) { RemoveAmountFromCurrentCredits(amount); }
    public void SetGameTypeToLoad(GameManager.GameType gameType) { _gameType = gameType; }

    //To get what type of game to load...
    GameManager.GameType _gameType;

    [SerializeField] private float _credits;
    private TextMeshProUGUI _creditText;

    public static CreditManager Instance { get; private set; }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameType = GameManager.Instance.GetGameTypeToLoad();
        LoadCreditsAccordingToGameType();
        _creditText = GameObject.Find("CreditText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _creditText.text = _credits.ToString();
    }

    void LoadCreditsAccordingToGameType()
    {
        if(_gameType == GameManager.GameType.NewGame)
        {
            _credits = 20f;
        }
        else if(_gameType == GameManager.GameType.LoadGame)
        {
            _credits = GameManager.Instance.GetPlayerCredits();
        }
    }

    private void AddAmountToCurrentCredits(float amount)
    {
        _credits += amount;
    }

    private void RemoveAmountFromCurrentCredits(float amount)
    {
        _credits -= amount;

        if(_credits <= 0f)
        {
            _credits = 0f;
        }
    }
}
