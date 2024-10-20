using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class objectiveSystem : MonoBehaviour
{
    public static objectiveSystem instance;

   

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
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void objectiveComplete()
    {
        if (obj1 != null)
        {
            if (playerStats.Stats.getPowerObjects() >= 3 && playerStats.Stats.getPowerObjects() < 6)
            { 
                obj1.isCompleted = true;
                
            
            }
        }
    }

}
