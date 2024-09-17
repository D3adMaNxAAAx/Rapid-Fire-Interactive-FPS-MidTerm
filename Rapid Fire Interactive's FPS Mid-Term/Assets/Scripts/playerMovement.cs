using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

//------------NOTES-------------
/*For this whole code to work IDamage and gameManager scripts must both be functional.
 After completing scripts please uncomment ONLY that part of the script!!!
 Each one will be labeled as //IDamage or //gameManager at the end of each line of code or function.*/
public class playerMovement : MonoBehaviour, IDamage 
{

    /// maybe add something to UI to indicate healing

    // -----MODIFIABLE VARIABLES-----
    // Unity object fields
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] GameObject playerShot;

    // Player modifiers
    // -- Attributes --
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int stamina;
    [SerializeField] int playerXPMax;

    // -- Movement --
    [SerializeField] int speedMod;
    [SerializeField] int maxJumps;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] float drainMod; // How quickly stamina points drain
    [SerializeField] float recoveryMod; // How quickly stamina points recovers
    [SerializeField] bool toggleSprint;  //Turns sprint on or off

    // -- Timer --
    [SerializeField] float dmgFlashTimer;
    [SerializeField] float ammoWarningTimer;
    
    // Player Default Weapon Mods
    [SerializeField] int damage;
    [SerializeField] float fireRate;
    [SerializeField] float bulletDistance;
    [SerializeField] int ammo;
    //[SerializeField] float bulletSpeed; //Is here if we wanna change to use bullets

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
    int update = 0;

    // Checks
    bool isSprinting;
    bool isShooting;
    bool isDraining; // To check if the player is currently losing stamina
    bool isRecovering; // To check if the player is currently recovering stamina
    bool lowHealth = false;
    bool readyToHeal = false;
    bool stopHealing = false; // was the player damaged while healing?
    bool onDashCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        staminaOrig = stamina;
        ammoOrig = ammo;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        Dash();

        // Check if sprinting -- Drain stamina as the player runs
        if (isSprinting && !isDraining)
            StartCoroutine(staminaDrain());

        update++;
        if (update == 60) { // slowing down constant checks
            update = 0;
            if (((float)HP / HPOrig) < .5) { // HEALING STARTS HERE, READY TO HEAL DETERMINATION STARTS IN TAKEDAMAGE()
                if (readyToHeal) {
                    StartCoroutine(healing()); // recursive method
                    readyToHeal = false; // stop healing
                }
            }
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
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
        {
            jumpCounter++;
            playerVel.y = jumpSpeed;
        }

        // Gravity Controller
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        // Check for Enemy (Reticle)
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, bulletDistance, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                Vector2 dangerSize = new Vector2(15, 15);
                gameManager.instance.changeReticle(true);
            } 
        }
        else
        {
            gameManager.instance.changeReticle(false);
        }

        shootGun();
    }

    void shootGun() {
        if (Input.GetButtonDown("Fire1") && !isShooting && !gameManager.instance.getPauseStatus()) {
            if (ammo > 0) {
                StartCoroutine(shoot());
                Instantiate(playerShot, Camera.main.transform.position, Camera.main.transform.rotation);
            }
            else {
                StartCoroutine(AmmoWarningFlash());
            }
        }
    }

    // Sprint Movement Func
    void sprint()
    {
        // Check if the player has stamina to sprint
        if(Input.GetButtonDown("Sprint") && stamina > 0)
        {
            speed *= speedMod;
            isSprinting = true;
        } else if (stamina <= 0 && isSprinting)
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
    }

    void Dash() {
        if (onDashCooldown == false) {
            //The below input needs to be changed, this is really bad coding practice to force a specific key, we want a designer to be able to change the keybind in unity, and if a player was givin the ability to
            //change keybinds, they would not be able to change this because it is hard coded in. Suggestion, create a keybind in the input manager labeled "Dash", and use "if (Input.GetButtonDown("Dash"))" This will
            //solve this issue, and be great practice for the real world.
            if (Input.GetKey(KeyCode.F)) {
                controller.Move(transform.forward * 5);
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
        //Set bool true at timer begin
        isShooting = true;

        // Decrement ammo count
        //ammo--; //We completely understand this doesn't need to be here now, but before it did, because when you shot you would want to decriment the amount of bullets when you shoot wether or not you hit something
        //but now it's not necessary because we also need to check wether or not we are clicking a button with our raycast, although I would like to see if we can change that to an "interact" button. 
        //from now on instead of leaving comments on code like "why is this here", ask the group, we can give you an answer.
        ///Reminder: remove this in a couple of days.

        //Create Raycast
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, bulletDistance, ~ignoreLayer))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(damage);
                --ammo;
            }
            else if (hit.collider.GetComponent<nextRoom>() == true)
            {
                // Debug.Log("Button Hit");
                gameManager.instance.completeMenu();
            }
            else { --ammo; }
        }

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
        HP -= amount;
        stopHealing = true; // STOP HEALING IF DAMAGED
        updatePlayerUI();
        StartCoroutine(damageFlash());

        //On Player Death
        if(HP <= 0) {
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
        gameManager.instance.getHPBar().fillAmount = (float)HP / HPOrig;
        gameManager.instance.getStamBar().fillAmount = (float)stamina / staminaOrig;
        gameManager.instance.getAmmoBar().fillAmount = (float)ammo / ammoOrig;
        gameManager.instance.getXPBar().fillAmount = (float)playerXP / playerXPMax;
    }

    public int getXP()
    {
        return playerXP;
    }

    public void setXP(int amount)
    {
        playerXP += amount;
        levelTracker(); // Check if the player can level up
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
            // TODO: HAVE TRAILING ZEROS FOR THE LEVELS
        }
    }

    // Getters
    public int getPlayerLevel()
    {
        return playerLevel;
    }

    // Setters
    public void setPlayerLevel(int _playerLevel)
    {
        _playerLevel = playerLevel;
    }
}
