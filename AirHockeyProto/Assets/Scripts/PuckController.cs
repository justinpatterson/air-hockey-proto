using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckController : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 5f)]
    float _maxSpeed = 2.5f;


    Rigidbody rb;
    
    public delegate void OnCollisionEvent(Collision collision);
    public static event OnCollisionEvent OnCollisionDelegate;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (rb) 
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, _maxSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");
        OnCollisionDelegate?.Invoke(collision);
    }

    public void Reset()
    {
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(0f, 0f, 0f); //NOTE - This should become some constant variable in Globals class or perhaps a tagged object on the Table
    }

}
