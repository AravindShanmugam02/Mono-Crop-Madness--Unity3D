using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpIndusInteractor : MonoBehaviour
{
    public RaycastHit GetHitObjectInfoFromCorpIndusInteractor() { return _hit; }
    public bool GetIsPlayerHit() { return _isPlayerhit; }
    public PlayerController GetPlayerController() { return _playerController; }

    [Header("Raycast")]
    [SerializeField] private float _rayCastDistance;
    [SerializeField] private Vector3 _interactorPosition;
    [SerializeField] private Vector3 _transformForward;
    [SerializeField] private Vector3 _rayDirection;
    [SerializeField] private float _fieldOfViewAngle;
    [SerializeField] private float _angle;
    [SerializeField] private bool _isPlayerhit;
    PlayerController _playerController;
    RaycastHit _hit;

    // Start is called before the first frame update
    void Start()
    {
        _rayCastDistance = 3.0f;
        _playerController = GameObject.Find("Lydia").GetComponent<PlayerController>();
        _fieldOfViewAngle = 135f;
        _isPlayerhit = false;
    }

    // Update is called once per frame
    void Update()
    {
        #region REFERENCE FROM FORUM TO IMPLEMENT FIELD-OF-VIEW FOR AI

        #region REFERENCE FROM FORUM TO IMPLEMENT FIELD-OF-VIEW FOR AI IN CONE SHAPE...
        //https://forum.unity.com/threads/raycasting-a-cone-instead-of-single-ray.39426/#post-320106
        //from [url]http://answers.unity3d.com/questions/8453/field-of-view-using-raycasting[/url]
        /*function CanSeePlayer() : boolean{

            var hit : RaycastHit;
            var rayDirection = ObjectToSee.transform.position - transform.position;

            // If the ObjectToSee is close to this object and is in front of it, then return true
            if ((Vector3.Angle(rayDirection, transform.forward)) < 90 
                    && (Vector3.Distance(transform.position, ObjectToSee.transform.position) <= CloseDistanceRange))
            {
                //Debug.Log("close");
                return true;
            }


            //Debug.Log("rayDirection - transform.forward =" + Vector3.Angle(rayDirection, transform.forward));



            if ((Vector3.Angle(rayDirection, transform.forward)) < fieldOfViewRangeInHalf)
            { // Detect if player is within the field of view
              //Debug.Log("within field of view");


                if (Physics.Raycast(transform.position, rayDirection, hit, HowFarCanISee))
                {

                    if (hit.collider.gameObject == ObjectToSee)
                    {
                        //Debug.Log("Can see player");
                        return true;
                    }
                    else
                    {
                        //Debug.Log("Can not see player");
                        return false;
                    }
                }
            }
        }*/
        #endregion

        #region REFERENCE FROM FORUM TO IMPLEMENT FIELD-OF-VIEW FOR AI IN CONE SHAPE WITH HEIGHT CALCULATION FOR UNEVEN TERRAIN SCENARIOS...
        //https://forum.unity.com/threads/raycasting-a-cone-instead-of-single-ray.39426/#post-2410665
        /*from http://forum.unity3d.com/threads/raycasting-a-cone-instead-of-single-ray.39426/
        bool CanSeeMonster(GameObject target)
        {
            float heightOfPlayer = 1.5f;

            Vector3 startVec = transform.position;
            startVec.y += heightOfPlayer;
            Vector3 startVecFwd = transform.forward;
            startVecFwd.y += heightOfPlayer;

            RaycastHit hit;
            Vector3 rayDirection = target.transform.position - startVec;

            // If the ObjectToSee is close to this object and is in front of it, then return true
            if ((Vector3.Angle(rayDirection, startVecFwd)) < 110 &&
                (Vector3.Distance(startVec, target.transform.position) <= 20f))
            {
                //Debug.Log("close");
                return true;
            }
            if ((Vector3.Angle(rayDirection, startVecFwd)) < 90 &&
                Physics.Raycast(startVec, rayDirection, out hit, 100f))
            { // Detect if player is within the field of view

                if (hit.collider.gameObject == target)
                {
                    //Debug.Log("Can see player");
                    return true;
                }
                else
                {
                    //Debug.Log("Can not see player");
                    return false;
                }
            }
            return false;
        }*/
        #endregion

        #region REFERENCE FROM FORUM TO IMPLEMENT FIELD-OF-VIEW FOR AI IN CONE SHAPE USING DOT PRODUCT...

        //https://forum.unity.com/threads/raycasting-a-cone-instead-of-single-ray.39426/#post-253511
        /*There's an easy way to get a cone using the dot product.
         * Get a vector pointing from the player to a potential target using their positions, and normalise that vector:-
        
        Code (csharp):
            var heading: Vector3 = (target.transform.position - transform.position).normalized;

        Then, get the dot product of this heading vector with the vector representing the player's direction of sight (this should also be normalised):-
        
        Code (csharp):
            var dot: float = Vector3.Dot(sightVec, heading);

        The dot product of two normalised vectors is equal to the cosine of the angle between them 
        (ie, it will be one when the directions are the same, zero when they are at 90º and 0.5 when they are at 60º).
        You can convert this to an angle if you want particular accuracy, but for most purposes, 
        you can just play with a value between 0 and 1 until you get good results. Basically, the check is whether 
        the dot is greater than your threshold value. Since the angle could be in any direction, up/down, left/right, 
        you effectively get a cone of sight this way.
         */

        //https://forum.unity.com/threads/raycasting-a-cone-instead-of-single-ray.39426/#post-254923
        /*Sorry - reading that again, I see I didn't make it very clear what I meant.
         * You can use the dot product thing as a quick test to see if the other object is roughly within the cone.
         * Once you've established that it is, you can use a raycast for a line of sight check.
         * Incidentally, you can also use the result of the dot product as a probability of spotting the player
         * (ie, 1 when straight ahead, 0 when at the side). */

        #endregion

        #endregion

        _transformForward = transform.TransformDirection(Vector3.forward);
        _interactorPosition = transform.position;
        _rayDirection = _playerController.GetPlayerPosition() - _interactorPosition; //To find direction from point A to point B we do B-A.

        //Just setting the height for the ray as it casted towards the exact position of the player, in which playerPosition.y is 0.
        //As a result the ray falls on the ground level all the time.
        //So, This is more likely implementing with height calculation.
        _rayDirection.y += 1f;

        if ((Vector3.Angle(_rayDirection, _transformForward)) <= (_fieldOfViewAngle / 2f))
        {
            DoRayCast(true);
        }
        else
        {
            DoRayCast(false);
        }

        _angle = Vector3.Angle(_rayDirection, _transformForward);
        Debug.DrawRay(_interactorPosition, _rayDirection, Color.red);
    }

    void DoRayCast(bool isInAngle)
    {
        RaycastHit hit;

        if (isInAngle)
        {
            if (Physics.Raycast(_interactorPosition, _rayDirection, out hit, _rayCastDistance))
            {
                if (hit.collider.tag == "Player")
                {
                    //Player hit is true only here. Because other branches of if else don't satisfy the requirements.
                    //In short, it is true only when player is within the angle defined above
                    //and is hit by the ray (defined with _rayCastDistance) casted by AI.
                    _isPlayerhit = true;

                    Debug.DrawRay(_interactorPosition, _rayDirection * hit.distance, Color.black);
                    //Black is when player is within the defined angle and is hit by the ray.
                }
                else
                {
                    _isPlayerhit = false;

                    Debug.DrawRay(_interactorPosition, _rayDirection * hit.distance, Color.green);
                    //Green is when an object is within the defined angle and is hit by ray. It is not player.
                }
            }
            else
            {
                _isPlayerhit = false;

                //White is when player is within the defined angle and not hit by the ray.
                Debug.DrawRay(_interactorPosition, _transformForward * _rayCastDistance, Color.white);
                //transform.TransformDirection(Vector3.forward) * _hit.distance ---> 1. Get direction of the ray, which is forward. So, transform.TransformDirection(Vector3.forward)
                //                                                              ---> 2. Now, since Vector.forward is a unit vector, we need the exact distance to draw the ray.
                //                                                              ---> 3. Therefore, by multiplying the _hit.distance with unit vector (i.e Vector.forward) gives us the exact distance and direction of the ray.
            }
        }
        else
        {
            if (Physics.Raycast(_interactorPosition, _rayDirection, out hit, _rayCastDistance))
            {
                if (hit.collider.tag == "Player")
                {
                    _isPlayerhit = false;

                    //Blue is when player is not inside the defined angle and is hit by the ray.
                    Debug.DrawRay(_interactorPosition, _rayDirection * hit.distance, Color.blue);
                }
                else
                {
                    _isPlayerhit = false;

                    Debug.DrawRay(_interactorPosition, _rayDirection * hit.distance, Color.cyan);
                    //Cyan is when an object is not inside the defined angle and is hit by ray. It is not player.
                }
            }
            else
            {
                _isPlayerhit = false;

                //Yellow is when player is not inside the defined angle and not hit by the ray.
                Debug.DrawRay(_interactorPosition, _transformForward * _rayCastDistance, Color.yellow);
            }
        }
        
        _hit = hit;
    }
}