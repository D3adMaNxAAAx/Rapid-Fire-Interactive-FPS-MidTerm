using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadout : MonoBehaviour
{

    //Loadout items selected by player
    [SerializeField] ScriptableObject _primaryWeapon;
    [SerializeField] ScriptableObject _secondaryWeapon;
    [SerializeField] ScriptableObject _consumable; 
    [SerializeField] ScriptableObject _consumable2;
    [SerializeField] ScriptableObject _throwable;

    //Players selected loadout
    List<ScriptableObject> selectedLoadout;
    List<ScriptableObject> presetLoadout1;
    List<ScriptableObject> presetLoadout2;

    //Preset objects loadout 1
    ScriptableObject _primaryLd1;
    ScriptableObject _secondaryLd1;
    ScriptableObject _cons1Ld1;
    ScriptableObject _cons2Ld1;
    ScriptableObject _thrwLd1;


    //Preset objects loadout 2
    ScriptableObject _primaryLd2;
    ScriptableObject _secondaryLd2;
    ScriptableObject _cons1Ld2;
    ScriptableObject _cons2Ld2;
    ScriptableObject _thrwLd2;

    //Lists for players to select items out of 
    [SerializeField] List<ScriptableObject> primaryWeapons; 
    [SerializeField] List<ScriptableObject> secondaryWeapons;
    [SerializeField] List<ScriptableObject> consumables;
    [SerializeField] List<ScriptableObject> throwables;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.statePause();
        

        presetLoadout1 = new List<ScriptableObject>();
        
        presetLoadout1.Add(_primaryLd1);
        presetLoadout1.Add(_secondaryLd1);
        presetLoadout1.Add(_cons1Ld1);
        presetLoadout1.Add(_cons2Ld1);
        presetLoadout1.Add(_thrwLd1);

        presetLoadout2 = new List<ScriptableObject>();

        presetLoadout2.Add(_primaryLd2);
        presetLoadout2.Add(_secondaryLd2);
        presetLoadout2.Add(_cons1Ld2);
        presetLoadout2.Add(_cons2Ld2);
        presetLoadout2.Add(_thrwLd2);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //click on the primary weapon slot
    //list of weapons show for primarys 
    //same for all other choices 
    //need a choose primary, seconday, consumable, and throwable button
    //already have back button 
    //loadout 1 button adds all loadout 1 preset scriptable weapons, cons, throws
    //same for loadout 2 button 

    //Confirm Loadout button sets loadout and starts game 
    //Quit game button as well 
    
    
}
