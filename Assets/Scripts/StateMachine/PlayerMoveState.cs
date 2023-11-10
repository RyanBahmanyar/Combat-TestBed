using UnityEngine;

//Sub state
public class PlayerMoveState : PlayerBaseState
{
    // private RaycastHit _slopeHit;

    public PlayerMoveState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        StateName = "MoveState";
    }

    public override void CheckSwitchStates()
    {
        if(!Context.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Entered Move Sub-State");

        Context.Animator.SetBool(Context.IsWalkingHash, true);
        Context.Animator.SetBool(Context.IsRunningHash, false);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        //Change player animation to running if their movement input reaches a certain threshold
        Vector2 movement = new Vector2(Context.VerticalInput, Context.HorizontalInput);
        Context.Animator.SetBool(Context.IsRunningHash, movement.magnitude >= 0.5f);
        Debug.Log(movement.magnitude);

        CheckSwitchStates();
    }
}
