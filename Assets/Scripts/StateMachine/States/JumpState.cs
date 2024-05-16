using System.Collections;
using UnityEngine;

public class JumpState : State
{

    bool isJumping = false;
    public JumpState(Entity entity, FiniteStateMachine stateMachine, string name) : base(entity, stateMachine, name)
    {

    }

    public override void Enter()
    {
        base.Enter();
        Vector3 mouseClickedPos = entity.GetMousePos();
        entity.Rb.AddForce((mouseClickedPos - entity.transform.position).normalized * entity.dataSO.jumpPower, ForceMode.Impulse);
        entity.Anim.SetBool("IsJumping", true);
        entity.StartCoroutine(JumpStateCoroutine());
    }

    public override void Exit()
    {
        base.Exit();
        entity.Anim.SetBool("IsJumping", false);

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (entity.IsGrounded() && isJumping)
        {
            if (Input.GetMouseButton(0))
            {
                stateMachine.ChangeState(entity.AttackState);
            }
            else
            {
                stateMachine.ChangeState(entity.RunState);
            }
        }
    }

    public override void PhysicalUpdate()
    {
        base.PhysicalUpdate();
    }

    private IEnumerator JumpStateCoroutine()
    {     
        yield return new WaitForSeconds(0.5f);

        isJumping = true;
    }

}
