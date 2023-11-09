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

        //Setting Animation
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
        Context.Animator.SetBool(Context.IsRunningHash, Context.CurrentMovementInput.magnitude >= 0.5f);

        CheckSwitchStates();
    }
}
