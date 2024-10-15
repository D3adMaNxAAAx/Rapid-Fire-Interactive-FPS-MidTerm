using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

//------------NOTES-------------
/* For this whole code to work IDamage and gameManager scripts must both be functional.
   After completing scripts please uncomment ONLY that part of the script!!!
   Each one will be labeled as //IDamage or //gameManager at the end of each line of code or function. */
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
    float damageUpgradeMod = 1;  // keep set = to 1, so damage can be upgraded (can just change damage var because it changes when swapping guns)

    // Player Default Weapon Mods
    [Header("-- Player Weapons --")]
    [SerializeField] List<gunStats> guns;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject shotFlash;
    [SerializeField] GameObject laserShot; // player shot visual for laser rifle
    [SerializeField] GameObject shotgunShot; // player shot visual for shotgun
    [SerializeField] LineRenderer laserSight;
    private GunInventoryManager gunInventoryManager;

    bool isSniper = false;
    bool isLaser = false;
    bool isShotgun = false;

    [SerializeField] float damage;
    [SerializeField] float fireRate;
    [SerializeField] float bulletDistance;
    //[SerializeField] int ammo;
    //[SerializeField] float bulletSpeed; // Is here if we wanna change to use bullets
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

    // Checks
    bool isSprinting;
    bool isShooting;
    bool isStepping;
    bool isDraining; // To check if the player is currently losing stamina
    bool isRecovering; // To check if the player is currently recovering stamina
    bool lowHealth = false;
    bool readyToHeal = false;
    bool stopHealing = false; // was the player damaged while healing?
    bool onDashCooldown = false;
    bool isHealing;
    bool damageAudioReady = true;
    bool isCrouching = false;
    bool infiniteStam = false;

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
        /// don't destroy on load
        HPOrig = HP;
        staminaOrig = stamina;
        startingLives = lives;
        speedOrig = (int)speed;
        // Update Player Information & Spawn
        updatePlayerUI();
        if (gameManager.instance.getPlayerSpawnPos() != null)
            spawnPlayer();

        gunInventoryManager = FindObjectOfType<GunInventoryManager>();
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.instance.getPauseStatus() == false) {
        
            if (Input.GetButton("Kill")) { takeDamage(999); }

            movement();
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
          

            //Working out null ref bug...put on pause for time being

                //if (Input.GetButton("Melee"))
                //    StartCoroutine(PlayerMelee());
        }
        HandleCrouch();

        sprint();
        // Check if sprinting -- Drain stamina as the player runs
        if (isSprinting && !isDraining)
            StartCoroutine(staminaDrain());

        if (isLaser) { // laser sight
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, bulletDistance, ~ignoreLayer);
            laserSight.SetPosition(0, shotFlash.transform.position); // first param in "index", 0 is line start
            laserSight.SetPosition(1, hit.point); // first param in "index", 1 is line end
        }
    }

    public void spawnPlayer() {
        // Secondary check to make sure the player can only respawn if they have lives.
        if (lives > 0) {
            controller.enabled = false;
            transform.position = gameManager.instance.getPlayerSpawnPos().transform.position;
            controller.enabled = true;
            HP = HPOrig;
            lowHealth = false;
            gameManager.instance.getHealthWarning().SetActive(false);
            updatePlayerUI();
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
        //crouching
        

        // Movement Controller
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);
       

        // Stamina Recovery
        if (stamina < staminaOrig && !isSprinting && !isRecovering)
            StartCoroutine(staminaRecover());

        // Jump Controller
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
        {
            jumpCounter++;
            playerVel.y = jumpSpeed;

            // Play Jump Sound
            aud.PlayOneShot(audioManager.instance.audJump[Random.Range(0, audioManager.instance.audJump.Length)], audioManager.instance.audJumpVol);
        }

        // Gravity Controller
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Check for Enemy (Reticle)
        if (guns.Count > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, bulletDistance, ~ignoreLayer))
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
        aud.PlayOneShot(audioManager.instance.audSteps[Random.Range(0, audioManager.instance.audSteps.Length)], audioManager.instance.audStepVol);

        // Check if the player is sprinting and play the sound faster if so
        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isStepping = false;
    }

    void shootGun() {
        // Reload
        // Check if the player is not shooting & if they pressed the reload button mapped in input manager
        // Also have an additional check if they aren't at max already.
        if (!isShooting && Input.GetButton("Reload") && guns[gunPos].ammoCur < guns[gunPos].ammoMag)
        {
            reload();
        } 
        // Have an additional check if the player has no ammo to auto-reload.
        else if (guns[gunPos].ammoCur == 0 && guns[gunPos].ammoMax > 0)
        {
            reload();
        }

        if (getCurGun().isAutomatic) {
            if (Input.GetButton("Fire1") && !isShooting && !gameManager.instance.getPauseStatus()) {
                if (getAmmo() > 0) {
                    StartCoroutine(shoot());
                    Instantiate(playerShot, Camera.main.transform.position, Camera.main.transform.rotation); 
                    aud.PlayOneShot(guns[gunPos].shootSound[Random.Range(0, guns[gunPos].shootSound.Length)], guns[gunPos].audioVolume); // Play the gun's shoot sound
                }
                // Another check for the player to auto-reload if they attempt to shoot with nothing in their barrel.
                else if (guns[gunPos].ammoCur == 0 && guns[gunPos].ammoMax > 0)
                {
                    reload();
                }
                else {
                    StartCoroutine(AmmoWarningFlash());
                }
            }
        }
        else {
            if (Input.GetButtonDown("Fire1") && !isShooting && !gameManager.instance.getPauseStatus()) {
                if (getAmmo() > 0) {
                    StartCoroutine(shoot());
                    if (isLaser) { // laser isn't automatic so only needs to be in this if statement
                        Instantiate(laserShot, shotFlash.transform.position, Camera.main.transform.rotation);
                    }
                    else if (isShotgun) { // shotgun isn't automatic so only needs to be in this if statement
                        Instantiate(shotgunShot, shotFlash.transform.position, Camera.main.transform.rotation); // object to make, position (var Transform), direction object is facing
                    }
                    else {
                        Instantiate(playerShot, shotFlash.transform.position, Camera.main.transform.rotation);
                    }
                    aud.PlayOneShot(guns[gunPos].shootSound[Random.Range(0, guns[gunPos].shootSound.Length)], guns[gunPos].audioVolume); // Play the gun's shoot sound
                }
                else {
                    StartCoroutine(AmmoWarningFlash());
                }
            }
        }
    }

    void reload()
    {
        // Gun is not at full, so reload

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

    // Sprint Movement Func
    void sprint()
    {
        // TODO: Make sprinting only happen on the ground. I commented out the check for the isGrounded because this
        // is still buggy & needs to be properly implemented into the checks. Ask Adam for help?
        //if (controller.isGrounded)
        //{
            // Check if the player has stamina to sprint
            if (Input.GetButtonDown("Sprint") && stamina > 0)
            {
                speed *= speedMod;
                isSprinting = true;
            }
            else if (stamina <= 0 && isSprinting)
            {
                // Stop sprinting if shift is up or player doesn't have stamina.
                speed /= speedMod;
                isSprinting = false;
            }
            else if (Input.GetButtonUp("Sprint") && isSprinting)
            {
                // This is a preventative measure for a bug that permanently decreases the player speed if
                // they run out of stamina while running and let go of shift.
                speed /= speedMod;
                isSprinting = false;
            }
        //}
    }

    // Shoot Timer
    IEnumerator shoot()
    {
        // Set bool true at timer begin
        isShooting = true;
        StartCoroutine(shotFlashTimer());

        //Create Raycast
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, bulletDistance, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            guns[gunPos].ammoCur--;
            playerStats.Stats.gunShot();

            if (dmg != null)
            {
                dmg.takeDamage((damage * damageBuffMult));
                if (guns[gunPos].hitEffects != null)
                    Instantiate(guns[gunPos].hitEffects, hit.point, Quaternion.identity);
            }
            else if (hit.collider.GetComponent<bossRoom>() == true)
            {
                // Debug.Log("Button Hit");
                // I've (Bryan) decided to keep this as an easter egg.
                // Because of this, I added a check to see if the interactUI is on so then it'll turn off if the player shoots it.
                if (gameManager.instance.getInteractUI().activeInHierarchy)
                    gameManager.instance.getInteractUI().SetActive(false);

                gameManager.instance.openContinueMenu();


            }
            else if (guns[gunPos].hitEffects != null) { 
                Instantiate(guns[gunPos].hitEffects, hit.point, Quaternion.identity);
            }
        }
        else { guns[gunPos].ammoCur--; } //had to put this here, there's a bug this was causing where if the bullet isn't hitting anything, it doesn't use ammo.

        // Update the UI
        updatePlayerUI();

        // Time Between Shots
        yield return new WaitForSeconds(fireRate);

        // Set bool false at timer end
        isShooting = false;        
    }

    // Player Damage Controller
    public void takeDamage(float amount)
    {
        // Further prevention from additional damage that may trigger things like lives lost multiple times or negative HP values.
        if (HP > 0)
        {
            // Player takes damage
            HP -= amount;
            playerStats.Stats.attacked(amount);
            stopHealing = true; // STOP HEALING IF DAMAGED

            // Play hurt sound for audio indication
            if (damageAudioReady)
            {
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
        AudioSource.PlayClipAtPoint(healSound, transform.position); // playing heal audio clip
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

    public void callBuff(int buff, Sprite icon) { // a seperate method is needed to call the Coroutine because if the pickup calls it directly it won't work after being destoryed
        if (buff == 1) {
            gameManager.instance.getBuffUI().SetActive(true);
            gameManager.instance.getBuffIcon().sprite = icon;
            StartCoroutine(damageBuff());
        }
        else if (buff == 2) {
            gameManager.instance.getBuffUI().SetActive(true);
            gameManager.instance.getBuffIcon().sprite = icon;
            StartCoroutine(healBuff());
        }
        else if (buff == 3) { // shield
            gameManager.instance.getBuffUI().SetActive(true);
            gameManager.instance.getBuffIcon().sprite = icon;

        }
        else if (buff == 4) {
            gameManager.instance.getBuffUI().SetActive(true);
            gameManager.instance.getBuffIcon().sprite = icon;
            StartCoroutine(staminaBuff());
        }
    }

    IEnumerator damageBuff() {
        damageBuffMult = 1.5f;
        yield return new WaitForSeconds(10);
        damageBuffMult = 1;
        gameManager.instance.getBuffUI().SetActive(false);
    }

    IEnumerator healBuff() {
        for (int i = 1; i <= 5; i++) {
            Heal(true);
            updatePlayerUI();
            yield return new WaitForSeconds(1);
        }
        gameManager.instance.getBuffUI().SetActive(false);
    }

    IEnumerator staminaBuff() {
        infiniteStam = true;
        yield return new WaitForSeconds(5);
        infiniteStam = false;
        gameManager.instance.getBuffUI().SetActive(false);
    }

    // Drain stamina as player runs
    IEnumerator staminaDrain() {
        if (infiniteStam == false) {
            isDraining = true;
            stamina--;
            updatePlayerUI();
            yield return new WaitForSeconds(drainMod);
            isDraining = false;
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

    // Update information on the UI
    public void updatePlayerUI() 
    {
        // Health Info
        gameManager.instance.getHPBar().fillAmount = HP / HPOrig;
        gameManager.instance.getHPText().text = HP.ToString("F0");

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
            playerStats.Stats.levelUp();
            playerXP -= playerXPMax; // Reset XP back to zero
            gameManager.instance.getLevelTracker().text = playerLevel.ToString("F0");
            
            // Run again if the player has enough remaining XP.
            if (playerXP >= playerXPMax)
                levelTracker();
        }
    }

    public void ammoPickup(int amount) {
        // Ammo pickups will add ammo to the player's shooting if possible.
        // If it goes over the mag limit, it'll be added to max instead.
        if (guns.Count > 0) {
            int _ammo = (int)(getAmmoOrig() * (amount / 100f));
            // Check if the player's ammo reserve isn't already full
            if (getCurGun().ammoMax < getCurGun().ammoOrig) {
                // Check if the ammo will go over mag size
                if (_ammo + getCurGun().ammoCur >= getAmmoMag()) {
                    // Does go over size, will be adding to reserve instead.
                    // Check if adding to reserve will hit capacity.
                    if (_ammo + getCurGun().ammoMax >= getAmmoOrig()) {
                        // Add any remaining ammo to mag size as long as it doesn't go over mag
                        _ammo -= getAmmoOrig() - getCurGun().ammoMax;
                        // Will hit capacity, set max to capacity.
                        getCurGun().ammoMax = getAmmoOrig();

                        if (_ammo > 0) {
                            // Check if it'll go over magazine size
                            if (_ammo + getCurGun().ammoCur > getCurGun().ammoMag) {
                                getCurGun().ammoCur = getAmmoMag();
                            }
                            else {
                                // It will not, so just add the leftover ammo.
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
            else {
                getCurGun().ammoCur = getCurGun().ammoMag; // player's reserve is full, set clip to mag size.
            }
        }
        updatePlayerUI(); // Update the UI to show ammo has been restored
    }

    public void toggleSprintOn()
    {
        toggleSprint = !toggleSprint;

        speedOrig = (int) speed;

        // if (toggleSprint == true)   //tried while loop but it broke it
        //{
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
                controller.center = new Vector3(0, crouchHeight / 2, 0);  
                playerCamera.localPosition = new Vector3(0, crouchHeight / 2, 0);  
               speed = speedOrig / 2f;  // Reduce speed while crouching
            }
            else
            {
                controller.height = normalHeight;  
                controller.center = new Vector3(0, normalHeight / 2, 0);  
                playerCamera.localPosition = new Vector3(0, normalHeight / 2, 0);  // Reset camera position
               speed = speedOrig;  // Restore original speed
            }
        }
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
            AudioSource.PlayClipAtPoint(grenadeStats.pinSound, grenadeInstance.transform.position); // playing audio for taking grenade pin out

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

    IEnumerator HandleGrenadeExplosion(GameObject grenadeInstance)
    {
        yield return new WaitForSeconds(grenadeStats.explosionDelay); //explosion delay

        if(grenadeStats.explosionEffect != null) // trigger explosion effect
        {
            Instantiate(grenadeStats.explosionEffect,grenadeInstance.transform.position, Quaternion.identity);
        }

        if (CameraShake.instance != null)
        {
            // Trigger the shake with intensity 0.7 and duration 0.3 seconds
            CameraShake.instance.TriggerShake(0.7f, 0.3f);
        }
        else
        {
            Debug.LogWarning("CameraShake instance not found.");
        }
        Collider[] colliders = Physics.OverlapSphere(grenadeInstance.transform.position,grenadeStats.explosionRadius); // damage thing around in explosion
        foreach(Collider nearbyObject in colliders)
        {
            IDamage damageable = nearbyObject.GetComponent<IDamage>();
            if(damageable != null)
            {
                damageable.takeDamage(grenadeStats.explosionDamage); // grenade does double damage to player

                
                BurningEffect burningEffect = nearbyObject.gameObject.AddComponent<BurningEffect>();
                burningEffect.duration = 5f;  // Set the duration for the burn effect
                burningEffect.ApplyEffect(nearbyObject.gameObject);

            }
        }

        AudioSource.PlayClipAtPoint(grenadeStats.explosionSound,grenadeInstance.transform.position);
        Destroy(grenadeInstance);
    }


    //Working out null ref bug...put on pause for time being

    //IEnumerator PlayerMelee()
    //{
    //    Vector3 currGunPos = getCurGun().GameObject().transform.position;
    //    Vector3 meleePos = new Vector3(getCurGun().GameObject().transform.position.x, getCurGun().GameObject().transform.position.y , getCurGun().GameObject().transform.position.z + 3f);


    //        if (guns[gunPos].GetComponentInChildren<BoxCollider>() != null)
    //        {
    //        Debug.Log("Melee Ready");
    //            getCurGun().GameObject().transform.forward += Vector3.Lerp(currGunPos, meleePos, 1f);
    //            yield return new WaitForSeconds(2);
    //            getCurGun().GameObject().transform.position = currGunPos;
    //        }
    //        else
    //            yield return new WaitForSeconds(2);

    //}

    IEnumerator HealPlayer() { // x
        if (HP != HPOrig) {
            isHealing = true;

            HP = Mathf.Min(HP + heals[0].healAmount, HPOrig);
            StartCoroutine(healIndicator()); // flashing screen green

            if (heals[0].healSound != null) {
                AudioSource.PlayClipAtPoint(heals[0].healSound, transform.position);
            }
            healCoolDown = heals[0].healCoolDown;
            gameManager.instance.getHealsUI().text = (heals.Count-1).ToString(); // -1 because it hasn't been removed yet
            updatePlayerUI();

            yield return new WaitForSeconds(heals[0].healCoolDown);
            isHealing = false;
            heals.Remove(heals[0]);

            if ((HP / HPOrig) > .25) {
                lowHealth = false;
                gameManager.instance.getHealthWarning().SetActive(false); // getting rid of low health state if not low anymore
            }
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
        bulletDistance = guns[gunPos].bulletDist;
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
        bulletDistance = _gun.bulletDist;
        //ammoOrig = _gun.ammoMax;
        isSniper = _gun.isSniper;
        isLaser = _gun.isLaser;
        isShotgun = _gun.isShotgun;
        if (isLaser) {
            laserSight.enabled = true;
        }
        else {
            laserSight.enabled = false;
        }

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

    public AudioSource getAudio()
    {
        return aud;
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

        // Update the UI to show it's been changed
        updatePlayerUI();
    }

    public void setAmmoMax(int newAmmoMax)
    {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoMax = newAmmoMax;

        // Update the UI to show it's been changed
        updatePlayerUI();
    }

    public void setAmmoOrig(int newAmmoOrig) {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoOrig = newAmmoOrig;

        // Update the UI to show it's been changed
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
        coins = newCoins;}

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
}
