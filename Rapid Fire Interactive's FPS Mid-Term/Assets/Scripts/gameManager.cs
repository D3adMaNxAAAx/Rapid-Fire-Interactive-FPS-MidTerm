using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // for using textmesh pro

public class gameManager : MonoBehaviour {

    /// to do: hp bar, stam bar, enemy counter, xp thing, boss count for boss tracker (Have boss tracker show when bossCount = 1) and boss progress  fill bar boss = boss hp 

    public static gameManager instance; // singleton

    [SerializeField] GameObject menuActive; // this will change depending on what menu is showing in the game
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text enemiesRemaningUI;
    [SerializeField] GameObject damagePanelFlash;
    public Image HPBar; /// make private with getters and setters

    [SerializeField] GameObject player; //Tracks player object
    [SerializeField] playerMovement playerScript; // Tracks playerController field

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

    /*public int getBossCount()
    { return bossCount; }
    public void setBossCount(int _amount)
        { bossCount = _amount; }*/

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
    void changeReticle()
    {

    }
}
