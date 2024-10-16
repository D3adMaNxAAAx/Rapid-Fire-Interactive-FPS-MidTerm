using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: Make the game unpaused while using storeManager so timers can be used!

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
    [SerializeField] TMP_Text transactionStatus;

    // Store Costs
    [Header("-- Store Modifiers --")]
    [SerializeField] float flashMod;
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;
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

    // No update needed.

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
        // Check if player can afford health & isn't at max HP.
        if (canAfford(healthCost) && gameManager.instance.getPlayerScript().getHP() < gameManager.instance.getPlayerScript().getHPOrig())
        {
            // Player can, so make transaction and give health.
            makeTransaction(healthCost);
            giveHealth();

            // Display that the transaction was successful.
            //displayTransactionStatus(true);
        }
        else
        {
            // Transaction could not go through.
            //displayTransactionStatus(false);
        }
    }

    public void onAmmoPurchase()
    {
        // Check if player can afford ammo & isn't already at max ammo
        if (canAfford(ammoCost) 
            && (gameManager.instance.getPlayerScript().getCurGun().ammoMax < gameManager.instance.getPlayerScript().getCurGun().ammoOrig
            || gameManager.instance.getPlayerScript().getCurGun().ammoCur < gameManager.instance.getPlayerScript().getCurGun().ammoMag))
        {
            // Player can, so make transaction and give ammo.
            makeTransaction(ammoCost);
            giveAmmo();

            // Display that the transaction was successful.
            //displayTransactionStatus(true);
        } 
        else
        {
            // Transaction could not go through.
            //displayTransactionStatus(false);
        }
    }

    // Private methods for internal store functions
    // Transaction method for checking if player can purchase anything from the store, taking in a cost.
    bool canAfford(int _cost)
    {
        bool _state;

        // Check if the player has enough to make the purchase
        if (gameManager.instance.getPlayerScript().getCoins() >= _cost)
            _state = true; // Player can afford the purchase, return true
        else _state = false;

        return _state;
    }

    void makeTransaction(int _cost)
    {
        // Designated function just in case transactions may be more deliberate
        // Method is called if canAfford returns true so player can afford something
       gameManager.instance.getPlayerScript().setCoins(gameManager.instance.getPlayerScript().getCoins() - _cost);
        playerStats.Stats.purchased();
    }

    IEnumerator displayTransactionStatus(bool status)
    {
        // Turn the transaction status text on.
        transactionStatus.gameObject.SetActive(true);

        if (status)
        {
            // status is good
            transactionStatus.text = "Purchase Successful!";

            // green for extra feedback
            transactionStatus.color = Color.green;
        } else
        {
            // Transaction failed
            // Check why the player cannot do the transaction and set text to reason
            if (!canAfford(healthCost))
            {
                transactionStatus.text = "Not enough coins!";
                Debug.Log("lol ur broke");
            }
            else if (gameManager.instance.getPlayerScript().getHP() == gameManager.instance.getPlayerScript().getHPOrig())
            {
                Debug.Log("why are you dumb");
                transactionStatus.text = "Already full!";
            }

            // Set color to red for extra feedback.
            transactionStatus.color = Color.red;
        }
        
        yield return new WaitForSeconds(2f);

        // Timer is up, turn off text
        transactionStatus.gameObject.SetActive(false);
    }

    // Methods to give the player what they purchased
    void giveHealth()
    {
        // Heal the player to full as per their purchase & update the UI
        gameManager.instance.getPlayerScript().setHP(gameManager.instance.getPlayerScript().getHPOrig());
        gameManager.instance.getPlayerScript().updatePlayerUI();

        // Update UI
        updateStoreUI();
    }
    
    void giveAmmo()
    {
        // Precautionary check if the player has a gun
        if (gameManager.instance.getPlayerScript().hasGun())
        {
            // Range-based loop that will go through the players gun inventory and give max ammo.
            foreach (gunStats gun in gameManager.instance.getPlayerScript().getGunList())
            {
                // Give the player max ammo as per their purchase & update the UI
                gun.ammoCur = gun.ammoMag;
                gun.ammoMax = gun.ammoOrig;
            }
            
            // Update UIs
            gameManager.instance.getPlayerScript().updatePlayerUI();
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

        // Color the cost text green/red depending on if the player can afford it
        if (canAfford(healthCost))
            healthCostText.color = Color.green;
        else
            healthCostText.color = Color.red;
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

            // Color the cost text green/red depending on if the player can afford it
            if (canAfford(ammoCost))
                ammoCostText.color = Color.green;
            else
                ammoCostText.color = Color.red;
        }
        else
        {
            // Edge case
            ammoCost = 0;
            ammoText.text = "No Weapon";
            ammoCostText.text = "Cost: N/A";
            ammoText.color = Color.red;
            ammoCostText.color = Color.red;
        }
    }
}
