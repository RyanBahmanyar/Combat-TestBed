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
    #region Player Movement Settings
    [Header("Player Movement Settings")]
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Animator _animator;
    private Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    bool _isMovementPressed;

    // Rigidbody _rb;

    Transform _playerObj;
    // Vector3 _moveDirection;
    // bool _isMovementPressed;
    private Transform _orientation;
    float _horizontalInput;
    float _verticalInput;
    private bool _grounded;
    // private float _distToGround;
    private float _speed;
    [SerializeField] float _gravity;
    // [SerializeField] float _groundSpeed;
    // [SerializeField] float _groundDrag;
    private float _rotationSpeed;

    #endregion

    #region Jump Settings
    [Header("Jump Setting")]
    [SerializeField] float _jumpHeight;
    private bool _isJumpPressed = false;
    private float _timeInAir = 0f;

    #endregion

    // #region Glide Settings
    // [Header("Glide Settings")]
    // public GameObject _gliderObject;
    // [SerializeField] float _glideDrag;
    // [SerializeField] float _glideSpeed;
    // private bool _isGlidePressed = false;
    // private bool _isAirBoostPressed = false;

    // #endregion

    #region Slope Settings
    [Header("Slope Settings")]
    [SerializeField] float _maxSlopeAngle;
    private RaycastHit _slopeHit;

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
    public bool IsJumpPressed { get { return _isJumpPressed; }}
    // public Rigidbody RB { get { return _rb; }}
    public float JumpHeight { get { return _jumpHeight; }}
    public float TimeInAir { get { return _timeInAir; } set { _timeInAir = value; }}
    public bool IsGrounded { get { return _grounded; } set { _grounded = value; }}
    // public float DistanceToGround { get { return _distToGround; }}
    public float Gravity { get { return _gravity; }}
    public bool IsMovementPressed { get { return _isMovementPressed; }}
    // public bool IsGlidePressed { get { return _isGlidePressed; }}
    // public bool IsAirBoostPressed { get { return _isAirBoostPressed; }}
    public float Speed { get { return _speed; } set { _speed = value; }}
    // public float GlideDrag { get {return _glideDrag; }}
    // public float GlideSpeed { get {return _glideSpeed; }}
    // public float GroundDrag { get {return _groundDrag; }}
    // public float GroundSpeed { get {return _groundSpeed; }}
    public float RotationSpeed { get {return _rotationSpeed; } set { _rotationSpeed = value; }}
    public Transform PlayerObj { get {return _playerObj; } set { _playerObj = value;}}
    public Transform Orientation { get {return _orientation; }}
    public Vector2 CurrentMovementInput { get {return _currentMovement; }}
    public float VerticalInput { get {return _currentMovement.x; }}
    public float HorizontalInput { get {return _currentMovement.z; }}
    public float CurrentMovementY { get {return _currentMovement.y; } set { _currentMovement.y = value; }}
    // public float MaxSlopeAngle { get {return _maxSlopeAngle; }}
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

        //setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

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

        // _rb = GetComponent<Rigidbody>();
        // _rb.freezeRotation = true;
        // _distToGround = GetComponent<Collider>().bounds.extents.y;
        // _gliderObject.SetActive(false);
        // _speed = _groundSpeed;
        _grounded = true;
        _isMovementPressed = false;
        // _rb.drag = _groundDrag;
        _rotationSpeed = 5f;
        _playerObj = GameObject.Find("PlayerObj").GetComponent<Transform>();
        _orientation = GameObject.Find("Orientation").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // _characterController.Move(_currentMovement * Time.deltaTime);
        _currentState.UpdateStates();
        // GetInput();
        // GroundCheck();
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
    }

    // private void GetInput()
    // {
    //     // _horizontalInput = Input.GetAxisRaw("Horizontal");
    //     // _verticalInput = Input.GetAxisRaw("Vertical");
    //     // _isJumpPressed = Input.GetButtonDown("Jump");
    //     // _isGlidePressed = Input.GetButtonDown("Glide");
    //     // _isAirBoostPressed = Input.GetButtonDown("Air Boost");
    //     // _isMovementPressed = _horizontalInput != 0 || _verticalInput != 0;
    // }

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

    // void GroundCheck()
    // {
    //     _grounded = Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.1f);
    //     //Debug.Log(_context.IsGrounded);
    // }
}
