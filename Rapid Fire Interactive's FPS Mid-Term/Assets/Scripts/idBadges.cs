using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class idBadges : MonoBehaviour, IInteractable
{

    [SerializeField] GameObject badge;
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
        playerStats.Stats.idBadgeFound();
        Destroy(badge);
        //isPickedUp = true;
        
        pickedUpFeedback.enabled = true;
        isOpen = true;

        if (journalIcon != null)
            journalIcon.SetActive(true);

    }

    private void OnTriggerStay(Collider other)
    {
        // Since it will await the player input, use OnTriggerStay.
        if (other.CompareTag("Player") && !badge.IsDestroyed())
        {
            // If interact menu isn't on, turn it on.
            if (!gameManager.instance.getInteractUI().activeInHierarchy )
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
        if (gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(false);
    }

    IEnumerator hideFeedback()
    {
        yield return new WaitForSeconds(1.2f);
        pickedUpFeedback.enabled = false;
        isOpen = false;
        Destroy(toDestroy);


    }
}
