using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=Vt8aZDPzRjI&ab_channel=iHeartGameDev

[System.Serializable]
public class CorpIndusPatrolStateInteractSubState : CorpIndusAbstractPatrolSubState
{
    public void SetWarningEventComplete(bool toggle) { _isWarningEventComplete = toggle; }

    private PlayerController _playerController;
    [SerializeField] private bool _isWarningEventComplete = false;

    public override void EnterSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusPatrolState corpIndusPatrolState)
    {
        //Debug.Log("Hi From Interaction Sub State");

        TipManager.Instance.ConstructATip("You are caught by Corp Industry people for sneaking into their lands to fix their farming pattern!", true);

        _playerController = corpIndusPatrolState.GetPlayerController();

        _isWarningEventComplete = false;
    }

    public override void SubStateCollisionEnter(CorpIndusPatrolState corpIndusPatrolState, Collision collision)
    {

    }

    public override void UpdateSubState(CorpIndusStateManager corpIndusStateManager, CorpIndusPatrolState corpIndusPatrolState)
    {
        if(!_isWarningEventComplete)
        {
            _playerController.SetCaughtByCorpIndus(true);

            TipManager.Instance.ConstructATip("Press 'Y' to escape if caught by Corporate Industry People!", false);

            GetInput();
        }
        else if (_isWarningEventComplete)
        {
            //Send player back to the spawing place with warning in place.
            _playerController.transform.position = new Vector3(_playerController.GetPlayerSpawnPosition().x, _playerController.transform.position.y, _playerController.GetPlayerSpawnPosition().z);
            #region Issue description and its fix - Can't move Character Controller with transform.position

            /*CharacterController overrides object's position when teleporting with Transform.Position

            Physics - Mar 08, 2019

            How to reproduce:
            1.Open attached project("CharacterControllerTransform.zip")
            2.Open SampleScene
            3.Play
            4.Use WASD to move and Space to transform to(0,0,0)

            Expected result: Character transforms to(0, 0, 0).
            Actual result: Character sometimes stays in the position it was.

            Reproducible with -2017.4.24f1, 2018.3.10f1, 2019.1.0b8, 2019.2.0a9

            Note: only reproducible in the Editor.

            Resolution Note:

                The problem here is that auto sync transforms is disabled in the physics settings, so characterController.Move() won't necessarily be aware of the new pose as set by the transform unless a FixedUpdate or Physics.Simulate() called happened in-between transform.position and CC.Move().

                To fix that, either enable auto sync transforms in the physics settings, or sync manually via Physics.SyncTransforms right before calling Move().*/

            #endregion
            //https://forum.unity.com/threads/transform-position-does-not-work.943026/#post-6155856
            //https://issuetracker.unity3d.com/issues/charactercontroller-overrides-objects-position-when-teleporting-with-transform-dot-position

            _playerController.SetCaughtByCorpIndus(false);

            corpIndusPatrolState.SetInteractionComplete(true);

            _playerController.SetWarnedStatusOfPlayer(true);
            //As soon as the warned status is set to player, need to set a cooldown system for the warned status to become false.
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _isWarningEventComplete = true;
        }
    }
}