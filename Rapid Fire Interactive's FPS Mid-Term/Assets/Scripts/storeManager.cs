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
    }

    void Update()
    {
        // temporary thing to check if the player has a gun
        if (gameManager.instance.getPlayerScript().hasGun())
            updateStoreUI(); // for now just update every frame
    }

    // Public methods for external store functions -- these will call internal store functions as necessary
    // Buy Button Methods
    public void onHealthPurchase()
    {
        if (healthTransaction())
            giveHealth();
        else
            StartCoroutine(flashRed());
    }

    public void onAmmoPurchase()
    {
        if (ammoTransaction())
            giveAmmo();
        else
            StartCoroutine(flashRed());
    }

    // Private methods for internal store functions
    // Transaction methods for checking if player can purchase anything from the store
    bool healthTransaction()
    {
        // Bool variable to store the result of the transaction
        bool _state;

        // Check if the player has enough to purchase health
        if (gameManager.instance.getPlayerScript().getCoins() >= healthCost)
        {
            // Player can afford health, so charge them and return true
            _state = true;
            gameManager.instance.getPlayerScript().setCoins(gameManager.instance.getPlayerScript().getCoins() - healthCost);
        }
        else
        {
            _state = false;
        }

        return _state;
    }

    bool ammoTransaction()
    {
        bool _state;

        // Check if the player has enough to purchase ammo
        if (gameManager.instance.getPlayerScript().getCoins() >= ammoCost)
        {
            // Player can afford ammo, so charge them and return true
            _state = true;
            gameManager.instance.getPlayerScript().setCoins(gameManager.instance.getPlayerScript().getCoins() - ammoCost);
        }
        else
        {
            _state = false;
        }

        return _state;
    }

    // Methods to give the player what they purchased
    void giveHealth()
    {
        // Heal the player to full as per their purchase & update the UI
        gameManager.instance.getPlayerScript().setHP(gameManager.instance.getPlayerScript().getHPOrig());
        gameManager.instance.getPlayerScript().updatePlayerUI();

        // Flash button green signaling success
        StartCoroutine(flashGreen());
    }

    void giveAmmo()
    {
        // Give the player max ammo as per their purchase & update the UI
        gameManager.instance.getPlayerScript().setAmmo(gameManager.instance.getPlayerScript().getAmmoOrig());
        gameManager.instance.getPlayerScript().updatePlayerUI();

        // Flash button green signaling success
        StartCoroutine(flashGreen());
    }

    // UI Display methods
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

    IEnumerator flashRed()
    {
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator flashGreen()
    {
        yield return new WaitForSeconds(0.5f);
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
