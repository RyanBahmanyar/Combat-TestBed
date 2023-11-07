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
    //If player presses glide, switch to Glide state
    public override void CheckSwitchStates()
    {
        if(Context.CharacterController.isGrounded /*&& Context.RB.velocity.y < 0*/)
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
        // Context.RB.velocity = new Vector3(Context.RB.velocity.x, 0f, Context.RB.velocity.z);
        // Context.RB.AddForce(Context.transform.up * Context.JumpHeight, ForceMode.Impulse);
        // Context.TimeInAir = 0f;

        Context.CurrentMovementY = Context.JumpHeight;
    }

    void HandleGravity()
    {
        // Context.RB.AddForce(Vector3.up * Context.Gravity, ForceMode.Acceleration);
        // Context.TimeInAir += Time.deltaTime;//tracking time in air for gliding

        Context.CurrentMovementY = Context.Gravity;
    }
}
