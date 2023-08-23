using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckController : MonoBehaviour
{
    Rigidbody rb;
    
    public delegate void OnCollisionEvent(Collision collision);
    public static event OnCollisionEvent OnCollisionDelegate;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public static float lastPuckSpeed = 0f;
    private void FixedUpdate()
    {
        if (rb && rb.isKinematic == false) //rb.isKinematic is controlled by Grabbing behaviors of strikers 
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, AirHockeyGlobals.PuckSettings.maxSpeed);
            lastPuckSpeed = rb.velocity.magnitude;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionDelegate?.Invoke(collision);
    }

    public void Reset()
    {
        lastPuckSpeed = 0f;
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(0f, 0f, 0f); //NOTE - This should become some constant variable in Globals class or perhaps a tagged object on the Table
    }

    public void Grab(bool isGrabbed, StrikerMovementController strikerPlayer) 
    {
        rb.isKinematic = isGrabbed;
        //I think I'll do follow behaviors in StrikerMovementController;
        rb.velocity = Vector3.zero; //when I ungrab, we likely need to do something so they don't immediately bump against the striker
    }
    public void GrabMove(Vector3 targetPosition) 
    {
        rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, Time.deltaTime * 5f));
    }

}
