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
    public float jumpVelocity, glideSpeed; // how much ms-1 you jump up at | gravatational acceleration when gliding
    float horizSpeed, depthSpeed; // Velocity on the X axis
    float desiredSpeed, desiredZSpeed; // Desired speed of travel, so if half out on joystick, half of max speed
    Rigidbody kevinRigidbody; // Rigidbody of the character
    //Transform playerTransform; // Transform of the character
    Collider playerCollider; // Collider of the character
    SpriteRenderer playerSprite; // Player Sprite Renderer
    float distToGround, distToEdge, distToDepthEdge; // Distances to the edges of the collider (X&Y&Z axis)
    public float airControlMultipler; // Multiplyer of acceleration whilst mid air, might change to glide 
    public List<Sprite> animationSprites = new List<Sprite>(); // initialise sprite array of frames
    float timeSinceLastFrame; // time since last frame alternate
    int currentFrame; // All animations bear 2 frames this inicates which one we are on 
    bool usedFlap, flapActive; // true if player has flapped this jump | if space let go mid air then allow flap
    public float cameraSpeed; // Speed of the cmera moving 
    //float previousXMove;
    public Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        //previousXMove = 0;
        flapActive = false;
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
        distToDepthEdge = playerCollider.bounds.extents.z;
    }

    // Update is called once per frame
    void Update()
    {

        // If grounded set used flap to 0 
        usedFlap = IsGrounded() ? false : usedFlap;

        // Get the X/Z - Axis input
        var moveDirection = moveAction.ReadValue<Vector2>();
        var xMove = moveDirection.x;
        var zMove = moveDirection.y;

        // Calculate speed accounting for changes in desired speed (maxSpeed * moveDirection)

        desiredSpeed = maxSpeed * xMove; //Get target speed
        desiredZSpeed = maxZSpeed * zMove;

        // Jump Action
        //Check if flap allowed. (must let go of jump button and press again to flap)
        if (jumpAction.ReadValue<float>() == 0 && !IsGrounded()) {
            flapActive = true;
        }
        //bool groundedAtTimeOfJump = IsGrounded();
        if (jumpAction.ReadValue<float>() == 1 && (IsGrounded() || (!usedFlap && flapActive)))
        {
            //Flapping not allowed until jump button released
            flapActive = false;
            //If jump was allowed and you are not on ground must be a flap execute flap code. 
            if (!IsGrounded())
            {
                usedFlap = true;
                playerSprite.sprite = animationSprites[6]; //if flapped change sprite to flapped sprite
            }
            kevinRigidbody.velocity = new Vector3(kevinRigidbody.velocity.x, jumpVelocity, kevinRigidbody.velocity.z);
            // If wanting to move left/right launch that way
            if (xMove != 0) {
                // Little funky calculation stops extreme launches
                horizSpeed = horizSpeed / desiredSpeed * desiredSpeed;
            }
            if (zMove != 0)
            {
                // Little funky calculation stops extreme launches
                depthSpeed = depthSpeed / desiredZSpeed * desiredZSpeed;
            }


        }
        // Gliding mechanic 
        //float glideAccel = 0;
        if (!IsGrounded() && xMove != 0 && kevinRigidbody.velocity.y <= 0) {
            playerSprite.sprite = animationSprites[7];

            // Allow sprite flip while gliding 
            if (xMove > 0)
            {
                playerSprite.flipX = false;
            } else if (xMove < 0) { playerSprite.flipX = true; }


            kevinRigidbody.useGravity = false;
            //glideAccel = glideGravity * Time.deltaTime;
            kevinRigidbody.velocity = new Vector3(kevinRigidbody.velocity.x, glideSpeed, kevinRigidbody.velocity.z);
        } else { kevinRigidbody.useGravity = true; }

        if (!IsGrounded() && xMove == 0 && kevinRigidbody.velocity.y <= 0)
        {
            playerSprite.sprite = animationSprites[2]; //Walk 1 as fall sprite
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
            playerSprite.sprite = animationSprites[currentFrame + 1];
        }

        if (timeSinceLastFrame >= (0.4 * (maxSpeed / (Mathf.Abs(horizSpeed) + maxSpeed)))) {
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
        else if (kevinRigidbody.velocity.y <= 0) {
            horizSpeed += acceleration * airControlMultipler * Time.deltaTime; //Execute the acceleration
            depthSpeed += depthAcceleration * airControlMultipler * Time.deltaTime;
        }


        horizSpeed = Mathf.Clamp(horizSpeed, -maxSpeed, maxSpeed); //Clamp the speed at the max speed
        depthSpeed = Mathf.Clamp(depthSpeed, -maxZSpeed, maxZSpeed); //Clamp the speed at the max speed

        //Stop Velocity at X collisions
        if (Physics.Raycast(transform.position, Vector3.right, distToEdge + 0.1f) || Physics.Raycast(transform.position - Vector3.up * (distToGround - 0.1f), Vector3.right, distToEdge + 0.1f) || Physics.Raycast(transform.position + Vector3.up * distToGround, Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed > 0 ? 0 : horizSpeed; } // stop horizontal velocity when going right
        if (Physics.Raycast(transform.position, -Vector3.right, distToEdge + 0.1f) || Physics.Raycast(transform.position - Vector3.up * (distToGround - 0.1f), -Vector3.right, distToEdge + 0.1f) || Physics.Raycast(transform.position + Vector3.up * distToGround, -Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed < 0 ? 0 : horizSpeed; ; } // stop horizontal velocity when going left

        //Stop Velocity at Z collisions
        if (Physics.Raycast(transform.position, Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(transform.position - Vector3.up * (distToGround - 0.1f), Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(transform.position + Vector3.up * distToGround, Vector3.forward, distToDepthEdge + 0.1f)) { depthSpeed = depthSpeed > 0 ? 0 : depthSpeed; } // stop horizontal velocity when going right
        if (Physics.Raycast(transform.position, -Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(transform.position - Vector3.up * (distToGround - 0.1f), -Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(transform.position + Vector3.up * distToGround, -Vector3.forward, distToDepthEdge + 0.1f)) { depthSpeed = depthSpeed < 0 ? 0 : depthSpeed; ; } // stop horizontal velocity when going left

        // Stops Z being more or less than max and min
        float clampedZ;
        clampedZ = Mathf.Clamp(this.transform.position.z, zBoundaries.x, zBoundaries.y);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, clampedZ);

        //Transfer horizontal velocity onto rigid body
        kevinRigidbody.velocity = (new Vector3(horizSpeed, kevinRigidbody.velocity.y, depthSpeed));

        // Set Velocity to the public vector3 for viewing in the engine.
        velocity = kevinRigidbody.velocity;

        
        // Add DT to time of last frame
        timeSinceLastFrame += Time.deltaTime;

        //For gliding set previous x move to the current one
        //previousXMove = xMove;
        
    }
    private void FixedUpdate()
    {
        // Set camera position
        if (this.transform.position.x >= cameraTransform.position.x + 3)
        {
            //cameraTransform.position = new Vector3(this.transform.position.x - 3, 3, -10);
            Vector3 targCamPos = new Vector3(this.transform.position.x, 3, -10);
            //Vector3 camVelocity = new Vector3(3, 0, 0);//Mathf.Abs(targCamPos.x - cameraTransform.position.x)
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targCamPos, cameraSpeed * Time.deltaTime);
        }
        else if (this.transform.position.x <= cameraTransform.position.x - 3)
        {
            Vector3 targCamPos = new Vector3(this.transform.position.x, 3, -10);
            //Vector3 camVelocity = new Vector3(targCamPos.x - cameraTransform.position.x - 3, 0, 0);
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targCamPos, cameraSpeed * Time.deltaTime);//Vector3.SmoothDamp(cameraTransform.position, targCamPos, ref camVelocity, 0.3f);
        }
    }
    // Little ground checker.
    bool IsGrounded() {
        return (Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(transform.position - Vector3.right * distToEdge - Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(transform.position + Vector3.right * distToEdge - Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(transform.position - Vector3.right * distToEdge + Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(transform.position + Vector3.right * distToEdge + Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f));
    }

}
