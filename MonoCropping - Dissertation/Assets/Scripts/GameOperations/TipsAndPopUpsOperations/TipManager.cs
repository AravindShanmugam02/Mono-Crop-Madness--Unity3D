using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class TipManager : MonoBehaviour
{
    [SerializeField] private GameObject _tipBoxPrefab;
    private Transform _tipPanel;
    private Transform _tipBoxContainer;
    //private Scrollbar _tipScroll;

    private GameObject _previousTipBox;

    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static TipManager Instance { get; private set; }

    //Making this class to be a singleton monobehaviour class - only one instace will be created for this class.
    private void Awake()
    {
        //if there is more than one instance, destroy the extras.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static Instance to this Instance.
            Instance = this;
        }

        //_tipBoxPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/2D/UI/TipBox.prefab", typeof(GameObject)) as GameObject;
        _tipPanel = GameObject.Find("TipPanel").GetComponent<Transform>();
        _tipBoxContainer = GameObject.Find("TipBoxContentBox").GetComponent<Transform>();
        //_tipScroll = GameObject.Find("TipBoxVerticalScroll").GetComponent<Scrollbar>();

        _tipPanel.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _previousTipBox = null;

        if (_tipBoxPrefab == null || _tipPanel == null || _tipBoxContainer == null/* || _tipScroll == null*/)
        {
            Debug.LogError("Error in TipManager. Items not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_tipBoxContainer.transform.childCount <= 0)
        {
            _tipPanel.gameObject.SetActive(false);
        }
    }

    public void ConstructATip(string message, bool isUrgent)
    {
        StartCoroutine(BuildATip(message, isUrgent));
    }

    public IEnumerator BuildATip(string message, bool isUrgent)
    {
        if (isUrgent)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(4f);
        }

        //to make way for the new pop up, we destroy the previous one!
        if(_previousTipBox != null)
        {
            Destroy(_previousTipBox);
        }

        _tipPanel.gameObject.SetActive(true);

        GameObject tip = Instantiate(_tipBoxPrefab, _tipBoxContainer);
        tip.GetComponent<Image>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 0.2f, 0.2f); //Random.ColorHSV(HueMin, HueMax, SaturationMin, SaturationMax, ValueMin, ValueMax, AlphaMin, AlphaMax)
        tip.GetComponentInChildren<TextMeshProUGUI>().text = message;

        _previousTipBox = tip;
    }
}
