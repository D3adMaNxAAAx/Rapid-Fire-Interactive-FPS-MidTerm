using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    //Tracks game instance 
    public static gameManager instance;

    

    //UI Active menu tracker 
    [SerializeField] GameObject menuActive;

    //UI Pause menu tracker 
    [SerializeField] GameObject menuPause;

    //UI Win menu tracker 
    [SerializeField] GameObject menuWin;

    //UI Lose menu tracker 
    [SerializeField] GameObject menuLose;

    //UI damage Panel tracker
    [SerializeField] GameObject damagePanelFlash;

    public GameObject getDmgFlash()
    { return damagePanelFlash; }

    public void setDmgFlash(GameObject _dmgPanel)
    { damagePanelFlash = _dmgPanel; }

    //Tracks playerController field
    [SerializeField]  playerMovement playerScript;

    public playerMovement getPlayerScript() 
    { return playerScript; }

    public void setPlayerScript(playerMovement _script)
    { playerScript = _script; }

    //Tracks player object
    [SerializeField] GameObject player;

    public GameObject getPlayer()
    { return player; }

    public void setPlayer(GameObject _player)
    { player = _player; }

    //Tracks game pause status
    bool isPaused;

    public bool getPauseStatus()
    { return isPaused; }

    public void setPauseStatus(bool _status)
    { isPaused = _status; }

    //Tracks all enemies in game 
    int enemyCount;

    //For when we make boss monster
    int bossCount;

    public int getBossCount()
    { return bossCount; }

    public void setBossCount(int _amount)
        { bossCount = _amount; }

    //Tracks & stores original game time scale 
    float timeScaleOrig; 




    // Start is called before the first frame update
    void Awake()
    {
        //set's game instance to this instance 
        instance = this;

        //Setting time scale on game awake to set scale 
        timeScaleOrig = Time.timeScale;

        //Setting player tracker to player object in engine by tag name set on player object
        setPlayer(GameObject.FindWithTag("Player"));

        //setting the player script from the above player tracker script component 
        setPlayerScript(getPlayer().GetComponent<playerMovement>());


    }

    // Update is called once per frame
    void Update()
    {
        //When ESC clicked
        if (Input.GetButtonDown("Cancel"))
        {
            //if no menu is active
            if (menuActive == null)
            {
                //pause game time 
                statePause();

                //Set the pause menu as active menu
                menuActive = menuPause;

                //Show active menu(pause menu)
                menuActive.SetActive(getPauseStatus());

            }

            //if we are on pause menu
            else if (menuActive == menuPause)
            {
                //Tell game to unpause and close menus 
                stateUnpause();
            }
        }
    }
    public void statePause()
    {
        //toggle switch for pause status (Set pause to opposite of what it is )
        setPauseStatus(!getPauseStatus());

        //set time scale back to original
        Time.timeScale = 0;

        //Hide cursor
        Cursor.visible = true;

        //Locks out cursor
        Cursor.lockState = CursorLockMode.Confined;

    }

    public void stateUnpause()
    {
        //toggle switch for pause status (Set pause to opposite of what it is )
        setPauseStatus(!getPauseStatus());

        //set time scale back to original
        Time.timeScale = timeScaleOrig;

        //Hide cursor
        Cursor.visible = false;

        //Locks out cursor
        Cursor.lockState = CursorLockMode.Locked;

        //sets active menu to pause based off Pause Status
        menuActive.SetActive(getPauseStatus());

        //Saftey Hard cade to make sure active menu is now empty
        menuActive = null;

    }

    public void updateGameGoal(int _enemyCount, int _bossCount = 0)
    {
        //Set enemy count to passed in count 
        enemyCount = _enemyCount;

        //Set boss count to passed in count default 0 till basic enemies defeated
        setBossCount(_bossCount);
        if (enemyCount <= 0 && getBossCount() <= 0)
        {
            //pause game time 
            statePause();

            //set active menu to win menu
            menuActive = menuWin;

            //tell it to show based off pause status
            menuActive.SetActive(getPauseStatus());
        }
    
    }

    //Show lose menu on player death
    public void youLose()
    {
        //pause game time
        statePause();

        //set active menu to lose menu
        menuActive = menuLose;

        //Show lose menu 
        //Hard code to true avoid glitch where menu won't show sometimes
        menuActive.SetActive(true);
    
    }
}
