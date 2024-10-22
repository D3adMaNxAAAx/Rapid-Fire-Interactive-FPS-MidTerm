using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectablePickup : MonoBehaviour {

    [SerializeField] Sprite buffIcon;
    [SerializeField] ObjectType type;
    enum ObjectType { secret, healBuff, attackBuff, shieldBuff, staminaBuff, coins }

    [SerializeField] AudioClip pickUpA;
    
    // Unused Variables
    //float pickUpV = 1;

    void Update() {
        transform.Rotate(0, 0.25f, 0 * Time.deltaTime); // making pickup object spin
    }

    private void OnTriggerEnter(Collider otherObject) {
        if (otherObject.CompareTag("Player")) {
            AudioSource.PlayClipAtPoint(pickUpA, transform.position);
            if (type == ObjectType.coins) {
                gameManager.instance.getPlayerScript().setCoins(2); // Add coins to player amount
                playerStats.Stats.gotMoney(2); // each coin pickup is 2 coins
            }
            else if (type == ObjectType.secret) {
                /// track this
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
