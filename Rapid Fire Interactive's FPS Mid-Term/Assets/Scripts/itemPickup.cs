using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour { // handles pickups for grenades and heals

    [SerializeField] GrenadeStats grenade; 
    [SerializeField] HealStats healPotion; 

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            gameManager.instance.getInteractUI().SetActive(true);
            if (Input.GetButton("Interact")) {
                if (healPotion != null) {
                    gameManager.instance.getInteractUI().SetActive(false);
                    if (gameManager.instance.getPlayerScript().addToHeals(healPotion)) { // returns true if successfully added
                        playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.itemPickupA, audioManager.instance.itemPickupVol);
                        Destroy(gameObject);
                    }
                    else {
                        gameManager.instance.getPickupFailUI().SetActive(true);
                    }
                }
                else if (grenade != null) {
                    gameManager.instance.getInteractUI().SetActive(false);
                    if (gameManager.instance.getPlayerScript().addToGrenades(grenade)) { // returns true if successfully added
                        playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.itemPickupA, audioManager.instance.itemPickupVol);
                        Destroy(gameObject);
                    }
                    else {
                        gameManager.instance.getPickupFailUI().SetActive(true);
                    }
                }
                else { // backpack
                    gameManager.instance.getInteractUI().SetActive(false);
                    playerMovement.player.addMarkers(5);
                    playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.itemPickupA, audioManager.instance.itemPickupVol);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            gameManager.instance.getPickupFailUI().SetActive(false);
            gameManager.instance.getInteractUI().SetActive(false);
        }
    }
}
