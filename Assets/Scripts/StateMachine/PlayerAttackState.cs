using System.Collections;
using UnityEngine;

//Concrete-State
internal class PlayerAttackState : PlayerBaseState
{
    //Reset the attack counter if the player does not press attack button X seconds after previous attack
    IEnumerator IAttackResetRoutine()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Attack not pressed, stopping combo");
        Context.AttackCount = 0;
        Context.Animator.SetInteger(Context.AttackCountHash, Context.AttackCount);
        Context.Animator.SetBool(Context.IsAttackingHash, false);
        CheckSwitchStates();
    }

    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        StateName = "AttackState";
    }

    public override void CheckSwitchStates()
    {
        if(Context.CharacterController.isGrounded)
        {
           SwitchState(Factory.Grounded());
        }
        else
        {
           SwitchState(Factory.Falling());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Entered the Attack State");
        InitializeSubState();
        Context.Animator.SetBool(Context.IsAttackingHash, true);
        HandleAttack();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        SetSubState(Factory.Idle());
    }

    public override void UpdateState()
    {
        //Animate the first attack, then if player presses attack within the timeframe, proceed to the next attack in combo
        if(Context.IsAttackPressed && !Context.RequiresNewAttackPress)
        {
            Debug.Log("Attacking again");
            HandleAttack();
        }
    }

    public void HandleAttack()
    {
        Context.RequiresNewAttackPress = true;

        //Don't reset attack counter if player is still pressing attack button; stop the reset coroutine
        if(Context.AttackCount < 3 && Context.AttackResetRoutine != null)
        {
            Context.StopCoroutine(Context.AttackResetRoutine);
        }

        Context.AttackCount += 1;
        Context.Animator.SetInteger(Context.AttackCountHash, Context.AttackCount);

        Context.AttackResetRoutine = Context.StartCoroutine(IAttackResetRoutine());
    }
}
