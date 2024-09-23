using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class storeManager : MonoBehaviour
{
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;
    [SerializeField] int ammoType;

    // Player Stats
    int playerHP;
    int playerAmmo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void giveHealth()
    {
        // Heal the player to full as per their purchase
        gameManager.instance.getPlayerScript().setHP(gameManager.instance.getPlayerScript().getHPOrig());
    }

    void giveAmmo()
    {
        // Give the player max ammo as per their purchase
        gameManager.instance.getPlayerScript().setAmmo(gameManager.instance.getPlayerScript().getAmmoOrig());
    }
}
