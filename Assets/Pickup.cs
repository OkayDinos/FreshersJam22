using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    void Awake()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnDropped();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPickedUp()
    {
        Destroy(gameObject);
    }

    public async void OnDropped()
    {
        float time = 1;

        float timer = 0;

        float dir = 1;

        transform.localRotation = Quaternion.Euler(0, 0, (Random.value * 90f) - 45f);

        if (Random.value > 0.5f)
        {
            dir = -1;
        }

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
