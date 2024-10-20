using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectiveSystem : MonoBehaviour
{
    public static objectiveSystem instance;

    [SerializeField] List<objectives> objectivesList;

    [SerializeField] objectives obj1;
    [SerializeField] objectives obj2;
    [SerializeField] objectives obj3;
    [SerializeField] objectives obj4;
    [SerializeField] objectives obj5;
    [SerializeField] objectives obj6;
    [SerializeField] objectives obj7;
    [SerializeField] objectives obj8;
    [SerializeField] objectives obj9;





    void Awake()
    {
        instance = this;
        objectivesList.Add(obj1);
        objectivesList.Add(obj2);
        objectivesList.Add(obj3);
        objectivesList.Add(obj4);
        objectivesList.Add(obj5);
        objectivesList.Add(obj6);
        objectivesList.Add(obj7);
        objectivesList.Add(obj8);
        objectivesList.Add(obj9);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void objectiveComplete()
    {
        if (objectivesList != null)
        {

        }
    }

}
