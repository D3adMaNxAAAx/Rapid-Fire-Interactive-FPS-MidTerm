using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextRoom : MonoBehaviour
{

    public static nextRoom instance;
    //tracks door button
    [SerializeField] GameObject doorButton;

    //tracks door trying to open
    [SerializeField] GameObject activeDoor;

    //Tell door where to move on open
    [SerializeField] float doorMoveX;
    [SerializeField] float doorMoveY;
    [SerializeField] float doorMoveZ;


    //menu that shows upgrade purchases before next room
    [SerializeField] GameObject menuNextRoom;

    //add ui element to gameManager to tell player to click button to move on to next room when all enemies are killed in current room

    //add functionality for button position click in and click out
    //add button click sound 
    //add door open sound

    Vector3 openPos;
    Vector3 closePos;

  


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        closePos = activeDoor.transform.position;
        openPos = new Vector3(closePos.x + doorMoveX, closePos.y + doorMoveY, closePos.z + doorMoveZ);

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Move on to next room
    void startNextRoom()
    {
        clickDoorButton();
        
    }

    private void OnTriggerExit(Collider other)
    {
        closeDoor();
    }

    //Tell door to open
    void openDoor()
    { activeDoor.transform.position = openPos; }

    //tell door to close
    void closeDoor()
    { activeDoor.transform.position = closePos; }

    void clickDoorButton()
    {

        //button is clicked on 
       // if ()
      //  {
            //hide enemy counter 


            //pause game 
      //      gameManager.instance.statePause();

            //show menu do you want to continue to next room 
                //differnt upgrade and buy menus buttons
                    //click show upgrade menu
                        //upgrade menu spend xp to upgrade max hp / max dmg / max stamina / max ammo
                            //after apply all on upgrade menu
                            //open store menu
                                //store menu spend money to buy hp / ammo
                                    //after confirm on store menu
                                    //continue button
                                        //close menus and unpause game
                                            //open door
                                                //player walks through door
                                                    //on trigger exit close door
                                                        //spawn enemies / show updated enemy counter

                    //backclick close currnt menu and unpause game if on room complete menu

            
      //  }
    }
}
