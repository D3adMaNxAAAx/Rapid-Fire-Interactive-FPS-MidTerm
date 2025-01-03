using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectSpawner : MonoBehaviour {

    [SerializeField] GameObject thing;

    bool spawned = false; 

    private void OnTriggerEnter(Collider otherObject) {
        if (otherObject.CompareTag("Player") && spawned == false) {
            thing.SetActive(true);
            spawned = true;
        }
    }
}
