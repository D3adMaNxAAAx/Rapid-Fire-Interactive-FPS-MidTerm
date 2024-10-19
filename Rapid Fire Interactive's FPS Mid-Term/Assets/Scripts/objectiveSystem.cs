using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectiveSystem : MonoBehaviour
{
    public static objectiveSystem instance;

    [SerializeField] List<objectives> objectivesList;


    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
