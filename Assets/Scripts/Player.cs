using System.Collections;
using UnityEditor.Build;
using UnityEngine;

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

        if (Input.GetMouseButtonUp(0) )
        {
            throwMechanism.gameObject.SetActive(false);
           
            Instantiate(dataSO.throwablePrefab, throwPosition.position, throwMechanism.rotation);
            throwableCount++;
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
