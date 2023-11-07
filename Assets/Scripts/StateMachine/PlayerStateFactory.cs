using System;
using System.Collections.Generic;

enum PlayerStates 
{
    IDLE,
    GROUNDED,
    JUMP,
    MOVE,
    FALLING
    // GLIDE
}

public class PlayerStateFactory
{
    PlayerStateMachine _context;

    //Using a Dictionary in order to prevent calling each state's constructor every time a state switches
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.IDLE] = new PlayerIdleState(_context, this);
        _states[PlayerStates.GROUNDED] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.JUMP] = new PlayerJumpState(_context, this);
        // _states[PlayerStates.WALK] = new PlayerWalkState(_context, this);
        _states[PlayerStates.FALLING] = new PlayerFallingState(_context, this);
        // _states[PlayerStates.GLIDE] = new PlayerGlideState(_context, this);
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.IDLE];
    }

    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.GROUNDED];
    }

    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.JUMP];
    }

    public PlayerBaseState Move()
    {
        return _states[PlayerStates.MOVE];
    }

    public PlayerBaseState Falling()
    {
        return _states[PlayerStates.FALLING];
    }

    // public PlayerBaseState Glide()
    // {
    //     return _states[PlayerStates.GLIDE];
    // }
}
