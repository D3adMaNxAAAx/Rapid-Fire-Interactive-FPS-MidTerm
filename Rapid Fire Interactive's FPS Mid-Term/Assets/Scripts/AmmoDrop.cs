using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : MonoBehaviour
{
    [SerializeField] GameObject ammoDrop;
    [SerializeField] float rotateSpeed;
    [SerializeField] bool rotateClockwise;

    Vector3 rotDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateClockwise)
            transform.Rotate(0, rotateSpeed, 0 * Time.deltaTime);
        else
            transform.Rotate(0, -rotateSpeed, 0 * Time.deltaTime);
    }
}
