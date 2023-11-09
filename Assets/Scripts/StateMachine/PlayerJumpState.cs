using UnityEngine;

//Concrete state
public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        StateName = "JumpState";
    }

    //If player is on the ground, switch to grounded state
    public override void CheckSwitchStates()
    {
        if(Context.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if(!Context.IsJumpPressed)
        {
            SwitchState(Factory.Falling());
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        Debug.Log("Entered the Jump State");
        JumpHandler();
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

    void JumpHandler()
    {
        Context.CurrentMovementY = Context.InitialJumpVelocity;
    }

    void HandleGravity()
    {
        Context.CurrentMovementY += Context.Gravity * Time.deltaTime;
    }
}
