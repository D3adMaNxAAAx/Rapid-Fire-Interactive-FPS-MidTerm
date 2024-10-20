using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour , IDamage
{
    [SerializeField] enum enemyType { basic, challenge, boss } // Allows selection of enemy type
    [Header("----- Components -----")]
    [SerializeField] enemyType type; // Tracks which type of enemy in play
    [SerializeField] Renderer model; // Allows Designer Communication to Model's Renderer
    [SerializeField] Animator anim; //Sets animation animator controller
    [Range(1,20)][SerializeField] int animSpeedTrans;
    [SerializeField] public NavMeshAgent agent; // Allows Designer Communication To NavMeshAgent Component 
    [SerializeField] Transform headPos; // Enemy Head Position Tracker(Line Of Sight) For Designer
    [SerializeField] Transform shootPos; // Enemy SHoot Point Origin Tracker For Designer
    [SerializeField] Transform ammoSpawn;
    [SerializeField] GameObject ammoDrop; //Ammo Drop prefab
    [SerializeField] AudioClip hitClip;
    [SerializeField] float hitVolume = 1.0f;
    [SerializeField] float soundCooldown = 2; //cooldown on hit sound 
    [SerializeField] float knockbackForce = 5f;  // Amount of force applied during knockback
    [SerializeField] float knockbackDuration = 0.2f;  // Duration of the knockback effect

    [SerializeField] BoxCollider miniBossheadCollider = null;
    [SerializeField] CapsuleCollider enemyHeadCollider = null;

    static BoxCollider miniBossheadColl;
    static CapsuleCollider enemyHeadColl;

    // -- Extra Checks --
    bool canPlaySound = true; // Tracks if the sound can be played
    bool isShooting; // Private Tracker For If Enemy Is Shooting 
    bool playerInRange; // Tracker of if player is in range of enemy detection radius
    Vector3 playerDir; // Tracks player Direction for AI rotation and player in range
    Vector3 spawnPos;
    Vector3 lastSeenPlayerPosition;
    [Header("----- Roam -----")]
    // All value fields for enemy view and roam settings 
    [Range(1, 359)][SerializeField] int viewAngle;
    [Range(1, 100)][SerializeField] int roamDist;
    [Range(0, 180)][SerializeField] int roamSpeed;
    [Range(1, 180)][SerializeField] int roamTimer;

    Vector3 startingPos;
    bool isRoaming; // Tracks if enemy is roaming 
    float angleToPlayer;
    float stoppingDistOrig;
    Coroutine aCoRoutine;
    [Header("----- Stats -----")]
    // -- Attributes --
    [Range(1, 300)][SerializeField] float HP; // Health Points Tracker and Modifier Field For Designer

    [Range(1, 30)][SerializeField] int faceTargetSpeed; // Sets enemy rotation look speed for turning towards enemy look direction
    [Range(1, 1000)][SerializeField] int dropXP; // How much XP enemy drops
    [Range(1, 1000)][SerializeField] int dropCoins; // How many coins enemy drops
    [Range(0,100)][SerializeField] int rngDropRate;

    [Header("----- Attack -----")]
    [SerializeField] GameObject rangedAttack; // Bullet Object Tracker and Communication Field For Designer
    [SerializeField] Collider meleeCol;
    
    [Range(.01f, 100)][SerializeField] float shootRate; // Enemy Fire Rate Modifier Field For Designer
    [Range(.01f, 30)][SerializeField] float damageFlashTimer; // Allows Designer To Set Damage Flash Timer 
    float HPOrig; // Private Tracker for enemy original HP
    Color colorOrig; // Enemy Model Original Color Private Tracker
    float bossHP; // tracks boss hp for boss fight progress bar
    float OGSpeed;
    int dropRNG;
    private List<StatusEffects> activeEffects = new List<StatusEffects>();

    bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        // Assign variables that need to be set at enemy creation
        colorOrig = model.material.color; // Sets our Models original color on scene start
        HPOrig = HP; // Set orginal hp value on scene open for enemy
        OGSpeed = agent.speed;

        // if enemy is boss original = enemy hp used for boss health progress bar
        if (type == enemyType.boss)
            bossHP = HP;

        // Tell gameManager To update game goal that an enemy has been added to gameGoal
        if (gameManager.instance != null)
        gameManager.instance.updateGameGoal(1);

        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;

        miniBossheadColl = miniBossheadCollider;
        enemyHeadColl = enemyHeadCollider;

        if (rangedAttack != null) {
            objectPool = FindObjectOfType<projectilePool>();
            Dtime = rangedAttack.GetComponent<damage>().getDTime();
            projectileType = rangedAttack.GetComponent<damage>().getProjectileType();
        }
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

        if (playerInRange && !canSeePlayer())
        {

            if (!isRoaming && agent.remainingDistance < .05f && aCoRoutine == null)
                aCoRoutine = StartCoroutine(roam());

        }
        else if (!playerInRange) {
            if (!playerInRange && agent.remainingDistance < .05f && aCoRoutine == null)
               aCoRoutine = StartCoroutine(roam());
                
        }

    }

    bool canSeePlayer()
    {
        // Setting direction of where player is in relation to enemy location when within detection rang
        playerDir = gameManager.instance.getPlayer().transform.position - headPos.position;

        //Creating an angle from our enemy forward direction to player direction in world 
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        //showing a line of sight reference in scene creator
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
                    if (angleToPlayer <= (viewAngle / 3)) { // enemies always shoots and hits you even if not fully rotated to you yet so nerfing view angle for when shooting at player
                        StartCoroutine(shoot());
                    }
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


    private projectilePool objectPool; // object pool class instance initialized in start I think, array of object pools
    public ObjectType projectileType; // for object pooling / recycling, ObjectType is enum in damage script, initialized in start by getting damage script and getting enum type
    int Dtime; // initialized in start by getting damage script and getting destroy time
    bool firstShot = true;

    IEnumerator shoot() {
        // Set shooting to true
        isShooting = true;
        if (gameObject.CompareTag("Heavy") || gameObject.CompareTag("Basic Melee")) {
            if (agent.remainingDistance <= agent.stoppingDistance + 1) {
                anim.SetTrigger("Melee");
            }
        }

        else if (gameObject.CompareTag("Basic") || gameObject.CompareTag("Light") || gameObject.CompareTag("Ranged Heavy") || gameObject.CompareTag("Challenge") || (gameObject.CompareTag("Demon Golem"))) {
            anim.SetTrigger("Shoot");
            // Demon Golem also has melee he does both at once
        }

        else if (gameObject.CompareTag("Boss")) {
            if (agent.remainingDistance <= agent.stoppingDistance)
                anim.SetTrigger("Melee");
            else
                anim.SetTrigger("Shoot");
        }
        if (rangedAttack != null) {
            if (firstShot) { // first shot for enemies was being weird so first shot is normal and still put in objectPool
                Instantiate(rangedAttack, shootPos.position, rotation); // OG method
                firstShot = false;
            }
            else {
                GameObject newProjectile = objectPool.getProjectileFromPool(projectileType); // setting bullet object to newProjectile
                                                                                             // if there is a bullet in the correct pool, it sets that to newProjectile. Else it makes a new ones and sets it to newProjectile
                newProjectile.transform.position = shootPos.position;
                newProjectile.transform.rotation = rotation;
                newProjectile.GetComponent<Rigidbody>().velocity = (playerDir.normalized) * (rangedAttack.GetComponent<damage>().getAttackSpeed()); // accessing damage script and getting bullet speed
                newProjectile.SetActive(true); // turning object on (it is set off when added to object pool)
                StartCoroutine(addObjectToPool(newProjectile)); // projectile is added to pool if it collides (see damage script), or if bullet "destory" time is hit
            }
        }
        yield return new WaitForSeconds(shootRate); // Timer setting shootRate time
        isShooting = false;
    }

    IEnumerator addObjectToPool(GameObject projectile) { // adding projectile to the pool that matches its type
        yield return new WaitForSeconds(Dtime); // I have no idea why but its adding extra time
        objectPool.addToPool(projectileType, projectile);
    }

    public void meleeOn()
    {
        meleeCol.enabled = true;
    } 
    public void meleeOff()
    {
        meleeCol.enabled = false;
    }

    Quaternion rotation; // for the enemiesShot, does not affect their look direction

    // Tell AI to face player, Quaterions used becasue we must rotate enemy velocity direction to always face current target
    void faceTarget() {

        rotation = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z)); // using Y axis direction

        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z)); // Create a rotation object to store direction to face (Direction of player)
        // Telling AI to transform(Move) in rotation direaction of set destions position in time and rotate at the desired speed set to be frame rate INDEPENDENT
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    // Calling our takeDamage method from interface class IDamage
    public void takeDamage(float _amount)
    {    
        // Deduct HP on damage recieved


        HP -= _amount;
        playerStats.Stats.attack(_amount);
        isRoaming = false;

        if (canPlaySound)
        {
            PlayHitSound();
            StartCoroutine(SoundCooldown());
        }
       

        if (type == enemyType.boss) {
            bossHP -= _amount;
        }
        
        // Flash Enemy Red To Indicate Damage Taken
        StartCoroutine(flashColor());

        // Decrement their health bar & update the UI
        //gameManager.instance.getBossHPBar().fillAmount = (float)bossHP / HPOrig;
        //updateBossHealth();
        gameManager.instance.updateBossBar(gameManager.instance.getBossHPBar(), bossHP, HPOrig);

        if (HP > 0)
            agent.SetDestination(gameManager.instance.getPlayer().transform.position); // makes enemys go to player

       

        // Check if enemy is dead
        if (HP <= 0 && !isDead)
        {
            isDead = true;
            if (agent.gameObject.CompareTag("Boss"))
            {
                anim.SetTrigger("Dead");
            }
            // Tells Game manager to take 1 enemy out of game goal enemy total
            gameManager.instance.updateGameGoal(-1);
            playerStats.Stats.enemyKilled();
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
            if (!gameObject.CompareTag("Boss"))
            {
                // Give the player XP & coins for defeating the enemy
                gameManager.instance.getPlayerScript().setXP(getEnemyXP()); // setXP will ADD the amount given.
                playerStats.Stats.gotXP(getEnemyXP());
                gameManager.instance.getPlayerScript().setCoins(gameManager.instance.getPlayerScript().getCoins() + getEnemyCoins()); // Add coins to player amount.
                playerStats.Stats.gotMoney(getEnemyCoins());
                Destroy(gameObject);
            }
            gameManager.instance.getPlayerScript().updatePlayerUI();
        }
    }

    public void takeDamage(float amount, Vector3 sourcePosition, StatusEffects effect = null)
    {
        takeDamage(amount); // Call the base takeDamage method

        if (effect != null)
        {
            ApplyStatusEffect(effect);
        }
        ApplyKnockback(sourcePosition);
    }
    public void takeDamage(float amount, Vector3 sourcePosition)
    {
        // Apply basic damage logic
        takeDamage(amount);

        // Apply knockback using the enemy's position
        ApplyKnockback(sourcePosition);
    }

    private void ApplyKnockback(Vector3 sourcePosition)
    {
        
        Vector3 knockbackDirection = (transform.position - sourcePosition).normalized;

       
        float knockbackForce = 5f;
        agent.velocity += knockbackDirection * knockbackForce;
    }

    private void ApplyStatusEffect(StatusEffects effect)
    {
        StatusEffects newEffect = gameObject.AddComponent(effect.GetType()) as StatusEffects;
        newEffect.duration = effect.duration;
        newEffect.ApplyEffect(gameObject);

          activeEffects.Add(newEffect);
    }
    private void PlayHitSound()
    {
        if (hitClip != null)
        {
            // Play the clip at the enemy's position
            AudioSource.PlayClipAtPoint(hitClip, transform.position, hitVolume);
        }
    }
    private IEnumerator SoundCooldown()
    {
        canPlaySound = false; // Disable sound playing
        yield return new WaitForSeconds(soundCooldown); // Wait for the cooldown period
        canPlaySound = true; // Re-enable sound playing
    }

    // Will flash Enemy mesh on damage taken
    IEnumerator flashColor() {
     model.material.color = Color.red;
     yield return new WaitForSeconds(damageFlashTimer);
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
    public float getEnemyHP()
    { return HP; }

    // Setter for Enemy HP from other classes
    public void setEnemyHP(int _hp)
    { HP = _hp; }


   static public BoxCollider getMiniBossHeadCollider()
    { return miniBossheadColl; }
    static public CapsuleCollider getEnemyHeadCollider()
    { return enemyHeadColl; }
  
}
