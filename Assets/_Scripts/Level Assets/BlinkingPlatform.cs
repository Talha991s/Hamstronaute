//Script By: Salick Talhah
//Edited By: 
//Date Created : Feburary 9th, 2021
//Date Edited : March 25th, 2021

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingPlatform : MonoBehaviour
{
    public bool isVisible;
    // Used as an offset
    public float initialBlinkTime;
    // Used as speed for blinking after offset
    public float finalBlinkTime;
    // Used for certain ones that turn back on quickly
    float quickBlinkTime = 0.55f;
    float quickBlinkTimer;

    public bool quickBlink = false;

    Vector3 startingScale;

    bool one = true;

    // Start is called before the first frame update
    void Start()
    {
        quickBlinkTimer = quickBlinkTime;
        startingScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Blink();
        Countdown();
    }

    void Countdown()
    {
        if (one)
        {
            initialBlinkTime -= Time.deltaTime;
            if (initialBlinkTime <= 0)
            {
                initialBlinkTime = finalBlinkTime;
                isVisible = !isVisible;
                one = false;
            }
        }
        if (quickBlink && !one)
        {
            quickBlinkTimer -= Time.deltaTime;
            if (quickBlinkTimer <= 0)
            {
                quickBlinkTimer = quickBlinkTime;
                isVisible = !isVisible;
                one = true;
            }
        }
        else if (!quickBlink)
        {
            one = true;
        }
    }

   
    void Blink()
    {
        transform.localScale = isVisible ? startingScale : new Vector3(0, 0, 0);
    }
}
