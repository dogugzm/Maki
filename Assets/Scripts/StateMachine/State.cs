
using UnityEngine;

public class State
{
    protected FiniteStateMachine stateMachine;
    protected Entity entity;
    public string name;

    protected float startingTime;

    public State(Entity entity, FiniteStateMachine stateMachine, string name)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.name = name;
    }

    public virtual void Enter()
    {
        entity.ShowState(name);
        startingTime = Time.time;
    }

    public virtual void Exit()
    {

    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicalUpdate()
    {

    }



}