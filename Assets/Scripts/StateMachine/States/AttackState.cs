using UnityEditor.Rendering;
using UnityEngine;

public class AttackState : State
{
    public AttackState(Entity entity, FiniteStateMachine stateMachine, string name) : base(entity, stateMachine, name)
    {

    }

    public override void Enter()
    {
        base.Enter();
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
