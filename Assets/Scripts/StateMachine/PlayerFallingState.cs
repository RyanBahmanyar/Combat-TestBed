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
    //If Player presses Glide button while in the air (falling or jumping), then switch to Glide state
    public override void CheckSwitchStates()
    {
        if(Context.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        // else if(Context.IsGlidePressed)
        // {
        //     SwitchState(Factory.Glide());
        // }
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
        // Context.RB.AddForce(Vector3.up * Context.Gravity, ForceMode.Acceleration);
        // Context.TimeInAir += Time.deltaTime;//tracking time in air for gliding
        Context.CurrentMovementY = Context.Gravity;
    }
}