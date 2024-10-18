using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    [SerializeField] Sprite buffIcon;
    [SerializeField] objectType type;
    enum objectType { secret, healBuff, attackBuff, shieldBuff, staminaBuff }

    [SerializeField] AudioClip pickUpA;
    float pickUpV = 1;

    void Update() {
        transform.Rotate(0, 0.25f, 0 * Time.deltaTime); // making pickup object spin
    }

    private void OnTriggerEnter(Collider otherObject) {
        if (otherObject.CompareTag("Player")) {
            AudioSource.PlayClipAtPoint(pickUpA, transform.position);
            if (type == objectType.secret) {
                /// track this
            }
            else if (type == objectType.healBuff) {
                gameManager.instance.getPlayerScript().callBuff(2, buffIcon); // 5 seconds of 10 hp per second
            }
            else if (type == objectType.attackBuff) {
                gameManager.instance.getPlayerScript().callBuff(1, buffIcon); // 10 seconds of 1.5x damage
            }
            else if (type == objectType.shieldBuff) {
                gameManager.instance.getPlayerScript().callBuff(3, buffIcon); // 10 seconds of 25 extra health (shield)
            }
            else if (type == objectType.staminaBuff) {
                gameManager.instance.getPlayerScript().callBuff(4, buffIcon); // 10 seconds of 1.5x damage
            }
        }
        else {
            return;
        }
        Destroy(gameObject);
    }
}
