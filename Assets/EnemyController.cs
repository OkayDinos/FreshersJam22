using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState { IDLE, WALKING, ATTACKING, INJURED, RUNNINGAWAY }

public enum EnemySprites { IDLE = 0, WALKING1 = 1, WALKING2 = 2, RUNNING1 = 3, RUNNING2 = 4, SHOO1 = 5, SHOO2 = 6, ATTACKING1 = 7, ATTACKING2 = 8 }

public class EnemyController : MonoBehaviour
{
    AIState currentState;

    EnemySprites currentSprite;

    float stepCD;

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

    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] List<Sprite> grandmaSprites = new List<Sprite>();
    [SerializeField] List<Sprite> grandpaSprites = new List<Sprite>();

    List<Sprite> activeSprites = new List<Sprite>();

    // Start is called before the first frame update

    void Awake()
    {
        toDelete = false;
    }

    void Start()
    {
        activeSprites = grandmaSprites;

        if (Random.value < 0.5f)
        {
            activeSprites = grandpaSprites;
        }

        stepCD = 0.2f;

        foodValue = 100;

        startedEating = false;

        currentState = AIState.IDLE;

        walkCD = Random.value * 5;

        flipped = (Random.value > 0.5f);

        SetSprite(EnemySprites.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AIState.IDLE:
                walkCD -= Time.deltaTime;
                SetSprite(EnemySprites.IDLE);
                if (walkCD < 0)
                {
                    SetSprite(EnemySprites.WALKING1);
                    ChangeDirection(0);
                    StartWalk();
                }
                EatStuff();
                break;
            case AIState.WALKING:
                transform.position = new Vector3(transform.position.x + (Time.deltaTime * walkVel), transform.position.y, transform.position.z);
                walkLength -= Time.deltaTime;

                stepCD -= Time.deltaTime;
                if (stepCD < 0)
                {
                    if (currentSprite == EnemySprites.WALKING1)
                    {
                        SetSprite(EnemySprites.WALKING2);
                    }
                    else
                    {
                        SetSprite(EnemySprites.WALKING1);
                    }
                    stepCD += 0.2f;
                }

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
                    ChangeDirection(dmgDir);
                    StartWalk(dmgDir);
                }
                break;
            case AIState.RUNNINGAWAY:
                stepCD -= Time.deltaTime;
                if (stepCD < 0)
                {
                    if (currentSprite == EnemySprites.RUNNING1)
                    {
                        SetSprite(EnemySprites.RUNNING2);
                    }
                    else
                    {
                        SetSprite(EnemySprites.RUNNING1);
                    }
                    stepCD += 0.2f;
                }
                break;
            default:
                break;
        }
    }

    void StartWalk(float _direction = 0)
    {   
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

    public void TakeDamage(Vector3 _playerPos, float _dmg, PointsType attackType)
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

        ChangeDirection(dmgDir);

        DamageReaction(_dmg, dmgDir);

        healthBar.transform.localScale = new Vector3(1-(angerValue / 100), 0.06f, 1);

        if (angerValue < 100)
        {
            stunTime = 0.2f;

            currentState = AIState.INJURED;

            SetSprite(EnemySprites.IDLE);
        }
        else
        {
            ChangeDirection(dmgDir);
            Runaway(dmgDir);
            currentState = AIState.RUNNINGAWAY;

            SetSprite(EnemySprites.RUNNING1);

            GameManager.instance.AddScore(attackType);
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

            spriteRenderer.GetComponent<SpriteRenderer>().color = new Color(1, Mathf.Lerp(0f, 1, timer / time), Mathf.Lerp(0, 1, timer / time));

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

            drop.GetComponent<Pickup>().OnDropped(PickupType.SAUSAGEROLL,foodValue);
        }
        else
        {
            GameObject drop = Instantiate(pickupDrop, transform.position, Quaternion.identity);

            drop.GetComponent<Pickup>().OnDropped(PickupType.WRAPPER);
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
                ChangeDirection(runawayDir);
                currentState = AIState.RUNNINGAWAY;
            }
        }
    }

    void SetSprite(EnemySprites _sprite)
    {
        spriteRenderer.sprite = activeSprites[(int)_sprite];
        currentSprite = _sprite;
    }

    void ChangeDirection(int _direction = 0)
    {
        if (_direction == 0)
        {
            _direction = 1;
            if (Random.value > 0.5f)
            {
                _direction = -1;
            }
        }

        if (_direction == 1)
        {
            spriteRenderer.flipX = true;
            flipped = false;
        }
        else if (_direction == -1)
        {
            spriteRenderer.flipX = false;
            flipped = true;
        }
    }
}
