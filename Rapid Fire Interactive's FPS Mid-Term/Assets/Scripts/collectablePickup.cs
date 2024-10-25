using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectablePickup : MonoBehaviour {

    [SerializeField] Sprite buffIcon;
    [SerializeField] ObjectType type;
    enum ObjectType { secret, healBuff, attackBuff, shieldBuff, staminaBuff, coinDrop, coinPickup }

    [SerializeField] AudioClip pickUpA;

    void Update() {
        if (type != ObjectType.coinPickup) {
            transform.Rotate(0, 0.25f, 0 * Time.deltaTime); // making pickup object spin
        }
    }

    private void OnTriggerEnter(Collider otherObject) {
        if (otherObject.CompareTag("Player")) {
            if (type == ObjectType.coinDrop) {
                gameManager.instance.getPlayerScript().setCoins(2); // Add coins to player amount
                playerStats.Stats.gotMoney(2); // each coin pickup is 2 coins
                playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.coinPickupA, audioManager.instance.coinPickupVol);
                Destroy(gameObject);
                return;
            }
            else if (type == ObjectType.coinPickup) {
                gameManager.instance.getPlayerScript().setCoins(1);
                playerStats.Stats.gotMoney(1);
                playerMovement.player.getAudioLocation().PlayOneShot(audioManager.instance.coinPickupA, audioManager.instance.coinPickupVol);
                Destroy(gameObject);
                return;
            }
            AudioSource.PlayClipAtPoint(pickUpA, transform.position);
            if (type == ObjectType.secret) {
                playerStats.Stats.collectableFound();
            }
            else if (type == ObjectType.healBuff) {
                gameManager.instance.getPlayerScript().callBuff(2, buffIcon); // 5 seconds of 10 hp per second
            }
            else if (type == ObjectType.attackBuff) {
                gameManager.instance.getPlayerScript().callBuff(1, buffIcon); // 10 seconds of 1.5x damage
            }
            else if (type == ObjectType.shieldBuff) {
                gameManager.instance.getPlayerScript().callBuff(3, buffIcon); // 10 seconds of 25 extra health (shield)
            }
            else if (type == ObjectType.staminaBuff) {
                gameManager.instance.getPlayerScript().callBuff(4, buffIcon); // 10 seconds of 1.5x damage
            }
        }
        else {
            return;
        }
        Destroy(gameObject);
    }
}
