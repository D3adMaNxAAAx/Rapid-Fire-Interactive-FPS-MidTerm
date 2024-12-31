using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class storeManager : MonoBehaviour {

    public static storeManager instance; // Singleton

    // Store Text
    /*[Header("-- Store Information --")]
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text healthCostText;
    [SerializeField] TMP_Text ammoCostText;
    [SerializeField] TMP_Text laserRifleCostText;
    [SerializeField] TMP_Text playerCoinsText;
    [SerializeField] TMP_Text transactionStatus;*/

    [Header("-- Terminal Store Information --")]
    [SerializeField] TMP_Text t_healthCostText;
    [SerializeField] TMP_Text t_ammoCostText;
    [SerializeField] TMP_Text shieldCostText;
    [SerializeField] TMP_Text inventoryCostText;
    [SerializeField] TMP_Text t_laserRifleCostText;
    [SerializeField] TMP_Text t_playerCoinsText;
    [SerializeField] TMP_Text t_transactionStatus;

    [Header("-- Store Weapons --")]
    [SerializeField] gunStats laserRifle; // This might be able to be made as a generalized weapon to sell variable

    // Store Costs
    [Header("-- Store Modifiers --")]
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;
    [SerializeField] int shieldCost;
    [SerializeField] int laserRifleCost;
    [SerializeField] int inventoryCost;
    int inventoryCostV2;

    Color healthColorOrig;
    Color ammoColorOrig;
    bool terminal;
    int InventoryVersion = 0;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        healthColorOrig = t_healthCostText.color; // Remember the original color of the text
        ammoColorOrig = t_ammoCostText.color; // Remember the original color of the text
        inventoryCostV2 = inventoryCost * 2;
    }

    public void setTerminalStatus(bool _state) {
        terminal = _state;
    }

    public void updateStoreUI() { // Public methods for external store functions - these will call internal store functions as necessary, Update the store UI as the player interacts with it
        updateCoinsDisplay();
        updateHealthDisplay();
        updateAmmoDisplay();
        updateLaserRifleDisplay();
        updateShieldDisplay();
        updateInventoryUpgradeDisplay(InventoryVersion);
    }

    // Buy Button Methods
    public void onHealthPurchase() {
        if (canAfford(healthCost) == false) {
            StartCoroutine(displayTransactionStatus(false));
        }
        else if ((playerMovement.player.getHP() < playerMovement.player.getHPOrig()) == false) {
            StartCoroutine(displayTransactionStatus(false, "HP already at Max!"));
        }
        else { // success
            makeTransaction(healthCost);
            giveHealth();
            StartCoroutine(displayTransactionStatus(true));
        }
    }

    public void onAmmoPurchase() {
        // Check if player can afford ammo & isn't already at max ammo
        if (canAfford(ammoCost) && (playerMovement.player.getCurGun().ammoMax < playerMovement.player.getCurGun().ammoOrig || playerMovement.player.getCurGun().ammoCur < playerMovement.player.getCurGun().ammoMag)) {
            makeTransaction(ammoCost);
            giveAmmo();
        }
        /// update and remember to add transaction status
    }

    public void onInventoryUpgrade() {
        if (InventoryVersion == 0) {
            if (canAfford(inventoryCost)) {
                makeTransaction(inventoryCost);
                playerMovement.player.inventoryUpgrade(false);
                StartCoroutine(displayTransactionStatus(true));
                InventoryVersion = 1;
                updateInventoryUpgradeDisplay(InventoryVersion);
                updateCoinsDisplay();
            }
            else { // fail
                StartCoroutine(displayTransactionStatus(false));
            }
        }
        else if (InventoryVersion == 1) { // second inventory upgrade
            if (canAfford(inventoryCostV2)) {
                makeTransaction(inventoryCostV2);
                playerMovement.player.inventoryUpgrade(true);
                StartCoroutine(displayTransactionStatus(true));
                InventoryVersion = 2;
                updateInventoryUpgradeDisplay(InventoryVersion);
                updateCoinsDisplay();
            }
            else { // fail
                StartCoroutine(displayTransactionStatus(false));
            }
        }
        else {
            StartCoroutine(displayTransactionStatus(false, "Max upgrades already purchased"));
        }
    }

    public void onShieldPurchase() {
        if (canAfford(shieldCost) == false) {
            StartCoroutine(displayTransactionStatus(false));
        }
        else if (playerMovement.player.getShieldHP() == 50) {
            StartCoroutine(displayTransactionStatus(false, "Shield already maxed out"));
        }
        else { // success
            makeTransaction(shieldCost);
            playerMovement.player.shieldBuff();
            StartCoroutine(displayTransactionStatus(true));
            updateShieldDisplay();
            updateCoinsDisplay();
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
        if (canAfford(laserRifleCost) == false) {
            StartCoroutine(displayTransactionStatus(false));
        }
        else if (hasLaserRifle) {
            StartCoroutine(displayTransactionStatus(false, "Error: Laser Rifle already purchased"));
        }
        else { // success
            makeTransaction(laserRifleCost);
            giveLaserRifle();
            StartCoroutine(displayTransactionStatus(true));
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

    void makeTransaction(int _cost) { // Designated function just in case transactions may be more deliberate
        playerMovement.player.setCoins(-_cost);
        playerStats.Stats.purchased();
    }

    IEnumerator displayTransactionStatus(bool success, string failMessage = "Not enough coins!") {
        if (!terminal) {
            /*transactionStatus.gameObject.SetActive(true);
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
            transactionStatus.gameObject.SetActive(false);*/
        } 
        else {
            t_transactionStatus.gameObject.SetActive(true);
            if (success) {
                t_transactionStatus.text = "Purchase Successful!";
                t_transactionStatus.color = Color.green;
            }
            else {
                t_transactionStatus.text = failMessage;
                t_transactionStatus.color = Color.red;
            }
            yield return new WaitForSecondsRealtime(1.5f);
            t_transactionStatus.gameObject.SetActive(false);
        }
    }

    void giveHealth() {
        playerMovement.player.setHP(playerMovement.player.getHPOrig()); // Heal the player to full as per their purchase & update the UI
        playerMovement.player.updatePlayerUI();
        updateHealthDisplay();
        updateCoinsDisplay();
    }
    
    void giveAmmo() { /// didn't seem to give ammo to all guns?
        if (playerMovement.player.hasGun()) { // Precautionary check if the player has a gun
            // Range-based loop that will go through the players gun inventory and give max ammo.
            foreach (gunStats gun in playerMovement.player.getGunList()) { // Give the player max ammo as per their purchase & update the UI
                gun.ammoCur = gun.ammoMag;
                gun.ammoMax = gun.ammoOrig;
            }
            playerMovement.player.updatePlayerUI();
            updateAmmoDisplay();
            updateCoinsDisplay();
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
        updateLaserRifleDisplay();
        updateCoinsDisplay();
    }

    // UI Display methods
    void updateCoinsDisplay() {
        if (!terminal) {
            // playerCoinsText.text = playerMovement.player.getCoins().ToString("F0");
        }
        else {
            t_playerCoinsText.text = playerMovement.player.getCoins().ToString("F0");
        }
    }

    void updateHealthDisplay() {
        if (!terminal) {
            /*healthText.text = playerMovement.player.getHP().ToString("F0") + " >> " + playerMovement.player.getHPOrig().ToString("F0");
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
            }*/
        }
        else {
            t_healthCostText.text = healthCost.ToString();
            t_healthCostText.text += " coins";
            if (canAfford(healthCost))
                t_healthCostText.color = Color.green;
            else
                t_healthCostText.color = Color.red;
        }
    }

    void updateAmmoDisplay() {
        if (!terminal) {
            /*if (playerMovement.player.hasGun()) {
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
            }*/
        }
        else {
            if (playerMovement.player.hasGun()) {
                t_ammoCostText.text = ammoCost.ToString();
                t_ammoCostText.text += " coins";
                if (canAfford(ammoCost))
                    t_ammoCostText.color = Color.green;
                else
                    t_ammoCostText.color = Color.red;
            }
        }
    }

    void updateLaserRifleDisplay() {
        if (!terminal) {
            /*laserRifleCostText.text = "Cost: " + laserRifleCost.ToString("F0");
            if (laserRifleCost == 1)
                laserRifleCostText.text += " coin";
            else
                laserRifleCostText.text += " coins";
            if (canAfford(laserRifleCost))
                laserRifleCostText.color = Color.green;
            else
                laserRifleCostText.color = Color.red;*/
        }
        else {
            t_laserRifleCostText.text = laserRifleCost.ToString("F0");
            t_laserRifleCostText.text += " coins";
            if (canAfford(laserRifleCost))
                t_laserRifleCostText.color = Color.green;
            else
                t_laserRifleCostText.color = Color.red;
        }
    }

    void updateShieldDisplay() {
        shieldCostText.text = shieldCost.ToString("F0");
        shieldCostText.text += " coins";
        if (canAfford(laserRifleCost)) {
            shieldCostText.color = Color.green;
        }
        else {
            shieldCostText.color = Color.red;
        }
    }

    void updateInventoryUpgradeDisplay(int upgradeV ) {
        int cost = 0;
        if (upgradeV == 0) {
            cost = inventoryCost;
        }
        else {
            cost = inventoryCostV2;
        }
        inventoryCostText.text = cost.ToString("F0");
        inventoryCostText.text += " coins";
        if (canAfford(cost)) {
            inventoryCostText.color = Color.green;
        }
        else {
            inventoryCostText.color = Color.red;
        }
    }
}
