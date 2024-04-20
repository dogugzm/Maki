using System.Collections;
using System.Net;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector2 direction;
    Rigidbody rb;

    [SerializeField] private PlayerDataSO dataSO;
    [SerializeField] private Transform groundCheckTransform;

    int dashCounter = 0;
    bool isFiring = false;
    bool isSlowingDown = false;
    public Transform throwPosition;
    public Transform throwMechanism;
    public Transform pivotTransform;

    Camera mainCamera;
    float distanceFromCamera;

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
        mainCamera = Camera.main;
        distanceFromCamera = mainCamera.transform.position.z;
    }

    void Update()
    {
        //if (IsGrounded() && Input.GetMouseButtonDown(0))
        //{
        //    dashCounter = 0;
        //}
        //if (dashCounter >= dataSO.dashMaxCounter)
        //{
        //    return;
        //}
        if (Input.GetMouseButtonDown(0))
        {
            throwMechanism.gameObject.SetActive(true);
           
            

            //if (!IsGrounded())
            //{
            //    Time.timeScale = dataSO.slowDownFactor;
            //    Time.fixedDeltaTime = 0.02f * Time.timeScale; // Fizik adýmlarýný da yavaþlatmak için
            //    isSlowingDown = true;
            //}
        }
        else if (Input.GetMouseButtonUp(0))
        {
            throwMechanism.gameObject.SetActive(false);

            Vector3 endPos = Camera.main.ScreenToWorldPoint( Input.mousePosition);
            //direction = endPos - mouseClickedPos;

            //if (direction.magnitude >= dataSO.minSwipeDistance)
            //{
            //    direction.Normalize();
                
            //    StartCoroutine(Fire(direction));
                
            //}

            //Time.timeScale = 1f; // Zaman ölçeðini tekrar normal hýza döndür
            //Time.fixedDeltaTime = 0.02f; // Normal fizik adýmlarýna geri dön
            //isSlowingDown = false;
        }

        // Eðer zamaný yavaþlatýyoruz ve fare basýlý deðilse, hýzý geri döndür
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 mouseClickedPos = mainCamera.ScreenToWorldPoint(mousePos);
            //mouseClickedPos.z = distanceFromCamera;
            direction = mouseClickedPos - pivotTransform.position;

            // Draw a line between click position and transform position
            //Debug.DrawLine(pivotTransform.position, mouseClickedPos, Color.red, 1f);
            throwMechanism.rotation = Quaternion.LookRotation(direction.normalized);

            //Time.timeScale = 1f; // Zaman ölçeðini tekrar normal hýza döndür
            //Time.fixedDeltaTime = 0.02f; // Normal fizik adýmlarýna geri dön
            //isSlowingDown = false;
            //Vector3 direction = mouseClickedPos - (Vector2)pivotTransform.position;

        }
    }

    private void CreateThrowable()
    {
        //Instantiate(dataSO.throwablePrefab, throwPosition.position, Quaternion.identity);

    }

    IEnumerator Fire(Vector2 fireDirection)
    {
        dashCounter++;
        isFiring = true;

        yield return new WaitForSeconds(0);
        //float timer = 0f;
        //while (timer < dataSO.dashDuration)
        //{
        //    rb.velocity = fireDirection * (dataSO.dashDistance / dataSO.dashDuration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}

        //// After the dash duration, continue moving in the dash direction
        //rb.velocity = fireDirection * (dataSO.dashDistance / dataSO.dashDuration);
        isFiring = false;
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
