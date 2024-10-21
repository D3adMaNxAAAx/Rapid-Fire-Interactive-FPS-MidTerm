using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] TMP_Text laserRifleCostText;
    [SerializeField] TMP_Text playerCoinsText;
    [SerializeField] TMP_Text transactionStatus;

    [Header("-- Terminal Store Information --")]
    [SerializeField] TMP_Text t_healthText;
    [SerializeField] TMP_Text t_ammoText;
    [SerializeField] TMP_Text t_healthCostText;
    [SerializeField] TMP_Text t_ammoCostText;
    [SerializeField] TMP_Text t_laserRifleCostText;
    [SerializeField] TMP_Text t_playerCoinsText;
    [SerializeField] TMP_Text t_transactionStatus;

    [Header("-- Store Weapons --")]
    [SerializeField] gunStats laserRifle; // This might be able to be made as a generalized weapon to sell variable

    // Store Costs
    [Header("-- Store Modifiers --")]
    [SerializeField] float flashMod;
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;
    [SerializeField] int laserRifleCost;

    // Memory
    Color healthColorOrig;
    Color ammoColorOrig;
    bool terminal;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton instance
        instance = this;

        // Remember the original color of the text
        healthColorOrig = healthCostText.color;
        ammoColorOrig = ammoCostText.color;
    }

    public void setTerminalStatus(bool _state)
    {
        terminal = _state;
    }

    // No update needed.

    // Public methods for external store functions -- these will call internal store functions as necessary
    // Update the store UI as the player interacts with it
    public void updateStoreUI()
    {
        updateCoinsDisplay();
        updateHealthDisplay();
        updateAmmoDisplay();
        updateLaserRifleDisplay();
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
            // Successful Transaction
        }
        else { /* Unsuccessful */ }
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
            // Successful Transaction
        }
        else { /* Unsuccessful */ }
    }

    public void onLaserRiflePurchase()
    {
        // (maybe account for a case where the player has a full loadout)
        // Check if player can afford the laser rifle & doesn't already have it
        bool hasLaserRifle = false;
        
        foreach (gunStats gun in gameManager.instance.getPlayerScript().getGunList())
        {
            if (gun.isLaser)
            {
                hasLaserRifle = true;
                break;
            }
        }

        if (canAfford(laserRifleCost) && !hasLaserRifle)
        {
            // Player can, so make transaction and give health.
            makeTransaction(laserRifleCost);
            giveLaserRifle();
            // Successful Transaction
        }
        else { /* Unsuccessful */ }
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
        if (!terminal)
        {
            // Turn the transaction status text on.
            transactionStatus.gameObject.SetActive(true);

            if (status)
            {
                // status is good
                transactionStatus.text = "Purchase Successful!";

                // green for extra feedback
                transactionStatus.color = Color.green;
            }
            else
            {
                // Transaction failed
                // Check why the player cannot do the transaction and set text to reason
                if (!canAfford(healthCost))
                {
                    transactionStatus.text = "Not enough coins!";
                }
                else if (gameManager.instance.getPlayerScript().getHP() == gameManager.instance.getPlayerScript().getHPOrig())
                {
                    transactionStatus.text = "Already full!";
                }

                // Set color to red for extra feedback.
                transactionStatus.color = Color.red;
            }

            yield return new WaitForSeconds(2f);

            // Timer is up, turn off text
            transactionStatus.gameObject.SetActive(false);
        } 
        else
        {
            // Turn the transaction status text on.
            t_transactionStatus.gameObject.SetActive(true);

            if (status)
            {
                // status is good
                t_transactionStatus.text = "Purchase Successful!";

                // green for extra feedback
                t_transactionStatus.color = Color.green;
            }
            else
            {
                // Transaction failed
                // Check why the player cannot do the transaction and set text to reason
                if (!canAfford(healthCost))
                {
                    t_transactionStatus.text = "Not enough coins!";
                }
                else if (gameManager.instance.getPlayerScript().getHP() == gameManager.instance.getPlayerScript().getHPOrig())
                {
                    t_transactionStatus.text = "Already full!";
                }

                // Set color to red for extra feedback.
                t_transactionStatus.color = Color.red;
            }

            yield return new WaitForSeconds(2f);

            // Timer is up, turn off text
            t_transactionStatus.gameObject.SetActive(false);
        }
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

    void giveLaserRifle()
    {
        // Precautionary check to make sure the player doesn't have a full loadout.
        // Check if they have less than 5 guns
        if (gameManager.instance.getPlayerScript().getGunList().Count < 5)
        {
            gameManager.instance.getPlayerScript().getGunStats(laserRifle);
        }
        // Otherwise, override the last gun with the laser rifle (FOR NOW)
        else
        {
            gameManager.instance.getPlayerScript().getGunList()[gameManager.instance.getPlayerScript().getGunList().Count - 1] = laserRifle;
        }

        // Update UIs
        gameManager.instance.getPlayerScript().updatePlayerUI();
        updateStoreUI();
    }

    // UI Display methods
    void updateCoinsDisplay()
    {
        if (!terminal)
        {
            playerCoinsText.text = gameManager.instance.getPlayerScript().getCoins().ToString("F0");
        }
        else
        {
            t_playerCoinsText.text = gameManager.instance.getPlayerScript().getCoins().ToString("F0");
        }
    }

    void updateHealthDisplay()
    {
        if (!terminal)
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
        else
        {
            // Update the Health Restoration Display
            t_healthText.text = gameManager.instance.getPlayerScript().getHP().ToString("F0") + " >> " + gameManager.instance.getPlayerScript().getHPOrig().ToString("F0");

            // Update the Health Cost
            t_healthCostText.text = "Cost: " + healthCost.ToString();

            // Append Coin or Coins at the end
            if (healthCost == 1)
                t_healthCostText.text += " coin";
            else
                t_healthCostText.text += " coins";

            // Color the cost text green/red depending on if the player can afford it
            if (canAfford(healthCost))
                t_healthCostText.color = Color.green;
            else
                t_healthCostText.color = Color.red;
        }
    }

    void updateAmmoDisplay()
    {
        if (!terminal)
        {
            if (gameManager.instance.getPlayerScript().hasGun())
            {
                // Update the Ammo Restoration Display -- to fix the commented line, there'd need to be a total ammo on gunstats.
                //ammoText.text = gameManager.instance.getPlayerScript().getAmmo().ToString("F0") + " >> " + gameManager.instance.getPlayerScript().getAmmoOrig().ToString("F0");
                ammoText.text = "Refills all ammo!";

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
        else
        {
            if (gameManager.instance.getPlayerScript().hasGun())
            {
                // Update the Ammo Restoration Display -- to fix the commented line, there'd need to be a total ammo on gunstats.
                //t_ammoText.text = gameManager.instance.getPlayerScript().getAmmo().ToString("F0") + " >> " + gameManager.instance.getPlayerScript().getAmmoOrig().ToString("F0");
                t_ammoText.text = "Refills all ammo!";

                // Update the Ammo Cost
                t_ammoCostText.text = "Cost: " + ammoCost.ToString();

                // Append Coin or Coins at the end
                if (ammoCost == 1)
                    t_ammoCostText.text += " coin";
                else
                    t_ammoCostText.text += " coins";

                // Color the cost text green/red depending on if the player can afford it
                if (canAfford(ammoCost))
                    t_ammoCostText.color = Color.green;
                else
                    t_ammoCostText.color = Color.red;
            }
            else
            {
                // Edge case
                ammoCost = 0;
                t_ammoText.text = "No Weapon";
                t_ammoCostText.text = "Cost: N/A";
                t_ammoText.color = Color.red;
                t_ammoCostText.color = Color.red;
            }
        }
    }

    void updateLaserRifleDisplay()
    {
        if (!terminal)
        {
            // Laser Rifle Cost
            laserRifleCostText.text = "Cost: " + laserRifleCost.ToString("F0");

            // Append Coin or Coins at the end
            if (laserRifleCost == 1)
                laserRifleCostText.text += " coin";
            else
                laserRifleCostText.text += " coins";

            // Color the cost text green/red depending on if the player can afford it
            if (canAfford(healthCost))
                laserRifleCostText.color = Color.green;
            else
                laserRifleCostText.color = Color.red;
        }
        else
        {
            // Laser Rifle Cost
            t_laserRifleCostText.text = "Cost: " + laserRifleCost.ToString("F0");

            // Append Coin or Coins at the end
            if (laserRifleCost == 1)
                t_laserRifleCostText.text += " coin";
            else
                t_laserRifleCostText.text += " coins";

            // Color the cost text green/red depending on if the player can afford it
            if (canAfford(healthCost))
                t_laserRifleCostText.color = Color.green;
            else
                t_laserRifleCostText.color = Color.red;
        }
    }
}
