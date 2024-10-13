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

    [SerializeField] Canvas activeDoc;
    [SerializeField] Canvas document1UI;
    [SerializeField] Canvas document2UI;
    [SerializeField] Canvas document3UI;
    [SerializeField] Canvas document4UI;
    [SerializeField] Canvas document5UI;
    [SerializeField] Canvas document6UI;
    [SerializeField] Canvas document7UI;
    [SerializeField] Canvas document8UI;
    [SerializeField] Canvas document9UI;

    bool isOpen;
    bool docIsOpen;
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
           openJournal();
            
        }
        else if (Input.GetButtonDown("OpenJournal") && isOpen)
        {
           closeJournal();
        }
    }

    public void openJournal()
    {
        if (!isOpen)
        {
            gameManager.instance.displayUI(false);
            gameManager.instance.statePause();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            journal.SetActive(true);
            isOpen = !isOpen;
            
        }
    }
    public void closeJournal()
    {
        if (isOpen)
        {
            gameManager.instance.stateUnpause();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            journal.SetActive(false);
            gameManager.instance.displayUI(true);
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

    public void closeDoc()
    {
            activeDoc.enabled = false;
            docIsOpen = false;
    }

    public void openDoc(Canvas thisDoc) 
    {
        if (!docIsOpen )
        {
            Debug.Log("DOC is Open");
            activeDoc = thisDoc;  
            activeDoc.enabled = true;
            docIsOpen = true; 
        } 
    }
}
