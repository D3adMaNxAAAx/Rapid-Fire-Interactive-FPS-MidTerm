using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class repairItems : MonoBehaviour
{
    [SerializeField] GameObject repairObj;
    [SerializeField] GameObject journalIcon;
    
    [SerializeField] Canvas pickedUpFeedback;


    bool isOpen;
    bool isPickedUp;


    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        StartCoroutine(hideFeedback());
    }


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            Destroy(repairObj);
            isPickedUp = true;
            pickedUpFeedback.enabled = true;
            isOpen = true;
            gameManager.instance.setPowerItems(1);
            journalIcon.SetActive(true);
        }
        else
            return;
    }

    IEnumerator hideFeedback()
    {
        yield return new WaitForSeconds(1.2f);
        pickedUpFeedback.enabled = false;
        isOpen = false;
        
    }
}
