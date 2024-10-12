using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idBadges : MonoBehaviour
{

    [SerializeField] GameObject badge;
    [SerializeField] GameObject journalIcon;

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
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Destroy(badge);
            isPickedUp = true;
            pickedUpFeedback.enabled = true;
            isOpen = true;
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
