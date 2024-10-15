using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeDoorTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        // Check which door to close
        if (doorControllerManual.instance.getDoorStatus())
            StartCoroutine(doorControllerManual.instance.closeDoor());
        
        if (safeRoom.instance.getSafeState())
            StartCoroutine(safeRoom.instance.closeDoor());

        if (bossRoom.instance.getDoorStatus())
        {
            // for some reason the boss door just isn't working correctly so this won't work for now..
            //bossRoom.instance.setDoorStatus(false);
        }
    }
}
