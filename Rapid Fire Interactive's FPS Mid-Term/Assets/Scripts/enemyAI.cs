using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour , IDamage
{
    // -- Enemy Information --
    [SerializeField] enum enemyType { basic, challenge, boss } // Allows selection of enemy type
    [SerializeField] enemyType type; // Tracks which type of enemy in play
    [SerializeField] Renderer model; // Allows Designer Communication to Model's Renderer
    [SerializeField] NavMeshAgent agent; // Allows Designer Communication To NavMeshAgent Component 
    [SerializeField] Transform headPos; // Enemy Head Position Tracker(Line Of Sight) For Designer
    [SerializeField] Transform shootPos; // Enemy SHoot Point Origin Tracker For Designer

    // -- Extra Checks --
    bool isShooting; // Private Tracker For If Enemy Is Shooting 
    bool playerInRange; // Tracker of if player is in range of enemy detection radius
    // bool seenPlayer; // Checks if the enemy has seen the player
    Vector3 playerDir; // Tracks player Direction for AI rotation and player in range
    // Vector3 playerLastPos; // Tracks where the player was last
    
    // -- Attributes --
    [SerializeField] int HP; // Health Points Tracker and Modifier Field For Designer
    [SerializeField] GameObject rangedAttack; // Bullet Object Tracker and Communication Field For Designer
    [SerializeField] float shootRate; // Enemy Fire Rate Modifier Field For Designer
    [SerializeField] float damageFlashTimer; // Allows Designer To Set Damage Flash Timer 
    [SerializeField] int faceTargetSpeed; // Sets enemy rotation look speed for turning towards enemy look direction
    [SerializeField] int dropXP; // How much XP enemy drops
    int HPOrig; // Private Tracker for enemy original HP
    Color colorOrig; // Enemy Model Original Color Private Tracker
    int bossHP; // tracks boss hp for boss fight progress bar

    // Start is called before the first frame update
    void Start()
    {
        // Assign variables that need to be set at enemy creation
        colorOrig = model.material.color; // Sets our Models original color on scene start
        HPOrig = HP; // Set orginal hp value on scene open for enemy

        // if enemy is boss original = enemy hp used for boss health progress bar
        if (type == enemyType.boss)
            bossHP = HP;

        // Tell gameManager To update game goal that an enemy has been added to gameGoal
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            // seenPlayer = true; // Enemy has now seen the player -- this will be used for a check later in the Update method

            // Setting direction of where player is in relation to enemy location when within detection range
            playerDir = gameManager.instance.getPlayer().transform.position - headPos.position;
            
            // Telling our ai to go to the location of the Player's position until game ends or enemy destroyed
            agent.SetDestination(gameManager.instance.getPlayer().transform.position);

            // Tell ai to always face player while not in same coordinates(Will never reach destination so will allow constant face direction update)
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
            // Checking if Enemy is shooting 
            if (!isShooting)
            {
                // Tell enemy to start shooting
                StartCoroutine(shoot());
            }

            // -- meant to be at the start of the method.
            // Check if the enemy is a boss -- this will be to display the health bar when the player is in range.
            //if (type == enemyType.boss)
            //{
            //    gameManager.instance.displayBossBar(true);
            //}


        }

        // TO-DO: Implement Enemy Memory for player position (maybe move to a Chase method and call it?)
        // If the player moves out of range, move the enemy to where it last saw the player.
        //if (seenPlayer && !playerInRange)
        //{
        //    // Move the enemy towards their last position
        //    agent.SetDestination(gameManager.instance.getPlayer().transform.position);

        //    // The enemy has no longer seen the player so mark it as false.
        //    seenPlayer = false;
        //}
        
        //Something like this but based off a key count each enemy killed adds key and keys required are equal to 
        //if (gameManager.instance.getEnemyCount() == 1 && gameManager.instance.getBossCount() == 1)
        //    gameManager.instance.displayBossBar(true);
        //else { gameManager.instance.displayBossBar(false); }

        // If player isn't in range or has defeated the boss, hide the bar.
        //if (gameManager.instance.getBossHP().activeSelf && !playerInRange)
        //{
        //    gameManager.instance.displayBossBar(false);
        //}

    }
    IEnumerator shoot()
    {
        // Set shooting to true
        isShooting = true;

        // Create our bullet and fire from the shootPos of Enemy 
        Instantiate(rangedAttack, shootPos.position, transform.rotation);

        // Timer setting shootRate time
        yield return new WaitForSeconds(shootRate);

        // Set shooting back to false
        isShooting = false;
    }

    // Trigger Enter for inRange method to tell Ai seek player because he is now in range
    private void OnTriggerEnter(Collider other)
    {
        // if our trigger object is a player then player is in range of enemy
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    // Trigger Exit for inRange method to tell Ai seek player because he is now out range 
    private void OnTriggerExit(Collider other)
    {
        // if trigger leaving range radius is tagged as player we know he left and can set player in range to false
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    // Tell AI to face player 
    // Quaterions used becasue we must rotate enemy velocity direction to always face current target
    void faceTarget()
    {
        // Create a rotation object to store direction to face (Direction of player)
        Quaternion rot = Quaternion.LookRotation(playerDir);

        // Telling AI to transform(Move) in rotation direaction of set destions position in time and rotate at the desired speed set to be frame rate INDEPENDENT
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    // Calling our takeDamage method from interface class IDamage
    public void takeDamage(int _amount)
    {    
        // Deduct HP on damage recieved
        HP -= _amount;
        if (type == enemyType.boss) {
            bossHP -= _amount;
        }
        
        // Flash Enemy Red To Indicate Damage Taken
        StartCoroutine(flashColor());

        // Decrement their health bar & update the UI
        gameManager.instance.getBossHPBar().fillAmount = (float)bossHP / HPOrig;

        if (HP <= 0)
        {
            // Tells Game manager to take 1 enemy out of game goal enemy total
            gameManager.instance.updateGameGoal(-1);

            // On enemy death add enemy dropped xp to player xp
            gameManager.instance.getPlayerScript().setXP(getEnemyXP()); // setXP will ADD the amount given.

            // Update UI
            gameManager.instance.getPlayerScript().updatePlayerUI();

            // Since No HP Delete Enemy Object
            Destroy(gameObject);
            gameManager.instance.getPlayerScript().updatePlayerUI();
        }
    }

    // Will flash Enemy mesh on damage taken
    IEnumerator flashColor()
    {
        model.material.color = Color.red;

        //Allows Designer to set flash timer 
        yield return new WaitForSeconds(damageFlashTimer);

        //Sets color back to original after timer elapse
        model.material.color = colorOrig;
    }

    // Gives other classes access to enemy xp value
    public int getEnemyXP()
    { return dropXP; }

    // Setter for enemy xp from other classes
    public void setEnemyXP(int _xp)
    { dropXP = _xp; }

    // Getter for Enemy HP
    public int getEnemyHP()
    { return HP; }

    // Setter for Enemy HP from other classes
    public void setEnemyHP(int _hp)
    { HP = _hp; }
}
