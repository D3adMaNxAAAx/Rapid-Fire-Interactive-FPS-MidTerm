using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// Despite the name of this script, it is also the main way that the power system works! Therefore, it must be interactable!
public class lightFlicker : MonoBehaviour, IInteractable
{
    // Serialized Member Fields used for particular 
    [Range(1, 100)] [SerializeField] float percentFlicker;
    [SerializeField] GameObject lightSys = null;
    [SerializeField] GameObject powerSys = null;
    [SerializeField] Light[] lights = null;
    [SerializeField] bool level2;
    [SerializeField] bool testing = false;

    // Power Variables
    [SerializeField] int pwrLvl;

    // Light Variables
    Light _light; // This was previously named light which was the same name of a variable it was inheriting from.
    int lightCount;

    // Random # Light Variables
    float randNum;
    float randFlickSpeed;

    // Checks
    static bool foundPower = false;
    bool isFlickering = false;
    bool waitBool = false;    
    // bool isOn;
    bool safeAccess;

    private void Awake() {
        if (lightSys == null) {
            _light = this.gameObject.GetComponentInChildren<Light>();
        }
        else if (lightSys != null) { 
            lights = lightSys.gameObject.GetComponentsInChildren<Light>();
            lightCount = lights.Count();

            for (int i = 0; i < lightCount - 1; ++i) {
                if (level2 == false) {
                    lights[i].range = 15;
                }
                if (lights[i].type != LightType.Spot) { // don't change spotlight intensity
                    lights[i].intensity = 0;
                }
                lights[i].enabled = false; // starting all lights off
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (powerSys == null) {
            randFlickSpeed = UnityEngine.Random.Range(0.2f, 1.5f);
            if (!waitBool) {
                StartCoroutine(getNewNumber());
            }
            flickerLight();
        } /// below for testing:
        else if (testing) {
            powerStages();
            testing = false;
        }
    }

    public void flickerLight() {
        if(!isFlickering && (_light.intensity != 0 && _light.intensity != 1.25f)) { // 1.25 is power level 3 intensity, so no flicker
            if(randNum <= percentFlicker) {
                StartCoroutine(lightAction());
            }
        }
    }

    IEnumerator lightAction() {
        if (lightSys == null) {
            isFlickering = true;
            _light.enabled = true;
            yield return new WaitForSeconds(randFlickSpeed);
            _light.enabled = false;
            yield return new WaitForSeconds(0.5f);
            isFlickering = false;
        }
    }

    void powerStages() {
        if (powerSys != null) {
            if (pwrLvl == 1) {
                for (int i = 0; i < lightCount - 1; ++i) {
                    lights[i].enabled = true;
                    if (lights[i].type != LightType.Spot) { // don't change spotlight intensity
                        lights[i].intensity = 0.5f;
                    }
                }
                if (!getWait())
                    StartCoroutine(getNewNumber());
            }
            else if (pwrLvl == 2) {
                for (int i = 0; i < lightCount - 1; ++i) {
                    if (lights[i].type != LightType.Spot) { // don't change spotlight intensity
                        lights[i].intensity = 0.75f;
                    }
                }
                if (!getWait())
                    StartCoroutine(getNewNumber());
            }
            else if (pwrLvl == 3) {
                for (int i = 0; i < lightCount - 1; ++i) {
                    if (lights[i].type != LightType.Spot) { // don't change spotlight intensity
                        lights[i].intensity = 1.25f;
                    }
                    lights[i].enabled = true;
                }
            }

            if (pwrLvl >= 1) {
                if (!playerMovement.player.getSafeAccess()) { // Grant access to the safe
                    playerMovement.player.setSafeAccess(true);
                }
                //if (!safeRoom.instance.getSafeAccess()) { safeRoom.instance.setSafeAccess(true); }
                // isOn = true; // Things will now turn on like the safe room door.
            }
        }
    }

    public IEnumerator getNewNumber() {
        waitBool = true;
        randNum = UnityEngine.Random.Range(0f, 100f);
        yield return new WaitForSeconds(1f);
        waitBool = false;
    }

    public void interact() {
        int calcPwrLvl = (int)Math.Floor((double)gameManager.instance.getPowerItems() / 3);
        if (calcPwrLvl > playerStats.Stats.getPWRLevel()) { // Only fire off this method if the player has found enough for the next power level
            pwrLvl = calcPwrLvl;             // Set pwrLvl to new pwrLvl
            playerStats.Stats.pwrLevel(calcPwrLvl);
            audioManager.instance.PlaySound(audioManager.instance.audPowerUp, audioManager.instance.audPowerUpVol);
            StartCoroutine(flashPowerPopup());

        } 
        else {
            if (calcPwrLvl < 3) {
                StartCoroutine(flashRepair(remainingItems()));
            }
        }
        if (powerSys != null) {
            powerStages();
        }
    }

    private void OnTriggerStay(Collider other) {
        if (lightSys != null && pwrLvl < 3) {
            if (!gameManager.instance.getInteractUI().activeInHierarchy) {
                gameManager.instance.getInteractUI().SetActive(true);
            }
            if (other.CompareTag("Player")) {
                if (Input.GetButton("Interact")) { interact(); }
            }
        }
        else {
            if (gameManager.instance.getInteractUI().activeInHierarchy) {
                gameManager.instance.getInteractUI().SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) { 
            foundPower = true; 
        }
    }

    private void OnTriggerExit(Collider other) {
        if (gameManager.instance.getInteractUI().activeInHierarchy) {
            gameManager.instance.getInteractUI().SetActive(false);
        }
    }

    int remainingItems() {
        int result = 0;
        if (gameManager.instance.getPowerItems() != 0) {
            result = 3 - (gameManager.instance.getPowerItems() % 3);
        }
        else {
            result = 3;
        }
        return result;
    }

    public IEnumerator flashRepair(int _remainingItems) {
        gameManager.instance.getRepairWarning().gameObject.SetActive(true);
        if (_remainingItems == 1) {
            gameManager.instance.getRepairWarning().text = _remainingItems.ToString("F0") + " Repair Item needed until next power level!";
        } 
        else {
            gameManager.instance.getRepairWarning().text = _remainingItems.ToString("F0") + " Repair Items needed until next power level!";

        }
        yield return new WaitForSeconds(3f);
        gameManager.instance.getRepairWarning().gameObject.SetActive(false);
    }

    public IEnumerator flashPowerPopup() {
        gameManager.instance.getPowerLevelPopup().SetActive(true);
        gameManager.instance.getPowerLevelText().text = "Level " + pwrLvl.ToString();
        yield return new WaitForSeconds(3f);
        gameManager.instance.getPowerLevelPopup().SetActive(false);
    }

    public bool getWait() { return waitBool; }
    public void setRandFlickSpd(float randSpd) { randFlickSpeed = randSpd; }
    static public bool getFoundPower() { return foundPower; }
    static public void setFoundPower(bool _state) { foundPower = _state; }
    public bool getSafeAccess() { return safeAccess; }
}
