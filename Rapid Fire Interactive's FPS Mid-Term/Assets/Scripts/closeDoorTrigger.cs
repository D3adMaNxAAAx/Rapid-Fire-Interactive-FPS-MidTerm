using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeDoorTrigger : MonoBehaviour
{
    [SerializeField] safeRoom safe;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check which door to close
            if (doorControllerManual.instance != null)
            {
                if (doorControllerManual.instance.getDoorStatus())
                    StartCoroutine(doorControllerManual.instance.closeDoor());
            }

            if (safe != null)
            {
                if (safe.getSafeState())
                    StartCoroutine(safe.closeDoor());
            }
        }
    }
}
