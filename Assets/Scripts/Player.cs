using System.Collections;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class Player : MonoBehaviour
{
    Vector2 direction;
    Rigidbody rb;

    [SerializeField] private PlayerDataSO dataSO;
    [SerializeField] private Transform groundCheckTransform;

    public Transform throwPosition;
    public Transform throwMechanism;
    public Transform pivotTransform;

    private int throwableCount;

    Camera mainCamera;
    float distanceFromCamera;
    private Vector2 _input;
    private Vector3 _movementVector;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        distanceFromCamera = mainCamera.transform.position.z;
    }

    void Update()
    {
        //Cleanerway to get input
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * dataSO.jumpPower, ForceMode.Impulse);
        }

        RotationControl();


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
    }

    private void RotationControl()
    {
        if (_input.x > 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
        }
        else if (_input.x < 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 270, transform.rotation.z);
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, 90, transform.rotation.z);
        }
    }

    void FixedUpdate()
    {
        //Keep the movement vector aligned with the player rotation
        _movementVector = Mathf.Abs(_input.x) * transform.forward * dataSO.speed;
        //Apply the movement vector to the rigidbody without effecting gravity
        rb.velocity = new Vector3(_movementVector.x, rb.velocity.y, rb.velocity.z);
    }

    // 'moveCharacter' Function for moving the game object
    void MoveCharacter(Vector3 _direction)
    {
        // We multiply the 'speed' variable to the Rigidbody's velocity...
        // and also multiply 'Time.fixedDeltaTime' to keep the movement consistant on all devices
        rb.velocity = dataSO.speed * Time.fixedDeltaTime * _direction;
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
