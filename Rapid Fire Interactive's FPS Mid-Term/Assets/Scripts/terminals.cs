using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class terminals : MonoBehaviour
{
    [SerializeField] Light terminalScreenLight;
    [SerializeField] GameObject terminal;

    bool isOpen; // if terminal menu is already open
    bool isOn; // if terminal can be accessed at all

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
            { 
                terminalScreenLight.gameObject.SetActive(true);
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
