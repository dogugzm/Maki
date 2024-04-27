using System.Collections;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

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

    private int throwableCount;

    Camera mainCamera;
    float distanceFromCamera;
    private Vector2 _input;
    private Vector3 _movementVector;

    public PlayerStates currentState;


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
    }

    void Update()
    {
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        RotationControl();


        switch (currentState)
        {
            case PlayerStates.RUN:
                animator.SetBool("IsJumping", false);

                if (Input.GetButtonDown("Jump") && IsGrounded())
                {
                    currentState = PlayerStates.JUMP;
                    rb.AddForce(Vector3.up * dataSO.jumpPower, ForceMode.Impulse);
                }

                break;
            case PlayerStates.JUMP:
                StartCoroutine(JumpCoroutine());
                break;
            case PlayerStates.ATTACK:
                if (throwableCount != 0)
                {
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    throwMechanism.gameObject.SetActive(true);
                }

                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = 10;
                    Vector3 mouseClickedPos = mainCamera.ScreenToWorldPoint(mousePos);
                    direction = mouseClickedPos - pivotTransform.position;

                    throwMechanism.rotation = Quaternion.LookRotation(direction.normalized);

                }

                if (Input.GetMouseButtonUp(0))
                {
                    throwMechanism.gameObject.SetActive(false);

                    Instantiate(dataSO.throwablePrefab, throwPosition.position, throwMechanism.rotation);
                    throwableCount++;
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
            currentState = PlayerStates.RUN;
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
