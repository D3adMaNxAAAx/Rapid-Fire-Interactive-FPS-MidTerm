using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerStats : MonoBehaviour {

    public static playerStats Stats; // singleton

    // stats being tracked
    int enemiesKilled; 
    int headShots; 
    float damageDealt; 
    float damageTaken; 
    int maxLevel;
    int pwrLvl;
    int totalXP; 
    int totalMoney;
    int deaths; 
    int nearDeaths; 
    int ammoUsed; 
    int upgradesPurchased; 
    int shopItemsPurchased; 
    string completionTime = "N/A";
    int idBadgesFound;
    int powerObjectsFound; 
    int docsFound; 
    int collectablesFound;

    // unused stats
    //float percentFound1; int maxPowerObjects = 9; // in relation to powerObjectsFound
    //float percentFound2; int maxDocs = 9; // in relation to docsFound

    // stats being tracked

    void Awake() {
        if (Stats == null){
            DontDestroyOnLoad(gameObject); // this script will stay between scenes
            Stats = this;
        }
        else if(Stats != null){
            Destroy(this.gameObject);
        }
    }

    public void Reset() { // reseting all tracked stats to 0
        enemiesKilled = headShots = maxLevel = totalXP = totalMoney = deaths = nearDeaths = ammoUsed = upgradesPurchased = shopItemsPurchased = powerObjectsFound = docsFound = 0;
        damageDealt = damageTaken = collectablesFound = 0;
        completionTime = "N/A";
    }

    // setters (for tracking):
    public void enemyKilled() {
        enemiesKilled++;
        gameManager.instance.updateGameGoal(-1); }

    public void enemyHeadShot() { 
        headShots++; }

    public void attack(float damage) {
        damageDealt += damage; }

    public void attacked(float damage) {
        damageTaken += damage; }

    public void levelUp() {
        maxLevel++; }

    public void pwrLevel(int _pwrLevel) {
        pwrLvl = _pwrLevel; }

    public void gotXP(int xp) {
        totalXP += xp; }

    public void gotMoney(int money) {
        totalMoney += money; }

    public void died() {
        deaths++; }

    public void almostDied() {
        nearDeaths++; }

    public void gunShot() {
        ammoUsed++; }

    public void upgraded() {
        upgradesPurchased++; }

    public void purchased() {
        shopItemsPurchased++; }

    public void currentTime(string time) {
        completionTime = time;
    }

    public void objectFound() {
        powerObjectsFound++; }

    public void idBadgeFound() {
        idBadgesFound++; }

    public void docFound() {
        docsFound++; }

    public void collectableFound() {
        collectablesFound++; }

    // getters (for displaying)
    public int getEnemiesKilled() { return enemiesKilled; }
    public int getHeadShots() { return headShots; }
    public float getDamageDealt() { return damageDealt; }
    public float getDamageTaken() { return damageTaken; }
    public int getLevel() { return maxLevel; }
    public int getPWRLevel() { return pwrLvl; }
    public int getTotalXP() { return totalXP; }
    public int getTotalMoney() { return totalMoney; }
    public int getDeaths() { return deaths; }
    public int getNearDeaths() { return nearDeaths; }
    public int getAmmoUsed() { return ammoUsed; }
    public int getUpgradesCount() { return upgradesPurchased; }
    public int getShopPurchases() { return shopItemsPurchased; }
    public string getTimeTaken() { return completionTime; }
    public int getPowerObjects() { return powerObjectsFound; }
    public int getNotesFound() { return docsFound; }
    public int getBadgesFound() { return idBadgesFound; }
    public int getCollectablesFound() {
        return collectablesFound * 10; }

    public void resetOBJStats()
    {
        idBadgesFound = 0;
        gameManager.instance.resetPwrItems();
        powerObjectsFound = 0;
        pwrLvl = 0;
    }
}
