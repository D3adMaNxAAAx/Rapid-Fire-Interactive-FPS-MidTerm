using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadout : MonoBehaviour
{
    [SerializeField] ScriptableObject primaryWeapon;
    [SerializeField] ScriptableObject secondaryWeapon;
    [SerializeField] ScriptableObject consumable; 
    [SerializeField] ScriptableObject consumable2;
    [SerializeField] ScriptableObject throwable;

    [SerializeField] List<ScriptableObject> selectedLoadout;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
