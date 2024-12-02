using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bossRoom : MonoBehaviour
{

    public static bossRoom instance;
    //tracks door button
    [SerializeField] GameObject doorButton;

    //tracks door trying to open
    [SerializeField] GameObject activeDoor;

    //Tell door where to move on open
    [SerializeField] float doorMoveX;
    [SerializeField] float doorMoveY;
    [SerializeField] float doorMoveZ;
    [SerializeField] float timeToOpen;

    // Bool for boss fight state
    bool isFightingBoss = false;


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
        // Check if the door is open
        if (isOpen) { StartCoroutine(openDoor()); }
        else { StartCoroutine(closeDoor()); }
    }

    // Move on to next room
    public void startNextRoom()
    {
        isOpen = true;
    }

    private void OnTriggerStay(Collider other)
    {
        // Since it will await the player input, use OnTriggerStay.
        if (other.CompareTag("Player"))
        {
            // If interact menu isn't on, turn it on.
            if (!gameManager.instance.getInteractUI().activeInHierarchy)
                gameManager.instance.getInteractUI().SetActive(true);

            if (Input.GetButton("Interact"))
            {
                gameManager.instance.getInteractUI().SetActive(false);
            }
        }
        else
            return;
    }

    private void OnTriggerExit(Collider other)
    {
        // Turn off the interact UI if the player isn't within range
        if (gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(false);
    }

    // Tell door to open
    public IEnumerator openDoor()
    { 
        if (activeDoor.transform.position != openPos)
        {
            a = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, openPos, a);
            isFightingBoss = true;
        }
        yield return null;
    }

    // tell door to close
    public IEnumerator closeDoor()
    { 
        if (activeDoor.transform.position != closePos)
        {
            a = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, closePos, a);
        }
        yield return null;
    }

    // Getter & Setter
    public bool getBossFightState()
    {
        return isFightingBoss;
    }

    public void setBossFightState(bool _state)
    { isFightingBoss = _state; }
    
    public bool getDoorStatus() { return isOpen; }

    public void setDoorStatus(bool _state) { isOpen = _state; }

}
