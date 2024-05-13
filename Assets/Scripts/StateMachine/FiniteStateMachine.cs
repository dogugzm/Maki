public class FiniteStateMachine
{
    public State CurrentState { get; private set; }

    public void Initialize(State startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public virtual void ChangeState(State nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
}