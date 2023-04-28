using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantingQuest", menuName = "Quest/PlantingQuest")]
public class PlantingQuest : QuestTemplate
{
    private QuestType _questType = QuestType.Planting;
}
