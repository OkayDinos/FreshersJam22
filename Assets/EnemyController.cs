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

    int dmgDir;

    float angerValue;

    float walkVel;

    float stunTime;

    public bool toDelete;

    float foodValue;

    bool startedEating;

    [SerializeField] GameObject healthBar;

    [SerializeField] GameObject eatBar;

    [SerializeField] GameObject pickupDrop;

    // Start is called before the first frame update

    void Awake()
    {
        toDelete = false;
    }
    void Start()
    {
        foodValue = 100;

        startedEating = false;

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
                    StartWalk();
                }
                EatStuff();
                break;
            case AIState.WALKING:
                transform.position = new Vector3(transform.position.x + (Time.deltaTime * walkVel), transform.position.y, transform.position.z);
                walkLength -= Time.deltaTime;
                if (walkLength < 0)
                {
                    currentState = AIState.IDLE;
                    walkCD = 2 + (Random.value * 3);
                }
                EatStuff();
                break;
            case AIState.ATTACKING:

                break;
            case AIState.INJURED:
                stunTime -= Time.deltaTime;
                if (stunTime < 0)
                {
                    StartWalk(dmgDir);
                }
                break;
            case AIState.RUNNINGAWAY:

                break;
            default:
                break;
        }
    }

    void StartWalk(float _direction = 0)
    {
        if (_direction == 0)
        {
            flipped = (Random.value > 0.5f);
        }
        else
        {
            flipped = (_direction < 0);
        }
        
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

    public void TakeDamage(Vector3 _playerPos, float _dmg)
    {
        if (currentState == AIState.INJURED || currentState == AIState.RUNNINGAWAY)
        {
            return;
        }

        angerValue += _dmg;

        if (_playerPos.x > transform.position.x)
        {
            dmgDir = -1;
        }
        else
        {
            dmgDir = 1;
        }

        DamageReaction(_dmg, dmgDir);

        healthBar.transform.localScale = new Vector3(1-(angerValue / 100), 0.06f, 1);

        if (angerValue < 100)
        {
            stunTime = 0.67f;

            currentState = AIState.INJURED;
        }
        else
        {
            Runaway(dmgDir);
            currentState = AIState.RUNNINGAWAY;
        }
    }

    async void Runaway(int _direction)
    {
        Drops();

        healthBar.SetActive(false);

        eatBar.SetActive(false);

        float time = 1.5f;

        float timer = 0;

        while (timer < time)
        {
            timer += Time.deltaTime;

            transform.position = new Vector3(transform.position.x + (_direction * Time.deltaTime * 20), transform.position.y, transform.position.z);

            await System.Threading.Tasks.Task.Yield();
        }

        toDelete = true;
    }

    async void DamageReaction(float _strength, int _direction)
    {
        float time = 0.5f;

        float timer = 0;

        while (timer < time)
        {
            timer += Time.deltaTime;

            float scale = Mathf.Lerp(0.85f, 1, timer / time);

            transform.localScale = new Vector3(scale, scale * 2, scale);

            GetComponent<SpriteRenderer>().color = new Color(1, Mathf.Lerp(0f, 1, timer / time), Mathf.Lerp(0, 1, timer / time));

            transform.position = new Vector3(transform.position.x + (_direction * Time.deltaTime * Mathf.Lerp(4, 0, timer / time)), transform.position.y, transform.position.z);

            await System.Threading.Tasks.Task.Yield();
        }

        transform.localScale = new Vector3(1,2,1);
    }

    void Drops()
    {
        if (foodValue > 0)
        {
            GameObject drop = Instantiate(pickupDrop, transform.position, Quaternion.identity);

            drop.GetComponent<Pickup>().OnDropped(foodValue);
        }
    }

    void EatStuff()
    {
        if (startedEating == false)
        {
            if (Mathf.Abs(transform.position.x - WorldManager.singleton.playerRef.transform.position.x) < 10f)
            {
                startedEating = true;
            }
        }   
        else
        {
            if (foodValue > 0)
            {
                foodValue -= Time.deltaTime * 10;

                eatBar.transform.localScale = new Vector3(foodValue / 100, 0.06f, 1);
            }
            else
            {
                int runawayDir = 1;

                if (transform.position.x < WorldManager.singleton.playerRef.transform.position.x)
                {
                    runawayDir = -1;
                }

                Runaway(runawayDir);
                currentState = AIState.RUNNINGAWAY;
            }
        }
    }
}
