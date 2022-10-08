using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCharacterController : MonoBehaviour
{

    //public SimpleControls Input;
    public InputAction moveAction; //Move inputs
    public InputAction jumpAction; //Jump input
    public Vector3 velocity; // Velocity 
    public float maxSpeed; // The maximum speed you can run 
    public float accelMultiplier; // How fast you reach the max speed 
    public float jumpVelocity; // how much ms-1 you jump up at
    float horizSpeed; // Velocity on the X axis
    float desiredSpeed; // Desired speed of travel, so if half out on joystick, half of max speed
    Rigidbody kevinRigidbody; // Rigidbody of the character
    //Transform playerTransform; // Transform of the character
    Collider playerCollider; // Collider of the character
    SpriteRenderer playerSprite; // Player Sprite Renderer
    float distToGround, distToEdge; // Distances to the edges of the collider (X&Y axis)

    public Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        kevinRigidbody = GetComponent<Rigidbody>();
        //transform = GetComponent<Transform>();
        playerCollider = GetComponent<BoxCollider>();
        playerSprite = GetComponent<SpriteRenderer>();
        moveAction.Enable();
        jumpAction.Enable();
        distToGround = playerCollider.bounds.extents.y;
        distToEdge = playerCollider.bounds.extents.x;
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
            kevinRigidbody.velocity = new Vector3(kevinRigidbody.velocity.x, jumpVelocity, kevinRigidbody.velocity.z);
            // If wanting to move left/right launch that way
            if (moveDirection != 0) {
                // Little funky calculation stops extreme launches
                horizSpeed = horizSpeed / desiredSpeed * desiredSpeed;
            }
        
        }

        // Change sprite facing direction
        if (moveDirection > 0 && IsGrounded())
        {
            playerSprite.flipX = false;
        }
        else if (moveDirection < 0 && IsGrounded()) {
            playerSprite.flipX = true;
        }

            float acceleration = desiredSpeed > horizSpeed ? accelMultiplier : -accelMultiplier; //Get acceleration needed to get there
        // Only accelerate if on the ground. 
        if (IsGrounded())
        {
            horizSpeed += acceleration * Time.deltaTime; //Execute the acceleration
        }
        horizSpeed = Mathf.Clamp(horizSpeed, -maxSpeed, maxSpeed); //Clamp the speed at the max speed
        
        if (Physics.Raycast(transform.position, Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed > 0 ? 0 : horizSpeed; } // stop horizontal velocity when going right
        if (Physics.Raycast(transform.position, -Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed < 0 ? 0 : horizSpeed; ; } // stop horizontal velocity when going left
        //Transfer horizontal velocity onto rigid body
        kevinRigidbody.velocity = (new Vector3(horizSpeed, kevinRigidbody.velocity.y, kevinRigidbody.velocity.z));

        // Set Velocity to the public vector3 for viewing in the engine.
        velocity = kevinRigidbody.velocity;

        cameraTransform.position = new Vector3(this.transform.position.x, 3, -10);
    }

    // Little ground checker.
    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

}
