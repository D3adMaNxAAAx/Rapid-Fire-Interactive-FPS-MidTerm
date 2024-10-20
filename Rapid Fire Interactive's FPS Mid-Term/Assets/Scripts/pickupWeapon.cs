using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupWeapon : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Show interact menu
            if (!gameManager.instance.getInteractUI().activeInHierarchy)
                gameManager.instance.getInteractUI().SetActive(true);

            if (Input.GetButton("Interact"))
            {
                interact();
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
        gameManager.instance.getPlayerScript().getGunStats(gun);
        Destroy(gameObject);
    }
}
