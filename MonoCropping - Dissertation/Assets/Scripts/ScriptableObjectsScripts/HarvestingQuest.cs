using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHarvestingQuest", menuName = "Quest/HarvestingQuest")]
public class HarvestingQuest : QuestTemplate
{
    private QuestType _questType = QuestType.Harvesting;
}
