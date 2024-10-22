using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupWeapon : MonoBehaviour
{
    [SerializeField] gunStats gun;
    bool isPickedUp = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Show interact menu
            if (!gameManager.instance.getInteractUI().activeInHierarchy && !isPickedUp)
                gameManager.instance.getInteractUI().SetActive(true);

            if (Input.GetButton("Interact") && !isPickedUp)
            {
                interact();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide interact menu
        if (gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(false);
    }

    public void interact()
    {
        isPickedUp = true;
        gameManager.instance.getPlayerScript().getGunStats(gun);

        // Hide interact menu
        if (gameManager.instance.getInteractUI().activeInHierarchy)
            gameManager.instance.getInteractUI().SetActive(false);
    }
}
