using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour , IDamage
{
    // -- Enemy Information --
    [SerializeField] enum enemyType { basic, challenge, boss } // Allows selection of enemy type
    [SerializeField] enemyType type; // Tracks which type of enemy in play
    [SerializeField] Renderer model; // Allows Designer Communication to Model's Renderer
    [SerializeField] Animator anim; //Sets animation animator controller
    [SerializeField] int animSpeedTrans;
    [SerializeField] NavMeshAgent agent; // Allows Designer Communication To NavMeshAgent Component 
    [SerializeField] Transform headPos; // Enemy Head Position Tracker(Line Of Sight) For Designer
    [SerializeField] Transform shootPos; // Enemy SHoot Point Origin Tracker For Designer
    [SerializeField] Transform ammoSpawn;
    [SerializeField] GameObject ammoDrop; //Ammo Drop prefab
    ///[SerializeField] Animator animator;

    // -- Extra Checks --
    bool isShooting; // Private Tracker For If Enemy Is Shooting 
    bool playerInRange; // Tracker of if player is in range of enemy detection radius
    Vector3 playerDir; // Tracks player Direction for AI rotation and player in range
    Vector3 spawnPos;
    Vector3 lastSeenPlayerPosition;

    // All value fields for enemy view and roam settings 
    [SerializeField] int viewAngle;
    [SerializeField] int roamDist;
    [SerializeField] int roamSpeed;
    [SerializeField] int roamTimer;

    Vector3 startingPos;
    bool isRoaming; // Tracks if enemy is roaming 
    float angleToPlayer;
    float stoppingDistOrig;
    Coroutine aCoRoutine;

    // -- Attributes --
    [SerializeField] int HP; // Health Points Tracker and Modifier Field For Designer
    [SerializeField] GameObject rangedAttack; // Bullet Object Tracker and Communication Field For Designer
    [SerializeField] float shootRate; // Enemy Fire Rate Modifier Field For Designer
    [SerializeField] float damageFlashTimer; // Allows Designer To Set Damage Flash Timer 
    [SerializeField] int faceTargetSpeed; // Sets enemy rotation look speed for turning towards enemy look direction
    [SerializeField] int dropXP; // How much XP enemy drops
    [SerializeField] int dropCoins; // How many coins enemy drops
    [Range(0,100)]
    [SerializeField] int rngDropRate;
    int HPOrig; // Private Tracker for enemy original HP
    Color colorOrig; // Enemy Model Original Color Private Tracker
    int bossHP; // tracks boss hp for boss fight progress bar
    float OGSpeed;
    int dropRNG;

    // Start is called before the first frame update
    void Start()
    {
        // Assign variables that need to be set at enemy creation
        colorOrig = model.material.color; // Sets our Models original color on scene start
        HPOrig = HP; // Set orginal hp value on scene open for enemy
        float OGSpeed = agent.speed;

        // if enemy is boss original = enemy hp used for boss health progress bar
        if (type == enemyType.boss)
            bossHP = HP;

        // Tell gameManager To update game goal that an enemy has been added to gameGoal
        gameManager.instance.updateGameGoal(1);

        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        //animation speed we go to 
        float agentSpeed = agent.velocity.normalized.magnitude;

        //current animation speed
        float animSpeed = anim.GetFloat("Speed");

        //smoothly transitioning animation speed over time 
        anim.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));

        dropRNG = Random.Range(0, 100);

        ///float agentSpeed = agent.velocity.normalized.magnitude;
        ///float animationSpeed = animator.GetFloat("Speed"); // getting current value of speed param in blend tree
        ///animator.SetFloat("Speed", Mathf.Lerp(animationSpeed, agentSpeed, 6 * Time.deltaTime)); // changing "Speed" param in blend tree, setting it to enemy's speed but in 0-1 form
        // lerp part helps smooth out transition from being still to the animation, 6 is how fast the transition is

        if (playerInRange && !canSeePlayer())
        {
            // seenPlayer = true; // Enemy has now seen the player -- this will be used for a check later in the Update method


            if (!isRoaming && agent.remainingDistance < .05f && aCoRoutine == null) {
                if (aCoRoutine == null) {
                    aCoRoutine = StartCoroutine(roam());
                }
            }
            else if (!playerInRange)
            {
                if(!playerInRange && agent.remainingDistance < .05f && aCoRoutine == null)
                    if (aCoRoutine == null) {
                        aCoRoutine = StartCoroutine(roam());
                    }
            }

           
            // -- meant to be at the start of the method.
            // Check if the enemy is a boss -- this will be to display the health bar when the player is in range.
            //if (type == enemyType.boss)
            //{
            //    gameManager.instance.displayBossBar(true);
            //}

        }
        
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

    bool canSeePlayer()
    {
        // Setting direction of where player is in relation to enemy location when within detection rang
        playerDir = gameManager.instance.getPlayer().transform.position - headPos.position;

        //Creating an angle from our enemy forward direction to player direction in world 
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        //TURN THIS OFF AFTER WORKING, showing a line of sight reference in scene creator
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit; //Tracks ray for enemy line of sight 

        if (Physics.Raycast(headPos.position, playerDir, out hit)) {
            //if ray hits player and player is within view cone
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle) {
                lastSeenPlayerPosition = gameManager.instance.getPlayer().transform.position;
                agent.SetDestination(lastSeenPlayerPosition); // makes enemys go to player
                if (agent.remainingDistance <= agent.stoppingDistance) { // when enemy is within stopping distance of player
                    faceTarget(); //face player
                }
                if (!isShooting) {
                    StartCoroutine(shoot());
                }
                //reset ai stopping dist
                agent.stoppingDistance = stoppingDistOrig;
                agent.speed = OGSpeed;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        agent.speed = roamSpeed;
        return false;   
    }

    // Trigger Enter for inRange method to tell Ai seek player because he is now in range
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) 
            return;
        // if our trigger object is a player then player is in range of enemy
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    // Trigger Exit for inRange method to tell Ai seek player because he is now out range 
    private void OnTriggerExit(Collider other) {
        // if trigger leaving range radius is tagged as player we know he left and can set player in range to false
        if (other.CompareTag("Player")) {
            playerInRange = false;
            agent.stoppingDistance = 0;
            agent.speed = roamSpeed;
            agent.SetDestination(lastSeenPlayerPosition); // makes the enemy "chase", goes to last location player was scene if the player runs out of his range
        }
    }

    //allows enemy to  roam 
    IEnumerator roam() {

        isRoaming = true;
        yield return new WaitForSeconds(roamTimer); //wait desired time 

        //set agent to reach right on random spot
        agent.stoppingDistance = 0;
        agent.speed = roamSpeed;
        Vector3 randomPos = Random.insideUnitSphere * roamDist;
        randomPos += startingPos; //adds random position to start position so that we can circle around the main start position

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1); // make sure position is valid
        agent.SetDestination(hit.position);

        isRoaming = false;

        aCoRoutine = null;
    }

    IEnumerator shoot() {
        // Set shooting to true
        isShooting = true;

        if(rangedAttack != null)
        // Create our bullet and fire from the shootPos of Enemy 
            Instantiate(rangedAttack, shootPos.position, transform.rotation);
        /// this needs to be changed to make during the shoot animation

        // Timer setting shootRate time
        yield return new WaitForSeconds(shootRate);

        // Set shooting back to false
        isShooting = false;
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
        isRoaming = false;

        if (type == enemyType.boss) {
            bossHP -= _amount;
        }
        // Flash Enemy Red To Indicate Damage Taken
        StartCoroutine(flashColor());
        // Decrement their health bar & update the UI
        gameManager.instance.getBossHPBar().fillAmount = (float)bossHP / HPOrig;

        agent.SetDestination(gameManager.instance.getPlayer().transform.position); // makes enemys go to player
        
        // Check if enemy is dead
        if (HP <= 0)
        {
            // Tells Game manager to take 1 enemy out of game goal enemy total
            gameManager.instance.updateGameGoal(-1);

            // Give the player XP & coins for defeating the enemy
            gameManager.instance.getPlayerScript().setXP(getEnemyXP()); // setXP will ADD the amount given.
            gameManager.instance.getPlayerScript().setCoins(getEnemyCoins()); // setCoins will ADD the amount given.

            // Update UI
            gameManager.instance.getPlayerScript().updatePlayerUI();

            // Roll for an ammo pickup
            if (rngDropRate > 0)
            {
                spawnPos = ammoSpawn.position;
                if (dropRNG <= rngDropRate)
                {
                    Instantiate<GameObject>(ammoDrop, spawnPos, Quaternion.identity);
                }
            }

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

    // Gives other classes access to enemy coins value
    public int getEnemyCoins()
    { return dropCoins; }

    // Setter for enemy coins from other classes
    public void setEnemyCoins(int _coins)
    { dropCoins = _coins; }

    // Getter for Enemy HP
    public int getEnemyHP()
    { return HP; }

    // Setter for Enemy HP from other classes
    public void setEnemyHP(int _hp)
    { HP = _hp; }
}
