using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


// Despite the name of this script, it is also the main way that the power system works! Therefore, it must be interactable!
public class lightFlicker : MonoBehaviour, IInteractable
{
    [SerializeField] bool doFlicker = false;
    [Range(1, 100)] [SerializeField] float percentFlicker;
    [SerializeField] GameObject lightSys = null;
    [SerializeField] GameObject powerSys = null;
    Light _light; // This was previously named light which was the same name of a variable it was inheriting from.
    [SerializeField] Light[] lights = null;
    float randNum;
    float randFlickSpeed;
    float lightIntensity;
    float[] lightsIntensity;
    bool isFlickering = false;
    bool waitBool = false;
    int lightCount;
    int pwrLvl;
    bool isOn;
    bool safeAccess;

    private void Awake()
    {
        if (lightSys == null)
        {
            _light = this.gameObject.GetComponentInChildren<Light>();
            if (_light != null)
                lightIntensity = _light.intensity;
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
        if (lightSys == null && doFlicker)
        {
            isFlickering = true;
            _light.intensity = 0f;
            yield return new WaitForSeconds(randFlickSpeed);
            _light.intensity = lightIntensity;
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

    public void interact()
    {
        
        if (gameManager.instance.getPowerItems() >= 9)
        {
            pwrLvl = 3;
            if (playerStats.Stats.getPWRLevel() == 2)
                playerStats.Stats.pwrLevel();
        }
        else if (gameManager.instance.getPowerItems() >= 6)
        {
            pwrLvl = 2;
            if (playerStats.Stats.getPWRLevel() == 1)
                playerStats.Stats.pwrLevel();
        }
        else if (gameManager.instance.getPowerItems() >= 3)
        {
            pwrLvl = 1;
            isOn = true;
            if (playerStats.Stats.getPWRLevel() < 1)
                playerStats.Stats.pwrLevel();
        }
        else
            return;
    }

    private void OnTriggerStay(Collider other)
    {
        // If interact menu isn't on, turn it on.
        if (!gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(true);

        if (other.CompareTag("Player"))
        {
            if (Input.GetButton("Interact")) { interact(); }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Turn off the interact UI if the player isn't within range
        if (gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(false);
    }

    void powerStages()
    {
        if (powerSys != null)
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
                //elevator on 
                //setElevatorAccess(true);

            }

            if (pwrLvl >= 1) 
            {
                if (!safeRoom.instance.getSafeAccess())
                    safeRoom.instance.setSafeAccess(true);
            }
        }
    }
}
