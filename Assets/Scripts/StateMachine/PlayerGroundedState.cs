using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Concrete state
public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        StateName = "GroundedState";
    }

    //If Player pressed Jump button, then switch to Jump state
    //If Player walks off a ledge, then switch to Falling state
    public override void CheckSwitchStates()
    {
        if(Context.IsJumpPressed)
        {
            // Context.IsGrounded = false;
            SwitchState(Factory.Jump());
        }
        else if(!Context.IsGrounded)
        {
            SwitchState(Factory.Falling());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        Debug.Log("Entered the Grounded State");
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        if(Context.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

    public override void UpdateState()
    {
        RotatePlayer();
        CheckSwitchStates();
    }

    private void RotatePlayer()
    {
        //rotate orientation
        Vector3 viewDirection = Context.transform.position - new Vector3(Camera.main.transform.position.x, Context.transform.position.y, Camera.main.transform.position.z);
        Context.Orientation.forward = viewDirection.normalized;

        //rotate player object
        Vector3 inputDirection = Context.Orientation.forward * Context.VerticalInput + Context.Orientation.right * Context.HorizontalInput;

        if(inputDirection != Vector3.zero)
        {
            Context.PlayerObj.forward = Vector3.Slerp(Context.PlayerObj.forward, inputDirection.normalized, Time.deltaTime * Context.RotationSpeed);
        }
    }
}
