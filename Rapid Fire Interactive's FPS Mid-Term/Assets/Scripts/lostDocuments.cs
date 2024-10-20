using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class lostDocuments : MonoBehaviour , IInteractable
{
    [SerializeField] GameObject lostDocument;
    [SerializeField] GameObject journalIcon;
    [SerializeField] Canvas pickedUpFeedback;
    [SerializeField] Image docOpened;
    [SerializeField] GameObject closeButton;
    [SerializeField] Image activeDoc;
    bool isOpen;

    // Unused variables
    // bool isPickedUp;
    // bool docOpen;
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
            StartCoroutine(hideFeedback());
        //interact button for picking up document 
    }

    public void interact()
    {
        Destroy(lostDocument);
        //isPickedUp = true;
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        
        pickedUpFeedback.enabled = true;
        isOpen = true;

        if (journalIcon != null)
            journalIcon.SetActive(true);
       
        
    }

    private void OnTriggerStay(Collider other)
    {
        // Since it will await the player input, use OnTriggerStay.
        if (other.CompareTag("Player"))
        {
            // If interact menu isn't on, turn it on.
            if (!gameManager.instance.getInteractUI().activeInHierarchy && !lostDocument.IsDestroyed())
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
        playerStats.Stats.docFound();
        


    }

    public void openDoc()
    {
        
        if (journalIcon.gameObject.activeInHierarchy && activeDoc == null)
        {
            docOpened.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);
            //docOpen = true;
            activeDoc = docOpened;
        }
        
    }

    public void closeDoc()
    {
        activeDoc.gameObject.SetActive(false); 
        activeDoc = null;
        //docOpen = false;
        closeButton.gameObject.SetActive(false);
        
    }


    //method to pick up document 
    //pickup doc in scene
    //open doc canvas
    //close doc canvas
    //add doc to journal


}
