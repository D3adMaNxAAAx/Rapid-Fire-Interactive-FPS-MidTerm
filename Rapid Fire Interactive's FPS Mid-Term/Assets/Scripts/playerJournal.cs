using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerJournal : MonoBehaviour
{
    

    [SerializeField] GameObject journal;
    
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject _menuObjectives;
    [SerializeField] GameObject _menuItems;
    [SerializeField] GameObject _menuStats;
    [SerializeField] GameObject _menuAchievements;

    bool isOpen;
    bool objOpen;
    bool itemsOpen;
    bool statsOpen;
    bool achieveOpen;

    private void Awake()
    {
        journal.SetActive(false);
        isOpen = false;
    }
    // Update is called once per frame
    void Update()
    {
       toggleJournal();
    }

    void toggleJournal()
    {
        if (Input.GetButtonDown("OpenJournal") && !isOpen)
        {
            Debug.Log("Opened");
            gameManager.instance.statePause();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            journal.SetActive(true);
            isOpen = !isOpen;
        }
        else if (Input.GetButtonDown("OpenJournal") && isOpen)
        {
            gameManager.instance.stateUnpause();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            journal.SetActive(false);
            isOpen = !isOpen;
        }
    }

    public void menuObj()
    {
        if(menuActive != null)
        { menuActive.SetActive(false); }

        menuActive = _menuObjectives;

        menuActive.SetActive(true);
    }
    public void menuItems()
    {
        if (menuActive != null)
        { menuActive.SetActive(false); }

        menuActive = _menuItems;
        menuActive.SetActive(true);
    } 
    public void menuStats()
    {
        if (menuActive != null)
        { menuActive.SetActive(false); }

        menuActive = _menuStats;
        menuActive.SetActive(true);
    } 
    public void menuachieve()
    {
        if (menuActive != null)
        { menuActive.SetActive(false); }

        menuActive = _menuAchievements;
        menuActive.SetActive(true);
    }
}
