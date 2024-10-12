using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class lostDocuments : MonoBehaviour
{
    [SerializeField] GameObject lostDocument;
    [SerializeField] GameObject journalIcon;

    [SerializeField] Canvas documentUI;

    bool isOpen;
    bool isPickedUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //interact button for picking up document 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Destroy(lostDocument);
            isPickedUp = true;
            documentUI.enabled = true;
            isOpen = true;
           
            journalIcon.SetActive(true);
        }
        else
            return;
    }

    IEnumerator hideFeedback()
    {
        yield return new WaitForSeconds(20);
        documentUI.enabled = false;
        isOpen = false;

    }

    //method to pick up document 
    //pickup doc in scene
    //open doc canvas
    //close doc canvas
    //add doc to journal


}
