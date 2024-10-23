using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class playerJournal :  MonoBehaviour
{
    

    [SerializeField] Canvas journal;
    
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
    [SerializeField] Canvas itemsMenu;

    [SerializeField] TMP_Text obj1;
    [SerializeField] TMP_Text obj2;
    [SerializeField] TMP_Text obj3;
    [SerializeField] TMP_Text obj4;
    [SerializeField] TMP_Text obj5;
    [SerializeField] TMP_Text obj6;
    [SerializeField] TMP_Text obj7;
    [SerializeField] TMP_Text obj8;
    

    [SerializeField] TMP_Text healthStat;
    [SerializeField] TMP_Text stamStat;
    [SerializeField] TMP_Text speedStat;
    [SerializeField] TMP_Text dmgStat;
    [SerializeField] TMP_Text xpStat;
    [SerializeField] TMP_Text repairToolHeld;
    [SerializeField] TMP_Text lostDocsHeld;
    [SerializeField] TMP_Text idBadgesHeld;
    [SerializeField] TMP_Text playerDeathCount;
    [SerializeField] TMP_Text playerNearDeathCount;
    [SerializeField] TMP_Text playerShotsFiredCount;
    [SerializeField] TMP_Text totalMoneyEarnedCount;
    [SerializeField] TMP_Text totalXPEarnedCount;
    [SerializeField] TMP_Text playerLevelStat;
    [SerializeField] TMP_Text powerLevelCount;

    // new stats being shown:
    [SerializeField] TMP_Text enemiesKilled;
    [SerializeField] TMP_Text damageDealt;
    [SerializeField] TMP_Text damageReceived;
    [SerializeField] TMP_Text secretsFound;
    [SerializeField] TMP_Text headShots;

    [SerializeField] TMP_Text currentMoney;

    bool isOpen;

    // Unused Variables
    //int enemyCountOrig = 0;

    //bool docIsOpen;
    //bool objOpen;
    //bool itemsOpen;
    //bool statsOpen;
    //bool achieveOpen;

    private void Start()
    {
        
        journal.enabled = false;
        isOpen = false;
    }
    // Update is called once per frame
    void Update()
    {
       toggleJournal();
        currentMoneyCounter();
        if (_menuStats.gameObject.activeInHierarchy)
            updateJournalStats();
        updateObjectives();
    }

    void currentMoneyCounter() {
        currentMoney.text = gameManager.instance.getPlayerScript().getCoins().ToString("F0");
    }
    void updateJournalStats() {

        healthStat.text = gameManager.instance.getPlayerScript().getHPOrig().ToString("F0");
        stamStat.text = gameManager.instance.getPlayerScript().getStaminaOrig().ToString("F0");
        speedStat.text = gameManager.instance.getPlayerScript().getSpeed().ToString("F0");
        dmgStat.text = gameManager.instance.getPlayerScript().getDamage().ToString("F0");
        xpStat.text = gameManager.instance.getPlayerScript().getXP().ToString("F0");
        repairToolHeld.text = playerStats.Stats.getPowerObjects().ToString("F0");
        lostDocsHeld.text = playerStats.Stats.getNotesFound().ToString("F0");
        idBadgesHeld.text = playerStats.Stats.getBadgesFound().ToString("F0");
        playerDeathCount.text = playerStats.Stats.getDeaths().ToString("F0");
        playerNearDeathCount.text = playerStats.Stats.getNearDeaths().ToString("F0");
        playerShotsFiredCount.text = playerStats.Stats.getAmmoUsed().ToString("F0");
        totalMoneyEarnedCount.text = playerStats.Stats.getTotalMoney().ToString("F0");
        totalXPEarnedCount.text = playerStats.Stats.getTotalXP().ToString("F0");
        playerLevelStat.text = playerStats.Stats.getLevel().ToString("F0");
        powerLevelCount.text = playerStats.Stats.getPWRLevel().ToString("F0");

        enemiesKilled.text = playerStats.Stats.getEnemiesKilled().ToString("F0");
        damageDealt.text = playerStats.Stats.getDamageDealt().ToString("F0");
        damageReceived.text = playerStats.Stats.getDamageTaken().ToString("F0");
        secretsFound.text = playerStats.Stats.getCollectablesFound().ToString("F0");
        headShots.text = playerStats.Stats.getHeadShots().ToString("F0");
    }

    void toggleJournal()
    {
        if (Input.GetButtonDown("OpenJournal") && gameManager.instance.getMenuActive() == null && !isOpen)
        {
            gameManager.instance.setMenuActive(journal.gameObject);
            openJournal();
        }
        else if (Input.GetButtonDown("OpenJournal") && isOpen)
        {
            gameManager.instance.setMenuActive(null);
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
            journal.enabled = true;
            isOpen = true;
            
        }
    }
    public void closeJournal()
    {
        if (isOpen)
        {
            gameManager.instance.stateUnpause();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            journal.enabled = false;
            gameManager.instance.displayUI(true);
            isOpen = false;
        }
    }
    public void menuObj()
    {
        if (menuActive != null && menuActive != _menuItems)
        { menuActive.SetActive(false); }
        else if (menuActive == _menuItems)
        { itemsMenu.enabled = false; }

        menuActive = _menuObjectives;

        menuActive.SetActive(true);
    }
    public void menuItems()
    {
        if (menuActive != null && menuActive != _menuItems)
        { menuActive.SetActive(false); }
        else if (menuActive == _menuItems)
        { itemsMenu.enabled = false; }

        menuActive = _menuItems;
        itemsMenu.enabled = true;
    } 
    public void menuStats()
    {
        if (menuActive != null && menuActive != _menuItems)
        { menuActive.SetActive(false); }
        else if (menuActive == _menuItems)
        { itemsMenu.enabled = false; }

        menuActive = _menuStats;
        menuActive.SetActive(true);
       
    } 
    public void menuachieve()
    {
        if (menuActive != null && menuActive != _menuItems)
        { menuActive.SetActive(false); }
        else if (menuActive == _menuItems)
        { itemsMenu.enabled = false; }

        menuActive = _menuAchievements;
        menuActive.SetActive(true);
    }

    public void closeDoc()
    {
            activeDoc.enabled = false;
            //docIsOpen = false;
    }

    public void updateObjectives()
    {
        obj1.text = playerStats.Stats.getPowerObjects().ToString("F0") + " / 9";

        if (lightFlicker.getFoundPower() == true)
        {
            obj2.text = "Completed";
            obj2.color = Color.green;
        }
        else if (lightFlicker.getFoundPower() == false) 
        {
            obj2.text = "Incomplete";
            obj2.color = Color.red;
        }

        obj3.text = playerStats.Stats.getPWRLevel().ToString("F0") + " / 3";

        if (playerStats.Stats.getBadgesFound() <= 3)
            obj4.text = playerStats.Stats.getBadgesFound().ToString("F0") + " / 3";

        obj5.text = playerStats.Stats.getBadgesFound().ToString("F0") + " / 9";

        obj6.text = gameManager.instance.getEnemyRemainCount().text;

        obj7.text = playerStats.Stats.getNotesFound().ToString("F0") + " / 9";

        if (playerStats.Stats.getPWRLevel() == 2)
        { 
            obj8.text = "Store Access Granted";
            obj8.color = Color.green;
        }
        else if (playerStats.Stats.getPWRLevel() == 3)
        { 
            obj8.text = "Store & Upgrades Access Granted";
            obj8.color = Color.green;
        }
        else 
        {
            obj8.text = "No Access Granted! Restore Power Level";
            obj8.color = Color.red;
        }
    }
  
}
