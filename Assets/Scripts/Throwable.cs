using System;
using UnityEngine;

public class Throwable : MonoBehaviour
{

    Rigidbody rb;
    [SerializeField] ThrowableSO dataSO;
    public static Action ThrowableDestroyed;

    Vector3 direction;
    private int attackCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attackCounter = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(transform.forward * dataSO.forcePower);
    }

    private void Update()
    {

        if (attackCounter==0)
        {
            return;
        }    

        if (Input.GetMouseButtonDown(0))
        {

        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 20;
            Vector3 mouseClickedPos = Camera.main.ScreenToWorldPoint(mousePos);
            direction = mouseClickedPos - transform.position;

            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
        else
        {
            //transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        if (Input.GetMouseButtonUp(0))
        {
            rb.velocity = Vector3.zero;
            attackCounter = 0;
            rb.AddForce(transform.forward * dataSO.forcePower);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
        
            Destroy(gameObject);
            ThrowableDestroyed?.Invoke();

        }

        if (other.CompareTag("Enemy"))
        {
            attackCounter = 1;
            Destroy(other.gameObject);
        }
    }


}



