using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Input = UnityEngine.Input;
using DG.Tweening;


public enum PlayerStates
{
    RUN,
    JUMP,
    ATTACK,
    TRANSFER
}

public class Player : MonoBehaviour
{
    public PlayerStates currentState;
    public Transform mouseTr;
    public Transform pivotTransform;
    public Transform throwMechanism;
    public Transform throwPosition;

    [Header("Animation Related")]
    [SerializeField] private PlayerDataSO dataSO;

    [SerializeField] private Animator animator;
    [SerializeField] private MultiAimConstraint armIK;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private MultiAimConstraint handIK;
    [SerializeField] private MultiAimConstraint headIK;
    [SerializeField] private MultiAimConstraint spineIK;

    private float cameraZ;
    private Vector2 direction;
    private Camera mainCamera;
    private Rigidbody rb;
    private int throwableCount;
    private Vector3 transferPosition = Vector3.zero;

    #region EVENT

    private void OnDisable()
    {
        Enemy.OnEnemyTriggeredPlayer -= OnEnemyTriggeredWithMe;
        Throwable.ThrowableDestroyed -= OnThrowableDestroyed;
        Throwable.ThrowableHitted -= OnThrowableHitted;

    }

    private void OnEnable()
    {
        Enemy.OnEnemyTriggeredPlayer += OnEnemyTriggeredWithMe;
        Throwable.ThrowableDestroyed += OnThrowableDestroyed;
        Throwable.ThrowableHitted += OnThrowableHitted;
    }

    private void OnThrowableHitted(Vector3 pos)
    {
        currentState = PlayerStates.TRANSFER;
        transferPosition = pos;
    }

    private void OnEnemyTriggeredWithMe()
    {
    }

    private void OnThrowableDestroyed()
    {
        throwableCount = 0;
    }

    #endregion EVENT

    public void SetIKAimTo(float weight, float overTime)
    {
        float layerWeight = animator.GetLayerWeight(1);
        animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, weight, overTime));

        spineIK.weight = weight;
        armIK.weight = weight;
        handIK.weight = weight;
        headIK.weight = weight;
    }

    private bool IsGrounded()
    {
        // Karakterin altýnda bir Sphere Collider varsa ve yerde ise true döndür
        Collider[] colliders = Physics.OverlapSphere(groundCheckTransform.position, dataSO.groundCheckRadius, dataSO.groundLayerMask);

        return colliders.Length > 0;
    }

    private IEnumerator JumpStateCoroutine()
    {
        animator.SetBool("IsJumping", true);

        yield return new WaitForSeconds(0.2f);

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

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PartTriggerCenter"))
        {
            Environment.OnPlayerCentered?.Invoke();
            yield return new WaitForEndOfFrame();
        }
    }

    //TODO: dotween ile smooth rotation
    private void RotationControl(Vector3 mousePos)
    {
        if (mousePos.x >= transform.position.x)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 270, transform.rotation.z);
        }
    }

    private void SetAimConstraint(MultiAimConstraint aim)
    {
        var data = aim.data.sourceObjects;
        data.Clear();
        data.Add(new WeightedTransform(mouseTr, 1));
        aim.data.sourceObjects = data;
        aim.GetComponentInParent<RigBuilder>().Build();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        currentState = PlayerStates.RUN;
        SetAimConstraint(spineIK);
        SetAimConstraint(handIK);
        SetAimConstraint(armIK);
        SetAimConstraint(headIK);
        cameraZ = mainCamera.transform.position.z;
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerStates.RUN:
                Time.timeScale = 1f;
                SetIKAimTo(0, Time.deltaTime * 2);

                if (Input.GetMouseButtonUp(0) && IsGrounded())
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = -cameraZ;
                    Vector3 mouseClickedPos = mainCamera.ScreenToWorldPoint(mousePos);
                    rb.AddForce((mouseClickedPos - transform.position).normalized * dataSO.jumpPower, ForceMode.Impulse);
                    currentState = PlayerStates.JUMP;
                }

                break;

            case PlayerStates.JUMP:
                Time.timeScale = 1f;
                StartCoroutine(JumpStateCoroutine());
                if (Input.GetMouseButtonDown(0))
                {
                    currentState = PlayerStates.ATTACK;
                }

                break;

            case PlayerStates.ATTACK:
                Time.timeScale = 0.2f;
                SetIKAimTo(1, Time.deltaTime * 2);

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
                    RotationControl(mouseClickedPos);

                    mouseTr.position = new Vector3(mouseClickedPos.x, mouseClickedPos.y, 0);

                    direction = mouseClickedPos - pivotTransform.position;

                    throwMechanism.rotation = Quaternion.LookRotation(direction.normalized);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    throwMechanism.gameObject.SetActive(false);

                    Instantiate(dataSO.throwablePrefab, throwPosition.position, throwMechanism.rotation);
                    throwableCount++;
                    currentState = PlayerStates.JUMP;
                }
                break;
            case PlayerStates.TRANSFER:
                rb.MovePosition(transferPosition);
                Time.timeScale = 0.1f;
                currentState = PlayerStates.JUMP;
                break;
            default:
                break;
        }
    }
}