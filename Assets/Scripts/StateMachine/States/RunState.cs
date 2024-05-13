using UnityEngine;

public class RunState : State
{
    public RunState(Entity entity, FiniteStateMachine stateMachine, string name) : base(entity, stateMachine, name)
    {

    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Here run" + entity + stateMachine + name);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicalUpdate()
    {
        base.PhysicalUpdate();
    }
}
