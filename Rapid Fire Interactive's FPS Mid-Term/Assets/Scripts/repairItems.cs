using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class repairItems : MonoBehaviour
{
    [SerializeField] GameObject repairObj;
    
    [SerializeField] Canvas pickedUpFeedback;


    bool isOpen;
    bool isPickedUp;


    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            Destroy(repairObj);
            isPickedUp = true;
            pickedUpFeedback.enabled = true;
            isOpen = true;
        }
        else
            return;
    }
}
