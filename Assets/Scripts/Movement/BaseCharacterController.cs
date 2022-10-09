using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class BaseCharacterController : MonoBehaviour
{


    //public SimpleControls Input;
    public InputAction moveAction; //Move inputs
    public InputAction jumpAction; //Jump input
    public InputAction attackAction; //Attack input

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
    Vector3 colliderCenter; // Centerpoint of the collider 

    //float distToGround, distToEdge; // Distances to the edges of the collider (X&Y axis)
    public Camera cameraRef;
    bool flipped;
    bool controlsDDisabled; // If the controls are disabled
    bool attackActive; // If the attack is active

    float timeSinceLastAttack; // time in s since last attacking move
    bool wasLastAttackKick; //false if punch true if kick
    float lastAttackButton, comboPath; //Stops holding buttons for attacks | holds a number if it will be a multi move combo
    

    float hunger, hungerMax; // Hunger of the character
    float timeAlive; // Time alive

    [SerializeField] GameObject hungerBar; // Health bar prefab

    // Start is called before the first frame update
    void Start()
    {
        timeAlive = 0;
        hungerMax = 100;
        hunger = hungerMax;
        flipped = false;
        attackActive = false;
        controlsDDisabled = true;
        cameraRef = FindObjectOfType<Camera>();
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
        attackAction.Enable();
        distToGround = playerCollider.bounds.extents.y;
        distToEdge = playerCollider.bounds.extents.x;
        distToDepthEdge = playerCollider.bounds.extents.z;

        lastAttackButton = 0;

        hungerBar.GetComponent<HungerBar>().UpdateHungerBar(hunger/ hungerMax);
    }

    // Update is called once per frame
    void Update()
    {
        //Check for pickups
        CheckPickup();

        //Update Hunfer
        GetHungry();

        // Attack input function
        if (attackAction.ReadValue<float>() == 1 && !attackActive && !controlsDDisabled && timeSinceLastAttack > 0.2 && lastAttackButton == 0)
        {
            attackActive = true;
            Attack();
        }

        // If grounded set used flap to 0 
        usedFlap = IsGrounded() ? false : usedFlap;

        // Create the X/Z - Axis input
        float xMove = 0;
        float zMove = 0;

        if (!controlsDDisabled) // If controls enabled
        {
            // Get the X/Z - Axis input
            Vector2 moveDirection = moveAction.ReadValue<Vector2>();
            xMove = moveDirection.x;
            zMove = moveDirection.y;
        }

        // Calculate speed accounting for changes in desired speed (maxSpeed * moveDirection)

        desiredSpeed = maxSpeed * xMove; //Get target speed
        desiredZSpeed = maxZSpeed * zMove;

        // Jump Action
        //Check if flap allowed. (must let go of jump button and press again to flap)
        if (jumpAction.ReadValue<float>() == 0 && !IsGrounded()) {
            flapActive = true;
        }
        //bool groundedAtTimeOfJump = IsGrounded();
        if (jumpAction.ReadValue<float>() == 1 && (IsGrounded() || (!usedFlap && flapActive)) && !controlsDDisabled)
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

        colliderCenter = playerCollider.bounds.center; //transform.position + new Vector3(0, 0.741446f, 0); //+ playerCollider.bounds.center;

        //Stop Velocity at X collisions
        if (Physics.Raycast(colliderCenter, Vector3.right, distToEdge + 0.1f) || Physics.Raycast(colliderCenter - Vector3.up * (distToGround - 0.1f), Vector3.right, distToEdge + 0.1f) || Physics.Raycast(colliderCenter + Vector3.up * distToGround, Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed > 0 ? 0 : horizSpeed; } // stop horizontal velocity when going right
        if (Physics.Raycast(colliderCenter, -Vector3.right, distToEdge + 0.1f) || Physics.Raycast(colliderCenter - Vector3.up * (distToGround - 0.1f), -Vector3.right, distToEdge + 0.1f) || Physics.Raycast(colliderCenter + Vector3.up * distToGround, -Vector3.right, distToEdge + 0.1f)) { horizSpeed = horizSpeed < 0 ? 0 : horizSpeed; ; } // stop horizontal velocity when going left

        //Stop Velocity at Z collisions
        if (Physics.Raycast(colliderCenter, Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(colliderCenter - Vector3.up * (distToGround - 0.1f), Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(colliderCenter + Vector3.up * distToGround, Vector3.forward, distToDepthEdge + 0.1f)) { depthSpeed = depthSpeed > 0 ? 0 : depthSpeed; } // stop horizontal velocity when going right
        if (Physics.Raycast(colliderCenter, -Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(colliderCenter - Vector3.up * (distToGround - 0.1f), -Vector3.forward, distToDepthEdge + 0.1f) || Physics.Raycast(colliderCenter + Vector3.up * distToGround, -Vector3.forward, distToDepthEdge + 0.1f)) { depthSpeed = depthSpeed < 0 ? 0 : depthSpeed; ; } // stop horizontal velocity when going left

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
        // Add DT to time since last attack
        timeSinceLastAttack += Time.deltaTime;

        //For gliding set previous x move to the current one | Not used
        lastAttackButton = attackAction.ReadValue<float>();


    }
    private void FixedUpdate()
    {
        // Set camera position
        if (this.transform.position.x >= cameraRef.transform.position.x + 3)
        {
            //cameraTransform.position = new Vector3(this.transform.position.x - 3, 3, -10);
            Vector3 targCamPos = new Vector3(this.transform.position.x, 3, -6.2f);
            //Vector3 camVelocity = new Vector3(3, 0, 0);//Mathf.Abs(targCamPos.x - cameraTransform.position.x)
            cameraRef.transform.position = Vector3.Lerp(cameraRef.transform.position, targCamPos, cameraSpeed * Time.deltaTime);
        }
        else if (this.transform.position.x <= cameraRef.transform.position.x - 3)
        {
            Vector3 targCamPos = new Vector3(this.transform.position.x, 3, -6.2f);
            //Vector3 camVelocity = new Vector3(targCamPos.x - cameraTransform.position.x - 3, 0, 0);
            cameraRef.transform.position = Vector3.Lerp(cameraRef.transform.position, targCamPos, cameraSpeed * Time.deltaTime);//Vector3.SmoothDamp(cameraTransform.position, targCamPos, ref camVelocity, 0.3f);
        }
    }
    // Little ground checker.
    bool IsGrounded() {
        //Debug.DrawRay(colliderCenter, new Vector3(0, -(distToGround + 0.1f), 0),Color.green);
        return (Physics.Raycast(colliderCenter, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(colliderCenter - Vector3.right * distToEdge - Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(colliderCenter + Vector3.right * distToEdge - Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(colliderCenter - Vector3.right * distToEdge + Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f) || Physics.Raycast(colliderCenter + Vector3.right * distToEdge + Vector3.forward * distToDepthEdge, -Vector3.up, distToGround + 0.1f));
    }

    public async void Begin(float _startPos)
    {
        float time = 1;
        
        float timer = 0;

        cameraRef.transform.SetPositionAndRotation(new Vector3(_startPos, 0.5f, 0.3f), Quaternion.identity);

        while (timer < time)
        {
            timer += Time.deltaTime;

            await System.Threading.Tasks.Task.Yield();
        }

        timer = 0;

        time = 2;

        while (timer < time)
        {
            timer += Time.deltaTime;

            cameraRef.transform.SetPositionAndRotation(new Vector3(_startPos, Mathf.Lerp(0.5f, 3, timer/ time), Mathf.Lerp(0.3f, -6.2f, timer/ time)), Quaternion.identity);

            await System.Threading.Tasks.Task.Yield();
        }

        controlsDDisabled = false;

        cameraRef.transform.SetPositionAndRotation(new Vector3(_startPos, 3, -6.2f), Quaternion.identity);
    }

    public async void Attack()
    {
        PointsType attackType = PointsType.NormalAttack;
        float time = 0.2f;
        float damageThisTime  = 25;
        //Combo Checker
        if (wasLastAttackKick && timeSinceLastAttack < 1.5 && this.transform.position.y <= 1.2)
        {
            damageThisTime = 50;
            comboPath = 0;
            playerSprite.color = new Color(255,200,0);
            attackType = PointsType.Uppercut;
        }
        else if (!wasLastAttackKick && timeSinceLastAttack < 0.5 && this.transform.position.y <= 1.2 && comboPath == 0) {
            comboPath = 1;
        }
        else if(!wasLastAttackKick && timeSinceLastAttack < 0.5 && this.transform.position.y <= 1.2 && comboPath == 1)
        {
            damageThisTime = 50;
            playerSprite.color = new Color(255, 200, 0);
            comboPath = 0;
            attackType = PointsType.FourForThree;
        }
        else
        {
            comboPath = 0;
        }

        timeSinceLastAttack = 0;

        float timer = 0;

        Sprite lastSprite = playerSprite.sprite;
        Sprite attackSprite;
        if (this.transform.position.y > 1.2)
        {
            attackSprite = animationSprites[4];
            wasLastAttackKick = true;
        }
        else
        {
            attackSprite = animationSprites[5];
            wasLastAttackKick = false;  
        }
        float atkDir = 1;

        if (flipped == true)
        {
            atkDir = -1;
        }

        while (timer < time)
        {
            timer += Time.deltaTime;

            playerSprite.sprite = attackSprite;

            Collider[] hit = Physics.OverlapBox(transform.position + new Vector3(distToEdge * atkDir, 0, 0), new Vector3(distToEdge, distToGround, 1), Quaternion.identity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide);

            foreach (Collider col in hit)
            {
                if (col.tag == "Enemy")
                {
                    col.GetComponent<EnemyController>().TakeDamage(transform.position, damageThisTime, attackType); // second argument is damage
                }
            }

            await System.Threading.Tasks.Task.Yield();
        }

        playerSprite.sprite = lastSprite;
        playerSprite.color = new Color(255, 255, 255);

        attackActive = false;
    }

    void CheckPickup()
    {
        Collider[] hit = Physics.OverlapBox(transform.position, new Vector3(distToEdge, distToGround, 1), Quaternion.identity, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide);

        foreach (Collider col in hit)
        {
            if (col.tag == "Pickup")
            {
                switch (col.GetComponent<Pickup>().pickupType)
                {
                    case PickupType.SAUSAGEROLL:
                        float hungerMultiplier = ((7 - (float)col.GetComponent<Pickup>().sausageState) / 7);
                        hunger += 30 * hungerMultiplier;
                        hunger = Mathf.Clamp(hunger, 0, hungerMax);
                        GameManager.instance.AddScore(PointsType.EatSausageRoll, hungerMultiplier);
                        break;
                    case PickupType.WRAPPER:
                        GameManager.instance.AddScore(PointsType.WrapperPickup);
                        break;
                    default:
                        break;
                }

                col.GetComponent<Pickup>().OnPickedUp();
            }
        }
    }

    void GetHungry()
    {
        timeAlive += Time.deltaTime;

        hunger -= timeAlive * 0.0003f;

        hungerBar.GetComponent<HungerBar>().UpdateHungerBar(hunger/ hungerMax);

        if (hunger <= 0)
        {
            // It's okay, Kevin knows karate
            GameManager.instance.GameOver();

            controlsDDisabled = true;
        }
    }

}
