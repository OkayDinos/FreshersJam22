using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCharacterController : MonoBehaviour
{

    //public SimpleControls Input;
    public InputAction moveAction;
    public InputAction jumpAction;
    public Vector3 velocity;
    public float maxSpeed;
    public float accelMultiplier;
    public float jumpVelocity;
    float horizSpeed;
    float desiredSpeed;
    Rigidbody rigidbody;
    Transform transform;
    Collider collider;
    float distToGround, distToEdge;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider>();
        moveAction.Enable();
        jumpAction.Enable();
        distToGround = collider.bounds.extents.y;
        distToEdge = collider.bounds.extents.x;
    }

    // Update is called once per frame
    void Update()
    {

        // Get the X - Axis input
        var moveDirection = moveAction.ReadValue<float>();

        // Calculate speed accounting for changes in desired speed (maxSpeed * moveDirection)

        desiredSpeed = maxSpeed * moveDirection; //Get target speed

        // Jump Action
        if (jumpAction.ReadValue<float>() == 1 && IsGrounded())
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpVelocity, rigidbody.velocity.z);
            // If wanting to move left/right launch that way
            if (moveDirection != 0) {
                horizSpeed = desiredSpeed;
            }
        
        }

        
        float acceleration = desiredSpeed > horizSpeed ? accelMultiplier : -accelMultiplier; //Get acceleration needed to get there
        // Only accelerate if on the ground. 
        if (IsGrounded())
        {
            horizSpeed += acceleration * Time.deltaTime; //Execute the acceleration
        }
        horizSpeed = Mathf.Clamp(horizSpeed, -maxSpeed, maxSpeed); //Clamp the speed at the max speed
        if ((Physics.Raycast(transform.position, Vector3.right, distToEdge + 0.1f) || Physics.Raycast(transform.position, -Vector3.right, distToEdge + 0.1f)) && !IsGrounded()) {horizSpeed = 0;} // Kill horizontal velocity on horizontal colison unless grounded 
        //Transfer horizontal velocity onto rigid body
        rigidbody.velocity = (new Vector3(horizSpeed, rigidbody.velocity.y, rigidbody.velocity.z));

        // Set Velocity to the public vector3 for viewing in the engine.
        velocity = rigidbody.velocity;
    }
    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
