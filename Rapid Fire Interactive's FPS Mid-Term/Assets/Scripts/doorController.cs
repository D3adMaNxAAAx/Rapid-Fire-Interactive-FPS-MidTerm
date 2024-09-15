using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    //Door Field.
    [SerializeField] GameObject aDoor;
    //Door Pos and Opening Speed Modifiers.
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;
    [SerializeField] float doorSpeed;

    //Door vectors for open and closed.
    Vector3 doorPosClose;
    Vector3 doorPosOpen;

    //Float value to use for time and speed.
    float a;

    //Used so the door can open and close in Update().
    bool isOpen;

    // Start is called before the first frame update.
    void Start()
    {
        doorPosClose = aDoor.transform.position; //Sets door close pos to original object position.
        doorPosOpen = new Vector3(doorPosClose.x + x, doorPosClose.y + y, doorPosClose.z + z); //Adds X, Y, and Z values to original values to open and close doors.
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            StartCoroutine(openDoor());
        }
        else
        {
            StartCoroutine(closeDoor());
        }
    }

    //Set if door is open or closed.
    private void OnTriggerEnter(Collider other)
    {
        isOpen = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isOpen = false;
    }

    //Open door timer
    IEnumerator openDoor()
    {
        if (aDoor.transform.position != doorPosOpen)
        {
            a = Time.deltaTime * doorSpeed;
            //Use of Lerp to get smooth door movement
            aDoor.transform.position = Vector3.Lerp(aDoor.transform.position, doorPosOpen, a);
        }
        yield return null;
    }

    //Close door timer
    IEnumerator closeDoor()
    {
        if (aDoor.transform.position != doorPosClose)
        {
            a = Time.deltaTime * doorSpeed;
            aDoor.transform.position = Vector3.Lerp(aDoor.transform.position, doorPosClose, a);
        }
        yield return null;
    }
}
