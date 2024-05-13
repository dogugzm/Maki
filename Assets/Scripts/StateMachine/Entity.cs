using UnityEngine;


public class Entity : MonoBehaviour
{
    public FiniteStateMachine StateMachine { get; private set; }
    public Rigidbody Rb { get; private set; }
    public Animator Anim { get; private set; }
    public RunState RunState { get; private set; }


    private void Awake()
    {
         RunState = new(this,StateMachine,"Run");
    }

    public virtual void Start()
    {
        Rb = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        StateMachine = new FiniteStateMachine();
        StateMachine.Initialize(RunState);
    }

    public virtual void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    public virtual void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicalUpdate();
    }

    public void ShowState(string name)
    {
        Debug.Log(name);
    }

    #region GIZMOS

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position, 2f);
    }

    #endregion
}