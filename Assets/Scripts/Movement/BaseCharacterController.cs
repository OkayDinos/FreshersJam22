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
    public float maxSpeed, maxZSpeed; // The maximum speed you can run 
    public Vector2 zBoundaries; // boundaries for the character on the z plane
    public float accelMultiplier; // How fast you reach the max speed 
    public float jumpVelocity; // how much ms-1 you jump up at
    float horizSpeed, depthSpeed; // Velocity on the X axis
    float desiredSpeed, desiredZSpeed; // Desired speed of travel, so if half out on joystick, half of max speed
    Rigidbody kevinRigidbody; // Rigidbody of the character
    //Transform playerTransform; // Transform of the character
    Collider playerCollider; // Collider of the character
    SpriteRenderer playerSprite; // Player Sprite Renderer
    float distToGround, distToEdge; // Distances to the edges of the collider (X&Y axis)
    public float airControlMultipler;
    public List<Sprite> animationSprites = new List<Sprite>();
    float timeSinceLastFrame;
    int currentFrame;

    public Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        currentFrame = 0;
        timeSinceLastFrame = 0;
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

        // Get the X/Z - Axis input
        var moveDirection = moveAction.ReadValue<Vector2>();
        var xMove = moveDirection.x;
        var zMove = moveDirection.y;

        // Calculate speed accounting for changes in desired speed (maxSpeed * moveDirection)

        desiredSpeed = maxSpeed * xMove; //Get target speed
        desiredZSpeed = maxZSpeed * zMove;

        // Jump Action
        if (jumpAction.ReadValue<float>() == 1 && IsGrounded())
        {
            kevinRigidbody.velocity = new Vector3(kevinRigidbody.velocity.x, jumpVelocity, kevinRigidbody.velocity.z);
            // If wanting to move left/right launch that way
            if (xMove != 0) {
                // Little funky calculation stops extreme launches
                horizSpeed = horizSpeed / desiredSpeed * desiredSpeed;
                //depthSpeed = depthSpeed / desiredZSpeed * desiredZSpeed;
            }
            if (zMove != 0)
            {
                // Little funky calculation stops extreme launches
                //horizSpeed = horizSpeed / desiredSpeed * desiredSpeed;
                depthSpeed = depthSpeed / desiredZSpeed * desiredZSpeed;
            }

        }

        // Change sprite facing direction
        if (xMove > 0 && IsGrounded())
        {
            playerSprite.flipX = false;
        }
        else if (xMove < 0 && IsGrounded()) {
            playerSprite.flipX = true;
        }

        if (xMove == 0 && IsGrounded()) { 
            playerSprite.sprite = animationSprites[currentFrame];
        }
        else if (xMove != 0 && IsGrounded()) { 
            playerSprite.sprite = animationSprites[currentFrame+1]; 
        }

        if (timeSinceLastFrame >= (0.4 * (maxSpeed/(Mathf.Abs(horizSpeed)+maxSpeed)))) {
            currentFrame = currentFrame == 1 ? 0 : 1;
            timeSinceLastFrame = 0;
        }
        float acceleration = desiredSpeed > horizSpeed ? accelMultiplier : -accelMultiplier; //Get acceleration needed to get there
        float depthAcceleration = desiredZSpeed > depthSpeed ? accelMultiplier : -accelMultiplier; //Get acceleration needed to get there
        // Only accelerate if on the ground. 
        if (IsGrounded())
        {
            horizSpeed += acceleration * Time.deltaTime; //Execute the acceleration
            depthSpeed += depthAcceleration * Time.deltaTime;
        }
        else {
            horizSpeed += acceleration * airControlMultipler * Time.deltaTime; //Execute the acceleration
            depthSpeed += depthAcceleration * airControlMultipler * Time.deltaTime;
        }
        

        horizSpeed = Mathf.Clamp(horizSpeed, -maxSpeed, maxSpeed); //Clamp the speed at the max speed
        depthSpeed = Mathf.Clamp(depthSpeed, -maxZSpeed, maxZSpeed); //Clamp the speed at the max speed
        
        if (Physics.Raycast(transform.position, Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed > 0 ? 0 : horizSpeed; } // stop horizontal velocity when going right
        if (Physics.Raycast(transform.position, -Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed < 0 ? 0 : horizSpeed; ; } // stop horizontal velocity when going left

        // Stops Z being more or less than max and min
        float clampedZ;
        clampedZ = Mathf.Clamp(this.transform.position.z, zBoundaries.x, zBoundaries.y);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, clampedZ);

        //Transfer horizontal velocity onto rigid body
        kevinRigidbody.velocity = (new Vector3(horizSpeed, kevinRigidbody.velocity.y, depthSpeed));
        
        // Set Velocity to the public vector3 for viewing in the engine.
        velocity = kevinRigidbody.velocity;

        // Set camera position
        cameraTransform.position = new Vector3(this.transform.position.x, 3, -10);

        // Add DT to time of last frame
        timeSinceLastFrame += Time.deltaTime;
    }

    // Little ground checker.
    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

}
