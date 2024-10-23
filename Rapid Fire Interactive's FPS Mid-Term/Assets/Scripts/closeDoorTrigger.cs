using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeDoorTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        // Check which door to close
        if (doorControllerManual.instance != null)
        {
            if (doorControllerManual.instance.getDoorStatus())
                StartCoroutine(doorControllerManual.instance.closeDoor());
        }

        if (safeRoom.instance != null)
        {
            if (safeRoom.instance.getSafeState())
                StartCoroutine(safeRoom.instance.closeDoor());
        }
    }
}
