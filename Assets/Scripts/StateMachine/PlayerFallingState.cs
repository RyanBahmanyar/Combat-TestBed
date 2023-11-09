using UnityEngine;

//Concrete state
public class PlayerFallingState : PlayerBaseState
{
    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        StateName = "FallingState";
    }

    //If Player lands on the ground, then switch to Grounded state
    public override void CheckSwitchStates()
    {
        if(Context.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        Debug.Log("Entered Falling State");
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

    void HandleGravity()
    {
        Context.CurrentMovementY += Context.Gravity * 2f * Time.deltaTime;
    }
}