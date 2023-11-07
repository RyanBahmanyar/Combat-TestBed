using System;
using UnityEngine;

/*
* Establishes methods and variables that concrete states will inherit when derived from this class
*/
public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _context;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSuperState;
    private PlayerBaseState _currentSubState;

    //for debugging
    private string _stateName;

    #region Getters and Setters
    public bool IsRootState { get { return _isRootState;} set { _isRootState = value; }}
    public PlayerStateMachine Context { get { return _context; }}
    public PlayerStateFactory Factory { get { return _factory; }}
    public string StateName { get { return _stateName; } set { _stateName = value; }}

    #endregion

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _context = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();

        if(_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    public void SwitchState(PlayerBaseState newState)
    {
        //current state exits state
        ExitState();

        //New state enters
        newState.EnterState();

        //Only switch context current state if it is at the top of the state chain
        if(_isRootState)
        {
            //switch current state
            _context.CurrentState = newState;
        }
        else if(_currentSuperState != null)
        {
            //set the current super states sub state to the new state
            _currentSuperState.SetSubState(newState);
        }
    }

    public void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
        Debug.Log("Current Super state: " + _currentSuperState._stateName);
    }

    public void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;//Set child substate for this state
        newSubState.SetSuperState(this);//Set this state as the parent for the new substate
        Debug.Log("Current Sub state: " + _currentSubState._stateName);
    }
}
