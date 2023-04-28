using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    private GameObject _crosshair;
    private RaycastHit _outHit;

    [SerializeField]
    private float _raycastDist = 1.0f;

    public RaycastHit GetHitInfo() { return _outHit; }

    //To make this class a singleton monobehaviour class, we don't want multiple instances of this class.
    public static CrosshairManager Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        _crosshair = GameObject.Find("CrosshairImage").gameObject;
        if(_crosshair == null)
        {
            Debug.LogError("_crosshair not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit _hit;

//https://www.reddit.com/r/Unity3D/comments/6kv36u/raycast_from_ui_image/
//Casts a ray from camera to screen UI element.
        Ray _ray = Camera.main.ScreenPointToRay(_crosshair.transform.position);

//https://docs.unity3d.com/ScriptReference/Camera.ScreenPointToRay.html
//Checks whether a ray from the screen UI element found by ray from camera, hits anything in _ray direction (usually forward).
        if (Physics.Raycast(_ray.origin, _ray.direction /*Vector3.forward*/, out _hit, _raycastDist))
        {
            _outHit = _hit;

            if (_outHit.transform != null)
            {
                //Debug.Log("Raycast :: " + _outHit.collider.name);
            }
        }
        else
        {
            _outHit = _hit;
        }
    }
}