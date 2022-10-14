using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    [SerializeField] Image fillRef;

    public void UpdateHungerBar(float _percentage)
    {
        GetComponent<Slider>().value = _percentage;

        if (_percentage > 0.5f)
        {
            fillRef.color = Color.Lerp(Color.yellow, Color.white, (_percentage - 0.5f) * 2);
        }
        else
        {
            fillRef.color = Color.Lerp(Color.red, Color.yellow, _percentage * 2);
        }
    }
}