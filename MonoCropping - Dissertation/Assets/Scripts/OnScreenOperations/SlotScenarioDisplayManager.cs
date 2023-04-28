using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotScenarioDisplayManager : MonoBehaviour
{
    public void SetSlotName(string name) { _slotName = name; }

    private string _slotName = "";
    private string _partOfDay = "";
    private bool _isPlayerWarned = false;

    [SerializeField] TextMeshProUGUI _informationTextBox;
    [SerializeField] PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InformationDisplay());
    }

    // Update is called once per frame
    void Update()
    {
        _partOfDay = TimeManager.Instance.GetCurrentPartOfTheDay();
        _isPlayerWarned = _playerController.GetWarnedStatusOfPlayer();
    }

    IEnumerator InformationDisplay()
    {
        while (true)
        {
            _informationTextBox.text = _slotName;

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = _partOfDay;

            yield return new WaitForSeconds(4f);

            if (_isPlayerWarned)
            {
                _informationTextBox.text = "You are in warning period by Corp Industry!";

                yield return new WaitForSeconds(4f);
            }
            else
            {
                _informationTextBox.text = "No warnings by Corp Industry!";

                yield return new WaitForSeconds(4f);
            }

            _informationTextBox.text = "Reach target food count to avoid decrease in community population!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Reach safe food count to increase community population count!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "If inventory is full, store crops in barn. But only edibe items count towards food for community!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "If watercan is empty, refil from the well!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Check out the shop for items to buy!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Grains and Seeds storage generates seeds for the edible crops that are present in barn!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Go to shop to buy tools and equipments for farming!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Try Greenhouse! Its worth checking!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "When you get caught, you will be warned and you can't access corp indus land for sometime!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "When you stand over a land, you can see left side HUD to check if it is infected or is infecting!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Try to Destroy(if cultivated) and Restore any land that is spreading monocropping!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Turn On/Off HUD Info, Audio from Pause Menu!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Game saves automatically post completion of 1st slot/scenario!";

            yield return new WaitForSeconds(4f);

            _informationTextBox.text = "Rain? Wait till wet season! That is - 2nd Slot!";

            yield return new WaitForSeconds(4f);
        }
    }
}