using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour { // handles pickups for grenades and heals

    [SerializeField] GrenadeStats grenade; 
    [SerializeField] HealStats healPotion; 

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (healPotion != null) {
                if (gameManager.instance.getPlayerScript().addToHeals(healPotion)) { // returns true if successfully added
                    playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.itemPickupA, audioManager.instance.itemPickupVol);
                    Destroy(gameObject);
                }
                else {
                    gameManager.instance.getPickupFailUI().SetActive(true);
                }
            }
            else if (grenade != null) {
                if (gameManager.instance.getPlayerScript().addToGrenades(grenade)) { // returns true if successfully added
                    playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.itemPickupA, audioManager.instance.itemPickupVol);
                    Destroy(gameObject);
                }
                else {
                    gameManager.instance.getPickupFailUI().SetActive(true);
                }
            }
            else { // backpack
                playerMovement.player.addMarkers(5);
                playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.itemPickupA, audioManager.instance.itemPickupVol);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            gameManager.instance.getPickupFailUI().SetActive(false);
        }
    }
}
