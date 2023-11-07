using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _animator;

    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    bool _isMovementPressed;

    void Awake() 
    {
        //Setting reference variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        //Storing player input
        _playerInput.PlayerControls.Move.started += OnMovementInput;
        _playerInput.PlayerControls.Move.canceled += OnMovementInput;
        _playerInput.PlayerControls.Move.performed += OnMovementInput;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _isMovementPressed = _currentMovement.x != 0 || _currentMovement.z != 0;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimation();
        _characterController.Move(_currentMovement * Time.deltaTime);
    }

    void HandleAnimation()
    {
        //Get parameter values from animator
        bool isWalking = _animator.GetBool("IsWalking");
        bool isRunning = _animator.GetBool("IsRunning");

        //Start walking if movement input is pressed and not already walking
        if(_isMovementPressed && !isWalking)
        {
            _animator.SetBool("IsWalking", true);
        }
        //Stop walking if movement input is not pressed and not already walking
        if(!_isMovementPressed && isWalking)
        {
            _animator.SetBool("IsWalking", false);
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
