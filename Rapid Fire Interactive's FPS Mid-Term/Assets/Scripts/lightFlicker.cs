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
    static bool foundPower = false;

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
        {
            powerStages();
        }
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

    public bool getWait()
    { return waitBool; }

    public void setRandFlickSpd(float randSpd)
    { randFlickSpeed = randSpd; }

    public void setFlicker(bool flicker)
    { doFlicker = flicker; }

    public void interact()
    {
        int calcPwrLvl = (int)Math.Floor((double)gameManager.instance.getPowerItems() / 3);

        // Only fire off this method if the player has found enough for the next power level
        if (calcPwrLvl > playerStats.Stats.getPWRLevel())
        {
            // Set pwrLvl to new pwrLvl
            pwrLvl = calcPwrLvl;
            playerStats.Stats.pwrLevel(calcPwrLvl);

            // Show the power popup
            StartCoroutine(flashPowerPopup());

        } else
        {
            if (calcPwrLvl < 3)
                StartCoroutine(flashRepair(remainingItems()));
        }

        // Need to do powerStages to update booleans and the power stage.
        if (powerSys != null)
            powerStages();
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { foundPower = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Turn off the interact UI if the player isn't within range
            if (gameManager.instance.getInteractUI().activeInHierarchy)
                gameManager.instance.getInteractUI().SetActive(false);
           
        }
    }

    int remainingItems()
    {
        int result = 0;

        if (gameManager.instance.getPowerItems() != 0)
            // Use the modulo since it's not 0.
            result = 3 - (gameManager.instance.getPowerItems() % 3);
        else
            result = 3;
        
        return result;
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
                // Copied pwr lvl 1 code just for an indicator that it's actually working.
                // Feel free to change this later.
                setRandFlickSpd(UnityEngine.Random.Range(0.2f, 1.5f));
                if (!getWait())
                    StartCoroutine(getNewNumber());
                flickerLight();
                setFlicker(true);

                //elevator on 
                //setElevatorAccess(true);
            }

            // Check the power level and update things accordingly.
            if (pwrLvl >= 1) 
            {
                // Grant access to the safe
                if (!safeRoom.instance.getSafeAccess())
                    safeRoom.instance.setSafeAccess(true);

                // Things will now turn on like the safe room door.
                isOn = true;
            }
        }
    }

    public IEnumerator getNewNumber()
    {
        waitBool = true;
        randNum = UnityEngine.Random.Range(0f, 100f);
        yield return new WaitForSeconds(1f);
        waitBool = false;
    }

    public IEnumerator flashRepair(int _remainingItems)
    {
        gameManager.instance.getRepairWarning().gameObject.SetActive(true);
        if (_remainingItems == 1)
        {
            gameManager.instance.getRepairWarning().text = _remainingItems.ToString("F0") + " Repair Item needed until next power level!";
        } else
        {
            gameManager.instance.getRepairWarning().text = _remainingItems.ToString("F0") + " Repair Items needed until next power level!";

        }
        yield return new WaitForSeconds(1f);
        gameManager.instance.getRepairWarning().gameObject.SetActive(false);
    }

    public IEnumerator flashPowerPopup()
    {
        gameManager.instance.getPowerLevelPopup().SetActive(true);
        gameManager.instance.getPowerLevelText().text = "Level " + pwrLvl.ToString();
        yield return new WaitForSeconds(1f);
        gameManager.instance.getPowerLevelPopup().SetActive(false);
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
    static public bool getFoundPower()
    { return foundPower; }
}
