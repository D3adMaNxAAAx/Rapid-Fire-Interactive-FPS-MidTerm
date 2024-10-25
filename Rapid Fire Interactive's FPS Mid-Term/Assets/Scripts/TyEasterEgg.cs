using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyEasterEgg : MonoBehaviour {

    [SerializeField] GameObject activateButton;
    [SerializeField] GameObject jumpTrigger;

    static bool inTriggerArea = false;

    public static bool getTriggerBool() {
        return inTriggerArea;
    }

    public static void activateEasterEgg(AudioSource location) {
        location.PlayOneShot(audioManager.instance.fartEgg, 1);
    }

    private void OnTriggerEnter(Collider other) {
        if (jumpTrigger != null) {
            if (other.CompareTag("Player")) {
                inTriggerArea = true;
            }
        }
        else { // button
            if (other.CompareTag("Player Bullet")) {
                playerMovement.player.Activate();
                StartCoroutine(confirmationPopUpTimer());
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (jumpTrigger != null) {
            if (other.CompareTag("Player")) {
                inTriggerArea = false;
            }
        }
    }

    IEnumerator confirmationPopUpTimer() {
        gameManager.instance.getTyPopUp().SetActive(true);
        yield return new WaitForSeconds(3);
        gameManager.instance.getTyPopUp().SetActive(false);
    }
}
