using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightFlicker : MonoBehaviour
{
    [SerializeField] bool doFlicker = false;
    [Range(1, 100)] [SerializeField] float percentFlicker;
    new Light light;
    float randNum;
    float randFlickSpeed;
    float lightIntensity;
    bool isFlickering = false;
    bool waitBool = false;

    private void Start()
    {
        light = this.gameObject.GetComponentInChildren<Light>();
        lightIntensity = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        randFlickSpeed = UnityEngine.Random.Range(0.2f, 1.5f);
        if (!waitBool)
            StartCoroutine(getNewNumber());
        flickerLight();
    }

    private void flickerLight()
    {
        if(doFlicker && !isFlickering)
        {
            if(randNum <= percentFlicker)
            {
                StartCoroutine(lightAction());
            }
        }
    }

    IEnumerator lightAction()
    {
        isFlickering = true;
        light.intensity = 0f;
        yield return new WaitForSeconds(randFlickSpeed);
        light.intensity = lightIntensity;
        yield return new WaitForSeconds(0.5f);
        isFlickering = false;
    }
    IEnumerator getNewNumber()
    {
        waitBool = true;
        randNum = UnityEngine.Random.Range(0f, 100f);
        yield return new WaitForSeconds(1f);
        waitBool = false;
    }
}
