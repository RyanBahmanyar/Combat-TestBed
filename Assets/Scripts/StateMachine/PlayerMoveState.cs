using UnityEngine;

//Sub state
public class PlayerMoveState : PlayerBaseState
{
    private RaycastHit _slopeHit;

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
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        MovePlayerRelativeToCamera();
        CheckSwitchStates();
    }

    void MovePlayerRelativeToCamera()
    {
        //Get Camera normalized directional vectors
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        //Create direction-relative input vectors
        Vector3 forwardRelativeVerticalInput = Context.VerticalInput * forward;
        Vector3 rightRelativeHorizontalInput = Context.HorizontalInput * right;

        //Create and apply camera relative movement
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;
        cameraRelativeMovement = cameraRelativeMovement.normalized;

        Context.RB.velocity = new Vector3(cameraRelativeMovement.x * Context.Speed, Context.RB.velocity.y, cameraRelativeMovement.z * Context.Speed);
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(Context.transform.position, Vector3.down, out _slopeHit, Context.DistanceToGround + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < Context.MaxSlopeAngle && angle != 0;
        }

        return false;
    }

    // private Vector3 GetSlopeMoveDirection()
    // {
    //     return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    // }
}
