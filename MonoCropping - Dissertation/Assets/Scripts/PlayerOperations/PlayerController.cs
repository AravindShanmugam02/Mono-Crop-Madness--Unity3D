using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ITimer, InventoryInfo, iHUDInfo
{
    public bool GetCaughtByCorpIndus() { return _isCaughtByCorpIndus; }
    public void SetCaughtByCorpIndus(bool toggle) { _isCaughtByCorpIndus = toggle; }
    public bool GetWarnedStatusOfPlayer() { return _isWarnedByCorpIndus; }
    public void SetWarnedStatusOfPlayer(bool toggle) { _isWarnedByCorpIndus = toggle; }
    public Vector3 GetPlayerPosition() { return transform.position; }
    public Vector3 GetPlayerSpawnPosition() { return _playerSpawnLocation; }
    public Vector3 GetPlayerSleepPoint() { return _playerSleepPoint; }
    public void SetPlayerSleepingStatus(bool toggle) { _isSleeping = toggle; }
    public void RenderPlayersHandNow() { RenderPlayerHandWithEquipedItems(); }

    private Animator _animator;
    private CharacterController _characterController;
    private float _horizontalInput, _verticalInput;
    [SerializeField] private Vector3 _movementInput, _direction, _jumpVelocity, _moveVelocity, _playerSpawnLocation, _playerSleepPoint;
    [SerializeField] private bool _isGrounded, _isMoving, _isRunning, _isSleeping;

    [Header("Movement")]
    [SerializeField] private float _playerSpeed = 0.01f;
    [SerializeField] private float _slerpRotationSpeed = 3.0f;
    [SerializeField] private float _jumpForce = 9.81f;

    [Header("World")]
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private Transform _thirdPersonCam;

    //Bools to see if input can be accepted...
    private bool _isInteracting;
    private bool _isBarnOpen;
    private bool _isGrainsAndSeedsStorageOpen;
    private bool _isGreenhouseOpen;
    private bool _isLandDetailsViewed;
    private bool _isShopOpen;
    private bool _isInventoryOpen;
    private bool _isModalScreenOpen;
    private bool _isFeedbackActive;
    //private bool _isPaused;

    //To Handle Caught situation...
    [SerializeField] private bool _isCaughtByCorpIndus = false;
    [SerializeField] private bool _isWarnedByCorpIndus = false;
    [SerializeField] private int _noOfTimesWarnedByCorpIndus = 0;

    //Toggle GameOver Status...
    //[SerializeField] private bool _isGameOver = false;

    //Rules of game..
    [SerializeField] private GameObject _rulesOfGamePanel;

    //Variables for equipment holding spots in left and right hand...
    [SerializeField] private GameObject _lHoldingSpot;
    [SerializeField] private GameObject _rHoldingSpot;

    //variables for storing equiped item data...
    private GameItemsSlotData _equipedItemSlotDataCS;
    private GameItemsSlotData _equipedItemSlotDataTE;

    //To handle the timer for player...
    private GameTimer _gameTimer;
    private GameTimer _ownPlayerTimerForWarning;
    private float _warningTimer;

    //To handle the movement update input...
    private bool _movementInputToggle;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _playerSpawnLocation = GameObject.Find("PlayerSpawnLocation").GetComponent<Transform>().position;
        _playerSleepPoint = GameObject.Find("PlayerSleepPoint").GetComponent<Transform>().position;

        _isMoving = false;
        _isRunning = false;
        _isSleeping = false;
        //_isGameOver = false;

        _equipedItemSlotDataCS = new GameItemsSlotData();
        _equipedItemSlotDataTE = new GameItemsSlotData();

        if (_lHoldingSpot == null || _rHoldingSpot == null)
        {
            Debug.LogError("Left or Right holding spot is missing in Player Controller!");
        }

        transform.position = new Vector3(_playerSpawnLocation.x, transform.position.y, _playerSpawnLocation.z);

        TimeManager.Instance.AddAsITimerListener(this);
        InventoryDisplayManager.Instance.AddAsInventoryInfoListener(this);
        HUDManager.Instance.AddAsIHUDListener(this);

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
    }

    // Update is called once per frame
    void Update()
    {
        if (_thirdPersonCam != null && 
            !_isInteracting && !_isBarnOpen && !_isGrainsAndSeedsStorageOpen && !_isGreenhouseOpen && !_isLandDetailsViewed && 
            !_isShopOpen && !_isInventoryOpen /*&& !_isModalScreenOpen && !_isFeedbackActive && !_isPaused*/ 
            && !_isCaughtByCorpIndus && !_isSleeping)
        {
            //Continue movement from updating...
            _movementInputToggle = true;
        }
        else
        {
            //Stop movement from updating...
            _movementInputToggle = false;
        }

        MovementUpdate();

        if (_isWarnedByCorpIndus && _ownPlayerTimerForWarning == null && _gameTimer != null)
        {
            SetTimerForPlayer();
        }
        else if(_isWarnedByCorpIndus && _ownPlayerTimerForWarning != null && _gameTimer != null)
        {
            UpdateTimerForPlayer();
        }

        if(_noOfTimesWarnedByCorpIndus >= 3)
        {
            //_isGameOver = true;
        }
    }

    void MovementUpdate()
    {
        if (_movementInputToggle)
        {
            //Moving Mechanism
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.T))
            {
                _rulesOfGamePanel.SetActive(true);
            }
            else
            {
                _rulesOfGamePanel.SetActive(false);
            }
        }
        else
        {
            _rulesOfGamePanel.SetActive(false);
            _horizontalInput = 0f;
            _verticalInput = 0f;
        }

        _movementInput = Quaternion.Euler(0.0f, _thirdPersonCam.transform.eulerAngles.y, 0.0f) * new Vector3(_horizontalInput, 0.0f, _verticalInput);
        _direction = _movementInput.normalized;
        _moveVelocity = _direction * _playerSpeed * Time.deltaTime;

        //__________________________________________________________________________________________

        //Jumping Mechanism
        //_isGrounded = _characterController.isGrounded;

        //if(_isGrounded)
        //{
        //    if(Input.GetButtonDown("Jump"))
        //    {
        //        _jumpVelocity.y += _jumpForce;
        //    }
        //}

        _characterController.Move((_moveVelocity + _jumpVelocity) * Time.deltaTime);

        _jumpVelocity.y += _gravityValue * Time.deltaTime;

        if (_jumpVelocity.y <= _gravityValue)
        {
            _jumpVelocity.y = _gravityValue;
        }

        //__________________________________________________________________________________________

        //Rotation Mechanics
        if (_direction != Vector3.zero)
        {
            Quaternion _rotationMovement = Quaternion.LookRotation(_direction, Vector3.up);
            //transform.rotation = _rotationMovement;
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotationMovement, _slerpRotationSpeed * Time.deltaTime);
        }

        //__________________________________________________________________________________________

        //Animation Machanism
        if (_moveVelocity != Vector3.zero)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }

        if(Input.GetButton("Sprint") && _isMoving == true)
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }

        _animator.SetBool("isMoving", _isMoving);
        _animator.SetBool("isRunning", _isRunning);
    }

    void SetTimerForPlayer()
    {
        _ownPlayerTimerForWarning = TimeManager.Instance.GetCurrentTimeStamp(_gameTimer);

        CheckWarnedCount();
    }

    void UpdateTimerForPlayer()
    {
        //Warning time is set to 2 hours...
        if(TimeManager.Instance.DifferenceInTimeUsingHours(_ownPlayerTimerForWarning, _gameTimer) >= 2) //High level of accuracy is not required for this, hence using hours difference
        {
            _warningTimer = TimeManager.Instance.DifferenceInTimeUsingHours(_ownPlayerTimerForWarning, _gameTimer);
            _isWarnedByCorpIndus = false;
            _ownPlayerTimerForWarning = null;
        }
    }

    void CheckWarnedCount()
    {
        if (_isWarnedByCorpIndus)
        {
            _noOfTimesWarnedByCorpIndus++;
        }
    }
     
    void RenderPlayerHandWithEquipedItems()
    {
        //Remove if any instantiated object is present after inventory slot changes...
        if (_lHoldingSpot.transform.childCount > 0) // LEFT
        {
            Destroy(_lHoldingSpot.transform.GetChild(0).gameObject);
        }

        //Remove if any instantiated object is present after inventory slot changes...
        if (_rHoldingSpot.transform.childCount > 0) // RIGHT
        {
            Destroy(_rHoldingSpot.transform.GetChild(0).gameObject);
        }

        //Render the item in hand of player that is in equiped slot of inventory...
        if (_equipedItemSlotDataCS.GetGameItemsData() != null) // CROPS & SEEDS
        {
            Instantiate(_equipedItemSlotDataCS.GetGameItemsData()._itemModel, _lHoldingSpot.transform);
        }

        //Render the item in hand of player that is in equiped slot of inventory...
        if (_equipedItemSlotDataTE.GetGameItemsData() != null) // TOOLS & EQUIPMENTS
        {
            Instantiate(_equipedItemSlotDataTE.GetGameItemsData()._itemModel, _rHoldingSpot.transform);
        }
    }

    public void TickUpdate(GameTimer gameTimer)
    {
        if(_gameTimer == null) { _gameTimer = gameTimer; }
    }

    public void InventoryInfoUpdate(GameItemsSlotData equipedItemSlotDataCS, GameItemsSlotData equipedItemSlotDataTE, GameItemsSlotData[] cropsSeedsSlotsData, GameItemsSlotData[] toolsEquipmentsSlotsData)
    {
        if(equipedItemSlotDataCS.GetGameItemsData() != null)
        {
            _equipedItemSlotDataCS = new GameItemsSlotData(equipedItemSlotDataCS);
        }
        else
        {
            _equipedItemSlotDataCS.EmptyIt(true);
        }

        if(equipedItemSlotDataTE.GetGameItemsData() != null)
        {
            _equipedItemSlotDataTE = new GameItemsSlotData(equipedItemSlotDataTE);
        }
        else
        {
            _equipedItemSlotDataTE.EmptyIt(true);
        }
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