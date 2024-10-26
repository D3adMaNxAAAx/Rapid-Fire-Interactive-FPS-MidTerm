using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour {

    public static gameManager instance; // singleton

    // -- Menus --
    [Header("-- Menus --")]
    [SerializeField] GameObject menuActive; // this will change depending on what menu is showing in the game
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuContinue;
    [SerializeField] GameObject menuConfirmation;
    [SerializeField] GameObject menuJournal;
    [SerializeField] GameObject menuQuit;
    [SerializeField] GameObject menuTerminal;
    [SerializeField] GameObject menuUpgrade;
    [SerializeField] GameObject menuTerminalUpgrade;
    [SerializeField] GameObject menuStore;
    [SerializeField] GameObject menuTerminalStore;
    [SerializeField] GameObject menuLoadout;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuHowTo;
    [SerializeField] GameObject menuTips;
    [SerializeField] GameObject menuControls;
    [SerializeField] GameObject menuStats;
    [SerializeField] GameObject menuLoseStats;
    [SerializeField] GameObject menuWinStats;

    [Header("-- Quit Buttons --")]
    [SerializeField] Button[] quitButtons;
    
    // First Selected Options (for use when selecting with arrow keys)
    [Header("-- First Selected Options --")]
    [SerializeField] GameObject loadoutMenuFirst;
    [SerializeField] GameObject pauseMenuFirst;
    [SerializeField] GameObject optionsMenuFirst;
    [SerializeField] GameObject confirmationMenuFirst;
    [SerializeField] GameObject quitMenuFirst;
    [SerializeField] GameObject settingsMenuFirst;
    [SerializeField] GameObject howToMenuFirst;
    [SerializeField] GameObject tipsMenuFirst;
    [SerializeField] GameObject controlsMenuFirst;
    [SerializeField] GameObject terminalMenuFirst;
    [SerializeField] GameObject storeMenuFirst;
    [SerializeField] GameObject terminalStoreMenuFirst;
    [SerializeField] GameObject upgradeMenuFirst;
    [SerializeField] GameObject terminalUpgradeMenuFirst;
    [SerializeField] GameObject continueMenuFirst;
    [SerializeField] GameObject loseMenuFirst;
    [SerializeField] GameObject winMenuFirst;
    [SerializeField] GameObject statsMenuFirst;
    [SerializeField] GameObject loseStatsMenuFirst;
    [SerializeField] GameObject winStatsMenuFirst;

    // -- Player --
    [Header("-- Player UI --")]
    [SerializeField] Image HPBar;
    [SerializeField] TMP_Text HPText;
    [SerializeField] Image shieldBarImage;
    [SerializeField] GameObject shieldBar;
    [SerializeField] TMP_Text ShieldHPText;
    [SerializeField] Image stamBar;
    [SerializeField] TMP_Text stamText;
    [SerializeField] Image XPBar;
    [SerializeField] TMP_Text XPText;
    [SerializeField] GameObject ammoUI;
    [SerializeField] Image ammoTrackerBar;
    [SerializeField] TMP_Text ammoText; // This text will include the current ammo loaded & the magazine size.
    [SerializeField] TMP_Text ammoReserveText; // This text will show the remaining ammo the player has in reserve. (NOT the capacity)
    [SerializeField] TMP_Text healsLeft; // used in playerMovement addToHeals
    [SerializeField] TMP_Text grenadesLeft; // used in playerMovement addToGrenades
    [SerializeField] Image playerReticle;
    [SerializeField] TMP_Text levelTracker;
    [SerializeField] GameObject sniperScope;
    [SerializeField] GameObject stamUI;
    [SerializeField] GameObject hpUI;
    [SerializeField] GameObject buffUI;
    [SerializeField] Image buffIcon;
    [SerializeField] GameObject buffUI2;
    [SerializeField] Image buffIcon2;
    [SerializeField] GameObject xpUI;
    [SerializeField] GameObject timer;
    [SerializeField] GameObject timerTracker; // this makes the timer run regardless if its hidden or not
    [SerializeField] TMP_Text timerText;
    [SerializeField] Button respawnButton;
    [SerializeField] TMP_Text livesText;
    [SerializeField] GameObject interactUI; // this makes the timer run regardless if its hidden or not
    [SerializeField] GameObject startFailMessage;
    [SerializeField] TMP_Text levelPopUp;
    [SerializeField] GameObject TyEasterEggPopUp;
  

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
    [SerializeField] GameObject healIndicator;
    [SerializeField] GameObject ammoWarning;
    [SerializeField] GameObject lowHealthWarning;
    [SerializeField] GameObject checkPointPopup;
    [SerializeField] GameObject powerLevelPopup;
    [SerializeField] TMP_Text powerLevelText;
    [SerializeField] TMP_Text idBadgeLvlWarning;
    [SerializeField] TMP_Text pwrLvlWarning;
    [SerializeField] TMP_Text repairWarning;

    // -- Objects --
    [Header("-- Game Components --")]
    [SerializeField] GameObject player; // Tracks player object
    [SerializeField] playerMovement playerScript; // Tracks playerController field
    [SerializeField] GameObject playerSpawnPos;

    [SerializeField] TMP_Text completionTime;
    [SerializeField] TMP_Text enemiesKilled;
    [SerializeField] TMP_Text deaths;
    [SerializeField] TMP_Text playerLevel;

    // Reticle Variables
    Vector2 reticleSize; // so the player can adjust reticle size through settings & also to change it to and from.
    Vector2 reticleSizeOrig;
    Color reticleColorOrig;

    // Dynamic Values
    int enemyCount;
    int enemyCountOrig = 0;
    int bossCount; // For when we make boss monster
    int powerItems;
    float timeScaleOrig; // Tracks & stores original game time scale
    bool isPaused;

    // Start is called before the first frame update, awake is before start
    void Awake() {
        instance = this;// making instance of this class (singleton)
        timeScaleOrig = Time.timeScale; // Setting time scale on game awake to set scale 
        reticleColorOrig = playerReticle.color;
        reticleSizeOrig = playerReticle.rectTransform.sizeDelta;
        setPlayer(GameObject.FindWithTag("Player")); // Setting player tracker to player object in engine by tag name set on player object
        setPlayerScript(getPlayer().GetComponent<playerMovement>()); // setting the player script from the above player tracker script component 
        setPlayerSpawnPos(GameObject.FindWithTag("PlayerSpawnPos")); //setting player spawn position by tag

        // Check for WebGL to disable the quit button from options.
        if (getPlatform() == RuntimePlatform.WebGLPlayer)
        {
            if (quitButtons != null)
            {
                foreach (Button quit in quitButtons)
                {
                    if (quit != null)
                        quit.interactable = false;
                }
            }
        }

        // Hiding player background ui, and showing loadout menu
        menuActive = menuLoadout;
        displayUI(false);
        EventSystem.current.SetSelectedGameObject(loadoutMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    // Update is called once per frame
    void Update() { 

        if (menuActive != menuLoadout)
        {
            // Check if the player is playing on a platform that supports ESC.
            if (Input.GetButtonDown("Cancel") && menuActive == null && getPlatform() != RuntimePlatform.WebGLPlayer)
            { // When ESC clicked
                scopeZoomOut();
                if (menuActive == null)
                {
                    // Turn off the interact UI if paused
                    if (getInteractUI().activeInHierarchy)
                        getInteractUI().SetActive(false);

                    statePause();
                    menuActive = menuPause; // Set the pause menu as active menu
                    menuActive.SetActive(getPauseStatus()); // Show active menu
                    EventSystem.current.SetSelectedGameObject(pauseMenuFirst); // Set eventsystem selected game object to the button assigned
                }
                else if (menuActive == menuPause)
                {
                    stateUnpause();
                }
            } 
            else if (Input.GetButtonDown("webGL_Cancel") && menuActive == null && getPlatform() == RuntimePlatform.WebGLPlayer) {
                scopeZoomOut();
                if (menuActive == null)
                {
                    // Turn off the interact UI if paused
                    if (getInteractUI().activeInHierarchy)
                        getInteractUI().SetActive(false);

                    statePause();
                    menuActive = menuPause; // Set the pause menu as active menu
                    menuActive.SetActive(getPauseStatus()); // Show active menu
                    EventSystem.current.SetSelectedGameObject(pauseMenuFirst); // Set eventsystem selected game object to the button assigned
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

        if (playerScript.getLeveledUp())
        {
            StartCoroutine(showLevelPopUp());
        }
    }

    public void statePause() {
        if (!isPaused) {
            scopeZoomOut();
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
            if (menuActive != null)
                menuActive.SetActive(getPauseStatus()); // Show active menu
            menuActive = null;
            if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null); // Null out any selected game objects too
        }
    }
    
    // Gets the platform the player is on
    public RuntimePlatform getPlatform()
    {
        return Application.platform;
    }

    public void updateBossBar(Image _bossHealthBar, float bossHP, float _health)
    {
        if (bossRoom.instance != null)
        // Check if the player has entered the boss room
        {
            if (bossRoom.instance.getBossFightState())
            {
                _bossHealthBar.fillAmount = bossHP / _health;
            }
        }
    }

    public void updateGameGoal(int _enemyCount, int _bossCount = 0) {
        enemyCount += _enemyCount;

        //EnemiesRemainingBar.fillAmount = (float)enemyCount / totalEnemies; TODO: Implement something for totalEnemies.
        EnemiesRemainingCount.text = enemyCount.ToString();

        bossCount += _bossCount;

        // Decrement the bossCount from enemy count.
        if (bossCount > 0) {
            enemyCount -= bossCount;
        }

        if (bossRoom.instance != null)
        {
            if (enemyCount <= 0 && bossCount <= 0)
            {
                StartCoroutine(preWinMenuThings());
                youWin();
            }
        }
    }

    public IEnumerator preWinMenuThings() {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioManager.instance.MusicMixerGroup;

        // Play the short victory audio first
        audioSource.PlayOneShot(audioManager.instance.VictoryA, audioManager.instance.VictoryVol);
        yield return new WaitForSeconds(audioManager.instance.VictoryA.length);

        // After the short clip, play the longer victory music
        audioSource.clip = audioManager.instance.VictoryMusicA;
        audioSource.volume = audioManager.instance.VictoryMusicVol; // This should use the configured volume
        audioSource.loop = false;
        audioSource.Play();
    }

    void youWin() {
        if (menuActive != null) {
            menuActive.SetActive(false);
        }
        statePause();
        menuActive = menuWin; // Set Win menu as active
        menuActive.SetActive(true); // Show Win menu
        EventSystem.current.SetSelectedGameObject(winMenuFirst);  // Set eventsystem selected game object to the button assigned
    }
    public void openWinStatsMenu()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false); // Close current menu
        }
        menuActive = menuWinStats; // Set WinStatsMenu as active
        menuActive.SetActive(true); // Show WinStatsMenu
        EventSystem.current.SetSelectedGameObject(winStatsMenuFirst); // Focus first button in WinStatsMenu

        // Update stats
        completionTime.text = playerStats.Stats.getTimeTaken();
        enemiesKilled.text = playerStats.Stats.getEnemiesKilled().ToString();
        deaths.text = playerStats.Stats.getDeaths().ToString();
        playerLevel.text = playerStats.Stats.getLevel().ToString();
    }

    public void openLoseStatsMenu()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false); // Close the current menu
        }
        menuActive = menuLoseStats; // Set the LoseStatsMenu as the active menu
        menuActive.SetActive(true); // Show it
        EventSystem.current.SetSelectedGameObject(loseStatsMenuFirst); // Make sure first button is selected
                                                                 
        completionTime.text = playerStats.Stats.getTimeTaken();
        enemiesKilled.text = playerStats.Stats.getEnemiesKilled().ToString();
        deaths.text = playerStats.Stats.getDeaths().ToString();
    }

    public void youLose() {
        playerMovement.player.setShieldOff();
        if (menuActive != null) {
            menuActive.SetActive(false);
        }
        statePause();
        if (getPlayerScript().getLives() <= 0)
        {
            getRespawnButton().interactable = false;
        }
        menuActive = menuLose; // set active menu to lose menu
        menuActive.SetActive(true); // Show active menu
        EventSystem.current.SetSelectedGameObject(loseMenuFirst); // Set eventsystem selected game object to the button assigned

        // Since text is now active, update it.
        getLivesText().text = getPlayerScript().getLives().ToString();
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
        displayPlayerHP(state);
        displayAmmoUI(state);
        displayEnemyCount(state);
        displayPlayerStam(state);
        displayXPTracker(state);
    }

    public void displayBossBar(bool state) {
        getBossHP().SetActive(state); }

    public void displayPlayerHP(bool state) {
        getHPBar().gameObject.SetActive(state);
        getHealthUI().gameObject.SetActive(state); }

    public void displayAmmoUI(bool state) {
        getAmmoUI().gameObject.SetActive(state); }

    public void displayPlayerStam(bool state) {
        getStamBar().gameObject.SetActive(state);
        getStamUI().gameObject.SetActive(state); }

    public void displayXPTracker(bool state) {
        getXPBar().gameObject.SetActive(state);
        getXpUI().gameObject.SetActive(state); }

    public void displayEnemyCount(bool state) {
        getEnemyRemainCount().gameObject.SetActive(state);
        getEnemyRemainLabel().gameObject.SetActive(state); }

    /// each menu needs another GameObject var nameMenuFirst for arrow key support in the menus, drag the first button in that menu into this slot in gameManager
    public void openSettingsMenu() {
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openConfirmationMenu()
    {
        menuActive.SetActive(false);
        menuActive = menuConfirmation;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(confirmationMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openQuitMenu()
    {
        menuActive.SetActive(false);
        menuActive = menuQuit;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(quitMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openOptionsMenu() {
        menuActive.SetActive(false);
        menuActive = menuOptions;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openHowToMenu() {
        menuActive.SetActive(false);
        menuActive = menuHowTo;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(howToMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openTipsMenu() {
        menuActive.SetActive(false);
        menuActive = menuTips;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(tipsMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openControlsMenu() {
        menuActive.SetActive(false);
        menuActive = menuControls;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(controlsMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openContinueMenu() {
        EnemiesRemainingCount.maxVisibleWords = 0;
        EnemiesRemainingLabel.maxVisibleWords = 0;
        statePause();
        menuActive = menuContinue;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(continueMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openStatsMenu() {
        menuActive.SetActive(false);
        menuActive = menuStats;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(statsMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void nextRoomContinue() {

        stateUnpause();
        menuActive = null;
        bossRoom.instance.startNextRoom();
        displayBossBar(true);

        // Note: There is a bug where the song will be interrupted if the player hits continue again.
        if (gameManager.instance.getPlayerScript().getAudioLocation() != audioManager.instance.bossMusic) {
            gameManager.instance.getPlayerScript().getAudioLocation().Stop();
            gameManager.instance.getPlayerScript().getAudioLocation().PlayOneShot(audioManager.instance.bossMusic, audioManager.instance.bossMusicVol);
        }
            
    }

    public void openTerminal()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }

        statePause();
        menuActive = menuTerminal;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(terminalMenuFirst);
    }

    public void openUpgradeMenu() {
        if (menuActive != null) {
            menuActive.SetActive(false);
        }
        menuActive = menuUpgrade;
        menuActive.SetActive(true);
        upgradeMenu.upgradeUI.setVars();
        EventSystem.current.SetSelectedGameObject(upgradeMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openTerminalUpgradeMenu()
    {
        if (playerStats.Stats.getPWRLevel() == 3)
        {
            if (menuActive != null)
            {
                menuActive.SetActive(false);
            }
            menuActive = menuTerminalUpgrade;
            menuActive.SetActive(true);
            upgradeMenu.upgradeUI.setTVars();
            EventSystem.current.SetSelectedGameObject(terminalUpgradeMenuFirst); // Set eventsystem selected game object to the button assigned
        }
    }

    public IEnumerator showLevelPopUp()
    {
        
        levelPopUp.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.7f);
        levelPopUp.gameObject.SetActive(false);
        playerScript.setLeveledUp(false);

    }

    public void openStoreMenu() {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }
        // Update the store before it is displayed
        storeManager.instance.setTerminalStatus(false);
        storeManager.instance.updateStoreUI();
        menuActive = menuStore;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(storeMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void openTerminalStoreMenu()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }
        // Update the store before it is displayed
        storeManager.instance.setTerminalStatus(true);
        storeManager.instance.updateStoreUI();
        menuActive = menuTerminalStore;
        menuActive.SetActive(true);
        EventSystem.current.SetSelectedGameObject(terminalStoreMenuFirst); // Set eventsystem selected game object to the button assigned
    }

    public void newGame() {
        if (playerMovement.player.getGunList().Count > 0) {
            startFailMessage.SetActive(false);
            stateUnpause();
            loadout.instance.setMenuOpen();
            timerTracker.SetActive(true);
            displayUI(true);
        }
        else {
            startFailMessage.SetActive(true);
        }
    }

    public void backButton() {
        if (menuActive == menuOptions) {
            menuActive.SetActive(false);
            menuActive = menuPause;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuSettings || menuActive == menuHowTo || menuActive == menuControls) {
            menuActive.SetActive(false);
            menuActive = menuOptions;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuUpgrade)
        {
            menuActive.SetActive(false);
            openContinueMenu();
        }
        else if (menuActive == menuStore)
        {
            menuActive.SetActive(false);
            menuActive = menuContinue;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuContinue)
        {
            menuActive.SetActive(false);
            stateUnpause();
            menuActive = null;
        }
        else if (menuActive == menuTips) {
            menuActive.SetActive(false);
            menuActive = menuHowTo;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuStats) {
            menuActive.SetActive(false);
            menuActive = menuJournal;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuLoseStats) {
            menuActive.SetActive(false);
            menuActive = menuLose;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuWinStats) {
            menuActive.SetActive(false);
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuConfirmation)
        {
            menuActive.SetActive(false);
            menuActive = menuPause;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuQuit)
        {
            menuActive.SetActive(false);
            menuActive = menuPause;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuTerminal)
        {
            menuActive.SetActive(false);
            stateUnpause();
            menuActive = null;
        }
        else if (menuActive == menuTerminalStore)
        {
            menuActive.SetActive(false);
            menuActive = menuTerminal;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuTerminalUpgrade)
        {
            menuActive.SetActive(false);
            menuActive = menuTerminal;
            menuActive.SetActive(true);
        }

        if (menuActive == menuPause)
        {
            EventSystem.current.SetSelectedGameObject(pauseMenuFirst); // Set eventsystem selected game object to the button assigned
        } 
        else if (menuActive == menuOptions)
        {
            EventSystem.current.SetSelectedGameObject(optionsMenuFirst); // Set eventsystem selected game object to the button assigned
        } 
        else if (menuActive == menuContinue)
        {
            EventSystem.current.SetSelectedGameObject(continueMenuFirst); // Set eventsystem selected game object to the button assigned
        }
        else if (menuActive == menuTerminal)
        {
            EventSystem.current.SetSelectedGameObject(terminalMenuFirst); // Set eventsystem selected game object to the button assigned
        }

    }

    public void scopeZoomIn() {
        if (menuActive == null) { // won't work if there is a menu active
            menuActive = sniperScope;
            getPlayerScript().getGunModel().SetActive(false);
            getSniperScope().SetActive(true);
            CameraMovement.state.getCam().fieldOfView = 20;
        }
    }

    public void scopeZoomOut() {
        if (menuActive == sniperScope) {
            menuActive = null;
            getPlayerScript().getGunModel().SetActive(true);
            getSniperScope().SetActive(false);
            CameraMovement.state.getCam().fieldOfView = CameraMovement.state.getNormalFOV();
        }
    }

    public int getEnemyCountOriginal()
    { return enemyCountOrig; }
    public void setEnemyCountOrig(int _count)
    { enemyCountOrig += _count; }

    public GameObject getMenuLoadout() {
        return menuLoadout; }

    public GameObject getUpgradeMenu() {
        return menuUpgrade; }

    public GameObject getPlayerSpawnPos() { return playerSpawnPos; }

    public void setPlayerSpawnPos(GameObject _playerSpawnPos) { playerSpawnPos = _playerSpawnPos; }

    public GameObject getCheckPointPopup() { return checkPointPopup; }

    public void setCheckPointPopup(GameObject _checkPointPopup) { checkPointPopup = _checkPointPopup; }

    public GameObject getDmgFlash() { return damagePanelFlash; }

    public void setDmgFlash(GameObject _dmgPanel) { damagePanelFlash = _dmgPanel; }

    public GameObject getHealIndicator() { return healIndicator; }

    public GameObject getAmmoWarning() { return ammoWarning; }

    public void setAmmoWarning(GameObject _ammoWarning) { ammoWarning = _ammoWarning; }

    public TMP_Text getHPText() {
        return HPText; }

    public TMP_Text getStamText() {
        return stamText; }

    public TMP_Text getXPText() {
        return XPText; }

    public GameObject getTimer() {
        return timer; }

    public GameObject getTimerTracker() {
        return timerTracker; }

    public TMP_Text getTimerText() {
        return timerText;
    }

    public TMP_Text getAmmoText() {
        return ammoText; }

    void setHPText(TMP_Text _HPText) {
        HPText = _HPText; }

    void setStamText(TMP_Text _stamText) {
        stamText = _stamText; }

    void setXPText(TMP_Text _XPText) {
        XPText = _XPText; }

    void setAmmoText(TMP_Text _ammoText) {
        ammoText = _ammoText;
    }

    public GameObject getAmmoUI() {
        return ammoUI; }

    void setAmmoUI(GameObject _ammoUI) {
        ammoUI = _ammoUI; }

    public GameObject getHealthWarning() { return lowHealthWarning; }

    public void setHealthWarning(GameObject _lowHealthWarning) { lowHealthWarning = _lowHealthWarning; }

    public GameObject getSniperScope() {
        return sniperScope; }

    public playerMovement getPlayerScript() { return playerScript; }

    public GameObject getPlayer() { return player; }

    public void setPlayer(GameObject _player) { player = _player; }

    public void setPlayerScript(playerMovement _script) { playerScript = _script; }

    public bool getPauseStatus() { return isPaused; }

    public void setPauseStatus(bool _status) { isPaused = _status; }

    public Image getPlayerReticle() { return playerReticle; }

    public void setPlayerReticle(Image newReticle) { playerReticle = newReticle; }

    public int getBossCount() { return bossCount; }

    public void setBossCount(int _amount) { bossCount = _amount; }

    public void setLevelTracker(TMP_Text newLevel) { levelTracker = newLevel; }

    public TMP_Text getLevelTracker() { return levelTracker; }

    public void setHPBar(Image newHPBar) { HPBar = newHPBar; }

    public Image getHPBar() { return HPBar; }

    public GameObject getShieldBar() { return shieldBar; }
    public Image getShieldBarImage() { return shieldBarImage; }
    public TMP_Text getShieldText() { return ShieldHPText; }

    public void setXPBar(Image newXPBar) { XPBar = newXPBar; }

    public Image getXPBar() { return XPBar; }

    public TMP_Text getHealsUI() { return healsLeft; }
    public TMP_Text getGrenadesUI() { return grenadesLeft; }

    public void setBossHP(GameObject newBossHP) { bossHP = newBossHP; }

    public GameObject getBossHP() { return bossHP; }

    public void setBossHPBar(Image newBossHPBar) { bossHPBar = newBossHPBar; }

    public Image getBossHPBar() { return bossHPBar; }

    public void setStamBar(Image newStamBar) { stamBar = newStamBar; }

    public Image getStamBar() { return stamBar; }

    public int getEnemyCount() { return enemyCount; }

    public void setEnemyCount(int _count) { enemyCount = _count; }

    public void setEnemyRemainCount(TMP_Text newEnemyCount) { EnemiesRemainingCount = newEnemyCount; }

    public TMP_Text getEnemyRemainCount() { return EnemiesRemainingCount; }

    public void setEnemyRemainLabel(TMP_Text newEnemyLabel) { EnemiesRemainingLabel = newEnemyLabel; }

    public TMP_Text getEnemyRemainLabel() { return EnemiesRemainingLabel; }

    public void setAmmoBar(Image newAmmoBar) { ammoTrackerBar = newAmmoBar; }

    public Image getAmmoBar() { return ammoTrackerBar; }

    public GameObject getStamUI() { return stamUI; }

    public GameObject getHealthUI() { return hpUI; }

    public GameObject getBuffUI() { return buffUI; }

    public Image getBuffIcon() { return buffIcon; }

    public GameObject getBuffUI2() { return buffUI2; }

    public Image getBuffIcon2() { return buffIcon2; }

    public GameObject getXpUI() { return xpUI; }

    public Button getRespawnButton() { return respawnButton; }

    public void setRespawnButton(Button _respawnButton) { respawnButton = _respawnButton; }

    public TMP_Text getLivesText() { return livesText; }

    public void setLivesText(TMP_Text _livesText) { livesText = _livesText; }

    public int getPowerItems()
    { return powerItems; }

    public void setPowerItems(int _items)
    { powerItems += _items; }

    public void resetPwrItems()
    {
        powerItems = 0;
    }
    public TMP_Text getAmmoReserveText() { return ammoReserveText; }

    public void setAmmoReserveText(TMP_Text _ammoReserveText)
    { ammoReserveText = _ammoReserveText; }

    public GameObject getInteractUI() { return interactUI; }

    public void setInteractUI (GameObject _interactUI) 
    { interactUI = _interactUI; }

    public TMP_Text getIDBadgeWarning()
    {
        return idBadgeLvlWarning;
    }

    public void setIDBadgeWarning(TMP_Text _idBadgeWarningText)
    {
        idBadgeLvlWarning = _idBadgeWarningText;
    }

    public TMP_Text getPwrLvlWarning()
    {
        return pwrLvlWarning;
    }

    public float getTimeScaleOrig()
    {
        return timeScaleOrig;
    }
    public GameObject getTyPopUp() { return TyEasterEggPopUp;  }

        public void setPwrLvlWarning(TMP_Text _pwrLevelWarningText)
    {
        pwrLvlWarning = _pwrLevelWarningText;
    }

    public TMP_Text getRepairWarning()
    {
        return repairWarning;
    }

    public void setRepairWarning(TMP_Text _repairWarningText)
    {
        repairWarning = _repairWarningText;
    }

    public GameObject getPowerLevelPopup()
    {
        return powerLevelPopup;
    }

    public void setRepairWarning(GameObject _powerLevelPopup)
    {
        powerLevelPopup = _powerLevelPopup;
    }

    public TMP_Text getPowerLevelText()
    {
        return powerLevelText;
    }

    public void setPowerLevelText(TMP_Text _powerLevelText)
    {
        powerLevelText = _powerLevelText;
    }

    public GameObject getConfirmationMenu()
    {
        return menuConfirmation;
    }

    public void setConfirmationMenu(GameObject _confirmationMenu)
    {
        menuConfirmation = _confirmationMenu;
    }

    public GameObject getMenuActive()
    {
        return menuActive;
    }

    public void setMenuActive(GameObject _menu)
    {
        menuActive = _menu;
    }
}
