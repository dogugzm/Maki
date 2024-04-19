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
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Fizik adýmlarýný da yavaþlatmak için
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

            Time.timeScale = 1f; // Zaman ölçeðini tekrar normal hýza döndür
            Time.fixedDeltaTime = 0.02f; // Normal fizik adýmlarýna geri dön
            isSlowingDown = false;
        }

        // Eðer zamaný yavaþlatýyoruz ve fare basýlý deðilse, hýzý geri döndür
        if (isSlowingDown && !Input.GetMouseButton(0))
        {
            Time.timeScale = 1f; // Zaman ölçeðini tekrar normal hýza döndür
            Time.fixedDeltaTime = 0.02f; // Normal fizik adýmlarýna geri dön
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
