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
    // -----MODIFIABLE VARIABLES-----
    // Unity object fields
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

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

    // Checks
    bool isSprinting;
    bool isShooting;
    bool isDraining; // To check if the player is currently losing stamina
    bool isRecovering; // To check if the player is currently recovering stamina
    bool lowHealth;

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
        // Check if sprinting -- Drain stamina as the player runs
        if (isSprinting && !isDraining)
            StartCoroutine(staminaDrain());
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

        // Shoot Controller
        if (Input.GetButtonDown("Fire1") && !isShooting && !gameManager.instance.getPauseStatus())
        {
            if (ammo > 0) {
                StartCoroutine(shoot());
            } else
            {
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

    // Shoot Timer
    IEnumerator shoot()
    {
        //Set bool true at timer begin
        isShooting = true;

        // Decrement ammo count
        //ammo--;

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
                Debug.Log("Button Hit");
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
        updatePlayerUI();
        StartCoroutine(damageFlash());

        //On Player Death
        if(HP <= 0) {
            gameManager.instance.youLose();
        }
        else if (((float)HP / HPOrig) <= .25) { // float to divsion returns decimal
            lowHealth = true;
            gameManager.instance.getHealthWarning().SetActive(true);
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
