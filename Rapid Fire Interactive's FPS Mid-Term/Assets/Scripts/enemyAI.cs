using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour , IDamage
{
    //Allows selection of enemy type
    [SerializeField] enum enemyType { basic, challenge, boss}

    //Tracks which type of enemy in play
    [SerializeField] enemyType type;

    //Allows Designer Communication to Model's Renderer
    [SerializeField] Renderer model;

    //Allows Designer Communication To NavMeshAgent Component 
    [SerializeField] NavMeshAgent agent;

    //Enemy Head Position Tracker(Line Of Sight) For Designer
    [SerializeField] Transform headPos;

    //Enemy SHoot Point Origin Tracker For Designer
    [SerializeField] Transform shootPos;

    //Health Points Tracker and Modifier Field For Designer
    [SerializeField] int HP;

    //Bullet Object Tracker and Communication Field For Designer
    [SerializeField] GameObject rangedAttack;

    //Enemy Fire Rate Modifier Field For Designer
    [SerializeField] float shootRate;

    //Allows Designer To Set Damage Flash Timer 
    [SerializeField] float damageFlashTimer;

    //Sets enemy rotation look speed for turning towards enemy look direction
    [SerializeField] int faceTargetSpeed;

    //How much XP enemy drops 
    [SerializeField] int dropXP;

    //Gives other classes access to enemy xp value
    public int getEnemyXP()
    { return dropXP; }

    //Setter for enemy xp from other classes
    public void setEnemyXP(int _xp)
    { dropXP = _xp; }

    //Enemy Model Original Color Private Tracker
    Color colorOrig;

    //Private Tracker For If Enemy Is Shooting 
    bool isShooting;

    //Private Tracker for enemy original HP
    int HPOrig;

    //Tracker of if player is in range of enemy detection radius
    bool playerInRange;

    //Tracks player Direction for AI rotation and player in range 
    Vector3 playerDir;

    //tracks boss hp for boss fight progress bar 
    int bossHP;

    // Start is called before the first frame update
    void Start()
    {
        //Sets our Models original color on scene start
        colorOrig = model.material.color ;

        //Set orginal hp value on scene open for enemy
        HPOrig = HP;

        //if enemy is boss original = enemy hp used for boss health progress bar
        if (type == enemyType.boss)
            bossHP = HP;


        //Tell gameManager To update game goal that an enemy has been added to gameGoal
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {

        if (playerInRange)
        {

            //Setting direction of where player is in relation to enemy location when within detection range
            playerDir = gameManager.instance.getPlayer().transform.position - headPos.position;

            //Telling our ai to go to the location of the Player's position until game ends or enemy destroyed
            agent.SetDestination(gameManager.instance.getPlayer().transform.position);

            //Tell ai to always face player while not in same coordinates(Will never reach destination so will allow constant face direction update)
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
            //Checking if Enemy is shooting 
            if (!isShooting)
            {
                //Tell enemy to start shooting
                StartCoroutine(shoot());
            }
        }
    }

    IEnumerator shoot()
    {
        //Set shooting to true
        isShooting = true;

        //Create our bullet and fire from the shootPos of Enemy 
        Instantiate(rangedAttack, shootPos.position, transform.rotation);

        //Timer setting shootRate time
        yield return new WaitForSeconds(shootRate);

        //Set shooting back to false
        isShooting = false;

    }

    //Trigger Enter for inRange method to tell Ai seek player because he is now in range
    private void OnTriggerEnter(Collider other)
    {
        //Tells ai to ignore trigger if it is not a trigger on player
        if (other.isTrigger)
            return;

        //if our trigger object is a player then player is in range of enemy
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    //Trigger Exit for inRange method to tell Ai seek player because he is now out range 
    private void OnTriggerExit(Collider other)
    {
        //if trigger leaving range radius is tagged as player we know he left and can set player in range to false
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    //Tell AI to face player 
    //Quaterions used becasue we must rotate enemy velocity direction to always face current target
    void faceTarget()
    {
        //Create a rotation object to store direction to face (Direction of player)
        Quaternion rot = Quaternion.LookRotation(playerDir);

        //Telling AI to transform(Move) in rotation direaction of set destions position in time and rotate at the desired speed set to be frame rate INDEPENDENT
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    //Calling our takeDamage method from interface class IDamage
    public void takeDamage(int _amount)
    {
        //Deduct HP on damage recieved
        HP -= _amount;

        //Flash Enemy Red To Indicate Damage Taken
        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            //Tells Game manager to take 1 enemy out of game goal enemy total
            gameManager.instance.updateGameGoal(-1);

            //On enemy death add enemy dropped xp to player xp
            
            //TODO Uncomment below

            //Uncomment below line when xp modifier in place for manager and delete this comment Please and Thank You 
            //gameManager.instance.getPlayerScript().xp += getEnemyXP();

            //Since No HP Delete Enemy Object
            Destroy(gameObject);
        }
    }



    //Will flash Enmey mesh on damage taken
    IEnumerator flashColor()
    {
        model.material.color = Color.red;

        //Allows Designer to set flash timer 
        yield return new WaitForSeconds(damageFlashTimer);

        //Sets color back to original after timer elapse
        model.material.color = colorOrig;

    }
}