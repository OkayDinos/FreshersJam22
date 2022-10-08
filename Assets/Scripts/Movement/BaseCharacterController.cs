using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCharacterController : MonoBehaviour
{

    //public SimpleControls Input;
    public InputAction moveAction; //Move inputs
    public InputAction jumpAction; //Jump inputs
    public Vector3 velocity; // Velocity 
    public float maxSpeed; // The maximum speed you can run 
    public float accelMultiplier; // How fast you reach the max speed 
    public float jumpVelocity; // how much ms-1 you jump up at
    float horizSpeed; // Velocity on the X axis
    float desiredSpeed; // Desired speed of travel, so if half out on joystick, half of max speed
    Rigidbody rigidbody; // Rigidbody of the character
    Transform transform; // Transform of the character
    Collider collider; // Collider of the character
    float distToGround, distToEdge; // Distances to the edges of the collider (X&Y axis)

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
                // Little funky calculation stops extreme launches
                horizSpeed = horizSpeed / desiredSpeed * desiredSpeed;
            }
        
        }

        
        float acceleration = desiredSpeed > horizSpeed ? accelMultiplier : -accelMultiplier; //Get acceleration needed to get there
        // Only accelerate if on the ground. 
        if (IsGrounded())
        {
            horizSpeed += acceleration * Time.deltaTime; //Execute the acceleration
        }
        horizSpeed = Mathf.Clamp(horizSpeed, -maxSpeed, maxSpeed); //Clamp the speed at the max speed
        
        // * Need to fix cannot launch jump at wall make this directional 
        if ((Physics.Raycast(transform.position, Vector3.right, distToEdge + 0.1f) || Physics.Raycast(transform.position, -Vector3.right, distToEdge + 0.1f)) && !IsGrounded()) {horizSpeed = 0;} // Kill horizontal velocity on horizontal colison unless grounded 
        
        //Transfer horizontal velocity onto rigid body
        rigidbody.velocity = (new Vector3(horizSpeed, rigidbody.velocity.y, rigidbody.velocity.z));

        // Set Velocity to the public vector3 for viewing in the engine.
        velocity = rigidbody.velocity;
    }

    // Little ground checker.
    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
