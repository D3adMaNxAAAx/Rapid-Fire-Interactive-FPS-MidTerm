using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    [SerializeField] GameObject aDoor;
    [SerializeField] float x;
    [SerializeField] float y;
    [SerializeField] float z;
    [SerializeField] float doorSpeed;

    Vector3 doorPosClose;
    Vector3 doorPosOpen;
    // Start is called before the first frame update
    void Start()
    {
        doorPosClose = aDoor.transform.position;
        doorPosOpen = new Vector3(doorPosClose.x + x, doorPosClose.y + y, doorPosClose.z + z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        aDoor.transform.position = Vector3.MoveTowards(transform.position, doorPosOpen, doorSpeed * Time.deltaTime);
    }
    private void OnTriggerExit(Collider other)
    {
        aDoor.transform.position = doorPosClose;
    }
}
