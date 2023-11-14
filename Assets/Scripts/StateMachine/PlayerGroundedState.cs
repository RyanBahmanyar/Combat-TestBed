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
    //If player presses attack button, switch to attack state
    public override void CheckSwitchStates()
    {
        if(Context.IsJumpPressed && !Context.RequiresNewJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
        else if(!Context.CharacterController.isGrounded)
        {
            SwitchState(Factory.Falling());
        }
        else if(Context.IsAttackPressed && !Context.RequiresNewAttackPress)
        {
            SwitchState(Factory.Attack());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Entered the Grounded State");
        InitializeSubState();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        if(Context.IsMovementPressed)
        {
            SetSubState(Factory.Move());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    //Even though the player is in the Grounded state, CharacterController.isGrounded variable requires some downward force in order to 
    //work properly
    private void HandleGravity()
    {
        Context.CurrentMovementY += Context.GroundGravity * Time.deltaTime;
    }
}
