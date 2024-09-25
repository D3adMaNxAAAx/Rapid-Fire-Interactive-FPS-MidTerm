using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // for using textmesh pro

public class gameManager : MonoBehaviour { 

    public static gameManager instance; // singleton

    // -- Menus --
    [Header("-- Menus --")]
    [SerializeField] GameObject menuActive; // this will change depending on what menu is showing in the game
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuComplete;
    [SerializeField] GameObject menuUpgrade;
    [SerializeField] GameObject menuStore;
    [SerializeField] GameObject menuLoadout;

    // -- Player --
    [Header("-- Player UI --")]
    [SerializeField] Image HPBar;
    [SerializeField] TMP_Text HPText;
    [SerializeField] Image stamBar;
    [SerializeField] TMP_Text stamText;
    [SerializeField] Image XPBar;
    [SerializeField] TMP_Text XPText;
    [SerializeField] GameObject ammoUI;
    [SerializeField] Image ammoTrackerBar;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] Image playerReticle;
    [SerializeField] TMP_Text levelTracker;
    [SerializeField] GameObject sniperScope;

    // -- Game --
    [Header("-- Enemy UI --")]
    [SerializeField] GameObject bossHP;
    [SerializeField] Image bossHPBar;

    //[SerializeField] Image EnemiesRemainingBar;
    [SerializeField] TMP_Text EnemiesRemainingCount;
    [SerializeField] TMP_Text EnemiesRemainingLabel;

    // -- Warnings --
    [Header("-- Popups --")]
    [SerializeField] GameObject damagePanelFlash;
    [SerializeField] GameObject ammoWarning;
    [SerializeField] GameObject lowHealthWarning;
    [SerializeField] GameObject checkPointPopup;

    // -- Objects --
    [Header("-- Game Components --")]
    [SerializeField] GameObject player; // Tracks player object
    [SerializeField] playerMovement playerScript; // Tracks playerController field
    [SerializeField] GameObject playerSpawnPos;
    [SerializeField] GameObject menuSettings;

    // Reticle Variables
    Vector2 reticleSize; // so the player can adjust reticle size through settings & also to change it to and from.
    Vector2 reticleSizeOrig;
    Color reticleColorOrig;

    // Dynamic Values
    int enemyCount;
    int bossCount; // For when we make boss monster
    float timeScaleOrig; // Tracks & stores original game time scale
    bool isPaused;

    public GameObject getMenuLoadout()
    {
        return menuLoadout;
    }

    public GameObject getPlayerSpawnPos()
    { return playerSpawnPos;}
  
    public void setPlayerSpawnPos(GameObject _playerSpawnPos)
    { playerSpawnPos = _playerSpawnPos; }
  
    public GameObject getCheckPointPopup()
    { return checkPointPopup; }
   
    public void setCheckPointPopup(GameObject _checkPointPopup)
    { checkPointPopup = _checkPointPopup; }
   
    public GameObject getDmgFlash()
        { return damagePanelFlash; }

    public void setDmgFlash(GameObject _dmgPanel)
        { damagePanelFlash = _dmgPanel; }

    public GameObject getAmmoWarning()
    { return ammoWarning; }

    public void setAmmoWarning(GameObject _ammoWarning)
    { ammoWarning = _ammoWarning; }

    public TMP_Text getHPText()
    {
        return HPText;
    }

    public TMP_Text getStamText()
    {
        return stamText;
    }

    public TMP_Text getXPText()
    {
        return XPText;
    }

    public TMP_Text getAmmoText()
    {
        return ammoText;
    }

    void setHPText(TMP_Text _HPText)
    {
        HPText = _HPText;
    }

    void setStamText(TMP_Text _stamText)
    {
        stamText = _stamText;
    }

    void setXPText(TMP_Text _XPText)
    {
        XPText = _XPText;
    }

    void setAmmoText(TMP_Text _ammoText)
    {
        ammoText = _ammoText;
    }

    public GameObject getAmmoUI()
    {
        return ammoUI;
    }

    void setAmmoUI(GameObject _ammoUI)
    {
        ammoUI = _ammoUI;
    }

    public GameObject getHealthWarning() 
        { return lowHealthWarning; }

    public void setHealthWarning(GameObject _lowHealthWarning) 
        { lowHealthWarning = _lowHealthWarning; }

    public GameObject getSniperScope() {
        return sniperScope;
    }

    public playerMovement getPlayerScript() 
        { return playerScript; }

    public GameObject getPlayer() 
        { return player; }

    public void setPlayer(GameObject _player) 
        { player = _player; }

    public void setPlayerScript(playerMovement _script)
        { playerScript = _script; }

    public bool getPauseStatus()
        { return isPaused; }

    public void setPauseStatus(bool _status)
        { isPaused = _status; }

    public Image getPlayerReticle()
        { return playerReticle; }

    public void setPlayerReticle(Image newReticle)
        { playerReticle = newReticle; }

    public int getBossCount() 
        { return bossCount; }

    public void setBossCount(int _amount) 
        { bossCount = _amount; }

    public void setLevelTracker(TMP_Text newLevel) 
        { levelTracker = newLevel; }

    public TMP_Text getLevelTracker() 
        { return levelTracker; }
    
    public void setHPBar(Image newHPBar) 
        { HPBar = newHPBar; }

    public Image getHPBar() 
        { return HPBar; }

    public void setXPBar(Image newXPBar)
    { XPBar = newXPBar; }

    public Image getXPBar()
    { return XPBar; }

    public void setBossHP(GameObject newBossHP) 
        { bossHP = newBossHP; }

    public GameObject getBossHP() 
        { return bossHP; }

    public void setBossHPBar(Image newBossHPBar)
    { bossHPBar = newBossHPBar; }

    public Image getBossHPBar()
    { return bossHPBar; }

    public void setStamBar(Image newStamBar) 
        { stamBar = newStamBar; }

    public Image getStamBar() 
        { return stamBar; }

    public int getEnemyCount()
    { return enemyCount;}

    public void setEnemyCount(int _count)
    { enemyCount = _count; }

    public void setEnemyRemainCount(TMP_Text newEnemyCount) 
        { EnemiesRemainingCount = newEnemyCount; }

    public TMP_Text getEnemyRemainCount() 
        { return EnemiesRemainingCount; }

    public void setEnemyRemainLabel(TMP_Text newEnemyLabel)
    { EnemiesRemainingLabel = newEnemyLabel; }

    public TMP_Text getEnemyRemainLabel()
    { return EnemiesRemainingLabel; }

    public void setAmmoBar(Image newAmmoBar) 
        { ammoTrackerBar = newAmmoBar; }

    public Image getAmmoBar() 
        { return ammoTrackerBar; }

    // Start is called before the first frame update, awake is before start
    void Awake() {

        instance = this; // making instance of this class (singleton)
        timeScaleOrig = Time.timeScale; // Setting time scale on game awake to set scale 
        reticleColorOrig = playerReticle.color;
        reticleSizeOrig = playerReticle.rectTransform.sizeDelta;
        setPlayer(GameObject.FindWithTag("Player")); // Setting player tracker to player object in engine by tag name set on player object
        setPlayerScript(getPlayer().GetComponent<playerMovement>()); // setting the player script from the above player tracker script component 
        setPlayerSpawnPos(GameObject.FindWithTag("PlayerSpawnPos")); //setting player spawn position by tag

        // Pausing game, hididng player background ui, and showing loadout menu
        
        menuActive = menuLoadout;

    }

    // Update is called once per frame
    void Update() {

        if (menuActive != menuLoadout)
        {
            if (Input.GetButtonDown("Cancel"))
            { // When ESC clicked
                scopeZoomOut();
                if (menuActive == null)
                {

                    statePause();
                    menuActive = menuPause; // Set the pause menu as active menu
                    menuActive.SetActive(getPauseStatus()); // Show active menu
                }
                else if (menuActive == menuPause)
                {
                    stateUnpause();
                }
            }
        }
        else if (menuActive == menuLoadout)
        {
            statePause();
            displayUI(false);
            menuActive.SetActive(true);
        }
    }

    public void statePause() {
        if(!isPaused)
        {
            isPaused = !isPaused; // toggles bool
            Time.timeScale = 0; // pauses everything except UI
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined; // unlocks cursor but keeps it confined to game window
        }
    }

    public void stateUnpause() {

        if (isPaused)
        {
            isPaused = !isPaused; // toggles bool
            Time.timeScale = timeScaleOrig; //set time scale back to original
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // locks cursor position
            menuActive.SetActive(getPauseStatus()); // Show active menu
            menuActive = null;
        }
    }

    public void updateGameGoal(int _enemyCount, int _bossCount = 0) {
        enemyCount += _enemyCount ;

        //EnemiesRemainingBar.fillAmount = (float)enemyCount / totalEnemies; TODO: Implement something for totalEnemies.
        EnemiesRemainingCount.text = enemyCount.ToString();

        bossCount += _bossCount;
        if (enemyCount <= 0 && bossCount <= 0) {
            statePause();
            menuActive = menuWin; // set active menu to win menu
            menuActive.SetActive(true); // Show active menu
        }
    }

    public void youLose() {
        statePause();
        menuActive = menuLose; // set active menu to lose menu
        menuActive.SetActive(true); // Show active menu
    }

    // Change reticle when aiming at an enemy
    public void changeReticle(bool hasIDamage) {
        if (hasIDamage) {
            playerReticle.color = Color.red;
            playerReticle.rectTransform.sizeDelta = 
                new Vector2(reticleSizeOrig.x * 2, reticleSizeOrig.y * 2);
        } else {
            playerReticle.color = Color.green;
            playerReticle.rectTransform.sizeDelta = reticleSizeOrig;
        }
    }
    public void displayUI(bool state)
    {
        // All of this only hides some things and not the entire thing. Refer to how the AmmoUI is hidden
        // Unhide these after loadout is picked.
        // Leave commented until above is addressed.
        //displayPlayerHP(state);
        //displayAmmoUI(state);
        //displayEnemyCount(state);
        //displayPlayerStam(state);
        //displayXPTracker(state);
    }

    public void displayBossBar(bool state) {
        getBossHP().SetActive(state);
        getBossHPBar().enabled = state;
    }

    public void displayPlayerHP(bool state)
    {
        getHPBar().gameObject.SetActive(state);
    }

    public void displayAmmoUI(bool state)
    {
        getAmmoUI().gameObject.SetActive(state);
    }

    public void displayPlayerStam(bool state) 
    {
        getStamBar().gameObject.SetActive(state);
    }

    public void displayXPTracker(bool state)
    {
        getXPBar().gameObject.SetActive(state);
    }

    public void displayEnemyCount(bool state)
    {
        getEnemyRemainCount().gameObject.SetActive(state);
        getEnemyRemainLabel().gameObject.SetActive(state);
        
    }

    public void settingsMenu() {
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void completeMenu() {
        EnemiesRemainingCount.maxVisibleWords = 0;
        EnemiesRemainingLabel.maxVisibleWords = 0;
        statePause();
        menuActive = menuComplete;
        menuActive.SetActive(true);
    }

    public void nextRoomContinue() {
        
        stateUnpause();
        menuActive = null;
        bossRoom.instance.startNextRoom();
        displayBossBar(true);
    }

    public void openUpgradeMenu() {
        menuActive.SetActive(false);
        menuActive = menuUpgrade;
        menuActive.SetActive(true);
        upgradeMenu.upgradeUI.setVars();
        /// this needs to be called somewhere around boss battle!!!
    }

    public void storeMenu() {
        menuActive.SetActive(false);
        // Update the store before it is displayed
        storeManager.instance.updateStoreUI();
        menuActive = menuStore;
        menuActive.SetActive(true);
    }

    public void newGame()
    {
        stateUnpause();
        displayUI(true);
    }

    //still working out the null reference and the rest of how the next two functions work


    //if you need to test just comment out 
    /*public void loadoutPreset1Pick()
    {
        loadout.instance.setSelectedLoadout(loadout.instance.getPreset1());
        loadout.instance.getPrmLd1Img().gameObject.SetActive(true);
        loadout.instance.getSecLd1Img().gameObject.SetActive(true);
        loadout.instance.getCnsm1Ld1Img().gameObject.SetActive(true);
        loadout.instance.getCnsm2Ld1Img().gameObject.SetActive(true);
        loadout.instance.getThrwLd1Img().gameObject.SetActive(true);
    }*/

    public void backButton() {
        if (menuActive == menuSettings)
        {
            menuActive.SetActive(false);
            menuActive = menuPause;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuUpgrade)
        {
            menuActive.SetActive(false);
            completeMenu();
        }
        else if (menuActive == menuStore)
        {
            menuActive.SetActive(false);
            menuActive = menuComplete;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuComplete)
        {
            menuActive.SetActive(false);
            stateUnpause();
            menuActive = null;
        }
    }
    public void scopeZoomIn() {
        if (menuActive == null) { // won't work if there is a menu active
            menuActive = sniperScope;
            getPlayerScript().getGunModel().SetActive(false);
            getSniperScope().SetActive(true);
        }
    }

    public void scopeZoomOut() {
        if (menuActive == sniperScope) {
            menuActive = null;
            getPlayerScript().getGunModel().SetActive(true);
            getSniperScope().SetActive(false);
        }
    }

}
