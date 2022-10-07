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
    float horizSpeed;
    float desiredSpeed;
    Rigidbody rigidbody;
    Transform transform;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        moveAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Get the X - Axis input
        var moveDirection = moveAction.ReadValue<float>();

        // Calculate speed accounting for changes in desired speed (maxSpeed * moveDirection)
        desiredSpeed = maxSpeed * moveDirection; //Get target speed
        float acceleration = desiredSpeed > horizSpeed ? accelMultiplier : -accelMultiplier; //Get acceleration needed to get there
        horizSpeed += acceleration * Time.deltaTime; //Execute the acceleration
        horizSpeed = Mathf.Clamp(horizSpeed, -maxSpeed, maxSpeed); //Clamp the speed at the max speed
        
        //Transfer horizontal velocity onto rigid body
        rigidbody.velocity = (new Vector3(horizSpeed, rigidbody.velocity.y, rigidbody.velocity.z));

        // Set Velocity to the public vector3 for viewing in the engine.
        velocity = rigidbody.velocity;
    }

}
