using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest")]
public class QuestTemplate : ScriptableObject
{
    protected enum QuestType
    {
        Harvesting,
        Planting,
        Finding,
        ShortGoal
    }

    [Header("Quest Properties")]
    public string _questName;
    public string _questDescription;
    public Sprite _questThumbnail;
    public int _questReward = 100;
    public int _questRequiredLevel = 3;
    //private bool _isQuestActive = false;
    //private bool _isQuestCompleted = false;
    //private QuestTemplate _activeQuest = null;
}
