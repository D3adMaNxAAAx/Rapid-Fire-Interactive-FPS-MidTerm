using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    [SerializeField] objectType type;
    enum objectType { powerUp }

    [SerializeField] AudioClip pickUPowerUpA;
    float pickUPowerUpV = 1;

    void Update() {
        transform.Rotate(0, 0.25f, 0 * Time.deltaTime); // making pickup object spin
    }

    private void OnTriggerEnter(Collider otherObject) {
        if (type == objectType.powerUp) {
            if (otherObject.CompareTag("Player")) {
                // play audio
                Destroy(gameObject);
            }
        }
    }
}
