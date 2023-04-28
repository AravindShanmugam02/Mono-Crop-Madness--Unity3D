using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static QuestManager Instance { get; private set; }

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
    }

    //Storage for the Quest data - Finding, Harvest, Planting, ShortGoal
    [Header("Quests")]
    [SerializeField]
    private List<QuestTemplate> _findingQuestsData = new List<QuestTemplate>();
    [SerializeField]
    private List<QuestTemplate> _harvestingQuestsData = new List<QuestTemplate>();
    [SerializeField]
    private List<QuestTemplate> _plantingQuestsData = new List<QuestTemplate>();
    [SerializeField]
    private List<QuestTemplate> _shortGoalQuestsData = new List<QuestTemplate>();

    //Storage for current quest
    private QuestTemplate _activeQuest = null;

    //Toggle quest
    private bool _isQuestActive = false;
    private bool _isQuestCompleted = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}