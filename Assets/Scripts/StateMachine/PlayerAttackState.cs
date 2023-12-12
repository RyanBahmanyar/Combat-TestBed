using System.Collections;
using UnityEngine;

//Concrete-State
internal class PlayerAttackState : PlayerBaseState
{
    private float _timer = 0;
    private float _attackTime;
    private Vector3 _targetPosition;
    
    //Reset the attack counter if the player does not press attack button X seconds after previous attack
    IEnumerator IAttackResetRoutine()
    {
        //Different attacks have varying animation times, so use the attack times dictionary to adjust wait period
        yield return new WaitForSeconds(_attackTime + 0.2f);

        Debug.Log("Attack not pressed, stopping combo");
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

        //disable movement and set movement to zero so player stays in place during attacks
        Context.PlayerInput.FindAction("Move").Disable();
        Context.VerticalInput = 0;
        Context.HorizontalInput = 0;
        Context.CurrentMovementY = 0;

        Context.Animator.SetBool(Context.IsAttackingHash, true);

        HandleAttack();
    }

    public override void ExitState()
    {
        Context.PlayerInput.FindAction("Move").Enable();

        Context.AttackCount = 0;
        Context.Animator.SetInteger(Context.AttackCountHash, Context.AttackCount);
        Context.Animator.SetBool(Context.IsAttackingHash, false);
    }

    public override void InitializeSubState()
    {
        SetSubState(Factory.Idle());
    }

    public override void UpdateState()
    {
        Debug.Log(Context.AttackCount);

        //Animate the attack, then if player presses attack within the timeframe (between attacktime and attacktime + x), proceed to the next attack in combo
        if(Context.IsAttackPressed && !Context.RequiresNewAttackPress && _timer >= _attackTime)
        {
            Debug.Log("Attacking again");

            Context.RequiresNewAttackPress = true;
            HandleAttack();
        }

        //Move character forward when attacking
        Context.transform.position = Vector3.Lerp(Context.transform.position, _targetPosition, 10f * Time.deltaTime);

        _timer += Time.deltaTime;
    }

    public void HandleAttack()
    {
        if(++Context.AttackCount <= 3)
        {
            _timer = 0;
            _attackTime = Context.AttackTimes[Context.AttackCount];

            //TODO adjust the target position to go towards an enemy if locked on/soft locked on, otherwise move forward like this
            _targetPosition = Context.transform.position + Context.PlayerObj.transform.forward * 3f;

            //Don't reset attack counter if player is still pressing attack button; stop the reset coroutine
            if(Context.AttackCount <= 3 && Context.AttackResetRoutine != null)
            {
                Context.StopCoroutine(Context.AttackResetRoutine);
            }

            Context.Animator.SetInteger(Context.AttackCountHash, Context.AttackCount);

            Context.AttackResetRoutine = Context.StartCoroutine(IAttackResetRoutine());
        }

    }
}
