using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour { // handles pickups for grenades and heals

    [SerializeField] GrenadeStats grenade; 
    [SerializeField] HealStats healPotion; 

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (healPotion != null) {
                gameManager.instance.getPlayerScript().addToHeals(healPotion);
                Destroy(gameObject);
            }
            else if (grenade != null) {
                gameManager.instance.getPlayerScript().addToGrenades(grenade);
                Destroy(gameObject);
            }
        }
    }
}
