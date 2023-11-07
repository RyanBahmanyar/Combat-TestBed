using UnityEngine;

//Sub state
public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        StateName = "IdleState";
    }

    public override void CheckSwitchStates()
    {
        if(Context.IsMovementPressed)
        {
            SwitchState(Factory.Move());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Entered Idle Sub-State");

        //Setting Animation
        Context.Animator.SetBool(Context.IsWalkingHash, false);
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
        CheckSwitchStates();
    }
}
