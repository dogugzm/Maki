using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector2 startPos;
    Vector2 direction;
    Rigidbody rb;

    [SerializeField] private PlayerDataSO dataSO;
    [SerializeField] private Transform groundCheckTransform;

    int dashCounter = 0;
    bool isDashing = false;
    bool isSlowingDown = false;

    private void OnEnable()
    {
        Enemy.OnEnemyTriggeredPlayer += OnEnemyTriggeredWithMe;
    }

    private void OnEnemyTriggeredWithMe()
    {
        dashCounter = (int)dataSO.dashMaxCounter - 1;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (IsGrounded() && Input.GetMouseButtonDown(0))
        {
            dashCounter = 0;
        }
        if (dashCounter >= dataSO.dashMaxCounter)
        {
            return;
        }
        if (!isDashing && Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            if (!IsGrounded())
            {
                Time.timeScale = dataSO.slowDownFactor;
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Fizik ad�mlar�n� da yava�latmak i�in
                isSlowingDown = true;
            }
        }
        else if (!isDashing && Input.GetMouseButtonUp(0))
        {
            Vector2 endPos = Input.mousePosition;
            direction = endPos - startPos;

            if (direction.magnitude >= dataSO.minSwipeDistance)
            {
                direction.Normalize();
                
                StartCoroutine(Dash(direction));
                
            }

            Time.timeScale = 1f; // Zaman �l�e�ini tekrar normal h�za d�nd�r
            Time.fixedDeltaTime = 0.02f; // Normal fizik ad�mlar�na geri d�n
            isSlowingDown = false;
        }

        // E�er zaman� yava�lat�yoruz ve fare bas�l� de�ilse, h�z� geri d�nd�r
        if (isSlowingDown && !Input.GetMouseButton(0))
        {
            Time.timeScale = 1f; // Zaman �l�e�ini tekrar normal h�za d�nd�r
            Time.fixedDeltaTime = 0.02f; // Normal fizik ad�mlar�na geri d�n
            isSlowingDown = false;
        }
    }

    IEnumerator Dash(Vector2 dashDirection)
    {
        dashCounter++;
        isDashing = true;
        float timer = 0f;

        while (timer < dataSO.dashDuration)
        {
            rb.velocity = dashDirection * (dataSO.dashDistance / dataSO.dashDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // After the dash duration, continue moving in the dash direction
        rb.velocity = dashDirection * (dataSO.dashDistance / dataSO.dashDuration);
        isDashing = false;
    }

    bool IsGrounded()
    {
        // Karakterin alt�nda bir Sphere Collider varsa ve yerde ise true d�nd�r
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
