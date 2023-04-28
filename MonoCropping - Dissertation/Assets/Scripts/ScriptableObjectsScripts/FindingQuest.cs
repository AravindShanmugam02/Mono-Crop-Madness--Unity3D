using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFindingQuest", menuName = "Quest/FindingQuest")]
public class FindingQuest : QuestTemplate
{
    private QuestType _questType = QuestType.Finding;
}
