using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

//------------NOTES-------------
/*For this whole code to work IDamage and gameManager scripts must both be functional.
 After completing scripts please uncomment ONLY that part of the script!!!
 Each one will be labeled as //IDamage or //gameManager at the end of each line of code or function.*/
public class playerMovement : MonoBehaviour, IDamage 
{
    //-----MODIFIABLE VARIABLES-----
    //Unity object fields
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    //Player modiffiers
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int speedMod;
    [SerializeField] int maxJumps;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] float dmgFlashTimer;
    [SerializeField] int stamina;

    //Player default weapon mods
    [SerializeField] int damage;
    [SerializeField] float fireRate;
    [SerializeField] float bulletDistance;
    [SerializeField] int ammo;
    //[SerializeField] float bulletSpeed; //Is here if we wanna change to use bullets

    //-----PRIVATE VARIABLES-----
    //Player movement variables
    Vector3 moveDir;
    Vector3 playerVel;

    //Count number of jumps
    int jumpCounter;

    // Trackers
    int HPOrig; // HP
    int playerXP; // XP
    int staminaOrig; // Stamina
    int ammoOrig; // Ammo

    //Checks
    bool isSprinting;
    bool isShooting;

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
    }

    //Player Movement Controlls
    void movement()
    {
        //Check to see if the player is on the ground to zero out y velocity and zero out the jump counter
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCounter = 0;
        }

        //Movement Controller
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        //Jump Controller
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
        {
            jumpCounter++;
            playerVel.y = jumpSpeed;
        }

        //Gravity Controller
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        //Shoot Controller
        if(Input.GetButtonDown("Fire1") && !isShooting && !gameManager.instance.getPauseStatus())
        {
            if (ammo > 0) {
                StartCoroutine(shoot());
            } else
            {
                //TODO: NO AMMO FLASH.
            }
        }
    }

    //Sprint Movement Func
    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= speedMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= speedMod;
            isSprinting = false;
        }
    }

    //Shoot Timer
    IEnumerator shoot()
    {
        //Set bool true at timer begin
        isShooting = true;
        //Debug.Log("Bang!!");

        // Decrement ammo count
        ammo--;

        //Create Raycast
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, bulletDistance, ~ignoreLayer))
        {
            //Debug.Log("Hit");
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if(dmg != null)             //IDamage
            {
                dmg.takeDamage(damage);
            }
        }

        // Update the UI
        updatePlayerUI();

        //Time Between Shots
        yield return new WaitForSeconds(fireRate);

        //Set bool false at timer end
        isShooting = false;        
    }

    //Player Damage Controller
    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(damageFlash());

        //On Player Death
        if(HP <= 0)
        {
            //Debug.Log("I Died :(");
            gameManager.instance.youLose(); //gameManager
        }
    }

    //Damage Flash Timer
    IEnumerator damageFlash()
    {
        gameManager.instance.getDmgFlash().SetActive(true); //gameManager
        yield return new WaitForSeconds(dmgFlashTimer);
        gameManager.instance.getDmgFlash().SetActive(false); //gameManager
    }

    //Update UI HPBar
    public void updatePlayerUI() 
    {
        gameManager.instance.getHPBar().fillAmount = (float)HP / HPOrig;
        gameManager.instance.getStamBar().fillAmount = (float)stamina / staminaOrig;
        gameManager.instance.getAmmoBar().fillAmount = (float)ammo / ammoOrig;
    }

    public int getXP()
    {
        return playerXP;
    }

    public void setXP(int amount)
    {
        playerXP += amount;
    }
}
