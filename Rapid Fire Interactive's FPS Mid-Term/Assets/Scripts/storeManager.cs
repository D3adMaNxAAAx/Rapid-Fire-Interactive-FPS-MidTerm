using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class storeManager : MonoBehaviour
{
    // Singleton
    public static storeManager instance;

    // Store Costs
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;

    // Player Stats
    int playerHP;
    int playerAmmo;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update() not needed yet.

    // Public methods for external store functions -- these will call internal store functions as necessary
    // Buy Button Methods
    public void onHealthPurchase()
    {
        giveHealth();
    }

    public void onAmmoPurchase()
    {
        giveAmmo();
    }

    // Private methods for internal store functions
    void giveHealth()
    {
        // Heal the player to full as per their purchase & update the UI
        gameManager.instance.getPlayerScript().setHP(gameManager.instance.getPlayerScript().getHPOrig());
        gameManager.instance.getPlayerScript().updatePlayerUI();
    }

    void giveAmmo()
    {
        // Give the player max ammo as per their purchase & update the UI
        gameManager.instance.getPlayerScript().setAmmo(gameManager.instance.getPlayerScript().getAmmoOrig());
        gameManager.instance.getPlayerScript().updatePlayerUI();
    }

    // Update the store UI as the player interacts with it
    void updateStoreUI()
    {

    }
}
