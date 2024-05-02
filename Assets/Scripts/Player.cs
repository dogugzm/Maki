using System.Collections;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using UnityEngine.Animations.Rigging;
using Unity.Burst.Intrinsics;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public enum PlayerStates
{
    RUN,
    JUMP,
    ATTACK
}

public class Player : MonoBehaviour
{
    Vector2 direction;
    Rigidbody rb;

    [SerializeField] private PlayerDataSO dataSO;
    [SerializeField] private Transform groundCheckTransform;

    public Transform throwPosition;
    public Transform throwMechanism;
    public Transform pivotTransform;

    [SerializeField] private Animator animator;
    [SerializeField] private MultiAimConstraint spineIK;
    [SerializeField] private MultiAimConstraint handIK;
    [SerializeField] private MultiAimConstraint armIK;
    [SerializeField] private MultiAimConstraint headIK;



    private int throwableCount;

    Camera mainCamera;
    float distanceFromCamera;
    private Vector2 _input;
    private Vector3 _movementVector;

    public PlayerStates currentState;
    public Transform mouseTr;

    float cameraZ;

    #region EVENT
    private void OnEnable()
    {
        Enemy.OnEnemyTriggeredPlayer += OnEnemyTriggeredWithMe;
        Throwable.ThrowableDestroyed += OnThrowableDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyTriggeredPlayer += OnEnemyTriggeredWithMe;
        Throwable.ThrowableDestroyed += OnThrowableDestroyed;
    }

    private void OnThrowableDestroyed()
    {
        throwableCount = 0;
    }

    private void OnEnemyTriggeredWithMe()
    {


    }
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        distanceFromCamera = mainCamera.transform.position.z;
        currentState = PlayerStates.RUN;
        SetAimConstraint(spineIK);
        SetAimConstraint(handIK);
        SetAimConstraint(armIK);
        SetAimConstraint(headIK);


        cameraZ = mainCamera.transform.position.z;
    }

    public void SetAimTo(float weight, float overTime)
    {
        float layerWeight = animator.GetLayerWeight(1);
        animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, weight, overTime));

        //float startTime = Time.time;
        //while (Time.time < startTime + overTime)
        //{           
        //    Mathf.Lerp(spineIK.weight, weight, (Time.time - startTime) / overTime);
        //    Mathf.Lerp(armIK.weight, weight, (Time.time - startTime) / overTime);
        //    Mathf.Lerp(handIK.weight, weight, (Time.time - startTime) / overTime);
        //    yield return null;
        //}
        spineIK.weight = weight;
        armIK.weight = weight;
        handIK.weight = weight;
        headIK.weight = weight;

    }

    private void SetAimConstraint(MultiAimConstraint aim)
    {
        var data = aim.data.sourceObjects;
        data.Clear();
        data.Add(new WeightedTransform(mouseTr, 1));
        aim.data.sourceObjects = data;
        aim.GetComponentInParent<RigBuilder>().Build();
    }

    void Update()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        RotationControl();

        switch (currentState)
        {
            case PlayerStates.RUN:
                SetAimTo(0, Time.deltaTime * 2);
                if (Input.GetButtonDown("Jump") && IsGrounded())
                {
                    rb.AddForce(Vector3.up * dataSO.jumpPower, ForceMode.Impulse);
                    currentState = PlayerStates.JUMP;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    currentState = PlayerStates.ATTACK;
                }

                break;
            case PlayerStates.JUMP:
                StartCoroutine(JumpCoroutine());
                if (Input.GetMouseButtonDown(0))
                {
                    currentState = PlayerStates.ATTACK;
                }

                break;
            case PlayerStates.ATTACK:

                SetAimTo(1, Time.deltaTime * 2);

                if (throwableCount != 0)
                {
                    return;
                }
                throwMechanism.gameObject.SetActive(true);

                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = -cameraZ;
                    Vector3 mouseClickedPos = mainCamera.ScreenToWorldPoint(mousePos);
                    mouseTr.position = new Vector3(mouseClickedPos.x, mouseClickedPos.y, 0);

                    direction = mouseClickedPos - pivotTransform.position;

                    throwMechanism.rotation = Quaternion.LookRotation(direction.normalized);

                }

                if (Input.GetMouseButtonUp(0))
                {
                    throwMechanism.gameObject.SetActive(false);

                    Instantiate(dataSO.throwablePrefab, throwPosition.position, throwMechanism.rotation);
                    throwableCount++;
                    currentState = PlayerStates.RUN;
                }
                break;
            default:
                break;
        }

    }

    IEnumerator JumpCoroutine()
    {
        animator.SetBool("IsJumping", true);

        yield return new WaitForSeconds(0.2f); // Adjust the duration as needed

        if (IsGrounded())
        {
            if (Input.GetMouseButton(0))
            {
                animator.SetBool("IsJumping", false);

                currentState = PlayerStates.ATTACK;
            }
            else
            {
                animator.SetBool("IsJumping", false);

                currentState = PlayerStates.RUN;
            }
        }
    }

    void FixedUpdate()
    {
        _movementVector = dataSO.speed * Mathf.Abs(_input.x) * transform.forward;
        rb.velocity = new Vector3(_movementVector.x, rb.velocity.y, rb.velocity.z);
    }

    //TODO: dotween ile smooth rotation
    private void RotationControl()
    {
        if (_input.x > 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
            animator.speed = 1.3f;
        }
        else if (_input.x < 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 270, transform.rotation.z);
            animator.speed = 1.3f;

        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
            animator.speed = 1f;
        }
    }

    bool IsGrounded()
    {
        // Karakterin altýnda bir Sphere Collider varsa ve yerde ise true döndür
        Collider[] colliders = Physics.OverlapSphere(groundCheckTransform.position, dataSO.groundCheckRadius, dataSO.groundLayerMask);

        return colliders.Length > 0;
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PartTriggerCenter"))
        {
            Environment.OnPlayerCentered?.Invoke();
            yield return new WaitForEndOfFrame();
        }
    }


}
