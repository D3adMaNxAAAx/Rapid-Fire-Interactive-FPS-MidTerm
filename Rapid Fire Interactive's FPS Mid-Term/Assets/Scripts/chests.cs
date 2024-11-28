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
    [SerializeField] GrenadeStats grenade;
    [SerializeField] HealStats heal;
    [SerializeField] int coins;
    [SerializeField] int ammo;
    [SerializeField] int grenades;
    [SerializeField] int heals;
    [SerializeField] int shieldOdds;
    [SerializeField] int staminaOdds;
    [SerializeField] int healOdds;
    [SerializeField] int attackOdds;

    // Generic Stats
    collectablePickup powerUp;

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
    }

    public void interact()
    {
        // Play the animation of the chest opening
        openChest();
        // Award loot
    }

    public void giveLoot()
    {
        // Give player loot
        gameManager.instance.getPlayerScript().setCoins(coins);
        gameManager.instance.getPlayerScript().addAmmo(ammo);
        gameManager.instance.getPlayerScript().addToGrenades(grenade);
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
        audioManager.instance.PlaySound(audioManager.instance.audChestOpen, audioManager.instance.audChestOpenVol);
        gameManager.instance.getInteractUI().SetActive(false);
    }
    
    private void OnTriggerStay(Collider other)
    {
        // Enable Interact Menu
        if (other.CompareTag("Player"))
        {
            if (!gameManager.instance.getInteractUI().activeInHierarchy && !isOpen)
            {
                gameManager.instance.getInteractUI().SetActive(true);
            }

            if (Input.GetButton("Interact") && !isOpen)
            {
                interact();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable Interact Menu
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.getInteractUI().activeInHierarchy)
            {
                gameManager.instance.getInteractUI().SetActive(false);
            }
        }
    }

}
