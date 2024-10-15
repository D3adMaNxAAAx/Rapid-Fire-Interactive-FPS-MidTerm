using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorControllerManual : MonoBehaviour, IInteractable
{
    // Using safeRoom as a reference

    // Member Fields
    // Singleton
    public static doorControllerManual instance;

    // Door Variables
    [Header("-- Door Variables --")]
    [SerializeField] GameObject activeDoor;

    [Header("-- Open Information --")]
    [SerializeField] float doorMoveX;
    [SerializeField] float doorMoveY;
    [SerializeField] float doorMoveZ;
    [SerializeField] float timeToOpen;

    Vector3 openPos;
    Vector3 closePos;

    // Checks / Bools
    bool isOpen;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        closePos = activeDoor.transform.position; // Grab door position on load to set as close position
        openPos = new Vector3(closePos.x + doorMoveX, closePos.y + doorMoveY, closePos.z + doorMoveZ);
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

            if (Input.GetButton("Interact"))
            {
                interact();
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

    // Door Methods to Open & Close
    public IEnumerator openDoor()
    {
        if (activeDoor.transform.position != openPos)
        {
            float smoothOpen = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, openPos, smoothOpen);
        }
        yield return null;
    }

    public IEnumerator closeDoor()
    {
        if (activeDoor.transform.position != closePos)
        {
            float smoothOpen = Time.deltaTime * timeToOpen;
            activeDoor.transform.position = Vector3.Lerp(activeDoor.transform.position, closePos, smoothOpen);
            isOpen = false;
        }
        yield return null;
    }

    // Getter
    public bool getDoorStatus() { return isOpen; }

    // Setter
    public void setDoorStatus(bool _state) { isOpen = _state; }
}
