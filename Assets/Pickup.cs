using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType { SAUSAGEROLL, WRAPPER }

public class Pickup : MonoBehaviour
{
    [SerializeField] List<Sprite> sausageStates = new List<Sprite>();

    int sausageState = 0;

    void Awake()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    public void OnPickedUp()
    {
        Destroy(gameObject);
    }

    void SetSausageState(float _value)
    {
        if(_value > 75f)
        {
            sausageState = 0;
        }
        else if(_value > 50f)
        {
            sausageState = 1;
        }
        else if(_value > 40f)
        {
            sausageState = 2;
        }
        else if(_value > 30f)
        {
            sausageState = 3;
        }
        else if(_value > 20f)
        {
            sausageState = 4;
        }
        else if(_value > 10f)
        {
            sausageState = 5;
        }
        else
        {
            sausageState = 6;
        }

        GetComponent<SpriteRenderer>().sprite = sausageStates[sausageState];
    }

    public async void OnDropped(PickupType _type, float _value = 0)
    {
        if (_type == PickupType.SAUSAGEROLL)
        {
            SetSausageState(_value);
        }
        else if (_type == PickupType.WRAPPER)
        {
            GetComponent<SpriteRenderer>().sprite = sausageStates[7 + Random.Range(0, 3)];
        }

        float time = 1;

        float timer = 0;

        float dir = 1;

        transform.localRotation = Quaternion.Euler(0, 0, (Random.value * 90f) - 45f);

        if (Random.value > 0.5f)
        {
            dir = -1;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);

        Vector3 originalPos = transform.position;

        while (timer < time)
        {
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(originalPos, originalPos + Vector3.up , Mathf.Sin(timer / time * 3.14f));

            transform.position += Vector3.Lerp(Vector3.zero, Vector3.right * dir, timer / time);

            await System.Threading.Tasks.Task.Yield();
        }

        GetComponent<BoxCollider>().enabled = true;
    }
}
