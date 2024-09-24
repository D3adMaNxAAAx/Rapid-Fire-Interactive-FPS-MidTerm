using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class storeManager : MonoBehaviour
{
    // Singleton
    public static storeManager instance;

    // Store Text
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text healthCostText;
    [SerializeField] TMP_Text ammoCostText;
    [SerializeField] TMP_Text playerCoinsText;
    
    // Store Costs
    [Header("-- Store Costs --")]
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;

    // Player Stats
    int playerHP;
    int playerAmmo;
    int playerCoins;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        updateStoreUI();
    }

    void Update()
    {
        updateStoreUI(); // for now just update every frame
    }

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

    void updateCoinsDisplay()
    {
        playerCoinsText.text = gameManager.instance.getPlayerScript().getCoins().ToString("F0");
    }

    void updateHealthDisplay()
    {
        // Update the Health Restoration Display
        healthText.text = gameManager.instance.getPlayerScript().getHP().ToString("F0") + " > " + gameManager.instance.getPlayerScript().getHPOrig().ToString("F0");

        // Update the Health Cost
        healthCostText.text = "Cost: " + healthCost.ToString();

        // Append Coin or Coins at the end
        if (healthCost == 1) 
            healthCostText.text += " coin";
        else
            healthCostText.text += " coins";
    }

    void updateAmmoDisplay()
    {
        // Update the Ammo Restoration Display
        ammoText.text = gameManager.instance.getPlayerScript().getAmmo().ToString("F0") + " > " + gameManager.instance.getPlayerScript().getAmmoOrig().ToString("F0");

        // Update the Ammo Cost
        ammoCostText.text = "Cost: " + ammoCost.ToString();

        // Append Coin or Coins at the end
        if (ammoCost == 1)
            ammoCostText.text += " coin";
        else
            ammoCostText.text += " coins";
    }

    // Update the store UI as the player interacts with it
    void updateStoreUI()
    {
        updateCoinsDisplay();
        updateHealthDisplay();
        updateAmmoDisplay();
    }
}
