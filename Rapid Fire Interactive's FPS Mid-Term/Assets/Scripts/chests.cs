using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class chests : MonoBehaviour, IInteractable
{
    [Header("-- Chest Information --")]
    [SerializeField] Animator anim;
    [SerializeField] collectablePickup shieldBuff;
    [SerializeField] collectablePickup staminaBuff;
    [SerializeField] collectablePickup healBuff;
    [SerializeField] collectablePickup attackBuff;
    [SerializeField] int coins;
    [SerializeField] int xp;
    [SerializeField] int ammo;
    [SerializeField] int grenades;
    [SerializeField] int heals;
    [SerializeField] int shieldOdds;
    [SerializeField] int staminaOdds;
    [SerializeField] int healOdds;
    [SerializeField] int attackOdds;

    // Generic Stats
    collectablePickup powerUp;
    GrenadeStats grenade;
    HealStats heal;

    // Tracks
    bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        // Randomly decide which buff should be in the chest
        int randomDropNum = Random.Range(0, 100); // if this num is less then dropChance, drop will happen

        if (attackOdds < randomDropNum) // 85
        {
            powerUp = attackBuff;
        } else if (healOdds < randomDropNum) // 65
        {
            powerUp = healBuff;
        } else if (staminaOdds < randomDropNum) // 40
        {
            powerUp = staminaBuff;
        } else if (shieldOdds < randomDropNum) // 0
        {
            powerUp = shieldBuff;
        }

        /* Visualizer (Collapsible):
        85 < 95, or 95 > 85 -- Passes Attack
        85 < 69, or 69 < 85 -- Fails Attack
        65 < 75, or 75 > 65 -- Passes Heal
        65 < 63, or 63 < 65 -- Fails Heal
        40 < 53, or 53 > 40 -- Passes Stamina
        40 < 39, or 39 < 40 -- Fails Stamina
        0 < 15, or 15 > 0 -- Passes Shield
        0 < 34, or 34 > 0 -- Passes Shield */
    }

    public void interact()
    {
        // Play the animation of the chest opening
        openChest();

        // Award loot
        // giveLoot() moved to the Animation Events of the Press Animation for the chest.
    }

    public void giveLoot()
    {
        // Give player loot
        gameManager.instance.getPlayerScript().setCoins(gameManager.instance.getPlayerScript().getCoins() + coins);
        gameManager.instance.getPlayerScript().setXP(xp); // THIS WILL ADD XP

        // Ammo
        gameManager.instance.getPlayerScript().addAmmo(ammo);

        // Grenades & Heals
        if (grenades != 0)
        {
            for (int i = 0; i < grenades; i++)
            {
                gameManager.instance.getPlayerScript().addToGrenades(grenade);
            }
        }

        if (heals != 0)
        {
            for (int i = 0; i < heals; i++)
            {
                gameManager.instance.getPlayerScript().addToHeals(heal);
            }
        }

        // Power Up
        Instantiate(powerUp, new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), Quaternion.identity);
        
        // Update UI
        gameManager.instance.getPlayerScript().updatePlayerUI();
    }

    // Method for triggering the chest to open
    void openChest()
    {
        isOpen = true;
        anim.CrossFade("Animated PBR Chest _Opening_UnCommon", 0.1f);
    }

    private void OnTriggerStay(Collider other)
    {
        // Enable Interact Menu
        if (!gameManager.instance.getInteractUI().activeInHierarchy)
        {
            gameManager.instance.getInteractUI().SetActive(true);
        }

        if (Input.GetButton("Interact") && !isOpen)
        {
            interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable Interact Menu
        if (gameManager.instance.getInteractUI().activeInHierarchy)
        {
            gameManager.instance.getInteractUI().SetActive(false);
        }
    }

}
