using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    [DefaultExecutionOrder(-5)]
#else
[DefaultExecutionOrder(5)]
#endif

public class playerMovement : MonoBehaviour, IDamage {

    public static playerMovement player; // singleton

    // Unity object fields
    [Header("-- Player Components --")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud; // Audio controller for the player
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] GameObject playerShot;
    [SerializeField] GameObject bloodSpew;
    [SerializeField] GameObject bloodSpew2;
    [SerializeField] GameObject bleedPos;

    // Player modifiers
    // -- Attributes --
    [Header("-- Player Attributes --")]
    [SerializeField] float HP = 100;
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
    [SerializeField] int markers;
    [SerializeField] GameObject Marker;

    float reloadTime = 1.5f;
    int healsMax = 5;
    int grenadesMax = 3;
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
    [SerializeField] float crouchHeight = 1f;
    //[SerializeField] float crouchSpeed = 1.0f;
   // [SerializeField] float normalSpeed = 6.0f;
    [SerializeField] Transform playerCamera;
    bool noCrouch = false;


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
    Vector3 cameraNormal;

    // Count number of jumps
    int jumpCounter;

    // Trackers
    float HPOrig; 
    float damageBuffMult = 1;
    int playerXP;
    int staminaOrig;
    int playerLevel;
    int gunPos = 0; // Weapon selected
    float speedOrig; 
    int startingLives;

    bool shieldOn = false;
    float shieldHP = 0;

    // Checks
    bool leveledUp = false;
    bool isDashing;
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
        speedOrig = speed;
        updatePlayerUI();
        if (gameManager.instance.getPlayerSpawnPos() != null) {
            spawnPlayer();
        }
        cameraNormal = playerCamera.localPosition;

        gunInventoryManager = FindObjectOfType<GunInventoryManager>();
        objectPool = FindObjectOfType<projectilePool>();
        HP = 100; // no clue why but all of a sudden whens starting from level 2 in editor the health was resetting to 0 and causing error
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.instance.getPauseStatus() == false) {
            movement();
            HandleCrouch();
            DashAll(); // at bottom of file, only does anything if specific key is pressed
            useMarker(); // at bottom of file, only does anything if specific key is pressed

            if (readyToHeal) {  // HEALING STARTS HERE, READY TO HEAL DETERMINATION STARTS IN TAKEDAMAGE()
                StartCoroutine(healing());  // recursive method
                readyToHeal = false;  // stop healing
            }
            if (guns.Count != 0) {
                selectGun();
            }
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
        else { // paused
            if (Input.GetKey("g") && Input.GetKey("i") && Input.GetKey("v") && Input.GetKey("e")) { /// shortcut to level 2
                lives = startingLives;
                gameManager.instance.getLivesText().text = lives.ToString();
            }
        }
        if (!isCrouching) {
            Camera.main.transform.localPosition = new Vector3(0, 0.9996f, 0);
            //StartCoroutine(handleCameraPos());
        }
        else {
            Camera.main.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        sprint(); // Check if sprinting -- Drain stamina as the player runs
        if (isSprinting && !isDraining) {
            StartCoroutine(staminaDrain());
        }
        if (hasGun()) { // preventing error when starting game
            if (guns[gunPos].weaponName == gunStats.ObjectType.LaserRifle) { // laser sight
                RaycastHit hit;
                Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~ignoreLayer);
                laserSight.SetPosition(0, shotFlash.transform.position); // first param in "index", 0 is line start
                laserSight.SetPosition(1, hit.point); // first param in "index", 1 is line end
            }
        }
    }

    public void spawnPlayer() {
        // Secondary check to make sure the player can only respawn if they have lives.
        if (lives > 0) {
            StopAllCoroutines();
            gameManager.instance.setLoseState(false);
            controller.enabled = false;
            transform.SetPositionAndRotation(gameManager.instance.getPlayerSpawnPos().transform.position, gameManager.instance.getPlayerSpawnPos().transform.rotation);
            controller.enabled = true;
            HP = HPOrig;
            lowHealth = false;
            gameManager.instance.getHealIndicator().SetActive(false);
            gameManager.instance.getDmgFlash().SetActive(false);
            gameManager.instance.getHealthWarning().SetActive(false);
            RemoveAllStatusEffects();
            readyToHeal = false;
            stopHealing = true;
            isShooting = false;
            foreach (GameObject bleedObject in bleeds) { // getting rid of blood falling out of player
                if (bleedObject != null) {
                    Destroy(bleedObject);
                }
            }
            updatePlayerUI();
            CameraShake.instance.setIsNotDead(true);
        }
    }

    private void RemoveAllStatusEffects()
    {
        // effects isn't populated.
        StatusEffects[] effects = GetComponents<StatusEffects>();

        foreach (StatusEffects effect in effects)
        {
            if (effect.statusCoroutine != null)
            {
                StopCoroutine(effect.statusCoroutine); 
            }
            effect.EndEffect(); 
        }
        StatusEffectUIManager uiManager = FindObjectOfType<StatusEffectUIManager>();
        if (uiManager != null)
        {
            uiManager.HideBurningEffect();
        }
    }

    void movement() {
        if (controller.isGrounded) { // Check to see if the player is on the ground to zero out y velocity and zero out the jump counter
            playerVel = Vector3.zero;
            jumpCounter = 0;
        }

        float moveSpeed = speed;
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward; // getting user input from Unity input system, accounting for direction being faced
        if (Input.GetAxis("Vertical") == 0) { // moving sideways
            moveSpeed = (speed / 4) * 3;
        }
        else if (Input.GetAxis("Vertical") < 0) { // moving backwards
            moveSpeed = (speed / 2) + 1;
        }
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        if (stamina < staminaOrig && !isSprinting && !isRecovering) {
            StartCoroutine(staminaRecover());
        }
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps) { // Jump Controller
            /// test methods here - ty
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
                        aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
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

    void reload() {
        aud.PlayOneShot(audioManager.instance.audReload, audioManager.instance.audReloadVol);
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

        // Update the UI for confirmation
        updatePlayerUI();
    }

    IEnumerator doReloadCooldown() {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime - 0.25f);
        aud.PlayOneShot(audioManager.instance.audReload, audioManager.instance.audReloadVol);
        yield return new WaitForSeconds(0.25f);
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
                    dmg.takeDamage(damage * damageBuffMult * damageUpgradeMod);
                    hit.collider.GetComponent<enemyAI>().showBleed(hit.point, bloodSpew2); // will display blood spew if conditions are met in enemyAI
                }
                else { // headshot
                    dmg.takeDamage(damage * damageBuffMult * damageUpgradeMod * headShotMult);
                    aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;  // Ensure correct mixer group
                    aud.PlayOneShot(audioManager.instance.headShotA, 0.5f);

                    playerStats.Stats.enemyHeadShot();
                   
                }
               
                if (guns[gunPos].weaponName == gunStats.ObjectType.Shotgun || guns[gunPos].weaponName == gunStats.ObjectType.Sniper) {
                    Vector3 sourcePosition = transform.position; // Get the source position (where the shot is coming from)
                    dmg.takeDamage(sourcePosition); // Apply knockback using the enemy's takeDamage method with the sourcePosition
                }

                if (guns[gunPos].hitEffects != null)
                    Instantiate(guns[gunPos].hitEffects, hit.point, Quaternion.identity);
            }
            else if (hit.collider.GetComponent<bossRoom>() == true)
            {
                if (gameManager.instance.getInteractUI().activeInHierarchy)
                    gameManager.instance.getInteractUI().SetActive(false);
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
            if (getCurGun().ammoMax < getCurGun().ammoOrig){
                // Check if the ammo will go over mag size
                if (_ammo + getCurGun().ammoCur >= getAmmoMag()){
                    // Does go over size, will be adding to reserve instead. Check if adding to reserve will hit capacity.
                    if (_ammo + getCurGun().ammoMax >= getAmmoOrig()){
                        // Add any remaining ammo to mag size as long as it doesn't go over mag
                        _ammo -= getAmmoOrig() - getCurGun().ammoMax;
                        getCurGun().ammoMax = getAmmoOrig();
                        if (_ammo > 0){
                            if (_ammo + getCurGun().ammoCur > getCurGun().ammoMag) {
                                getCurGun().ammoCur = getAmmoMag();
                            }
                            else {
                                getCurGun().ammoCur += _ammo;
                            }
                        }
                    }
                    else {
                        getCurGun().ammoMax += _ammo; // Will not hit capacity, so add normally.
                    }
                }
                else
                    getCurGun().ammoCur += _ammo; // Does not go over mag size, so just add to clip.
            }
            else{
                getCurGun().ammoCur = getCurGun().ammoMag; // player's reserve is full, set clip to mag size.
            }
        }
        updatePlayerUI();
    }


    Queue<GameObject> bleeds = new Queue<GameObject>();

    public void takeDamage(float amount) {
        if (gameManager.instance.getWinState() == true) {
            return;
        }
        if (isDashing) {
            amount = amount / 2;
        }
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
                if (HP == 1) {
                    HP = 0; // player dead
                }
                else if (amount >= HP) {
                    HP = 1;  // Leave the player at 1 HP temporarily
                }
                else {
                    HP -= amount;
                }
            }
            else {
                HP -= amount;
            }
            playerStats.Stats.attacked(amount);
            stopHealing = true; // STOP HEALING IF DAMAGED
            if (amount >= 5) { 
                ApplyBleedingEffect(gameObject);
                GameObject Bleed = Instantiate(bloodSpew, bleedPos.transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity, bleedPos.transform); 
                bleeds.Enqueue(Bleed); // adding to queue
                Destroy(Bleed, 20);
            }
            // Play hurt sound for audio indication
            if (damageAudioReady)
            {
                aud.outputAudioMixerGroup = audioManager.instance.SFXMixerGroup;
                aud.PlayOneShot(audioManager.instance.audHurt[Random.Range(0, audioManager.instance.audHurt.Length)], audioManager.instance.audHurtVol);
                StartCoroutine(damageAudioCooldown());
            }
            updatePlayerUI();
            StartCoroutine(damageFlash());

            if (HP <= 0) { // On Player Death
                HP = 0; // set HP to 0 for no weirdness in code/visuals
                CameraShake.instance.setIsNotDead(false);
                if (lives > 0) { lives--; }
                playerStats.Stats.died();
                gameManager.instance.youLose();
            }
            else if ((HP / HPOrig) <= .25) {
                lowHealth = true;
                gameManager.instance.getHealthWarning().SetActive(true);
                playerStats.Stats.almostDied();
            }
            if ((HP / HPOrig) < .5) {
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
    public void ApplyBurningEffect() {
        if (this.gameObject.GetComponent<BurningEffect>() == null) {
            BurningEffect burningEffect = this.gameObject.AddComponent<BurningEffect>();
            burningEffect.duration = 5f;  
            burningEffect.damagePerSecond = 2f; // does half damage
            burningEffect.ApplyEffect(this.gameObject);
        }
    }
    public void takeDamage(Vector3 sourcePosition)
    {
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
            float currHP = HP;
            if ((HP / HPOrig) > .5) { // only heals to half HP
                readyToHeal = false; // HEALING OVER
                stopHealing = true;
                if (((currHP - 3) / HPOrig) <= .5) { // accounting for bug where you use heal potion to go over half while healing over time and then resets your hp to half
                    HP = (HPOrig / 2); // if HP goes over half, reset to half
                }
            }
            else {
                StartCoroutine(healing()); // KEEP HEALING IF NOT AT HALF HP (RECURSION)
            }
        }
        if (HP > HPOrig) {
            HP = HPOrig;
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
            /// heal potion will cancel heal over time
            stopHealing = false;
            readyToHeal = true;
        }
        else { // player did not go 3 seconds without taking damage, do not heal
            readyToHeal = false;
            stopHealing = true;
        }
    }

    // Damage Flash Timer
    IEnumerator damageFlash()
    {
        gameManager.instance.getDmgFlash().SetActive(true);
        yield return new WaitForSeconds(dmgFlashTimer);
        gameManager.instance.getDmgFlash().SetActive(false);
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

    public bool shieldBuff() {
        if (shieldHP == 50) {
            return false; // can't add shield
        } // else
        shieldHP += 25;
        if (shieldHP > 50) {
            shieldHP = 50;
        }
        gameManager.instance.getShieldBar().SetActive(true);
        gameManager.instance.getShieldText().text = shieldHP.ToString("F0");
        gameManager.instance.getShieldBarImage().fillAmount = shieldHP / 50;
        shieldOn = true;
        return true;
    }

    public void setShieldOff() {
        shieldOn = false;
        shieldHP = 0;
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
    IEnumerator staminaRecover() {
        isRecovering = true;
        stamina++;
        updatePlayerUI();
        yield return new WaitForSeconds(recoveryMod);
        isRecovering = false;
    }

    public void updatePlayerUI() { // Update information on the UI
        gameManager.instance.getHPBar().fillAmount = HP / HPOrig;
        gameManager.instance.getHPText().text = HP.ToString("F0");
        if (shieldOn) {
            gameManager.instance.getShieldText().text = shieldHP.ToString("F0");
            gameManager.instance.getShieldBarImage().fillAmount = shieldHP / 50;
        }
        gameManager.instance.getStamBar().fillAmount = (float)stamina / staminaOrig;
        gameManager.instance.getStamText().text = stamina.ToString("F0");
        if (guns.Count > 0) {
            gameManager.instance.getAmmoBar().fillAmount = (float)getCurGun().ammoCur / getCurGun().ammoMag;
            gameManager.instance.getAmmoText().text = getAmmo().ToString("F0") + " / " + getAmmoMag().ToString("F0");
            gameManager.instance.getAmmoReserveText().text = getAmmoMax().ToString("F0");
        }
        gameManager.instance.getXPBar().fillAmount = (float)playerXP / playerXPMax;
        gameManager.instance.getXPText().text = playerXP.ToString("F0") + " / " + playerXPMax.ToString("F0");
        gameManager.instance.getMarkersUI().text = markers.ToString("F0");
    }
    
    public void levelTracker() { // Tracks the player's xp and levels them up and gives them a skill point
        // Check if player XP meets requirement to level up (XP Max)
        if (playerXP >= playerXPMax && playerLevel < 50)
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

    public void toggleSprintOn() {
        toggleSprint = !toggleSprint;

        speedOrig = speed;

     
        speed *= speedMod;
        isSprinting = true;
        staminaDrain();

        if (stamina == 0) {
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

            if (!isCrouching)
            {
                isCrouching = true;
                controller.height = crouchHeight;  
                controller.center = new Vector3(0, 0.3f, 0);
                Camera.main.transform.localPosition = new Vector3(0,0.3f,0);
                speed = speedOrig / 2f;  // Reduce speed while crouching
            }
            else if (isCrouching)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Vector3.up, out hit, 5f, ~ignoreLayer))
                {
                    if (hit.distance > 1.3f)
                    {
                        isCrouching = false;
                        controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y + 0.5f, controller.transform.position.z);
                        controller.height = normalHeight;
                        controller.center = new Vector3(0, 0, 0);
                        playerCamera.localPosition = new Vector3(0, normalHeight, 0); ;  // Reset camera position
                        speed = speedOrig;  // Restore original speed 
                    }
                }
                else
                {
                    isCrouching = false;
                    controller.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y + 0.5f, controller.transform.position.z);
                    controller.height = normalHeight;
                    controller.center = new Vector3(0, 0, 0);
                    // Reset camera position
                    speed = speedOrig;  // Restore original speed 
                }
            }
        }
    }
    IEnumerator handleCameraPos()
    {
        //playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, cameraNormal, 1f);
        yield return new WaitForSeconds(0.1f);
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

    IEnumerator HealPlayer() { // f
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

    public bool addToHeals(HealStats newHeal) {
        if (heals.Count < healsMax) {
            heals.Add(newHeal);
            gameManager.instance.getHealsUI().text = heals.Count.ToString();
            return true;
        }
        // else, did not add
        return false;
    }

    public bool addToGrenades(GrenadeStats newGrenade) {
        if (grenades.Count < grenadesMax) {
            grenades.Add(newGrenade);
            gameManager.instance.getGrenadesUI().text = grenades.Count.ToString();
            return true;
        }
        // else, did not add
        return false;

    }
    public void removeFromHeals() {
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

    public void changeGun() {
        damage = guns[gunPos].damage;
        range = guns[gunPos].range;
        fireRate = guns[gunPos].fireRate;
        reloadTime = guns[gunPos].reloadTime;
        if (guns[gunPos].weaponName == gunStats.ObjectType.LaserRifle) {
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

    public void getGunStats(gunStats _gun) {
        _gun.ammoCur = _gun.ammoMag; // Set the current to the magazine size.
        _gun.ammoMax = _gun.ammoOrig; // Set the max to the original gun ammo capacity.
        guns.Add(_gun);
        gunPos = guns.Count - 1;
        updatePlayerUI();

        damage = _gun.damage;
        fireRate = _gun.fireRate;
        reloadTime = _gun.reloadTime;
        range = _gun.range;
        if (guns[gunPos].weaponName == gunStats.ObjectType.LaserRifle) {
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
        storeManager.instance.setGunPos(gunPos, guns[gunPos].weaponName);
    }

    public void useMarker() {
        if (Input.GetButtonDown("PlaceMarker")) {
            if (markers > 0 && isCrouching == false && jumpCounter == 0) {
                markers--;
                Instantiate(Marker, new Vector3(transform.position.x, transform.position.y - 0.65f, transform.position.z), Quaternion.Euler(90, 0, 0));
                gameManager.instance.getMarkersUI().text = markers.ToString("F0");
                aud.PlayOneShot(audioManager.instance.placeA, audioManager.instance.placeVol);
            }
        }
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
                if (onDashCooldown == false && time1W - time2W < 0.2f && isDashing == false) { // time that both key pressed need to be done within
                    isDashing = true;
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(transform.forward * 1.5f);
                        yield return new WaitForSeconds(0.025f);
                    }
                    isDashing = false;
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
                if (onDashCooldown == false && time1A - time2A < 0.2f && isDashing == false) { // time that both key pressed need to be done within
                    isDashing = true;
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(-transform.right * 1.5f); // negative right = left
                        yield return new WaitForSeconds(0.025f);
                    }
                    isDashing = false;
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
                if (onDashCooldown == false && time1S - time2S < 0.2f && isDashing == false) { // time that both key pressed need to be done within
                    isDashing = true;
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(-transform.forward * 1.5f); // negative forward = backwards
                        yield return new WaitForSeconds(0.025f);
                    }
                    isDashing = false;
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
                if (onDashCooldown == false && time1D - time2D < 0.2f && isDashing == false) { // time that both key pressed need to be done within
                    isDashing = true;
                    for (int i = 1; i <= 4; i++) {
                        controller.Move(transform.right * 1.5f);
                        yield return new WaitForSeconds(0.025f);
                    }
                    isDashing = false;
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
        yield return new WaitForSeconds(3);
        onDashCooldown = false;
    }

    public void inventoryUpgrade(bool V2) {
        if (V2 == false) { // first upgrade
            healsMax = 10;
            grenadesMax = 5;
        }
        else {
            healsMax = 15;
            grenadesMax = 8;
        }
        gameManager.instance.setGHMaxesUI(healsMax, grenadesMax);
    }

    // -- GETTERS --
    public CharacterController getController() {
        return controller;
    }
    public float getHP() {
        return HP;}

    public float getHPOrig() {
        return HPOrig;}

    public float getShieldHP() {
        return shieldHP; }

    public int getHealsCount() {
        return heals.Count; }
    public int getHealsMax() {
        return healsMax; }
    public int getGrenadesCount() {
        return grenades.Count; }
    public int getGrenadesMax() {
        return grenadesMax; }

    public int getStamina() {
        return stamina;}

    public int getStaminaOrig() {
        return staminaOrig;}
    public void setOGStamina(int newStam) {
        staminaOrig = newStam; }

    public int getXP() {
        return playerXP;}

    public gunStats getCurGun() {
        return guns[gunPos];}
    public gunStats getSpecificGun(int pos) {
        return guns[pos];}
    public int getAmmo() {
        return getCurGun().ammoCur;}
    public int getAmmoMag() {
        return getCurGun().ammoMag;}
    public int getAmmoMax() {
        return getCurGun().ammoMax; }
    public int getAmmoOrig() {
        return getCurGun().ammoOrig;}

    public bool getIsSniper() {
        bool isSniper = false;
        if (guns[gunPos].weaponName == gunStats.ObjectType.Sniper) {
            isSniper = true;
        }
        return isSniper;
    }

    public GameObject getGunModel() {
        return gunModel;}

    public float getSpeed() {
        return speed;}
    public float getNormOGSpeed() {
        return speedOrig; }

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

    public AudioSource getAudioLocation() {
        return aud; }

    public bool getSafeAccess() {
        return safeAccess; }

    public int getPlayerLevel() {
        return playerLevel;
    }

    public int getSkillPoints() { return skillPoints; }
    public bool getLeveledUp() {
        return leveledUp; }

    // Setters
    public void addMarkers(int newMarkers) {
        markers += newMarkers;
        gameManager.instance.getMarkersUI().text = markers.ToString("F0");
    }

    public void setHP(float newHP) {
        HP = newHP;
    }
    public void setHPOrig(float newHPOrig) {
        HPOrig = newHPOrig;
    }
    public void setAmmo(int newAmmo) {
        if (guns != null || guns.Count != 0)
            getCurGun().ammoCur = newAmmo;
        updatePlayerUI();
    }

    public void setAmmoMag(int newAmmoMag) {
        if (guns != null || guns.Count != 0)
            getCurGun().ammoMag = newAmmoMag;
        updatePlayerUI();
    }

    public void setAmmoMax(int newAmmoMax) {
        if (guns != null || guns.Count != 0)
            getCurGun().ammoMax = newAmmoMax;
        updatePlayerUI();
    }

    public void setAmmoOrig(int newAmmoOrig) {
        if (guns != null || guns.Count != 0)
            getCurGun().ammoOrig = newAmmoOrig;
        updatePlayerUI();
    }

    public void setCurrGun(int _gunPos) {
        gunPos = _gunPos;
        changeGun();
    }

    public void setSpeed(float newSpeed) {
        speed = newSpeed;}

    public void setNormOGSpeed(float newSpeed) {
        speedOrig = newSpeed;
    }

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

    public void setLeveledUp(bool _state)
    { 
        leveledUp = _state; 
    }

    public void setSafeAccess(bool _state)
    {
        safeAccess = _state;
    }

    public void setNoCrouch(bool _crouch)
    {
        noCrouch = _crouch;
    }
}
