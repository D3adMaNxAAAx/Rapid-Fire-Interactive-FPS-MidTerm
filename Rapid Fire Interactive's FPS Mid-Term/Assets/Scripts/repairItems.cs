using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class repairItems : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject repairObj;
    [SerializeField] GameObject journalIcon;
    [SerializeField] GameObject toDestroy;
    [SerializeField] Canvas pickedUpFeedback;
    bool isOpen;
    
    // Unused Variables
    //bool isPickedUp;

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
            StartCoroutine(hideFeedback());
    }

    public void interact()
    {
        isOpen = true;
        Destroy(repairObj);
        //isPickedUp = true;

        pickedUpFeedback.enabled = true;
        gameManager.instance.setPowerItems(1);
        playerStats.Stats.objectFound();

        if (journalIcon != null)
            journalIcon.SetActive(true);
        gameManager.instance.getInteractUI().SetActive(false);
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!isOpen)
        {
            // Since it will await the player input, use OnTriggerStay.
            if (other.CompareTag("Player") && !repairObj.IsDestroyed())
            {
                // If interact menu isn't on, turn it on.
                if (!gameManager.instance.getInteractUI().activeInHierarchy && !repairObj.IsDestroyed())
                    gameManager.instance.getInteractUI().SetActive(true);

                if (Input.GetButton("Interact"))
                {
                    interact();
                }
            }
            else
                return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(false);
    }

    IEnumerator hideFeedback()
    {
        yield return new WaitForSeconds(1.2f);
        pickedUpFeedback.enabled = false;
        Destroy(toDestroy);
    }
}
