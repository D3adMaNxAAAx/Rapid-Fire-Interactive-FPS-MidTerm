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

    bool isPickedUp;
    bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
            StartCoroutine(hideFeedback());
    }

    public void interact()
    {
        Destroy(badge);
        isPickedUp = true;
        pickedUpFeedback.enabled = true;
        isOpen = true;
        journalIcon.SetActive(true);

    }

    private void OnTriggerStay(Collider other)
    {
        // Since it will await the player input, use OnTriggerStay.
        if (other.CompareTag("Player"))
        {
            // If interact menu isn't on, turn it on.
            if (!gameManager.instance.getInteractUI().activeInHierarchy && !badge.IsDestroyed())
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
        playerStats.Stats.idBadgeFound();
        Destroy(toDestroy);


    }
}
