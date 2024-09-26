using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

//------------NOTES-------------
/* For this whole code to work IDamage and gameManager scripts must both be functional.
   After completing scripts please uncomment ONLY that part of the script!!!
   Each one will be labeled as //IDamage or //gameManager at the end of each line of code or function. */
public class playerMovement : MonoBehaviour, IDamage
{

    public static playerMovement player; // singleton

    /// maybe add something to UI to indicate healing

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
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] int stamina;
    [SerializeField] int coins;
    [SerializeField] int playerXPMax;
    float damageUpgradeMod = 1;  // keep set = to 1, so damage can be upgraded (can just change damage var because it changes when swapping guns)

    // Player Default Weapon Mods
    [Header("-- Player Weapons --")]
    [SerializeField] List<gunStats> guns;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject shotFlash;

    bool isSniper = false;
    [SerializeField] int damage;
    [SerializeField] float fireRate;
    [SerializeField] float bulletDistance;
    //[SerializeField] int ammo;
    //[SerializeField] float bulletSpeed; // Is here if we wanna change to use bullets
    [SerializeField] GameObject grenade;
    [SerializeField] GrenadeStats grenadeStats;
    [SerializeField] int totalGrenades = 3;

    //player heals 
    [SerializeField] HealStats healItem;
    [SerializeField] int healItemCount = 3;
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

    // -- Timer --
    [Header("-- Timers --")]
    [SerializeField] float dmgFlashTimer;
    [SerializeField] float ammoWarningTimer;

    // -----PRIVATE VARIABLES-----
    // Player movement variables
    Vector3 moveDir;
    Vector3 playerVel;

    // Count number of jumps
    int jumpCounter;

    // Trackers
    int HPOrig; // HP
    int playerXP; // XP
    int staminaOrig; // Stamina
    int playerLevel; // Level
    int ammoOrig; // Ammo
    int gunPos = 0; // Weapon selected
    int speedOrig;  // Original Speed

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
    // Start is called before the first frame update
    void Start() {
        // Variable Initialization
        player = this;
        HPOrig = HP;
        staminaOrig = stamina;

        // Update Player Information & Spawn
        updatePlayerUI();
        if (gameManager.instance.getPlayerSpawnPos() != null)
            spawnPlayer();
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.instance.getPauseStatus() == false) {
            movement();
            StartCoroutine(Dash());

            if (readyToHeal) {  // HEALING STARTS HERE, READY TO HEAL DETERMINATION STARTS IN TAKEDAMAGE()
                StartCoroutine(healing());  // recursive method
                readyToHeal = false;  // stop healing
            }
        }

        sprint();
        // Check if sprinting -- Drain stamina as the player runs
        if (isSprinting && !isDraining)
            StartCoroutine(staminaDrain());

        if (guns.Count != 0)
            selectGun();

        if (Input.GetButtonDown("ThrowGrenade") && totalGrenades > 0)
        {
            throwGrenade();
        }
        if (Input.GetButtonDown("Heal Item") && healItemCount > 0 && healCoolDown <= 0f && !isHealing)
        {
            StartCoroutine(HealPlayer());
        }
        if(healCoolDown > 0f)
        {
            healCoolDown -= Time.deltaTime;
        }

    }

    public void spawnPlayer()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.getPlayerSpawnPos().transform.position;
        controller.enabled = true;

        HP = HPOrig;
        gameManager.instance.getHealthWarning().SetActive(false);
        updatePlayerUI();
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

    void shootGun() {
        if (getCurGun().isAutomatic)
        {
            if (Input.GetButton("Fire1") && !isShooting && !gameManager.instance.getPauseStatus())
            {
                if (getAmmo() > 0)
                {
                    StartCoroutine(shoot());
                    Instantiate(playerShot, Camera.main.transform.position, Camera.main.transform.rotation);
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
                    Instantiate(playerShot, Camera.main.transform.position, Camera.main.transform.rotation);
                }
                else
                {
                    StartCoroutine(AmmoWarningFlash());
                }
            }
        }
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

    IEnumerator Dash() {
        if (Input.GetButtonDown("Dash")) {
            if (onDashCooldown == false) {
                controller.Move(transform.forward * 1.5f);
                yield return new WaitForSeconds(0.05f);
                controller.Move(transform.forward * 1.5f);
                yield return new WaitForSeconds(0.05f);
                controller.Move(transform.forward * 1.5f);
                yield return new WaitForSeconds(0.05f);
                controller.Move(transform.forward * 1.5f);
                yield return new WaitForSeconds(0.05f);
                controller.Move(transform.forward * 1.5f);
                StartCoroutine(dashCooldown()); // can't dash again for 3 seconds
            }
        }
    }

    IEnumerator dashCooldown() {
        onDashCooldown = true;
        yield return new WaitForSeconds(3);
        onDashCooldown = false;
    }

    // Shoot Timer
    IEnumerator shoot()
    {
        // Set bool true at timer begin
        isShooting = true;
        StartCoroutine(shotFlashTimer());
        // Play the gun's shoot sound
        aud.PlayOneShot(guns[gunPos].shootSound[Random.Range(0, guns[gunPos].shootSound.Length)], guns[gunPos].audioVolume);

        //Create Raycast
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, bulletDistance, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(damage);
                guns[gunPos].ammoCur--;
                if (guns[gunPos].hitEffects != null)
                    Instantiate(guns[gunPos].hitEffects, hit.point, Quaternion.identity);
            }
            else if (hit.collider.GetComponent<bossRoom>() == true)
            {
                // Debug.Log("Button Hit");
                gameManager.instance.completeMenu();
            }
            else { guns[gunPos].ammoCur--;
                if (guns[gunPos].hitEffects != null)
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
    public void takeDamage(int amount)
    {
        // Player takes damage
        HP -= amount;
        stopHealing = true; // STOP HEALING IF DAMAGED

        // Play hurt sound for audio indication
        aud.PlayOneShot(audioManager.instance.audHurt[Random.Range(0, audioManager.instance.audHurt.Length)], audioManager.instance.audHurtVol);

        // Update UI & Flash Screen red
        updatePlayerUI();
        StartCoroutine(damageFlash());

        // On Player Death
        if (HP <= 0) {
            HP = 0; // set HP to 0 for no weirdness in code/visuals
            gameManager.instance.youLose();
        }
        else if (((float)HP / HPOrig) <= .25) { 
            lowHealth = true;
            gameManager.instance.getHealthWarning().SetActive(true);
        }
        if (((float)HP / HPOrig) < .5) { 
            StartCoroutine(noDamageTime()); // TIMER FOR WHEN PLAYER CAN START TO HEAL
        }
    }

    IEnumerator shotFlashTimer() {
        shotFlash.SetActive(true);
        yield return new WaitForSeconds(0.075f);
        shotFlash.SetActive(false);
    }

    public void Heal() {
        HP += 3; // HEAL THE PLAYER
        if (lowHealth == true) {
            if (((float)HP / HPOrig) > .25) { 
                lowHealth = false;
                gameManager.instance.getHealthWarning().SetActive(false); // getting rid of low health state if not low anymore
            }
        }
        if (((float)HP / HPOrig) > .5) { // only heals to half HP
            HP = (HPOrig / 2); // if HP goes over half, reset to half
            readyToHeal = false; // HEALING OVER
        }
        else {
            StartCoroutine(healing()); // KEEP HEALING IF NOT AT HALF HP (RECURSION)
        }
    }

    IEnumerator healing() {
        if (stopHealing == false) { // stopping healing if interupted
            yield return new WaitForSeconds(1);
            Heal();
            updatePlayerUI(); // updating health bar
        }
    }

    IEnumerator noDamageTime() { // time till healing (if no damage was taken)
        int currentHP = HP;
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

    // Flash No Ammo
    IEnumerator AmmoWarningFlash()
    {
        gameManager.instance.getAmmoWarning().SetActive(true); //gameManager
        yield return new WaitForSeconds(ammoWarningTimer);
        gameManager.instance.getAmmoWarning().SetActive(false); //gameManager
    }

    // Drain stamina as player runs
    IEnumerator staminaDrain()
    {
        isDraining = true;
        stamina--;
        updatePlayerUI();
        yield return new WaitForSeconds(drainMod);
        isDraining = false;
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
        gameManager.instance.getHPBar().fillAmount = (float)HP / HPOrig;
        gameManager.instance.getHPText().text = HP.ToString("F0");

        // Stamina Info
        gameManager.instance.getStamBar().fillAmount = (float)stamina / staminaOrig;
        gameManager.instance.getStamText().text = stamina.ToString("F0");

        // Attack Info
        if (guns.Count > 0)
        {
            gameManager.instance.getAmmoBar().fillAmount = (float)getCurGun().ammoCur / getCurGun().ammoMax;
            gameManager.instance.getAmmoText().text = getAmmo().ToString("F0") + " / " + getAmmoOrig().ToString("F0");
        }

        // XP Info
        gameManager.instance.getXPBar().fillAmount = (float)playerXP / playerXPMax;
        gameManager.instance.getXPText().text = playerXP.ToString("F0") + " / " + playerXPMax.ToString("F0");
    }
    
    // Tracks the player's xp and levels them up
    public void levelTracker()
    {
        // Check if player XP meets requirement to level up (XP Max)
        if (playerXP >= playerXPMax && playerLevel < 999)
        {
            playerLevel++;
            playerXP = 0; // Reset XP back to zero
            gameManager.instance.getLevelTracker().text = playerLevel.ToString("F0");
        }
    }

    

    public void ammoPickup(int amount)
    {
        if (guns.Count > 0)
        {
            int a = (int)(getAmmoOrig() * (amount / 100f));
            if (a + getCurGun().ammoCur >= getAmmoOrig())
                getCurGun().ammoCur = getAmmoOrig();
            else
                getCurGun().ammoCur += a;
        }
        updatePlayerUI();
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

            //if (stamina >= (staminaOrig / 2))
            //{

            ////toggleSprintOn();
            //    speed *= speedMod;
            //    isSprinting = true;
            //    staminaDrain();
            //}

        //}
        updatePlayerUI();
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            gunPos++;
            if (gunPos == guns.Count)
                gunPos = 0;
            changeGun();
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            gunPos--;
            if (gunPos < 0)
                gunPos = guns.Count - 1;
            changeGun();
        }
    }

    void changeGun()
    {
        float damTemp = guns[gunPos].damage * damageUpgradeMod;
        damage = (int)damTemp;
        bulletDistance = guns[gunPos].bulletDist;
        fireRate = getCurGun().fireRate;
        isSniper = getCurGun().isSniper;
        updatePlayerUI();

        gunModel.GetComponent<MeshFilter>().sharedMesh = getCurGun().gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = getCurGun().gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void throwGrenade()
    {
        if (grenade == null)
        {
            grenade = grenadeStats.grenadeModel;
        }
        if (totalGrenades > 0)
        {
            GameObject grenadeInstance = Instantiate(grenade, transform.position + transform.forward, Quaternion.identity);

            Rigidbody rb = grenadeInstance.GetComponent<Rigidbody>(); //throw force 
            if (rb != null)
            {
                rb.AddForce(Camera.main.transform.forward * grenadeStats.throwForce, ForceMode.VelocityChange);
            }

            StartCoroutine(HandleGrenadeExplosion(grenadeInstance));// start explosion count down 

            totalGrenades--;
        }
    }

    IEnumerator HandleGrenadeExplosion(GameObject grenadeInstance)
    {
        yield return new WaitForSeconds(grenadeStats.explosionDelay); //explosion delay

        if(grenadeStats.explosionEffect != null) // trigger explosion effect
        {
            Instantiate(grenadeStats.explosionEffect,grenadeInstance.transform.position, Quaternion.identity);
        }
       
        Collider[] colliders = Physics.OverlapSphere(grenadeInstance.transform.position,grenadeStats.explosionRadius); // damage thing around in explosion
        foreach(Collider nearbyObject in colliders)
        {
            IDamage damageable = nearbyObject.GetComponent<IDamage>();
            if(damageable != null)
            {
                damageable.takeDamage(grenadeStats.explosionDamage);
            }
        }

        if(grenadeStats.explosionSound != null) // play explosion sound
        {
            AudioSource.PlayClipAtPoint(grenadeStats.explosionSound,grenadeInstance.transform.position);
        }

        Destroy(grenadeInstance);
    }

    IEnumerator HealPlayer()
    {
        isHealing = true;

        HP = Mathf.Min(HP + healItem.healAmmount,HPOrig);
        healItemCount--;

        if(healItem.healSound != null)
        {
            AudioSource.PlayClipAtPoint(healItem.healSound, transform.position);
        }
        updatePlayerUI();

        yield return new WaitForSeconds(healItem.healCoolDown);
       
        isHealing = false;

        healCoolDown = healItem.healCoolDown;
    }

    // temporary check if the player has a gun -- can remove later, using for debug purposes
    public bool hasGun()
    {
        if (guns.Count != 0)
            return true;
        else
            return false;
    }

    IEnumerator playFootsteps()
    {
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

    // -- GETTERS --
    public void getGunStats(gunStats _gun)
    {
        _gun.ammoCur = _gun.ammoMax;
        guns.Add(_gun);
        gunPos = guns.Count - 1;
        updatePlayerUI();

        float damTemp = _gun.damage * damageUpgradeMod; //reason I did this is because we can't supply a float value to an int.
        damage = (int)damTemp; //so then we can cast it back as an int so we aren't using decimals for damage on enemies.
        fireRate = _gun.fireRate;
        bulletDistance = _gun.bulletDist;
        //ammoOrig = _gun.ammoMax;
        isSniper = _gun.isSniper;

        gunModel.GetComponent<MeshFilter>().sharedMesh = _gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = _gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public int getHP()
    {
        return HP;
    }

    public int getHPOrig()
    {
        return HPOrig;
    }

    public int getStamina()
    {
        return stamina;
    }

    public int getStaminaOrig()
    {
        return staminaOrig;
    }

    public int getXP()
    {
        return playerXP;
    }

    public int getAmmo()
    {
        return getCurGun().ammoCur;
    }

    public int getAmmoOrig()
    {
        return getCurGun().ammoMax;
    }

    public bool getIsSniper()
    {
        return isSniper;
    }

    public GameObject getGunModel()
    {
        return gunModel;
    }

    public float getSpeed()
    {
        return speed;
    }
    
    public int getCoins()
    {
        return coins;
    }

    // Setters
    public void setHP(int newHP)
    {
        HP = newHP;
    }
    public void setHPOrig(int newHPOrig)
    {
        HPOrig = newHPOrig;
    }
    public void setAmmo(int newAmmo)
    {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoCur = newAmmo;

        // Update the UI to show it's been changed
        // (might not be necessary?)
        updatePlayerUI();
    }
    public void setAmmoOrig(int newAmmoOrig)
    {
        // Check if the player has a gun
        if (guns != null || guns.Count != 0)
            getCurGun().ammoMax = newAmmoOrig;

        // Update the UI to show it's been changed
        // (might not be necessary?)
        updatePlayerUI();
    }
    public int getPlayerLevel()
    {
        return playerLevel;
    }

    public gunStats getCurGun()
    {
        return guns[gunPos];
    }

    // -- SETTERS --
    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    public void setStamina(int newStamina)
    {
        stamina = newStamina;
    }

    public void setCoins(int newCoins)
    {
        coins = newCoins;
    }

    public void setDamageMod(float newDamageMod)
    {
        damageUpgradeMod = newDamageMod;
    }

    public void setXP(int amount)
    {
        playerXP += amount;
        levelTracker(); // Check if the player can level up
    }

    public void setPlayerLevel(int _playerLevel)
    {
        _playerLevel = playerLevel;
    }





    public List<gunStats> getGunList() 
        { return guns; }
    public void setGunList(List<gunStats> _list)
        { guns = _list; }
}
