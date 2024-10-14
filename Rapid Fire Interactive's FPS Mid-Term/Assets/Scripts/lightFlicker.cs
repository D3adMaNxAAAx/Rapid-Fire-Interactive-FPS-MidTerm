using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class lightFlicker : MonoBehaviour
{
    [SerializeField] bool doFlicker = false;
    [Range(1, 100)] [SerializeField] float percentFlicker;
    [SerializeField] GameObject lightSys = null;
    [SerializeField] GameObject powerSys = null;
    new Light light;
    [SerializeField] new Light[] lights = null;
    float randNum;
    float randFlickSpeed;
    float lightIntensity;
    new float[] lightsIntensity;
    
    bool isFlickering = false;
    bool waitBool = false;
    int lightCount;

    int pwrLvl;
    bool isOn;
   




    private void Awake()
    {
        if (lightSys == null)
        {
            light = this.gameObject.GetComponentInChildren<Light>();
            lightIntensity = light.intensity;
        }
        else if (lightSys != null)
        { 
        lights = lightSys.gameObject.GetComponentsInChildren<Light>();
        lightCount = lights.Count();

            for (int i = 0; i < lightCount - 1; ++i)
            {
                lights[i].enabled = false;
            }
          

        }
        
    }

    // Update is called once per frame
    void Update()
    {
      

        if (powerSys == null)
        {
            randFlickSpeed = UnityEngine.Random.Range(0.2f, 1.5f);
            if (!waitBool)
                StartCoroutine(getNewNumber());
            flickerLight();
        }

        else if (powerSys != null && isOn == true)
            powerStages();

        
       
    }

    public void flickerLight()
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
        if (lightSys == null)
        {
            isFlickering = true;
            light.intensity = 0f;
            yield return new WaitForSeconds(randFlickSpeed);
            light.intensity = lightIntensity;
            yield return new WaitForSeconds(0.5f);
            isFlickering = false;
        }
        else 
        {
            isFlickering = true;
            for (int i = 0; i < lights.Count(); ++i)
            {
                lights[i].enabled = true;
                
               
            }
            yield return new WaitForSeconds(randFlickSpeed);
            for (int i = 0; i < lights.Count(); ++i)
            {
                lights[i].enabled = false;
            }
            yield return new WaitForSeconds(0.5f);
            isFlickering = false;
        }
    }
    public IEnumerator getNewNumber()
    {
        waitBool = true;
        randNum = UnityEngine.Random.Range(0f, 100f);
        yield return new WaitForSeconds(1f);
        waitBool = false;
    }

    public bool getWait()
    { return waitBool; }

    public void setRandFlickSpd(float randSpd)
    { randFlickSpeed = randSpd; }

    public void setFlicker(bool flicker)
    { doFlicker = flicker; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {


            if (gameManager.instance.getPowerItems() == 3)
            {
                pwrLvl = 1;
                isOn = true;
            }
            if (gameManager.instance.getPowerItems() == 6)
            {
                pwrLvl = 2;
            }
            if (gameManager.instance.getPowerItems() == 9)
            {
                pwrLvl = 3;
            }
            else
                return;


        }
    }
    
    void powerStages()
    {
        if (pwrLvl == 1)
        {
            setRandFlickSpd(UnityEngine.Random.Range(0.2f, 1.5f));
            if (!getWait())
                StartCoroutine(getNewNumber());
            flickerLight();
            setFlicker(true);
            
        }
        if (pwrLvl == 2)
        {
            setFlicker(false);
            for (int i = 0; i < lightCount - 1; ++i)
            {
                lights[i].enabled = true;
            }
        }
        if (pwrLvl == 3)
        {

        }
    }
}
