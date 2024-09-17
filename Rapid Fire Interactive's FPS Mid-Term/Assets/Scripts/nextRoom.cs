using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] float timeToOpen;
    [SerializeField] LayerMask target;

  

    //add ui element to gameManager to tell player to click button to move on to next room when all enemies are killed in current room

    //add functionality for button position click in and click out
    //add button click sound 
    //add door open sound

    Vector3 openPos;
    Vector3 closePos;

    float a; //This will be used as our door's timer that links to real time

    bool isOpen;


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
        if (isOpen)
        {
            StartCoroutine(openDoor());
        }
        else
        {
            StartCoroutine(closeDoor());
        }
    }

    //Move on to next room
    public void startNextRoom()
    {
        //clickDoorButton();
        isOpen = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //LayerMask a = other.gameObject.layer;
        //if (other.gameObject.layer == target) //still trying to figure this out
        //{
            isOpen = false;
        //}
    }

    //Tell door to open
    IEnumerator openDoor()
    { 
        //activeDoor.transform.position = openPos;
        if (activeDoor.transform.position != openPos)
        {
            a = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, openPos, a);
        }
        yield return null;
    }

    //tell door to close
    IEnumerator closeDoor()
    { 
        //activeDoor.transform.position = closePos;
        if (activeDoor.transform.position != closePos)
        {
            a = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, closePos, a);
        }
        yield return null;
    }

    void clickDoorButton()
    {

        //button is clicked on 
       // if ()
      //  {
            //hide enemy counter 


            //pause game 
      

           //menu options work once ty has store made 
                   
                        //upgrade menu spend xp to upgrade max hp / max dmg / max stamina / max ammo
                            
                            //open store menu
                                //store menu spend money to buy hp / ammo
                                   
                                        //continue button
                                         //close menus and unpause game
                                            //open door

                                                //player walks through door
                                                    //on trigger exit close door
                                                        //spawn enemies / show updated enemy counter

                    //back click close currnt menu and unpause game if on room complete menu

            
      //  }
    }
}
