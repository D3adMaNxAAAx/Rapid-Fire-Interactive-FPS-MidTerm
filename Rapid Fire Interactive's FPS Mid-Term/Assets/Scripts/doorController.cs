using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    [SerializeField] GameObject aDoor;
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;
    [SerializeField] float timeToOpen;

    Vector3 doorPosClose;
    Vector3 doorPosOpen;

    float a;

    bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        doorPosClose = aDoor.transform.position;
        doorPosOpen = new Vector3(doorPosClose.x + x, doorPosClose.y + y, doorPosClose.z + z);
        //aDoor.transform.position = Vector3.(transform.position, doorPosOpen, doorSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        //if (isOpen)
        //{
        //    StartCoroutine(openDoor());
        //}
        //else
        //{
        //    StartCoroutine(closeDoor());
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        isOpen = true;
        StartCoroutine(openDoor());
        //aDoor.transform.position = Vector3.MoveTowards(transform.position, doorPosOpen, doorSpeed * Time.deltaTime);
    }
    private void OnTriggerExit(Collider other)
    {
        isOpen = false;
        //aDoor.transform.position = doorPosClose;
    }

    IEnumerator openDoor()
    {
        if (aDoor.transform.position != doorPosOpen)
        {
                a = Time.deltaTime * timeToOpen;
                aDoor.transform.position = Vector3.Lerp(doorPosClose, doorPosOpen, a);
        }
        a = 0f;
        yield return null;
    }

    IEnumerator closeDoor()
    {
        yield return null;
    }
}
