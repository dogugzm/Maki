using Unity.Burst.Intrinsics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class Entity : MonoBehaviour
{
    private float cameraZ;
    private Camera mainCamera;
    private string currentState = "Empty";
    [SerializeField] private Animator anim;

    public FiniteStateMachine StateMachine { get; private set; }
    public Rigidbody Rb { get; private set; }
    public Animator Anim { get => anim; private set => anim = value; }
    public RunState RunState { get; private set; }
    public JumpState JumpState { get; private set; }
    public AttackState AttackState { get; private set; }


    [Header("Animation Related")]
    [SerializeField] private MultiAimConstraint armIK;
    [SerializeField] private MultiAimConstraint handIK;
    [SerializeField] private MultiAimConstraint headIK;
    [SerializeField] private MultiAimConstraint spineIK;

    [SerializeField] private Transform groundCheckTransform;


    public PlayerDataSO dataSO;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        StateMachine = new FiniteStateMachine();
        RunState = new(this,StateMachine,"Run");
        JumpState = new(this, StateMachine, "Jump");
        AttackState = new(this, StateMachine, "Attack");

    }

    public virtual void Start()
    {
        mainCamera = Camera.main;
        cameraZ = mainCamera.transform.position.z;
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

    public Vector3 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cameraZ;
        Vector3 mouseClickedPos = mainCamera.ScreenToWorldPoint(mousePos);
        return mouseClickedPos;
    }

    public void ShowState(string name)
    {
        currentState = name;
    }

    public bool IsGrounded()
    {
        // Karakterin altında bir Sphere Collider varsa ve yerde ise true döndür
        Collider[] colliders = Physics.OverlapSphere(groundCheckTransform.position, dataSO.groundCheckRadius, dataSO.groundLayerMask);

        return colliders.Length > 0;
    }

    public void SetIKAimTo(float weight, float overTime)
    {
        float layerWeight = Anim.GetLayerWeight(1);
        Anim.SetLayerWeight(1, Mathf.Lerp(layerWeight, weight, overTime));

        spineIK.weight = weight;
        armIK.weight = weight;
        handIK.weight = weight;
        headIK.weight = weight;
    }

    #region GIZMOS

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 60;
        style.normal.textColor = Color.red;
        GUILayout.Label(currentState, style,GUILayout.Width(500), GUILayout.Height(100));
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position, 2f);
    }

    #endregion
}