using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noCrouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerMovement.player != null)
            {
                playerMovement.player.setNoCrouch(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerMovement.player != null)
            {
                playerMovement.player.setNoCrouch(false);
            }
        }
    }
}
