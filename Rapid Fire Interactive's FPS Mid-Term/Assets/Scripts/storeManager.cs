using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class storeManager : MonoBehaviour {

    public static storeManager instance; // Singleton

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

    Color healthColorOrig;
    Color ammoColorOrig;
    bool terminal;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        healthColorOrig = healthCostText.color; // Remember the original color of the text
        ammoColorOrig = ammoCostText.color; // Remember the original color of the text
    }

    public void setTerminalStatus(bool _state) {
        terminal = _state;
    }

    public void updateStoreUI() { // Public methods for external store functions - these will call internal store functions as necessary, Update the store UI as the player interacts with it
        updateCoinsDisplay();
        updateHealthDisplay();
        updateAmmoDisplay();
        updateLaserRifleDisplay();
    }

    // Buy Button Methods
    public void onHealthPurchase() {
        if (canAfford(healthCost) && playerMovement.player.getHP() < playerMovement.player.getHPOrig()) { // Check if player can afford health & isn't at max HP
            makeTransaction(healthCost);
            giveHealth();
        }
    }

    public void onAmmoPurchase() {
        // Check if player can afford ammo & isn't already at max ammo
        if (canAfford(ammoCost) && (playerMovement.player.getCurGun().ammoMax < playerMovement.player.getCurGun().ammoOrig || playerMovement.player.getCurGun().ammoCur < playerMovement.player.getCurGun().ammoMag)) {
            makeTransaction(ammoCost);
            giveAmmo();
        }
    }

    public void onLaserRiflePurchase() {
        bool hasLaserRifle = false;
        foreach (gunStats gun in playerMovement.player.getGunList()) { // Check if player can afford the laser rifle & doesn't already have it
            if (gun.isLaser) {
                hasLaserRifle = true;
                break;
            }
        }
        if (canAfford(laserRifleCost) && !hasLaserRifle) {
            makeTransaction(laserRifleCost);
            giveLaserRifle();
        }
    }

    // Private methods for internal store functions
    bool canAfford(int _cost) { // Transaction method for checking if player can purchase anything from the store, taking in a cost.
        bool _state;
        if (playerMovement.player.getCoins() >= _cost) { // Check if the player has enough to make the purchase
            _state = true; 
        }
        else {
            _state = false;
        }
        return _state;
    }

    void makeTransaction(int _cost) { /// Designated function just in case transactions may be more deliberate, Method is called if canAfford returns true so player can afford something
        playerMovement.player.setCoins(-_cost);
        playerStats.Stats.purchased();
    }

    IEnumerator displayTransactionStatus(bool status) {
        if (!terminal) {
            transactionStatus.gameObject.SetActive(true);
            if (status) {
                transactionStatus.text = "Purchase Successful!";
                transactionStatus.color = Color.green;
            }
            else {
                if (!canAfford(healthCost)) {
                    transactionStatus.text = "Not enough coins!";
                }
                else if (playerMovement.player.getHP() == playerMovement.player.getHPOrig()) {
                    transactionStatus.text = "Already full!";
                }
                transactionStatus.color = Color.red;
            }
            yield return new WaitForSeconds(2f);
            transactionStatus.gameObject.SetActive(false);
        } 
        else {
            t_transactionStatus.gameObject.SetActive(true);
            if (status) {
                t_transactionStatus.text = "Purchase Successful!";
                t_transactionStatus.color = Color.green;
            }
            else {
                if (!canAfford(healthCost)) {
                    t_transactionStatus.text = "Not enough coins!";
                }
                else if (playerMovement.player.getHP() == playerMovement.player.getHPOrig()) {
                    t_transactionStatus.text = "Already full!";
                }
                t_transactionStatus.color = Color.red;
            }
            yield return new WaitForSeconds(2f);
            t_transactionStatus.gameObject.SetActive(false);
        }
    }

    void giveHealth() {
        playerMovement.player.setHP(playerMovement.player.getHPOrig()); // Heal the player to full as per their purchase & update the UI
        playerMovement.player.updatePlayerUI();
        updateStoreUI();
    }
    
    void giveAmmo() { /// didn't seem to give ammo to all guns?
        if (playerMovement.player.hasGun()) { // Precautionary check if the player has a gun
            // Range-based loop that will go through the players gun inventory and give max ammo.
            foreach (gunStats gun in playerMovement.player.getGunList()) { // Give the player max ammo as per their purchase & update the UI
                gun.ammoCur = gun.ammoMag;
                gun.ammoMax = gun.ammoOrig;
            }
            playerMovement.player.updatePlayerUI();
            updateStoreUI();
        }
    }

    public void giveLaserRifle() {
        if (playerMovement.player.getGunList().Count < 5) { // Precautionary check to make sure the player doesn't have a full loadout, Check if they have less than 5 guns
            playerMovement.player.getGunStats(laserRifle);
        }
        else { // Otherwise, override the last gun with the laser rifle (FOR NOW)
            playerMovement.player.getGunList()[playerMovement.player.getGunList().Count - 1] = laserRifle;
        }
        playerMovement.player.updatePlayerUI();
        updateStoreUI();
    }

    // UI Display methods
    void updateCoinsDisplay() {
        if (!terminal) {
            playerCoinsText.text = playerMovement.player.getCoins().ToString("F0");
        }
        else {
            t_playerCoinsText.text = playerMovement.player.getCoins().ToString("F0");
        }
    }

    void updateHealthDisplay() {
        if (!terminal) {
            healthText.text = playerMovement.player.getHP().ToString("F0") + " >> " + playerMovement.player.getHPOrig().ToString("F0");
            healthCostText.text = "Cost: " + healthCost.ToString();
            if (healthCost == 1) { // Append Coin or Coins at the end
                healthCostText.text += " coin";
            }
            else {
                healthCostText.text += " coins";
            }
            if (canAfford(healthCost)) {
                healthCostText.color = Color.green;
            }
            else {
                healthCostText.color = Color.red;
            }
        }
        else {
            t_healthText.text = playerMovement.player.getHP().ToString("F0") + " >> " + playerMovement.player.getHPOrig().ToString("F0");
            t_healthCostText.text = "Cost: " + healthCost.ToString();
            if (healthCost == 1) { // Append Coin or Coins at the end
                t_healthCostText.text += " coin";
            }
            else {
                t_healthCostText.text += " coins";
            }
            if (canAfford(healthCost))
                t_healthCostText.color = Color.green;
            else
                t_healthCostText.color = Color.red;
        }
    }

    void updateAmmoDisplay() {
        if (!terminal) {
            if (playerMovement.player.hasGun()) {
                ammoText.text = "Refills all ammo!";
                ammoCostText.text = "Cost: " + ammoCost.ToString();
                if (ammoCost == 1) { // Append Coin or Coins at the end
                    ammoCostText.text += " coin";
                }
                else {
                    ammoCostText.text += " coins";
                }
                if (canAfford(ammoCost))
                    ammoCostText.color = Color.green;
                else
                    ammoCostText.color = Color.red;
            }
            else { // Edge case
                ammoCost = 0;
                ammoText.text = "No Weapon";
                ammoCostText.text = "Cost: N/A";
                ammoText.color = Color.red;
                ammoCostText.color = Color.red;
            }
        }
        else {
            if (playerMovement.player.hasGun()) {
                t_ammoText.text = "Refills all ammo!";
                t_ammoCostText.text = "Cost: " + ammoCost.ToString();
                if (ammoCost == 1) { // Append Coin or Coins at the end
                    t_ammoCostText.text += " coin";
                }
                else {
                    t_ammoCostText.text += " coins";
                }
                if (canAfford(ammoCost))
                    t_ammoCostText.color = Color.green;
                else
                    t_ammoCostText.color = Color.red;
            }
            else { // Edge case
                ammoCost = 0;
                t_ammoText.text = "No Weapon";
                t_ammoCostText.text = "Cost: N/A";
                t_ammoText.color = Color.red;
                t_ammoCostText.color = Color.red;
            }
        }
    }

    void updateLaserRifleDisplay() {
        if (!terminal) {
            laserRifleCostText.text = "Cost: " + laserRifleCost.ToString("F0");
            if (laserRifleCost == 1)
                laserRifleCostText.text += " coin";
            else
                laserRifleCostText.text += " coins";
            if (canAfford(laserRifleCost))
                laserRifleCostText.color = Color.green;
            else
                laserRifleCostText.color = Color.red;
        }
        else {
            t_laserRifleCostText.text = "Cost: " + laserRifleCost.ToString("F0");
            if (laserRifleCost == 1)
                t_laserRifleCostText.text += " coin";
            else
                t_laserRifleCostText.text += " coins";
            if (canAfford(laserRifleCost))
                t_laserRifleCostText.color = Color.green;
            else
                t_laserRifleCostText.color = Color.red;
        }
    }
}
