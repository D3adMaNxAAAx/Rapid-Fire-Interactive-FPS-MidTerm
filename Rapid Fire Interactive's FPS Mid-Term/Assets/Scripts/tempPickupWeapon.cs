using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempPickupWeapon : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.getPlayerScript().getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
