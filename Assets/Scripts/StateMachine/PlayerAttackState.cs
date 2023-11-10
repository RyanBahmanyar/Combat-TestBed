using UnityEngine;

//Concrete-State
internal class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        StateName = "AttackState";
    }

    public override void CheckSwitchStates()
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entered the Attack State");
        InitializeSubState();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        SetSubState(null);
    }

    public override void UpdateState()
    {
    }
}
