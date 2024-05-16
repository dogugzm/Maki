using UnityEditor.Rendering;
using UnityEngine;

public class RunState : State
{
    public RunState(Entity entity, FiniteStateMachine stateMachine, string name) : base(entity, stateMachine, name)
    {

    }

    public override void Enter()
    {
        base.Enter();
        Time.timeScale = 1f;
        entity.SetIKAimTo(0, Time.deltaTime * 2);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Input.GetMouseButtonUp(0) && entity.IsGrounded())
        {
            stateMachine.ChangeState(entity.JumpState);
        }
    }

    

    public override void PhysicalUpdate()
    {
        base.PhysicalUpdate();
    }
}
