using System;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

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

    #endregion

    #region Player Movement Settings
    [Header("Player Movement Settings")]
    private Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    bool _isMovementPressed;
    [SerializeField] float _speed;
    private float _groundGravity;
    private float _rotationSpeed;

    #endregion

    #region Jump Settings
    [Header("Jump Setting")]
    [SerializeField] float _maxJumpHeight;
    [SerializeField] float _maxJumpTime;
    private float _timeToApex;
    private bool _isJumpPressed = false;
    private bool _requiresNewJumpPressed;
    private float _gravity;
    private float _initialJumpVelocity;

    #endregion

    #region State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    #endregion

    #region Animator Hash References
    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isJumpingHash;

    #endregion

    #region Getters and Setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; }}
    public CharacterController CharacterController { get { return _characterController; }}
    public Animator Animator { get { return _animator; }}
    public Transform PlayerObj { get {return _playerObj; } set { _playerObj = value;}}
    public Transform Orientation { get {return _orientation; }}
    public Vector2 CurrentMovementInput { get {return _currentMovement; }}
    public float VerticalInput { get {return _currentMovement.z; }}
    public float HorizontalInput { get {return _currentMovement.x; }}
    public float CurrentMovementY { get {return _currentMovement.y; } set { _currentMovement.y = value; }}
    public bool IsMovementPressed { get { return _isMovementPressed; }}
    public bool IsJumpPressed { get { return _isJumpPressed; }}
    public bool RequiresNewJumpPressed { get { return _requiresNewJumpPressed; } set { _requiresNewJumpPressed = value; }}
    public float JumpHeight { get { return _maxJumpHeight; }}
    public float InitialJumpVelocity { get { return _initialJumpVelocity; }}
    public float Gravity { get { return _gravity; }}
    public float GroundGravity { get { return _groundGravity; }}
    public float Speed { get { return _speed; } set { _speed = value; }}
    public float RotationSpeed { get {return _rotationSpeed; } set { _rotationSpeed = value; }}
    public int IsWalkingHash { get {return _isWalkingHash; }}
    public int IsRunningHash { get {return _isRunningHash; }}
    public int IsJumpingHash { get {return _isJumpingHash; }}

    #endregion

    void Awake()
    {
        //Setting reference variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerObj = GameObject.Find("Jammo_LowPoly").GetComponent<Transform>();
        _orientation = GameObject.Find("Orientation").GetComponent<Transform>();


        //Set up animation hash references
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");
        _isJumpingHash = Animator.StringToHash("IsJumping");

        //Storing player input callbacks
        _playerInput.PlayerControls.Move.started += OnMovementInput;
        _playerInput.PlayerControls.Move.canceled += OnMovementInput;
        _playerInput.PlayerControls.Move.performed += OnMovementInput;
        _playerInput.PlayerControls.Jump.started += OnJump;
        _playerInput.PlayerControls.Jump.canceled += OnJump;

        //Set up movement variables
        _isMovementPressed = false;
        _rotationSpeed = 5f;
        _groundGravity = -0.5f;

        //Jump equations found at https://www.youtube.com/watch?v=h2r3_KjChf4
        _timeToApex = _maxJumpTime / 2;
        _gravity = -2 * _maxJumpHeight / Mathf.Pow(_timeToApex, 2);
        _initialJumpVelocity = 2 * _maxJumpHeight / _timeToApex;

        //setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayerRelativeToCamera();
        RotatePlayer();
        _currentState.UpdateStates();
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _isMovementPressed = _currentMovement.x != 0 || _currentMovement.z != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requiresNewJumpPressed = false;
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
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        cameraRelativeMovement.y = _currentMovement.y;
        // cameraRelativeMovement = cameraRelativeMovement.normalized;
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
