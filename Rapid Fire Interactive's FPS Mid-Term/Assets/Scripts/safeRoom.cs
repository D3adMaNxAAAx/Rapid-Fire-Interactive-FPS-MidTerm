using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class safeRoom : MonoBehaviour, IInteractable {

    [SerializeField] int roomNum;
    [Header("-- Door Variables --")]
    [SerializeField] GameObject safeSpawnPos;
    [SerializeField] GameObject badgeScanner;
    [SerializeField] GameObject activeDoor;
    //[SerializeField] bool safeAccess;

    [Header("-- Open Information --")]
    [SerializeField] float doorMoveX;
    [SerializeField] float doorMoveY;
    [SerializeField] float doorMoveZ;
    [SerializeField] float timeToOpen;

    Vector3 openPos;
    Vector3 closePos;

    bool isSafe;
    bool isOpen;
    int badgesNeeded;


    // Start is called before the first frame update
    void Start()
    {
        closePos = activeDoor.transform.position; // Grab door position on load to set as close position
        openPos = new Vector3(closePos.x + doorMoveX, closePos.y + doorMoveY, closePos.z + doorMoveZ);
        badgesNeeded = 3 * roomNum;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the door has been opened
        if (isOpen) { StartCoroutine(openDoor()); }
        else { StartCoroutine(closeDoor()); }
    }

    public void interact()
    {
        // Player pressed button, open door.
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

            if (Input.GetButton("Interact") && playerMovement.player.getSafeAccess() && playerStats.Stats.getBadgesFound() >= badgesNeeded)
            {
                interact();
                gameManager.instance.getInteractUI().SetActive(false);
            } 
            else if (Input.GetButton("Interact") && !playerMovement.player.getSafeAccess())
            {
                StartCoroutine(flashPowerWarning());
            } 
            else if (Input.GetButton("Interact") && playerStats.Stats.getBadgesFound() < badgesNeeded)
            {
                StartCoroutine(flashIDWarning());
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

    // Door Methods to Open & Close
    public IEnumerator openDoor()
    {
        if (activeDoor.transform.position != openPos)
        {
            float smoothOpen = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, openPos, smoothOpen);
            isSafe = true;
        }
        yield return null;
    }

    public IEnumerator closeDoor()
    {
        if (activeDoor.transform.position != closePos)
        {
            float smoothOpen = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, closePos, smoothOpen);

            if (isSafe) { gameManager.instance.setPlayerSpawnPos(safeSpawnPos); }

            isSafe = false;
            isOpen = false;
        }

        yield return null;
    }

    public IEnumerator flashPowerWarning()
    {
        gameManager.instance.getIDBadgeWarning().gameObject.SetActive(true);
        gameManager.instance.getIDBadgeWarning().text = "SafeRoom " + roomNum + ": Power Level 1 needed!";
        yield return new WaitForSeconds(2.5f);
        gameManager.instance.getIDBadgeWarning().gameObject.SetActive(false);
    }

    public IEnumerator flashIDWarning()
    {
        gameManager.instance.getIDBadgeWarning().gameObject.SetActive(true);
        
        gameManager.instance.getIDBadgeWarning().text = "SafeRoom " + roomNum + ": " + badgesNeeded + " ID Badges Needed!";
        yield return new WaitForSeconds(2.5f);
        gameManager.instance.getIDBadgeWarning().gameObject.SetActive(false);
    }

    // Getters
    public bool getSafeState() { return isSafe; }
    //public bool getSafeAccess() { return safeAccess; }

    // Setters
    public void setSafeState(bool _state) { isSafe = _state; }

    //public void setSafeAccess(bool _state) { safeAccess = _state; }
}
