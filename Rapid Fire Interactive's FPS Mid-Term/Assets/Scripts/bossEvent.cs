using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossEvent : MonoBehaviour, IDamage
{

    [SerializeField] Transform bossHeadPos;

    [SerializeField] Transform bossRangePos;

    [SerializeField] Transform bossMeleePos;



    [SerializeField] GameObject bossRangedAttack;

    [SerializeField] GameObject bossMeleeAttack;

    [SerializeField] int bossRangeDamage;

    [SerializeField] int bossMeleeDamage;

    [SerializeField] int rangeDist;

    [SerializeField] int meleeDist;

    [SerializeField] int rangeSpeed;

    [SerializeField] int meleeSpeed;





    [SerializeField] int bossHP;

    [SerializeField] int bossSpeed;



    [SerializeField] Renderer model;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] float dmgFlashTimer;
   

  

    [SerializeField] int bossDmgMod;

    [SerializeField] int bossHPMod;

    [SerializeField] int bossSpeedMod;

    [SerializeField] int rangeDistMod;

    [SerializeField] int meleeDistMod;

    [SerializeField] int rangeSpeedMod;

    [SerializeField] int meleeSpeedMod;


    

    int keysHeld;

    int keysNeeded;


    bool isShooting;

    Color colorOrig;


    int bossHPOrig;

    int bossRngDmgOrig;

    int bossMeleeDmgOrig;

    int bossSpeedOrig;


    //boss door switch object tracker 
    [SerializeField] GameObject bossSwitch;

    public GameObject getBossSwitch()
        { return bossSwitch; }

    public void setBossSwitch(GameObject _switch)
        { bossSwitch = _switch; }

    public int getKeyCount()
        { return keysHeld; }

    public void setKeyCount(int _count)
        { keysHeld = _count; }


    // Start is called before the first frame update
    void Start()
    {
        bossHPOrig = bossHP;

        bossSpeedOrig = bossSpeed;

        bossRngDmgOrig = bossRangeDamage;

        bossMeleeDmgOrig = bossMeleeDamage; 

        colorOrig = model.material.color; 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bossRangedAttack, bossRangePos.position, transform.rotation);

        yield return new WaitForSeconds(dmgFlashTimer);

        isShooting = false;
    }

    public void takeDamage(int _amount)
    {
        bossHP -= _amount;
    }

    //called if switch clicked on

    void openBossDoor()
    {
        //when switch clicked on 
        


        if (keysHeld == keysNeeded)
        {
            //pause game state 

            //show enter boss fight confirmation screen

            //show cursor

            //confine cursor

            //disable shoot if not already implemented 

            //have button to continue and button to cancel

            //on cancel click we close menu and unpause game 

            //on okay click we open new menu allowing to purchase health, ammo, spend xp to boost player damage and player hp 

            //have button to start fight under upgrade buttons

            //on start fight click unpause game

            //open boss room door



            //player walks through door activates door trigger

            //on trigger enter hide enemy count bar, hide xp tracker


            //on trigger exit close door, set game audio boss music if we can 


        }
        else //dont have enough keys 
        {
            //pause game 

            //show scrren not enough keys with okay button to click 

            //on okay ckick close prompt message 

            //unpause game 
        }

        
    }

    void spawnBoss()
    {
        //set boss spawn in boss room 

        
    }

    void bossFight()
    {
        //if door switch clicked
        openBossDoor();

        //spawn boss to room
    }
}
