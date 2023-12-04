using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
* Stores the persistent state data that is passed to the active concrete states.
* The data is used for their logic and switching between states.
*/
public class PlayerStateMachine : MonoBehaviour
{
    #region Reference Variables
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Animator _animator;
    Transform _playerObj;
    private Transform _orientation;
    private Coroutine _attackResetRoutine;

    private LockOnSystem _lockOnSystemScript;

    #endregion

    #region Player Movement Variables
    [Header("Player Movement Settings")]
    Vector3 _currentMovement;
    bool _isMovementPressed = false;
    [SerializeField] float _speed;
    private float _groundGravity;
    private float _rotationSpeed;

    #endregion

    #region Jump Variables
    [Header("Jump Setting")]
    [SerializeField] float _maxJumpHeight;
    [SerializeField] float _maxJumpTime;
    private float _timeToApex;
    private bool _isJumpPressed = false;
    private bool _requiresNewJumpPressed;
    private float _gravity;
    private float _initialJumpVelocity;

    #endregion

    #region Attack Variables
    private int _attackCount;
    private bool _isAttackPressed = false;
    private bool _requiresNewAttackPressed;
    private bool _isLockOnPressed = false;
    private bool _requiresNewLockOnPress;
    private Dictionary<int, float> _attackTimes = new Dictionary<int, float>();

    #endregion

    #region State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    #endregion

    #region Animator Hash References
    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isJumpingHash;
    private int _isAttackingHash;
    private int _attackCountHash;

    #endregion

    #region Getters and Setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; }}
    public PlayerInput PlayerInput { get { return _playerInput; }}
    public CharacterController CharacterController { get { return _characterController; }}
    public Animator Animator { get { return _animator; }}
    public Transform PlayerObj { get { return _playerObj; } set { _playerObj = value;}}
    public Transform Orientation { get { return _orientation; }}
    public Coroutine AttackResetRoutine { get { return _attackResetRoutine; } set { _attackResetRoutine = value; }}
    public float VerticalInput { get { return _currentMovement.z; } set { _currentMovement.z = value; }}
    public float HorizontalInput { get { return _currentMovement.x; } set { _currentMovement.x = value; }}
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; }}
    public bool IsMovementPressed { get { return _isMovementPressed; }}
    public bool IsJumpPressed { get { return _isJumpPressed; }}
    public bool RequiresNewJumpPressed { get { return _requiresNewJumpPressed; } set { _requiresNewJumpPressed = value; }}
    public float JumpHeight { get { return _maxJumpHeight; }}
    public float InitialJumpVelocity { get { return _initialJumpVelocity; }}
    public float Gravity { get { return _gravity; }}
    public float GroundGravity { get { return _groundGravity; }}
    public float Speed { get { return _speed; } set { _speed = value; }}
    public float RotationSpeed { get { return _rotationSpeed; } set { _rotationSpeed = value; }}
    public int AttackCount { get { return _attackCount; } set { _attackCount = value; }}
    public bool IsAttackPressed { get { return _isAttackPressed; }}
    public Dictionary<int, float> AttackTimes { get { return _attackTimes; }}
    public bool RequiresNewAttackPress { get { return _requiresNewAttackPressed; } set { _requiresNewAttackPressed = value; } }
    public int IsWalkingHash { get { return _isWalkingHash; }}
    public int IsRunningHash { get { return _isRunningHash; }}
    public int IsJumpingHash { get { return _isJumpingHash; }}
    public int IsAttackingHash { get { return _isAttackingHash; }}
    public int AttackCountHash { get { return _attackCountHash; }}

    #endregion

    void Awake()
    {
        //Setting reference variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerObj = GameObject.Find("Jammo_LowPoly").GetComponent<Transform>();
        _orientation = GameObject.Find("Orientation").GetComponent<Transform>();
        _attackResetRoutine = null;
        _lockOnSystemScript = GetComponent<LockOnSystem>();

        //Set up animation hash references
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _isAttackingHash = Animator.StringToHash("IsAttacking");
        _attackCountHash = Animator.StringToHash("AttackCount");

        //Storing player input callbacks
        _playerInput.PlayerControls.Move.started += OnMovementInput;
        _playerInput.PlayerControls.Move.canceled += OnMovementInput;
        _playerInput.PlayerControls.Move.performed += OnMovementInput;
        _playerInput.PlayerControls.Jump.started += OnJump;
        _playerInput.PlayerControls.Jump.canceled += OnJump;
        _playerInput.PlayerControls.Attack.started += OnAttack;
        _playerInput.PlayerControls.Attack.canceled += OnAttack;
        _playerInput.PlayerControls.LockOn.started += OnLockOnInput;
        _playerInput.PlayerControls.LockOn.canceled += OnLockOnInput;

        //Set up movement variables
        _rotationSpeed = 4f;
        _groundGravity = -0.5f;

        //Jump equations found at https://www.youtube.com/watch?v=h2r3_KjChf4
        _timeToApex = _maxJumpTime / 2;
        _gravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToApex, 2);
        _initialJumpVelocity = 2 * _maxJumpHeight / _timeToApex;

        //Set up attack timings
        _attackTimes[1] = 1f;
        _attackTimes[2] = 1.2f;
        _attackTimes[3] = 1.1f;

        //setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        if(_isLockOnPressed && !_requiresNewLockOnPress)
        {
            _lockOnSystemScript.ToggleLockOn();
            _requiresNewLockOnPress = true;
        }

        MovePlayerRelativeToCamera();
        RotatePlayer();
        _currentState.UpdateStates();
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        //GetAxis() seems to be a better option for smoother analog control using joystick
        _currentMovement.x = Input.GetAxis("Horizontal");
        _currentMovement.z = Input.GetAxis("Vertical");
        _isMovementPressed = _currentMovement.x != 0 || _currentMovement.z != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requiresNewJumpPressed = false;
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        _isAttackPressed = context.ReadValueAsButton();
        _requiresNewAttackPressed = false;
    }

    private void OnLockOnInput(InputAction.CallbackContext context)
    {
        _isLockOnPressed = context.ReadValueAsButton();
        _requiresNewLockOnPress = false;
    }

    void MovePlayerRelativeToCamera()
    {
        //Get Camera normalized directional vectors
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        //Create direction-relative input vectors
        Vector3 forwardRelativeVerticalInput = _currentMovement.z * forward;
        Vector3 rightRelativeHorizontalInput = _currentMovement.x * right;

        //Create and apply camera relative movement
        Vector3 cameraRelativeMovement = Vector3.ClampMagnitude(forwardRelativeVerticalInput + rightRelativeHorizontalInput, 1);
        cameraRelativeMovement.y = _currentMovement.y;
        _characterController.Move(cameraRelativeMovement * _speed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        //rotate orientation
        Vector3 viewDirection = transform.position - new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        _orientation.forward = viewDirection.normalized;

        //rotate player object
        Vector3 inputDirection = _orientation.forward * _currentMovement.z + _orientation.right * _currentMovement.x;

        if(inputDirection != Vector3.zero)
        {
            _playerObj.forward = Vector3.Slerp(_playerObj.forward, inputDirection.normalized, Time.deltaTime * _rotationSpeed);
        }
    }

    //Enable the Player Controls action map
    void OnEnable()
    {
        _playerInput.PlayerControls.Enable();
    }

    //Disable the Player Controls action map
    void OnDisable()
    {
        _playerInput.PlayerControls.Disable();
    }
}
