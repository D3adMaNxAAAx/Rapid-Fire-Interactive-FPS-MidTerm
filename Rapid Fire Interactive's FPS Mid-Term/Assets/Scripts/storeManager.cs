using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class storeManager : MonoBehaviour
{
    // Singleton
    public static storeManager instance;

    // Store Text
    [Header("-- Store Information --")]
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text healthCostText;
    [SerializeField] TMP_Text ammoCostText;
    [SerializeField] TMP_Text playerCoinsText;
    
    // Store Costs
    [Header("-- Store Modifiers --")]
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;
    [SerializeField] float flashMod;

    // Memory
    Color healthColorOrig;
    Color ammoColorOrig;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton instance
        instance = this;

        // Remember the original color of the text
        healthColorOrig = healthCostText.color;
        ammoColorOrig = ammoCostText.color;
    }

    void Update()
    {
        // Debug for now
        if (gameManager.instance.getPlayerScript().hasGun())
            updateStoreUI();
    }

    // Public methods for external store functions -- these will call internal store functions as necessary
    // Update the store UI as the player interacts with it
    public void updateStoreUI()
    {
        updateCoinsDisplay();
        updateHealthDisplay();
        updateAmmoDisplay();
    }

    // Buy Button Methods
    public void onHealthPurchase()
    {
        if (tryTransaction(healthCost))
            giveHealth();
        else
            // If the transaction fails, flash text red.
            StartCoroutine(flashText(healthCostText, Color.red, healthColorOrig));
    }

    public void onAmmoPurchase()
    {
        if (tryTransaction(ammoCost))
            giveAmmo();
        else
            // If the transaction fails, flash text red.
            StartCoroutine(flashText(ammoCostText, Color.red, ammoColorOrig));
    }

    // Private methods for internal store functions
    // Transaction method for checking if player can purchase anything from the store, taking in a cost.
    bool tryTransaction(int _cost)
    {
        bool _state;

        // Check if the player has enough to make the purchase
        if (gameManager.instance.getPlayerScript().getCoins() >= _cost)
        {
            // Player can afford the purchase, so charge them and return true
            _state = true;
            gameManager.instance.getPlayerScript().setCoins(gameManager.instance.getPlayerScript().getCoins() - _cost);
        } else { _state = false; }

        return _state;
    }

    // Methods to give the player what they purchased
    void giveHealth()
    {
        // Heal the player to full as per their purchase & update the UI
        gameManager.instance.getPlayerScript().setHP(gameManager.instance.getPlayerScript().getHPOrig());
        gameManager.instance.getPlayerScript().updatePlayerUI();

        // Flash text green signaling success
        StartCoroutine(flashText(healthCostText, Color.green, healthColorOrig));

        // Update UI
        updateStoreUI();
    }

    void giveAmmo()
    {
        // Precautionary check if the player has a gun
        if (gameManager.instance.getPlayerScript().hasGun())
        {
            // Give the player max ammo as per their purchase & update the UI
            gameManager.instance.getPlayerScript().setAmmo(gameManager.instance.getPlayerScript().getAmmoOrig());
            gameManager.instance.getPlayerScript().updatePlayerUI();

            // Flash text green signaling success
            StartCoroutine(flashText(ammoCostText, Color.green, ammoColorOrig));

            // Update UI
            updateStoreUI();
        }
    }

    // UI Display methods
    void updateCoinsDisplay()
    {
        playerCoinsText.text = gameManager.instance.getPlayerScript().getCoins().ToString("F0");
    }

    void updateHealthDisplay()
    {
        // Update the Health Restoration Display
        healthText.text = gameManager.instance.getPlayerScript().getHP().ToString("F0") + " >> " + gameManager.instance.getPlayerScript().getHPOrig().ToString("F0");

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
        if (gameManager.instance.getPlayerScript().hasGun())
        {
            // Update the Ammo Restoration Display
            ammoText.text = gameManager.instance.getPlayerScript().getAmmo().ToString("F0") + " >> " + gameManager.instance.getPlayerScript().getAmmoOrig().ToString("F0");

            // Update the Ammo Cost
            ammoCostText.text = "Cost: " + ammoCost.ToString();

            // Append Coin or Coins at the end
            if (ammoCost == 1)
                ammoCostText.text += " coin";
            else
                ammoCostText.text += " coins";
        }
        else
        {
            // Edge case
            ammoCost = 0;
            ammoText.text = "No Weapon";
            ammoCostText.text = "Cost: N/A";
        }
    }

    // Flash Timers -- Pass in three parameters: one for the text to flash, the color to flash it as, and the original color of the text
    IEnumerator flashText(TMP_Text _text, Color _flashColor, Color _origColor)
    {
        _text.color = _flashColor;
        yield return new WaitForSeconds(flashMod);
        _text.color = _origColor;
    }
}
