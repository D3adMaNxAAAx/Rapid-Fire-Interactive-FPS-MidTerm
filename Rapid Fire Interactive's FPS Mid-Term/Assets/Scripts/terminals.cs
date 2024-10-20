using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class terminals : MonoBehaviour
{

    
    [SerializeField] Light terminalScreenLight;

    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuUpg;
    [SerializeField] GameObject menuStore;
    [SerializeField] GameObject buttonUpg;
    [SerializeField] GameObject buttonStore;
    [SerializeField] GameObject buttonClose;
    [SerializeField] GameObject menuActive;

    bool isOpen;
    bool isOn;

    // Start is called before the first frame update
    void Awake()
    {
        isOpen = false;
        isOn = false;
        terminalScreenLight.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStats.Stats != null)
        if (playerStats.Stats.getPWRLevel() > 2 && !isOn)
        { terminalScreenLight.gameObject.SetActive(true);
            isOn = true;
        }
        
            
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            // If interact menu isn't on, turn it on.
            if (!gameManager.instance.getInteractUI().activeInHierarchy)
                gameManager.instance.getInteractUI().SetActive(true);

            if (Input.GetButton("Interact"))
                if (playerStats.Stats.getPWRLevel() >= 2)
                {
                    // turn off interact menu
                    if (gameManager.instance.getInteractUI().activeInHierarchy)
                        gameManager.instance.getInteractUI().SetActive(false);
                    openTerminal();
                }
                else
                {
                    StartCoroutine(flashWarning());
                }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(false);
    }

    public IEnumerator flashWarning()
    {
        gameManager.instance.getPwrLvlWarningText().gameObject.SetActive(true);
        gameManager.instance.getPwrLvlWarningText().text = "Power Level 2 needed!";
        yield return new WaitForSeconds(0.75f);
        gameManager.instance.getPwrLvlWarningText().gameObject.SetActive(false);
    }

    public void openTerminal() 
    {
        
        if (!isOpen)
        {
            
            gameManager.instance.displayUI(false);
            gameManager.instance.statePause();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            
            menuMain.SetActive(true);
            isOpen = !isOpen;

        }
    }

    public void closeTerminal()
    {
       
        
        menuMain.SetActive(false);
        gameManager.instance.stateUnpause();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            gameManager.instance.displayUI(true);
            isOpen = !isOpen;
        
    }

    public void openUpgrades()
    {
        
        if (menuActive != null)
        { menuActive.SetActive(false); }
        if (playerStats.Stats.getPWRLevel() == 3)
        {
            menuActive = menuUpg;
            menuActive.SetActive(true);
            buttonStore.SetActive(false);
            buttonUpg.SetActive(false);
            buttonClose.SetActive(false);
        }
    }

    public void openStore()
    {
        if (menuActive != null)
        { menuActive.SetActive(false); }
        if (playerStats.Stats.getPWRLevel() >= 2)
        {
            menuActive = menuStore;
            menuActive.SetActive(true);
            buttonStore.SetActive(false);
            buttonUpg.SetActive(false);
            buttonClose.SetActive(false);
        }
    }

    public void closeMenuActive()
    {
        if (menuActive != menuMain)
        {
            menuActive.SetActive(false);
            buttonUpg.SetActive(true);
            buttonStore.SetActive(true);
            buttonClose.SetActive(true);
        }
        else
            menuActive.SetActive(false);
        menuActive = null;
    }
}
