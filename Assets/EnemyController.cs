using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState { IDLE, WALKING, ATTACKING, INJURED, RUNNINGAWAY }

public class EnemyController : MonoBehaviour
{
    AIState currentState;

    float walkCD;
    float walkLength;

    bool flipped;

    float angerValue;

    float walkVel;

    // Start is called before the first frame update
    void Start()
    {
        currentState = AIState.IDLE;

        walkCD = Random.value * 5;

        flipped = (Random.value > 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AIState.IDLE:
                walkCD -= Time.deltaTime;
                if (walkCD < 0)
                {
                    flipped = (Random.value > 0.5f);
                    currentState = AIState.WALKING;
                    walkLength = 3 + (Random.value * 3);

                    if (flipped)
                    {
                        walkVel = -1 + (Random.value * -0.3f);
                    }
                    else
                    {
                        walkVel = 1 + (Random.value * 0.3f);
                    }
                }
                break;
            case AIState.WALKING:
                transform.position = new Vector3(transform.position.x + (Time.deltaTime * walkVel), transform.position.y, transform.position.z);
                walkLength -= Time.deltaTime;
                if (walkLength < 0)
                {
                    currentState = AIState.IDLE;
                    walkCD = 2 + (Random.value * 3);
                }
                break;
            case AIState.ATTACKING:
                
                break;
            case AIState.INJURED:

                break;
            case AIState.RUNNINGAWAY:

                break;
            default:
                break;
        }
    }

    void Injured()
    {
        Debug.Log("Injured");
    }
}
