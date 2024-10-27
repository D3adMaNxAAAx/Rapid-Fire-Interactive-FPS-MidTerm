using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour, IDamage
{

    public static playerMovement player; // singleton

    // -----MODIFIABLE VARIABLES-----
    // Unity object fields
    [Header("-- Player Components --")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud; // Audio controller for the player
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] GameObject playerShot;

    // Player modifiers
    // -- Attributes --
    [Header("-- Player Attributes --")]
    [SerializeField] float HP;
    [SerializeField] float speed;
    [SerializeField] int stamina;
    [SerializeField] int coins;
    [SerializeField] int playerXPMax;
    [SerializeField] int lives;
    [SerializeField] int skillPoints;
    [SerializeField] float criticalHealthThreshold = 0.1f;
    float damageUpgradeMod = 1;  // keep set = to 1, so damage can be upgraded (can just change damage var because it changes when swapping guns)

    // Player Default Weapon Mods
    [Header("-- Player Weapons --")]
    [SerializeField] List<gunStats> guns;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject shotFlash;
    [SerializeField] LineRenderer laserSight;
    private GunInventoryManager gunInventoryManager;

    bool isSniper = false;
    bool isLaser = false;
    bool isShotgun = false;

    [SerializeField] float damage;
    [SerializeField] float fireRate;
    [SerializeField] int range;
    [Range(1f,3f)][SerializeField] float headShotMult;
    [SerializeField] List<GrenadeStats> grenades;
    [SerializeField] GameObject grenade;
    [SerializeField] GrenadeStats grenadeStats;

    //player heals
    [SerializeField] List<HealStats> heals;
    [SerializeField] float healCoolDown = 5f;


    // -- Movement --
    [Header("-- Player Movement --")]
    [SerializeField] float speedMod;
    [SerializeField] int maxJumps;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] float drainMod;  // How quickly stamina points drain
    [SerializeField] float recoveryMod;  // How quickly stamina points recovers
    [SerializeField] bool toggleSprint;   // Turns sprint on or off

    // --Crouching---
    [SerializeField] float normalHeight = 6.0f;
    [SerializeField] float crouchHeight = 1.0f;
    //[SerializeField] float crouchSpeed = 1.0f;
   // [SerializeField] float normalSpeed = 6.0f;
    [SerializeField] Transform playerCamera;


    //HitBox
    [SerializeField] float hitboxHeightMultiplier = 0.9f;
    [SerializeField] float hitboxWidthMultiplier = 0.9f; 

    // -- Timer --
    [Header("-- Timers --")]
    [SerializeField] float dmgFlashTimer;
    [SerializeField] float ammoWarningTimer;

    [SerializeField] AudioClip healSound;

    // -----PRIVATE VARIABLES-----
    // Player movement variables
    Vector3 moveDir;
    Vector3 playerVel;

    // Count number of jumps
    int jumpCounter;

    // Trackers
    float HPOrig; // HP
    float damageBuffMult = 1;
    int playerXP; // XP
    int staminaOrig; // Stamina
    int playerLevel; // Level
    int gunPos = 0; // Weapon selected
    int speedOrig;  // Original Speed
    int startingLives;
    float reloadCooldown = 1.0f; // Time before the player can shoot after reload

    bool shieldOn = false;
    float shieldHP = 25;

    // Checks
    bool leveledUp = false;
    bool isSprinting;
    bool isShooting;
    bool isStepping;
    bool isDraining;
    bool isRecovering;
    bool isReloading;
    bool lowHealth = false;
    bool readyToHeal = false;
    bool stopHealing = false; // was the player damaged while healing?
    bool onDashCooldown = false;
    bool isHealing;
    bool damageAudioReady = true;
    bool isCrouching = false;
    bool infiniteStam = false;
    bool isCriticalHealth;
    bool safeAccess = false;

    // Ty's Easter Egg vars
    bool activated = false;
    int jumpInAreaCount = 0;

    public void Activate() { activated = true; }

    private void Awake()
    {
        if (player == null)
        {
            DontDestroyOnLoad(gameObject);
            player = this;
        }
        else if (player != null)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        HPOrig = HP;
        staminaOrig = stamina;
        startingLives = lives;
        speedOrig = (int)speed;
        updatePlayerUI();
        if (gameManager.instance.getPlayerSpawnPos() != null) {
            spawnPlayer();
        }

        gunInventoryManager = FindObjectOfType<GunInventoryManager>();
        objectPool = FindObjectOfType<projectilePool>();
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.instance.getPauseStatus() == false) {
            movement();
            HandleCrouch();
            DashAll(); // at bottom of file, only does anything if specific key is pressed

            if (readyToHeal) {  // HEALING STARTS HERE, READY TO HEAL DETERMINATION STARTS IN TAKEDAMAGE()
                StartCoroutine(healing());  // recursive method
                readyToHeal = false;  // stop healing
            }

            if (guns.Count != 0)
                selectGun();

            if (Input.GetButtonDown("ThrowGrenade") && grenades.Count > 0) {
                throwGrenade();
            }
            if (Input.GetButtonDown("Heal Item") && heals.Count > 0 && healCoolDown <= 0f && !isHealing) {
                StartCoroutine(HealPlayer());
            }
            if (healCoolDown > 0f) {
                healCoolDown -= Time.deltaTime;
            }
        }
      

        sprint();
        // Check if sprinting -- Drain stamina as the player runs
        if (isSprinting && !isDraining)
            StartCoroutine(staminaDrain());

        if (isLaser) { // laser sight
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~ignoreLayer);
            laserSight.SetPosition(0, shotFlash.transform.position); // first param in "index", 0 is line start
            laserSight.SetPosition(1, hit.point); // first param in "index", 1 is line end
        }
    }

    public void spawnPlayer() {
        // Secondary check to make sure the player can only respawn if they have lives.
        if (lives > 0) {
            controller.enabled = false;
            transform.position = gameManager.instance.getPlayerSpawnPos().transform.position;
            transform.rotation = gameManager.instance.getPlayerSpawnPos().transform.rotation;
            controller.enabled = true;
            HP = HPOrig;
            lowHealth = false;
            gameManager.instance.getHealthWarning().SetActive(false);
            RemoveAllStatusEffects();
            updatePlayerUI();
            CameraShake.instance.setIsNotDead(true);
        }
    }

    private void RemoveAllStatusEffects()
    {
        StatusEffects[] effects = GetComponents<StatusEffects>();
        //Debug.Log("You tried!");
        foreach (StatusEffects effect in effects)
        {
            //StopCoroutine(effect.statusCoroutine);
            Destroy(effect);
        }

    }

    // Player Movement Controls
    void movement()
    {
        // Check to see if the player is on the ground to zero out y velocity and zero out the jump counter
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCounter = 0;
        }
        
        // Movement Controller
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);
       

        // Stamina Recovery
        if (stamina < staminaOrig && !isSprinting && !isRecovering)
            StartCoroutine(staminaRecover());

        // Jump Controller
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps) {
            jumpCounter++;
            playerVel.y = jumpSpeed;
            aud.PlayOneShot(audioManager.instance.audJump[Random.Range(0, audioManager.instance.audJump.Length)], audioManager.instance.audJumpVol);

            // For Ty's easter egg:
            if (activated) {
                if (TyEasterEgg.getTriggerBool() == true) {
                    jumpInAreaCount++;
                    if (jumpInAreaCount == 5) {
                        TyEasterEgg.activateEasterEgg(aud);
                    }
                }
            }
        }

        // Gravity Controller
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Check for Enemy (Reticle)
        if (guns.Count > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~ignoreLayer))
            {
                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    gameManager.instance.changeReticle(true);
                }
                else
                {
                    gameManager.instance.changeReticle(false);
                }
            }

            shootGun();
        }

        // Play footsteps as the player moves on the ground
        // Need additional checks to make sure sound isn't funky & only plays when the player is on the ground.
        if (controller.isGrounded && moveDir.magnitude > 0.3f && !isStepping)
            StartCoroutine(playFootsteps());
    }

    IEnumerator playFootsteps() {
        isStepping = true;

        // Play step sound
        aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
        aud.PlayOneShot(audioManager.instance.audSteps[Random.Range(0, audioManager.instance.audSteps.Length)], audioManager.instance.audStepVol);

        // Check if the player is sprinting and play the sound faster if so
        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isStepping = false;
    }


    private projectilePool objectPool; // object pool class instance initialized in start I think, array of object pools
    public ObjectType projectileType; // for object pooling / recycling, ObjectType is enum in projectilePool script

    void shootGun() {
        // Check if the player is not shooting & if they pressed the reload button mapped in input manager
        if (!isShooting && Input.GetButton("Reload") && guns[gunPos].ammoCur < guns[gunPos].ammoMag)
        {
            reload();
        } 
        // Have an additional check if the player has no ammo to auto-reload.
        else if (guns[gunPos].ammoCur == 0 && guns[gunPos].ammoMax > 0)
        {
            reload();
        }
        if (!isReloading)
        {
            if (getCurGun().isAutomatic)
            {
                if (Input.GetButton("Fire1") && !isShooting && !gameManager.instance.getPauseStatus())
                {
                    if (getAmmo() > 0)
                    {
                        StartCoroutine(shoot());

                        //Instantiate(playerShot, Camera.main.transform.position, Camera.main.transform.rotation); // OG method
                        GameObject newProjectile = objectPool.getProjectileFromPool(projectileType); // setting bullet object to newProjectile
                                                                                                     // if there is a bullet in the correct pool, it sets that to newProjectile. Else it makes a new ones and sets it to newProjectile
                        newProjectile.transform.position = playerCamera.transform.position; // player camera needs to be used instead of Camera.main (I don't know why)
                        newProjectile.transform.rotation = playerCamera.transform.rotation;
                        newProjectile.GetComponent<Rigidbody>().velocity = playerCamera.transform.forward * (playerShot.GetComponent<damage>().getAttackSpeed()); // accessing damage script and getting bullet speed
                        newProjectile.SetActive(true); // turning object on (it is set off when added to object pool)
                        playerShot.GetComponent<damage>().setCurrentPosAndRange(newProjectile.transform.position, range);
                        // setting bullet start position, update in damage will track its distance and "delete" the bullet when it goes past the distance
                        aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
                        aud.PlayOneShot(guns[gunPos].shootSound[Random.Range(0, guns[gunPos].shootSound.Length)], guns[gunPos].audioVolume); // Play the gun's shoot sound
                    }
                    else
                    {
                        StartCoroutine(AmmoWarningFlash());
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && !isShooting && !gameManager.instance.getPauseStatus())
                {
                    if (getAmmo() > 0)
                    {
                        StartCoroutine(shoot());

                        GameObject newProjectile = objectPool.getProjectileFromPool(projectileType); // setting bullet object to newProjectile
                                                                                                     // if there is a bullet in the correct pool, it sets that to newProjectile. Else it makes a new ones and sets it to newProjectile
                        newProjectile.transform.position = playerCamera.transform.position; // player camera needs to be used instead of Camera.main (I don't know why)
                        newProjectile.transform.rotation = playerCamera.transform.rotation;
                        newProjectile.GetComponent<Rigidbody>().velocity = playerCamera.transform.forward * (playerShot.GetComponent<damage>().getAttackSpeed()); // accessing damage script and getting bullet speed
                        newProjectile.SetActive(true); // turning object on (it is set off when added to object pool)
                        playerShot.GetComponent<damage>().setCurrentPosAndRange(newProjectile.transform.position, range);
                        // setting bullet start position, update in damage will track its distance and "delete" the bullet when it goes past the distance

                        aud.PlayOneShot(guns[gunPos].shootSound[Random.Range(0, guns[gunPos].shootSound.Length)], guns[gunPos].audioVolume); // Play the gun's shoot sound
                    }
                    else
                    {
                        StartCoroutine(AmmoWarningFlash());
                    }
                }
            }
        }
    }

    void reload()
    {
        StartCoroutine(doReloadCooldown());
        // Check if there's enough spare ammo to fill a mag
        if (guns[gunPos].ammoMax - (guns[gunPos].ammoMag - guns[gunPos].ammoCur) >= 0)
        {
            guns[gunPos].ammoMax -= (guns[gunPos].ammoMag - guns[gunPos].ammoCur);
            
            // Set cur to mag size
            guns[gunPos].ammoCur = guns[gunPos].ammoMag;
        }
        else
        {
            // If there's not enough for a mag, force add any remaining bullets left over to ammoCur.
            guns[gunPos].ammoCur += guns[gunPos].ammoMax;
            guns[gunPos].ammoMax = 0;
        }

        // Play reload sound
        aud.PlayOneShot(audioManager.instance.audReload, audioManager.instance.audReloadVol);

        // Update the UI for confirmation
        updatePlayerUI();
    }

    IEnumerator doReloadCooldown() {
        isReloading = true;
        yield return new WaitForSeconds(reloadCooldown);
        isReloading = false;
    }

    // Shoot Timer
    IEnumerator shoot()
    {
        isShooting = true;
        StartCoroutine(shotFlashTimer());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~ignoreLayer)) {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            guns[gunPos].ammoCur--;
            playerStats.Stats.gunShot();

            if (dmg != null) {
                if (hit.collider != hit.collider.GetComponent<enemyAI>().getMiniBossHeadCollider() && hit.collider != hit.collider.GetComponent<enemyAI>().getEnemyHeadCollider()) {
                    dmg.takeDamage((damage * damageBuffMult)); 
                }
                
                else { // headshot
                    dmg.takeDamage((damage * damageBuffMult) * headShotMult);
                    aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;  // Ensure correct mixer group
                    aud.PlayOneShot(audioManager.instance.headShotA, 0.5f);

                    playerStats.Stats.enemyHeadShot();
                   
                }
               
                if (guns[gunPos].isSniper || guns[gunPos].isShotgun)
                {
                    // Get the source position (where the shot is coming from)
                    Vector3 sourcePosition = transform.position;

                    // Apply knockback using the enemy's takeDamage method with the sourcePosition
                    dmg.takeDamage(damage, sourcePosition);
                }

                if (guns[gunPos].hitEffects != null)
                    Instantiate(guns[gunPos].hitEffects, hit.point, Quaternion.identity);
            }
            else if (hit.collider.GetComponent<bossRoom>() == true)
            {
                if (gameManager.instance.getInteractUI().activeInHierarchy)
                    gameManager.instance.getInteractUI().SetActive(false);

                gameManager.instance.openContinueMenu();


            }
            else if (guns[gunPos].hitEffects != null) { 
                Instantiate(guns[gunPos].hitEffects, hit.point, Quaternion.identity);
            }
       }
        else { guns[gunPos].ammoCur--; } //had to put this here, there's a bug this was causing where if the bullet isn't hitting anything, it doesn't use ammo.

        updatePlayerUI();

        // Time Between Shots
        yield return new WaitForSeconds(fireRate);

        isShooting = false;        
    }

    public void addAmmo(int amount)
    {
        // Generic method that will add the given amount to the player's CURRENT gun.
        // If it goes over the mag limit, it'll be added to max instead.
        if (guns.Count > 0)
        {
            int _ammo = (int)(getAmmoOrig() * (amount / 100f));
            // Check if the player's ammo reserve isn't already full
            if (getCurGun().ammoMax < getCurGun().ammoOrig)
            {
                // Check if the ammo will go over mag size
                if (_ammo + getCurGun().ammoCur >= getAmmoMag())
                {
                    // Does go over size, will be adding to reserve instead.
                    // Check if adding to reserve will hit capacity.
                    if (_ammo + getCurGun().ammoMax >= getAmmoOrig())
                    {
                        // Add any remaining ammo to mag size as long as it doesn't go over mag
                        _ammo -= getAmmoOrig() - getCurGun().ammoMax;
                        // Will hit capacity, set max to capacity.
                        getCurGun().ammoMax = getAmmoOrig();

                        if (_ammo > 0)
                        {
                            // Check if it'll go over magazine size
                            if (_ammo + getCurGun().ammoCur > getCurGun().ammoMag)
                            {
                                getCurGun().ammoCur = getAmmoMag();
                            }
                            else
                            {
                                // It will not, so just add the leftover ammo.
                                getCurGun().ammoCur += _ammo;
                            }
                        }
                    }
                    else
                    {
                        getCurGun().ammoMax += _ammo; // Will not hit capacity, so add normally.
                    }
                }
                else
                    getCurGun().ammoCur += _ammo; // Does not go over mag size, so just add to clip.
            }
            else
            {
                getCurGun().ammoCur = getCurGun().ammoMag; // player's reserve is full, set clip to mag size.
            }
        }
        updatePlayerUI();
    }

    // Player Damage Controller
    public void takeDamage(float amount) {
        if (HP > 0) { // Further prevention from additional damage that may trigger things like lives lost multiple times or negative HP values.
            criticalHealthThreshold = 0.1f;
            isCriticalHealth = HP <= HPOrig * criticalHealthThreshold;
            if (shieldOn) {
                shieldHP -= amount;
                if (shieldHP <= 0) {
                    setShieldOff();
                }
            }
            else if (isCriticalHealth) {
                // Reduce damage by half when in critical health
                amount *= 0.5f;

                // If the player's HP is already 1 or the damage is still lethal, let the player die
                if (HP == 1)
                {
                    HP = 0; // player dead
                }
                else if (amount >= HP)
                {
                    HP = 1;  // Leave the player at 1 HP temporarily
                }
                else
                {
                    HP -= amount;
                }
            }
            else {
                
                HP -= amount;
            }
            playerStats.Stats.attacked(amount);
            stopHealing = true; // STOP HEALING IF DAMAGED

            if (amount >= 5) // If the damage taken exceeds 3, apply the bleeding effect
            {
                ApplyBleedingEffect(gameObject);
            }

            // Play hurt sound for audio indication
            if (damageAudioReady)
            {
                aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
                aud.PlayOneShot(audioManager.instance.audHurt[Random.Range(0, audioManager.instance.audHurt.Length)], audioManager.instance.audHurtVol);
                StartCoroutine(damageAudioCooldown());
            }

            // Update UI & Flash Screen red
            updatePlayerUI();
            StartCoroutine(damageFlash());

            // On Player Death
            if (HP <= 0)
            {
                HP = 0; // set HP to 0 for no weirdness in code/visuals
                CameraShake.instance.setIsNotDead(false);
                if (lives > 0) { lives--; }
                playerStats.Stats.died();
                gameManager.instance.youLose();
            }
            else if ((HP / HPOrig) <= .25)
            {
                lowHealth = true;
                gameManager.instance.getHealthWarning().SetActive(true);
                playerStats.Stats.almostDied();
            }
            if ((HP / HPOrig) < .5)
            {
                StartCoroutine(noDamageTime()); // TIMER FOR WHEN PLAYER CAN START TO HEAL
            }
        }
    }

    public void ApplyBleedingEffect(GameObject target)
    {
        // Check if the player already has a BleedingEffect to avoid stacking
        if (target.GetComponent<BleedingEffect>() == null)
        {
            // Add the bleeding effect and configure it
            BleedingEffect bleedingEffect = target.AddComponent<BleedingEffect>();
            bleedingEffect.duration = 5f;  // Bleeding duration in seconds
            bleedingEffect.damagePerTick = 1f;  // Mild damage per tick
            bleedingEffect.ApplyEffect(target);
        }
    }
    public void takeDamage(float amount, Vector3 sourcePosition)
    {
        takeDamage(amount);
        ApplyKnockback(sourcePosition);

    }
    private void ApplyKnockback(Vector3 sourcePosition)
    {
        Vector3 knockbackDirection = (transform.position - sourcePosition).normalized;
        float knockbackForce = 5f;  

        
        controller.Move(knockbackDirection * knockbackForce * Time.deltaTime);
    }

    IEnumerator damageAudioCooldown() { // so player doesnt go UH AH UH AH ER UMF UH AH every half second
        damageAudioReady = false;
        yield return new WaitForSeconds(1);
        damageAudioReady = true;
    }

    IEnumerator shotFlashTimer() {
        shotFlash.SetActive(true);
        yield return new WaitForSeconds(0.075f);
        shotFlash.SetActive(false);
    }

    public void Heal(bool buff) { // if true, it is a buff and should not be recursive or follow the half health limit
        HP += 3; // HEAL THE PLAYER
        AudioSource healAudioSource = gameObject.AddComponent<AudioSource>();
        healAudioSource.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
        healAudioSource.PlayOneShot(audioManager.instance.audHeal, audioManager.instance.audHealVol);
        // playing heal audio clip
        StartCoroutine(healIndicator()); // flashing screen green
        if (lowHealth == true) {
            if ((HP / HPOrig) > .25) { 
                lowHealth = false;
                gameManager.instance.getHealthWarning().SetActive(false); // getting rid of low health state if not low anymore
            }
        }
        if (buff == false) {
            if ((HP / HPOrig) > .5) { // only heals to half HP
                HP = (HPOrig / 2); // if HP goes over half, reset to half
                readyToHeal = false; // HEALING OVER
            }
            else {
                StartCoroutine(healing()); // KEEP HEALING IF NOT AT HALF HP (RECURSION)
            }
        }
        else {
            if (HP >= HPOrig) {
                HP = HPOrig;
            }
        }
    }

    IEnumerator healing() {
        if (stopHealing == false) { // stopping healing if interupted
            yield return new WaitForSeconds(1);
            Heal(false);
            updatePlayerUI(); // updating health bar
        }
    }

    IEnumerator noDamageTime() { // time till healing (if no damage was taken)
        float currentHP = HP;
        yield return new WaitForSeconds(3);
        if (currentHP == HP) { // PLAYER CAN START HEALING (SEE UPDATE())
            stopHealing = false;
            readyToHeal = true;
        }
        else { // player did not go 3 seconds without taking damage, do not heal
            readyToHeal = false;
        }
    }

    // Damage Flash Timer
    IEnumerator damageFlash()
    {
        gameManager.instance.getDmgFlash().SetActive(true); //gameManager
        yield return new WaitForSeconds(dmgFlashTimer);
        gameManager.instance.getDmgFlash().SetActive(false); //gameManager
    }

    IEnumerator healIndicator() {
        gameManager.instance.getHealIndicator().SetActive(true); // flashing screen green when healing
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.getHealIndicator().SetActive(false);
    }

    // Flash No Ammo
    IEnumerator AmmoWarningFlash()
    {
        gameManager.instance.getAmmoWarning().SetActive(true); //gameManager
        yield return new WaitForSeconds(ammoWarningTimer);
        gameManager.instance.getAmmoWarning().SetActive(false); //gameManager
    }


    bool buffActive = false;

    public void callBuff(int buff, Sprite icon) { // a seperate method is needed to call the Coroutine because if the pickup calls it directly it won't work after being destoryed
        if (buffActive == false) {
            buffActive = true;
            gameManager.instance.getBuffUI().SetActive(true);
            gameManager.instance.getBuffIcon().sprite = icon;
            if (buff == 1) {
                StartCoroutine(damageBuff(1));
            }
            else if (buff == 2) {
                StartCoroutine(healBuff(1));
            }
            else if (buff == 3) {
                StartCoroutine(shieldBuff(1));
            }
            else if (buff == 4) {
                StartCoroutine(staminaBuff(1));
            }
        }
        else { // if first buff UI slot is active, put buff in 2nd UI slot
            gameManager.instance.getBuffUI2().SetActive(true);
            gameManager.instance.getBuffIcon2().sprite = icon;
            if (buff == 1) {
                StartCoroutine(damageBuff(2));
            }
            else if (buff == 2) {
                StartCoroutine(healBuff(2));
            }
            else if (buff == 3) {
                StartCoroutine(shieldBuff(2));
            }
            else if (buff == 4) {
                StartCoroutine(staminaBuff(2));
            }
        }
    }

    IEnumerator damageBuff(int slot) {
        damageBuffMult = 1.5f;
        yield return new WaitForSeconds(10);
        damageBuffMult = 1;
        if (slot == 1) {
            buffActive = false;
            gameManager.instance.getBuffUI().SetActive(false);
        }
        else {
            gameManager.instance.getBuffUI2().SetActive(false);
        }
    }

    IEnumerator healBuff(int slot) {
        for (int i = 1; i <= 5; i++) {
            Heal(true);
            updatePlayerUI();
            yield return new WaitForSeconds(1);
        }
        if (slot == 1) {
            buffActive = false;
            gameManager.instance.getBuffUI().SetActive(false);
        }
        else {
            gameManager.instance.getBuffUI2().SetActive(false);
        }
    }

    IEnumerator shieldBuff(int slot) {
        gameManager.instance.getShieldBar().SetActive(true);
        shieldOn = true;
        yield return new WaitForSeconds(10);
        setShieldOff();
        if (slot == 1) {
            buffActive = false;
            gameManager.instance.getBuffUI().SetActive(false);
        }
        else {
            gameManager.instance.getBuffUI2().SetActive(false);
        }
    }

    public void setShieldOff() {
        shieldOn = false;
        shieldHP = 50;
        gameManager.instance.getShieldBar().SetActive(false);
    }

    IEnumerator staminaBuff(int slot) {
        infiniteStam = true;
        yield return new WaitForSeconds(5);
        infiniteStam = false;
        if (slot == 1) {
            buffActive = false;
            gameManager.instance.getBuffUI().SetActive(false);
        }
        else {
            gameManager.instance.getBuffUI2().SetActive(false);
        }
    }

    void sprint() {
        // Check if the player has stamina to sprint
        if (Input.GetButtonDown("Sprint") && stamina > 0) {
            speed *= speedMod;
            isSprinting = true;
        }
        else if (stamina <= 0 && isSprinting) {
            // Stop sprinting if shift is up or player doesn't have stamina.
            speed /= speedMod;
            isSprinting = false;
        }
        else if (Input.GetButtonUp("Sprint") && isSprinting) {
            // This is a preventative measure for a bug that permanently decreases the player speed if
            // they run out of stamina while running and let go of shift.
            speed /= speedMod;
            isSprinting = false;
        }
    }

    // Drain stamina as player runs
    IEnumerator staminaDrain() {
        if (infiniteStam == false) {
            if (controller.isGrounded && moveDir.magnitude > 0.25f) { // checking if player is not in air amd there is movement input
                isDraining = true;
                stamina--;
                updatePlayerUI();
                yield return new WaitForSeconds(drainMod);
                isDraining = false;
            }
        }
    }

    // Recover stamina as player walks
    IEnumerator staminaRecover()
    {
        isRecovering = true;
        stamina++;
        updatePlayerUI();
        yield return new WaitForSeconds(recoveryMod);
        isRecovering = false;
    }

    public void updatePlayerUI() { // Update information on the UI
        // Health Info
        gameManager.instance.getHPBar().fillAmount = HP / HPOrig;
        gameManager.instance.getHPText().text = HP.ToString("F0");
        if (shieldOn) {
            gameManager.instance.getShieldText().text = shieldHP.ToString("F0");
            gameManager.instance.getShieldBarImage().fillAmount = shieldHP / 25;
        }
        // Stamina Info
        gameManager.instance.getStamBar().fillAmount = (float)stamina / staminaOrig;
        gameManager.instance.getStamText().text = stamina.ToString("F0");
        // Attack Info
        if (guns.Count > 0)
        {
            gameManager.instance.getAmmoBar().fillAmount = (float)getCurGun().ammoCur / getCurGun().ammoMag;
            gameManager.instance.getAmmoText().text = getAmmo().ToString("F0") + " / " + getAmmoMag().ToString("F0");
            gameManager.instance.getAmmoReserveText().text = getAmmoMax().ToString("F0");
        }
        // XP Info
        gameManager.instance.getXPBar().fillAmount = (float)playerXP / playerXPMax;
        gameManager.instance.getXPText().text = playerXP.ToString("F0") + " / " + playerXPMax.ToString("F0");
    }
    
    // Tracks the player's xp and levels them up and gives them a skill point
    public void levelTracker()
    {
        // Check if player XP meets requirement to level up (XP Max)
        if (playerXP >= playerXPMax && playerLevel < 999)
        {
            playerLevel++;
            skillPoints++;
            leveledUp = true;
            playerStats.Stats.levelUp();
            playerXP -= playerXPMax; // Reset XP back to zero
            gameManager.instance.getLevelTracker().text = playerLevel.ToString("F0");
            
            // Run again if the player has enough remaining XP.
            if (playerXP >= playerXPMax)
                levelTracker();
        }
    }

    public void toggleSprintOn()
    {
        toggleSprint = !toggleSprint;

        speedOrig = (int) speed;

     
        speed *= speedMod;
        isSprinting = true;
        staminaDrain();

        if (stamina == 0)
        {
                speed = speedOrig;
                isSprinting = false;
                staminaRecover();
            
                
        }
        updatePlayerUI();
    }

    void HandleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))  
        {
            isCrouching = !isCrouching;  // Toggle crouch state

            if (isCrouching)
            {
                controller.height = crouchHeight;  
                controller.center = new Vector3(0, crouchHeight / 4, 0);
                playerCamera.localPosition = new Vector3(0, crouchHeight * 0.75f, 0);

                speed = speedOrig / 2f;  // Reduce speed while crouching
            }
            else
            {
                controller.height = normalHeight;  
                controller.center = new Vector3(0, normalHeight / 6, 0);
                playerCamera.localPosition = new Vector3(0, normalHeight * 0.75f, 0); ;  // Reset camera position
               speed = speedOrig;  // Restore original speed
            }
        }
    }
    void AdjustHitbox(float baseHeight)
    {
        controller.height = baseHeight * hitboxHeightMultiplier;  // Adjust height
        controller.radius = controller.radius * hitboxWidthMultiplier;  // Adjust width
        controller.center = new Vector3(0, controller.height / 2, 0);  // Adjust center
    }

    void throwGrenade()
    {
        if (grenades[0] == null)
        {
            grenade = grenadeStats.grenadeModel;
        }
        if (grenades.Count > 0)
        {
            GameObject grenadeInstance = Instantiate(grenade, transform.position + transform.forward, Quaternion.identity);
            aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
            aud.PlayOneShot(grenadeStats.pinSound, 1.0f); // playing audio for taking grenade pin out

            Rigidbody rb = grenadeInstance.GetComponent<Rigidbody>(); //throw force 
            if (rb != null)
            {
                rb.AddForce(Camera.main.transform.forward * grenadeStats.throwForce, ForceMode.VelocityChange);
            }

            StartCoroutine(HandleGrenadeExplosion(grenadeInstance));// start explosion count down 

            grenades.Remove(grenades[0]);
            gameManager.instance.getGrenadesUI().text = grenades.Count.ToString();
        }
    }

    float distanceFromGrenade;
    float grenadeMult = 1;
    IEnumerator HandleGrenadeExplosion(GameObject grenadeInstance) {
        yield return new WaitForSeconds(grenadeStats.explosionDelay); //explosion delay

        if(grenadeStats.explosionEffect != null) // trigger explosion effect
        {
            Instantiate(grenadeStats.explosionEffect,grenadeInstance.transform.position, Quaternion.identity);
        }
        aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
        aud.PlayOneShot(grenadeStats.explosionSound, 1.0f);

        distanceFromGrenade = Vector3.Distance(controller.transform.position, grenadeInstance.transform.position);
        if (distanceFromGrenade < 5) {
            CameraShake.instance.TriggerShake(1f, 0.75f);
        }
        else if (distanceFromGrenade < 10) {
            CameraShake.instance.TriggerShake(0.75f, 0.75f);
        }
        else if (distanceFromGrenade < 15) {
            CameraShake.instance.TriggerShake(0.6f, 0.75f);
        }
        else if (distanceFromGrenade < 20) {
            CameraShake.instance.TriggerShake(0.4f, 0.75f);
        }
        else {
            CameraShake.instance.TriggerShake(0.2f, 0.75f);
        }

        if (grenadeStats.shockwavePrefab != null) {
            // Create the shockwave at the grenade's position
            Instantiate(grenadeStats.shockwavePrefab, grenadeInstance.transform.position, Quaternion.identity);
        }
        Collider[] colliders = Physics.OverlapSphere(grenadeInstance.transform.position,grenadeStats.explosionRadius); // damage thing around in explosion
        foreach(Collider nearbyObject in colliders)
        {
            IDamage damageable = nearbyObject.GetComponent<IDamage>();
            if(damageable != null) { // bleed should already be applied because of the damage amount, grenade does double damage for some reason
                if (nearbyObject.CompareTag("Player")) {
                    grenadeMult = 1; // double damage
                }
                else if (nearbyObject.CompareTag("Demon Golem") || nearbyObject.CompareTag("Elder Demon")) {
                    grenadeMult = 0.25f; // boss, half damage
                }
                else {
                    grenadeMult = 0.5f; // enemies, normal damage
                }
                damageable.takeDamage((grenadeStats.explosionDamage) * grenadeMult); // grenade does double damage to player
            }
        }
        Destroy(grenadeInstance);
    }

    IEnumerator HealPlayer() { // x
        if (HP != HPOrig) {
            isHealing = true;

            HP = Mathf.Min(HP + heals[0].healAmount, HPOrig);
            StartCoroutine(healIndicator()); // flashing screen green

            if (heals[0].healSound != null) {
                aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
                aud.PlayOneShot(heals[0].healSound, audioManager.instance.audHealVol);
            }
            healCoolDown = heals[0].healCoolDown;
            gameManager.instance.getHealsUI().text = (heals.Count-1).ToString(); // -1 because it hasn't been removed yet
            if ((HP / HPOrig) > .25) {
                lowHealth = false;
                gameManager.instance.getHealthWarning().SetActive(false); // getting rid of low health state if not low anymore
            }
            updatePlayerUI();

            yield return new WaitForSeconds(heals[0].healCoolDown);
            isHealing = false;
            heals.Remove(heals[0]);

        }
    }

    // temporary check if the player has a gun -- can remove later, using for debug purposes
    public bool hasGun()
    {
        if (guns.Count != 0)
            return true;
        else
            return false;
    }

    public void addToHeals(HealStats newHeal) {
        heals.Add(newHeal);
        gameManager.instance.getHealsUI().text = heals.Count.ToString();
    }

    public void addToGrenades(GrenadeStats newGrenade) {
        grenades.Add(newGrenade);
        gameManager.instance.getGrenadesUI().text = grenades.Count.ToString();

    }public void removeFromHeals() {
        heals.Clear();
        gameManager.instance.getHealsUI().text = heals.Count.ToString();
    }

    public void removeFromGrenades() {
        grenades.Clear();
        gameManager.instance.getGrenadesUI().text = grenades.Count.ToString();
    }

    void selectGun() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            gunPos++;
            if (gunPos == guns.Count)
                gunPos = 0;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            gunPos--;
            if (gunPos < 0)
                gunPos = guns.Count - 1;
            changeGun();
        }
        
    }

    void changeGun() {
        float damTemp = guns[gunPos].damage * damageUpgradeMod;
        damage = damTemp;
        range = guns[gunPos].range;
        fireRate = getCurGun().fireRate;
        isSniper = getCurGun().isSniper;
        isLaser = getCurGun().isLaser;
        isShotgun = getCurGun().isShotgun;
        
        if (isLaser) {
            laserSight.enabled = true;
        }
        else {
            laserSight.enabled = false;
        }
        updatePlayerUI();
        playerShot = guns[gunPos].projectile;
        gunFlashColor.gunFlash.changeColor((int)guns[gunPos].color);
        projectileType = playerShot.GetComponent<damage>().getProjectileType(); // accessing damage script and getting enum type
        gunModel.GetComponent<MeshFilter>().sharedMesh = getCurGun().gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = getCurGun().gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public int getCurrGunIndex()
    { return gunPos; }

    public void getGunStats(gunStats _gun)
    {
        _gun.ammoCur = _gun.ammoMag; // Set the current to the magazine size.
        _gun.ammoMax = _gun.ammoOrig; // Set the max to the original gun ammo capacity.
        guns.Add(_gun);
        gunPos = guns.Count - 1;
        updatePlayerUI();

        float damTemp = _gun.damage * damageUpgradeMod; //reason I did this is because we can't supply a float value to an int.
        damage = damTemp;
        fireRate = _gun.fireRate;
        range = _gun.range;
        isSniper = _gun.isSniper;
        isLaser = _gun.isLaser;
        isShotgun = _gun.isShotgun;
        if (isLaser) {
            laserSight.enabled = true;
        }
        else {
            laserSight.enabled = false;
        }
        gunFlashColor.gunFlash.changeColor((int)_gun.color);
        playerShot = _gun.projectile;
        projectileType = playerShot.GetComponent<damage>().getProjectileType(); // accessing damage script and getting enum type
        gunModel.GetComponent<MeshFilter>().sharedMesh = _gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = _gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        FindObjectOfType<GunInventoryManager>().UpdateGunInventoryUI();
    }

    public void DashAll() {
        StartCoroutine(DashW());
        StartCoroutine(DashA());
        StartCoroutine(DashS());
        StartCoroutine(DashD());
    }
    // 3 vars for each dash directions, w a s d
    float time1W;
    float time2W;
    bool isTapW = false;
    float time1A;
    float time2A;
    bool isTapA = false;
    float time1S;
    float time2S;
    bool isTapS = false;
    float time1D;
    float time2D;
    bool isTapD = false;

    IEnumerator DashW() {
        if (Input.GetButtonDown("Dash W")) {
            if (isTapW == true) {
                time1W = Time.time;
                isTapW = false;
                if (onDashCooldown == false && time1W - time2W < 0.2f) { // time that both key pressed need to be done within
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(transform.forward * 1.5f);
                        yield return new WaitForSeconds(0.05f);
                    }
                    StartCoroutine(dashCooldown()); // can't dash again for x seconds
                }
            }
        }
        else {
            if (isTapW == false) {
                time2W = Time.time;
                isTapW = true;
            }
        }
    }
    IEnumerator DashA() {
        if (Input.GetButtonDown("Dash A")) {
            if (isTapA == true) {
                time1A = Time.time;
                isTapA = false;
                if (onDashCooldown == false && time1A - time2A < 0.2f) { // time that both key pressed need to be done within
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(-transform.right * 1.5f); // negative right = left
                        yield return new WaitForSeconds(0.05f);
                    }
                    StartCoroutine(dashCooldown()); // can't dash again for x seconds
                }
            }
        }
        else {
            if (isTapA == false) {
                time2A = Time.time;
                isTapA = true;
            }
        }
    }
    IEnumerator DashS() {
        if (Input.GetButtonDown("Dash S")) {
            if (isTapS == true) {
                time1S = Time.time;
                isTapS = false;
                if (onDashCooldown == false && time1S - time2S < 0.2f) { // time that both key pressed need to be done within
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(-transform.forward * 1.5f); // negative forward = backwards
                        yield return new WaitForSeconds(0.05f);
                    }
                    StartCoroutine(dashCooldown()); // can't dash again for x seconds
                }
            }
        }
        else {
            if (isTapS == false) {
                time2S = Time.time;
                isTapS = true;
            }
        }
    }
    IEnumerator DashD() {
        if (Input.GetButtonDown("Dash D")) {
            if (isTapD == true) {
                time1D = Time.time;
                isTapD = false;
                if (onDashCooldown == false && time1D - time2D < 0.2f) { // time that both key pressed need to be done within
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(transform.right * 1.5f);
                        yield return new WaitForSeconds(0.05f);
                    }
                    StartCoroutine(dashCooldown()); // can't dash again for x seconds
                }
            }
        }
        else {
            if (isTapD == false) {
                time2D = Time.time;
                isTapD = true;
            }
        }
    }

    IEnumerator dashCooldown() {
        onDashCooldown = true;
        yield return new WaitForSeconds(2);
        onDashCooldown = false;
    }

    // -- GETTERS --
    public CharacterController getController() {
        return controller;
    }
    public float getHP() {
        return HP;}

    public float getHPOrig() {
        return HPOrig;}

    public int getStamina() {
        return stamina;}

    public int getStaminaOrig() {
        return staminaOrig;}

    public int getXP() {
        return playerXP;}

    public int getAmmo() {
        return getCurGun().ammoCur;}

    public int getAmmoMag()
    {
        return getCurGun().ammoMag;}

    public int getAmmoMax()
    {
        return getCurGun().ammoMax;
    }

    public int getAmmoOrig() {
        return getCurGun().ammoOrig;}

    public bool getIsSniper() {
        return isSniper;}

    public GameObject getGunModel() {
        return gunModel;}

    public float getSpeed() {
        return speed;}

    public int getCoins() {
        return coins;}

    public float getDamage() {
        return damage;}

    public float getDamageMod() {
        return damageUpgradeMod;}

    public int getLives() { 
        return lives;}

    public List<gunStats> getGunList()
    { return guns; }

    public AudioSource getAudioLocation()
    {
        return aud;
    }

    public bool getSafeAccess()
    {
        return safeAccess;
    }

    // Setters
    public void setHP(float newHP) {
        HP = newHP;
    }
    public void setHPOrig(float newHPOrig) {
        HPOrig = newHPOrig;
    }
    public void setAmmo(int newAmmo) {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoCur = newAmmo;

        // Update the UI to show it's been changed
        updatePlayerUI();
    }

    public void setAmmoMag(int newAmmoMag)
    {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoMag = newAmmoMag;

        updatePlayerUI();
    }

    public void setAmmoMax(int newAmmoMax)
    {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoMax = newAmmoMax;
        updatePlayerUI();
    }

    public void setAmmoOrig(int newAmmoOrig) {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoOrig = newAmmoOrig;

        updatePlayerUI();
    }

    public int getPlayerLevel() {
        return playerLevel;}

    public gunStats getCurGun() {
        return guns[gunPos];}

    public void setCurrGun(int _gunPos)
    {
        gunPos = _gunPos;
        changeGun();
    }

    public int getSkillPoints() { return skillPoints; }

    public void setSpeed(float newSpeed) {
        speed = newSpeed;}

    public void setStamina(int newStamina) {
        stamina = newStamina;}

    public void setCoins(int newCoins) {
        coins += newCoins;}

    public void setDamageMod(float newDamageMod) {
        damageUpgradeMod = newDamageMod;}

    public void setXP(int amount) {
        playerXP += amount;
        levelTracker(); }// Check if the player can level up

    public void setPlayerLevel(int newPlayerLevel) {
        playerLevel = newPlayerLevel;}

    public void setSkillPoints(int newSkillPoints) {
        skillPoints = newSkillPoints;}

    public void setDamage(float newDamage) {
        damage = newDamage;}

    public void setGunList(List<gunStats> _list)
        { guns = _list; }

    public void setLives(int _lives)
        { lives = _lives; }

    public void setAudio(AudioSource _aud)
    {
        aud = _aud;
    }
    public void setSprintBool(bool value)
    {
        toggleSprint = value;
    }

    public bool getLeveledUp()
    {
        return leveledUp;
    }

    public void setLeveledUp(bool _state)
    { 
        leveledUp = _state; 
    }

    public void setSafeAccess(bool _state)
    {
        safeAccess = _state;
    }
}
