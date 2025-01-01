using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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
    [SerializeField] TMP_Text healPotionCostText;
    [SerializeField] TMP_Text grenadeCostText;
    [SerializeField] TMP_Text t_ammoCostText;
    [SerializeField] TMP_Text shieldCostText;
    [SerializeField] TMP_Text inventoryCostText;
    [SerializeField] TMP_Text t_laserRifleCostText;
    [SerializeField] TMP_Text t_playerCoinsText;
    [SerializeField] TMP_Text t_transactionStatus;
    [SerializeField] TMP_Text ARAmmoCostText;
    [SerializeField] TMP_Text SMGAmmoCostText;
    [SerializeField] TMP_Text PistolAmmoCostText;
    [SerializeField] TMP_Text SniperAmmoCostText;
    [SerializeField] TMP_Text ShotgunAmmoCostText;
    [SerializeField] TMP_Text HandCannonAmmoCostText;

    [Header("-- Store Weapons / Items --")]
    [SerializeField] gunStats laserRifle; //
    [SerializeField] GrenadeStats grenade;
    [SerializeField] HealStats healPotion;

    // Store Costs
    [Header("-- Store Modifiers --")]
    [SerializeField] int healthCost;
    [SerializeField] int ammoCost;
    [SerializeField] int shieldCost;
    [SerializeField] int healPotionCost;
    [SerializeField] int grenadeCost;
    [SerializeField] int laserRifleCost;
    [SerializeField] int inventoryCost;
    int inventoryCostV2;
    int OGHealCost;
    int OGGrenadeCost;
    int ARPos = -1;
    int PistolPos = -1;
    int SniperPos = -1;
    int ShotgunPos = -1;
    int SMGPos = -1;
    int HandCannonPos = -1;

    Color healthColorOrig;
    Color ammoColorOrig;
    bool terminal;
    int InventoryVersion = 0;

    Coroutine feedbackTimer = null;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        healthColorOrig = t_healthCostText.color; // Remember the original color of the text
        ammoColorOrig = t_ammoCostText.color; // Remember the original color of the text
        inventoryCostV2 = inventoryCost * 2;
        OGHealCost = healPotionCost;
        OGGrenadeCost = grenadeCost;
    }

    public void setTerminalStatus(bool _state) {
        terminal = _state;
    }

    public void updateStoreUI() { // Public methods for external store functions - these will call internal store functions as necessary, Update the store UI as the player interacts with it
        updateCoinsDisplay();
        updateHealthDisplay();
        updateHealPotionDisplay();
        updateGrenadeDisplay();
        updateAmmoDisplays();
        updateLaserRifleDisplay();
        updateShieldDisplay();
        updateInventoryUpgradeDisplay(InventoryVersion);
    }

    // Buy Button Methods (except ammo ones are at the bottom)
    public void onHealthPurchase() {
        resetCoroutine();
        if (canAfford(healthCost) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false));
        }
        else if ((playerMovement.player.getHP() < playerMovement.player.getHPOrig()) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false, "HP already at Max!"));
        }
        else { // success
            makeTransaction(healthCost);
            giveHealth();
            feedbackTimer = StartCoroutine(displayTransactionStatus(true));
        }
    }

    public void onHealPotionPurchase() {
        resetCoroutine();
        if (canAfford(healPotionCost) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false));
        }
        else if ((playerMovement.player.getHealsCount() < playerMovement.player.getHealsMax()) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Inventory Full!"));
        }
        else { // success
            makeTransaction(healPotionCost);
            playerMovement.player.addToHeals(healPotion);
            healPotionCost += 5;
            updateHealPotionDisplay();
            updateCoinsDisplay();
            feedbackTimer = StartCoroutine(displayTransactionStatus(true));
        }
    }

    public void onGrenadePurchase() {
        resetCoroutine();
        if (canAfford(grenadeCost) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false));
        }
        else if ((playerMovement.player.getGrenadesCount() < playerMovement.player.getGrenadesMax()) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Inventory Full!"));
        }
        else { // success
            makeTransaction(grenadeCost);
            playerMovement.player.addToGrenades(grenade);
            grenadeCost += 5;
            updateGrenadeDisplay();
            updateCoinsDisplay();
            feedbackTimer = StartCoroutine(displayTransactionStatus(true));
        }
    }

    public void onInventoryUpgrade() {
        resetCoroutine();
        if (InventoryVersion == 0) {
            if (canAfford(inventoryCost)) {
                makeTransaction(inventoryCost);
                playerMovement.player.inventoryUpgrade(false);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
                InventoryVersion = 1;
                updateInventoryUpgradeDisplay(InventoryVersion);
                updateCoinsDisplay();
            }
            else { // fail
                feedbackTimer = StartCoroutine(displayTransactionStatus(false));
            }
        }
        else if (InventoryVersion == 1) { // second inventory upgrade
            if (canAfford(inventoryCostV2)) {
                makeTransaction(inventoryCostV2);
                playerMovement.player.inventoryUpgrade(true);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
                InventoryVersion = 2;
                updateInventoryUpgradeDisplay(InventoryVersion);
                updateCoinsDisplay();
            }
            else { // fail
                feedbackTimer = StartCoroutine(displayTransactionStatus(false));
            }
        }
        else {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Max upgrades already purchased"));
        }
    }

    public void onShieldPurchase() {
        resetCoroutine();
        if (canAfford(shieldCost) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false));
        }
        else if (playerMovement.player.getShieldHP() == 50) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Shield already maxed out"));
        }
        else { // success
            makeTransaction(shieldCost);
            playerMovement.player.shieldBuff();
            feedbackTimer = StartCoroutine(displayTransactionStatus(true));
            updateShieldDisplay();
            updateCoinsDisplay();
        }
    }

    public void onLaserRiflePurchase() {
        resetCoroutine();
        bool hasLaserRifle = false;
        foreach (gunStats gun in playerMovement.player.getGunList()) { // Check if player can afford the laser rifle & doesn't already have it
            if (gun.weaponName == gunStats.ObjectType.LaserRifle) {
                hasLaserRifle = true;
                break;
            }
        }
        if (canAfford(laserRifleCost) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false));
        }
        else if (hasLaserRifle) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Error: Laser Rifle already purchased"));
        }
        else { // success
            makeTransaction(laserRifleCost);
            giveLaserRifle();
            feedbackTimer = StartCoroutine(displayTransactionStatus(true));
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

    public void resetCoroutine() {
        if (feedbackTimer != null) {
            StopCoroutine(feedbackTimer);
            feedbackTimer = null;
        }
    }

    void giveHealth() {
        playerMovement.player.setHP(playerMovement.player.getHPOrig()); // Heal the player to full as per their purchase & update the UI
        playerMovement.player.updatePlayerUI();
        updateHealthDisplay();
        updateCoinsDisplay();
    }
    
    void giveAmmo(int gunPosition) {
        playerMovement.player.getSpecificGun(gunPosition).ammoCur = playerMovement.player.getSpecificGun(gunPosition).ammoMag;
        playerMovement.player.getSpecificGun(gunPosition).ammoMax = playerMovement.player.getSpecificGun(gunPosition).ammoOrig;
        playerMovement.player.updatePlayerUI();
        updateCoinsDisplay();
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
        t_healthCostText.text = healthCost.ToString();
        t_healthCostText.text += " coins";
        if (canAfford(healthCost))
            t_healthCostText.color = Color.green;
        else
            t_healthCostText.color = Color.red;
    }

    void updateHealPotionDisplay() {
        healPotionCostText.text = healPotionCost.ToString();
        healPotionCostText.text += " coins";
        if (canAfford(healPotionCost))
            healPotionCostText.color = Color.green;
        else
            healPotionCostText.color = Color.red;
    }

    void updateGrenadeDisplay() {
        grenadeCostText.text = grenadeCost.ToString();
        grenadeCostText.text += " coins";
        if (canAfford(grenadeCost))
            grenadeCostText.color = Color.green;
        else
            grenadeCostText.color = Color.red;
    }

    void updateLaserRifleDisplay() {
        t_laserRifleCostText.text = laserRifleCost.ToString("F0");
        t_laserRifleCostText.text += " coins";
        if (canAfford(laserRifleCost))
            t_laserRifleCostText.color = Color.green;
        else
            t_laserRifleCostText.color = Color.red;
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

    void updateInventoryUpgradeDisplay(int upgradeV) {
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




    void updateAmmoDisplays(string ammo = "") { // if not name is passed in, all buttons will be updated
        if (ammo == "AR" || ammo == "") {
            ARAmmoCostText.text = ammoCost.ToString("F0");
            ARAmmoCostText.text += " coins";
            if (canAfford(ammoCost)) {
                ARAmmoCostText.color = Color.green;
            }
            else {
                ARAmmoCostText.color = Color.red;
            }
        }
        if (ammo == "HandCannon" || ammo == "") {
            HandCannonAmmoCostText.text = ammoCost.ToString("F0");
            HandCannonAmmoCostText.text += " coins";
            if (canAfford(ammoCost)) {
                HandCannonAmmoCostText.color = Color.green;
            }
            else {
                HandCannonAmmoCostText.color = Color.red;
            }
        }
        if (ammo == "Shotgun" || ammo == "") {
            ShotgunAmmoCostText.text = ammoCost.ToString("F0");
            ShotgunAmmoCostText.text += " coins";
            if (canAfford(ammoCost)) {
                ShotgunAmmoCostText.color = Color.green;
            }
            else {
                ShotgunAmmoCostText.color = Color.red;
            }
        }
        if (ammo == "Sniper" || ammo == "") {
            SniperAmmoCostText.text = ammoCost.ToString("F0");
            SniperAmmoCostText.text += " coins";
            if (canAfford(ammoCost)) {
                SniperAmmoCostText.color = Color.green;
            }
            else {
                SniperAmmoCostText.color = Color.red;
            }
        }
        if (ammo == "Pistol" || ammo == "") {
            PistolAmmoCostText.text = ammoCost.ToString("F0");
            PistolAmmoCostText.text += " coins";
            if (canAfford(ammoCost)) {
                PistolAmmoCostText.color = Color.green;
            }
            else {
                PistolAmmoCostText.color = Color.red;
            }
        }
        if (ammo == "SMG" || ammo == "") {
            SMGAmmoCostText.text = ammoCost.ToString("F0");
            SMGAmmoCostText.text += " coins";
            if (canAfford(ammoCost)) {
                SMGAmmoCostText.color = Color.green;
            }
            else {
                SMGAmmoCostText.color = Color.red;
            }
        }
    }


    public void onAmmoPurchase(string ammo) {
        resetCoroutine();
        if (canAfford(ammoCost) == false) {
            feedbackTimer = StartCoroutine(displayTransactionStatus(false));
        }
        else if (ammo == "AR") {
            if (ARPos == -1) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "You don't have this gun"));
            }
            else if (playerMovement.player.getSpecificGun(ARPos).ammoMax == playerMovement.player.getSpecificGun(ARPos).ammoOrig && playerMovement.player.getSpecificGun(ARPos).ammoCur == playerMovement.player.getSpecificGun(ARPos).ammoMag) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Ammo already full"));
            }
            else { // success
                makeTransaction(ammoCost);
                giveAmmo(ARPos);
                updateAmmoDisplays(ammo);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
            }
        }
        else if (ammo == "HandCannon") {
            if (HandCannonPos == -1) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "You don't have this gun"));
            }
            else if (playerMovement.player.getSpecificGun(HandCannonPos).ammoMax == playerMovement.player.getSpecificGun(HandCannonPos).ammoOrig && playerMovement.player.getSpecificGun(HandCannonPos).ammoCur == playerMovement.player.getSpecificGun(HandCannonPos).ammoMag) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Ammo already full"));
            }
            else { // success
                makeTransaction(ammoCost);
                giveAmmo(HandCannonPos);
                updateAmmoDisplays(ammo);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
            }
        }
        else if (ammo == "Pistol") {
            if (PistolPos == -1) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "You don't have this gun"));
            }
            else if (playerMovement.player.getSpecificGun(PistolPos).ammoMax == playerMovement.player.getSpecificGun(PistolPos).ammoOrig && playerMovement.player.getSpecificGun(PistolPos).ammoCur == playerMovement.player.getSpecificGun(PistolPos).ammoMag) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Ammo already full"));
            }
            else { // success
                makeTransaction(ammoCost);
                giveAmmo(PistolPos);
                updateAmmoDisplays(ammo);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
            }
        }
        else if (ammo == "Shotgun") {
            if (ShotgunPos == -1) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "You don't have this gun"));
            }
            else if (playerMovement.player.getSpecificGun(ShotgunPos).ammoMax == playerMovement.player.getSpecificGun(ShotgunPos).ammoOrig && playerMovement.player.getSpecificGun(ShotgunPos).ammoCur == playerMovement.player.getSpecificGun(ShotgunPos).ammoMag) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Ammo already full"));
            }
            else { // success
                makeTransaction(ammoCost);
                giveAmmo(ShotgunPos);
                updateAmmoDisplays(ammo);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
            }
        }
        else if (ammo == "Sniper") {
            if (SniperPos == -1) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "You don't have this gun"));
            }
            else if (playerMovement.player.getSpecificGun(SniperPos).ammoMax == playerMovement.player.getSpecificGun(SniperPos).ammoOrig && playerMovement.player.getSpecificGun(SniperPos).ammoCur == playerMovement.player.getSpecificGun(SniperPos).ammoMag) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Ammo already full"));
            }
            else { // success
                makeTransaction(ammoCost);
                giveAmmo(SniperPos);
                updateAmmoDisplays(ammo);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
            }
        }
        else if (ammo == "SMG") {
            if (SMGPos == -1) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "You don't have this gun"));
            }
            else if (playerMovement.player.getSpecificGun(SMGPos).ammoMax == playerMovement.player.getSpecificGun(SMGPos).ammoOrig && playerMovement.player.getSpecificGun(SMGPos).ammoCur == playerMovement.player.getSpecificGun(SMGPos).ammoMag) {
                feedbackTimer = StartCoroutine(displayTransactionStatus(false, "Ammo already full"));
            }
            else { // success
                makeTransaction(ammoCost);
                giveAmmo(SMGPos);
                updateAmmoDisplays(ammo);
                feedbackTimer = StartCoroutine(displayTransactionStatus(true));
            }
        }
    }

    public void setGunPos(int pos, gunStats.ObjectType weapon) {
        if (weapon == gunStats.ObjectType.AR) {
            ARPos = pos;
        }
        else if (weapon == gunStats.ObjectType.HandCannon) {
            HandCannonPos = pos;
        }
        else if (weapon == gunStats.ObjectType.Shotgun) {
            ShotgunPos = pos;
        }
        else if (weapon == gunStats.ObjectType.Sniper) {
            SniperPos = pos;
        }
        else if (weapon == gunStats.ObjectType.Pistol) {
            PistolPos = pos;
        }
        else if (weapon == gunStats.ObjectType.SMG) {
            SMGPos = pos;
        }
    }
}
