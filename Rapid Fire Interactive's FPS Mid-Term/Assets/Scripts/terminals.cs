using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class terminals : MonoBehaviour
{
    [SerializeField] Light terminalScreenLight;
    [SerializeField] GameObject terminal;
    [SerializeField] GameObject terminalUpgrade;

    bool isOpen; // if terminal menu is open
    bool isOn; // if terminal can be accessed at all

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.CompareTag("Do Not Destroy"))
        DontDestroyOnLoad(gameObject);
        terminal = holdMe.instance.getTerminalMainMenu();
        terminalUpgrade = holdMe.instance.getTerminalUpgradeMenu();
        isOpen = false;
        isOn = false;
        terminalScreenLight.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Unlock Terminal
        if (playerStats.Stats != null)
            if (playerStats.Stats.getPWRLevel() > 2 && !isOn)
            { 
                terminalScreenLight.gameObject.SetActive(true);
                isOn = true;
            }

        // Unlock Upgrade Menu
        if (playerStats.Stats != null)
            if (playerStats.Stats.getPWRLevel() == 3 && isOn)
            {
                if (!terminalUpgrade.activeInHierarchy)
                {
                    terminalUpgrade.SetActive(true);
                }
            }

       // Keypress to close terminal

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

                    // turn on the terminal
                    terminal.SetActive(true);
                    gameManager.instance.openTerminal();
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
        gameManager.instance.getPwrLvlWarning().gameObject.SetActive(true);
        gameManager.instance.getPwrLvlWarning().text = "Power Level 2 needed!";
        yield return new WaitForSeconds(0.75f);
        gameManager.instance.getPwrLvlWarning().gameObject.SetActive(false);
    }
    public void turnOff()
    {
        isOpen = false;
        terminal.SetActive(false);
    }
}
