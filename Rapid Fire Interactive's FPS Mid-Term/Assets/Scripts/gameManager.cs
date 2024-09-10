using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using static UnityEngine.UIElements.UxmlAttributeDescription;

public class gameManager : MonoBehaviour {
    public static gameManager manager; // singleton

    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuActive; // this will change depending on what menu is showing in the game
    [SerializeField] GameObject menuPause;
    GameObject damageFlashPanel;

    int enemyCount;
    float timeScaleOrig; // Tracker for the original time scale of game 
    bool isPaused; 
    GameObject player;
    playerController playerScript;

    // getter / setter for damageFlashPanel
    public GameObject getDamageFlash() {
        return damageFlashPanel;
    }
    public void setDamageFlash(GameObject flashPanel) {
        damageFlashPanel = flashPanel;
    }

    // getter / setter for isPaused
    public bool getPauseStatus() {
        return isPaused;
    }
    public void setPauseStatus(bool pauseStatus) {
        isPaused = pauseStatus;
    }

    // getter / setter for player (game object)
    public GameObject getPlayer() {
        return player;
    }
    public void setPlayer(GameObject newPlayer) {
        player = newPlayer;
    }

    // getter / setter for playerScript
    public playerController getPlayerScript() {
        return playerScript;
    }
    public void setPlayerScript(playerController newPlayerScript) {
        playerScript = newPlayerScript;
    }

    // Start is called before the first frame update
    void Awake() {
        manager = this; // making instance of this class (singleton)
        timeScaleOrig = Time.timeScale; // Set the original time scale to the time scale of game no matter what it may be
        player = GameObject.FindWithTag("Player"); // needs to be tagged in Unity
        playerScript = gameObject.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if (menuActive == null) {
                SetPauseStatus();
                menuActive = menuPause; 
                menuActive.SetActive(isPaused); // show / hides the menu in the game
            }
            else if (menuActive == menuPause) {
                stateUnpause();
            }
        }
    }

    public void SetPauseStatus() {
        isPaused = !isPaused;
        Time.timeScale = 0; // pauses everything except UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined; // unlocks cursor but keeps it confined to game window
    }

    public void stateUnpause() {
        isPaused = !isPaused; // toggles bool
        Time.timeScale = timeScaleOrig; // unpauses everything
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused); // show / hides the menu in the game
        menuActive = null;
    }

    public void updateGameGoal(int amount) {
        enemyCount += amount;
        if (enemyCount <= 0) {
            SetPauseStatus();
            menuActive = menuWin;
            menuActive.SetActive(true); // show / hides the menu in the game
        }
    }

    public void gameLost() {
        SetPauseStatus();
        menuActive = menuLose;
        menuActive.SetActive(true); // show / hides the menu in the game
    }
}
