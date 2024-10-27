using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeDoorTrigger : MonoBehaviour
{
    [SerializeField] safeRoom safe;
    [SerializeField] doorControllerManual exit;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check which door to close
            if (exit != null)
            {
                if (exit.getDoorStatus())
                    StartCoroutine(exit.closeDoor());
            }

            if (safe != null)
            {
                if (safe.getSafeState())
                    StartCoroutine(safe.closeDoor());
            }
        }
    }
}
