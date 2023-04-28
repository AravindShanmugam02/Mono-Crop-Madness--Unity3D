using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class Links: InteractOptionsManager
//Interface Links: InteractInfo
//GameObjects Links:

public class CameraController : MonoBehaviour, iHUDInfo
{
    public Vector3 GetCameraSpawnLocation() { return _cameraSpwanLocation; }
    public void SetPlayerSleepingStatus(bool toggle) { _isPlayerSleeping = toggle; }

    [Header("Camera")]
    [SerializeField] private float rotationSpeed = 3.0f;
    [SerializeField] private float distanceFromTarget = 0.7f;
    [SerializeField] private float OffsetToRight = 0.5f;
    //[SerializeField]
    private Transform _target;
    //[SerializeField]
    private GameObject _crosshair;
    [SerializeField] private float _smoothTime = 0.1f;
    [SerializeField] private Vector2 _rotationXLimits = new Vector2(0, 45);

    //This will only be used by the SmoothDamp function for storing smooth velocity.
    private Vector3 _smoothDampVelocity = Vector3.zero;
    private float _rotationX = 0, _rotationY = 0;
    private Vector3 _newRotation, _currentRotation, _currentPosition, _newPosition, _cameraSpwanLocation;

    [Header("Interactions")]
    //Bools to see if input can be accepted...
    [SerializeField] private bool _isInteracting;
    [SerializeField] private bool _isBarnOpen;
    [SerializeField] private bool _isGrainsAndSeedsStorageOpen;
    [SerializeField] private bool _isGreenhouseOpen;
    [SerializeField] private bool _isLandDetailsViewed;
    [SerializeField] private bool _isShopOpen;
    [SerializeField] private bool _isInventoryOpen;
    [SerializeField] private bool _isModalScreenOpen;
    [SerializeField] private bool _isFeedbackActive;
    //[SerializeField] private bool _isPaused;

    //Handle for Player Controller...
    private PlayerController _playerController;

    //Handle for Mouse Manager...
    private MouseManager _mouseManager;

    //Bool to hold player caught by corp indus value...
    private bool _hasPlayerBeenCaught = false;
    private bool _isPlayerSleeping = false;

    // Start is called before the first frame update
    void Start()
    {
        //_inventoryManager = GameObject.Find("InventoryManager");
        _target = GameObject.Find("CamTarget").transform;
        _crosshair = GameObject.Find("CrosshairImage");
        _cameraSpwanLocation = GameObject.Find("CameraSpawnLocation").transform.position;
        //_interactOptionsManager = GameObject.Find("InteractOptionsManager");

        _playerController = GameObject.Find("Lydia").GetComponent<PlayerController>();
        _mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();

        if(_playerController == null || _mouseManager == null || _target == null || _crosshair == null || _cameraSpwanLocation == null)
        {
            Debug.LogError("EIther _playerController or _mouseManager or _target or _crosshair or _cameraSpwanLocation or all of them might be null. Check them in Camera Controller!");
        }

        _isInteracting = false;
        _isBarnOpen = false;
        _isGrainsAndSeedsStorageOpen = false;
        _isGreenhouseOpen = false;
        _isLandDetailsViewed = false;
        _isShopOpen = false;
        _isInventoryOpen = false;
        _isModalScreenOpen = false;
        _isFeedbackActive = false;
        //_isPaused = false;

        transform.position = new Vector3(_cameraSpwanLocation.x, _cameraSpwanLocation.y, _cameraSpwanLocation.z);

        HUDManager.Instance.AddAsIHUDListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        _hasPlayerBeenCaught = _playerController.GetCaughtByCorpIndus();
    }

    private void FixedUpdate()
    {
        //When Nothing Interacting!
        if (!_isInteracting && !_isBarnOpen && !_isGrainsAndSeedsStorageOpen && !_isGreenhouseOpen && !_isLandDetailsViewed && 
            !_isShopOpen && !_isInventoryOpen /*&& !_isModalScreenOpen && !_isFeedbackActive && !_isPaused*/
            && !_hasPlayerBeenCaught && !_isPlayerSleeping)
        {
            //Cursor.visible = false;
            //_mouseManager.ToggleMouseLock(true); //This will set mouse cursor visibility to false in its function.

            UpdateCameraControl();
        }
        else
        {
            //Cursor.visible = true;
            //_mouseManager.ToggleMouseLock(false); //This will set mouse cursor visibility to true in its function.
        }
    }

    void UpdateCameraControl()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        _rotationY += mouseX;
        _rotationX += mouseY;

        //Apply clamping for x axis rotation
        _rotationX = Mathf.Clamp(_rotationX, _rotationXLimits.x, _rotationXLimits.y);

        //Getting new rotation values from mouse axis.
        _newRotation = new Vector3(_rotationX, _rotationY, 0.0f);

        //Apply damping between rotation changes
        _currentRotation = Vector3.SmoothDamp(_currentRotation, _newRotation, ref _smoothDampVelocity, _smoothTime);

        //For updating the rotation of camera.
        transform.localEulerAngles = _currentRotation;

        //Subtract forward vector of the GameObject from the Target's position to point the GameObject's forward vector to the Target.
        //For updating the position of camera according to it's rotation so that it focuses on Target.

        _newPosition = _target.position - (transform.forward * distanceFromTarget) + (transform.right * OffsetToRight);

        _currentPosition = transform.position;

        //transform.position = /*_currentPosition;*/ _target.position - (transform.forward * distanceFromTarget) + (transform.right * OffsetToRight);
        transform.position = Vector3.Lerp(_currentPosition, _newPosition, 0.35f);
    }

    public void HUDInfoUpdate(bool isInteracting, bool isBarnOpen, bool isGrainsAndSeedsStorageOpen, bool isGreenhouseOpen,
        bool isLandDetailsViewed, bool isShopOpen, bool isInventoryOpen, bool isModalScreenOpen, bool isFeedbackActive, bool isPaused)
    {
        _isInteracting = isInteracting;
        _isBarnOpen = isBarnOpen;
        _isGrainsAndSeedsStorageOpen = isGrainsAndSeedsStorageOpen;
        _isGreenhouseOpen = isGreenhouseOpen;
        _isLandDetailsViewed = isLandDetailsViewed;
        _isShopOpen = isShopOpen;
        _isInventoryOpen = isInventoryOpen;
        _isModalScreenOpen = isModalScreenOpen;
        _isFeedbackActive = isFeedbackActive;
        //_isPaused = isPaused;
    }
}