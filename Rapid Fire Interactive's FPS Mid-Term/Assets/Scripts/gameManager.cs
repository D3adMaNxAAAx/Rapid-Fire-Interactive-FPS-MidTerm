using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations; // for using textmesh pro

public class gameManager : MonoBehaviour { 

    public static gameManager instance; // singleton

    [SerializeField] GameObject menuActive; // this will change depending on what menu is showing in the game
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject damagePanelFlash;
    [SerializeField] Image playerReticle;

    [SerializeField] GameObject player; //Tracks player object
    [SerializeField] playerMovement playerScript; // Tracks playerController field

    Vector2 reticleSize; // so the player can adjust reticle size through settings & also to change it to and from.
    int enemyCount;
    int bossCount; // For when we make boss monster
    float timeScaleOrig; // Tracks & stores original game time scale
    bool isPaused;

    public GameObject getDmgFlash()
        { return damagePanelFlash; }

    public void setDmgFlash(GameObject _dmgPanel)
        { damagePanelFlash = _dmgPanel; }

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

    [SerializeField] TMP_Text levelTracker; /// make private with getters and setters
    public Image HPBar; /// make private with getters and setters
    public Image bossHP; /// make private with getters and setters
    public Image stamBar; /// make private with getters and setters
    public Image EnemiesRemainingBar; /// make private with getters and setters
    public Image ammoTrackerBar; /// make private with getters and setters

    public void setLevelTracker(TMP_Text newLevel) 
        { levelTracker = newLevel; }

    public TMP_Text getLevelTracker() 
        { return levelTracker; }
    
    public void setHPBar(Image newHPBar) 
        { HPBar = newHPBar; }

    public Image getHPBar() 
        { return HPBar; }

    public void setBossHP(Image newBossHP) 
        { bossHP = newBossHP; }

    public Image getBossHP() 
        { return bossHP; }

    public void setStamBar(Image newStamBar) 
        { stamBar = newStamBar; }

    public Image getStamBar() 
        { return stamBar; }

    public void setEnemyBar(Image newEnemyBar) 
        { EnemiesRemainingBar = newEnemyBar; }

    public Image getEnemyBar() 
        { return EnemiesRemainingBar; }

    public void setAmmoBar(Image newAmmoBar) 
        { ammoTrackerBar = newAmmoBar; }

    public Image getAmmoBar() 
        { return ammoTrackerBar; }

    // Start is called before the first frame update, awake is before start
    void Awake() {

        instance = this; // making instance of this class (singleton)
        timeScaleOrig = Time.timeScale; // Setting time scale on game awake to set scale 
        setPlayer(GameObject.FindWithTag("Player")); // Setting player tracker to player object in engine by tag name set on player object
        setPlayerScript(getPlayer().GetComponent<playerMovement>()); // setting the player script from the above player tracker script component 
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetButtonDown("Cancel")) { // When ESC clicked
            if (menuActive == null) {

                statePause();
                menuActive = menuPause; // Set the pause menu as active menu
                menuActive.SetActive(getPauseStatus()); // Show active menu
            }
            else if (menuActive == menuPause) {
                stateUnpause();
            }
        }
    }

    public void statePause() {
        isPaused = !isPaused; // toggles bool
        Time.timeScale = 0; // pauses everything except UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined; // unlocks cursor but keeps it confined to game window
    }

    public void stateUnpause() {
        isPaused = !isPaused; // toggles bool
        Time.timeScale = timeScaleOrig; //set time scale back to original
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // locks cursor position
        menuActive.SetActive(getPauseStatus()); // Show active menu
        menuActive = null;
    }

    public void updateGameGoal(int _enemyCount, int _bossCount = 0) {
        enemyCount += _enemyCount;
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
    void changeReticle(Vector2 reticleSize) {
        playerReticle.color = Color.red;
        playerReticle.rectTransform.sizeDelta = reticleSize;
    }
}