using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerStats : MonoBehaviour {

    public static playerStats Stats; // singleton

    // stats being tracked
    int enemiesKilled; 
    int headShots; // if implemented
    float damageDealt; 
    float damageTaken; 
    int maxLevel;
    int totalXP; 
    int totalMoney;
    int deaths; 
    int nearDeaths; 
    int ammoUsed; 
    int upgradesPurchased;
    int shopItemsPurchased;
    string completionTime = "N/A";
    int powerObjectsFound; float percentFound1; int maxPowerObjects = 9;
    int notesFound; float percentFound2; int maxNotes = 10; /// prob needs to be changed
    int collectablesFound; float percentFound3; int maxCollectables = 10; // if implemented
    // stats being tracked

    void Start() {
        /// don't destroy on load

        Stats = this;
    }

    public void Reset() { // reseting all tracked stats to 0
        enemiesKilled = headShots = maxLevel = totalXP = totalMoney = deaths = nearDeaths = ammoUsed = upgradesPurchased = shopItemsPurchased = powerObjectsFound = notesFound = collectablesFound = 0;
        damageDealt = damageTaken = 0;
        completionTime = "N/A";
    }

    public void enemyKilled() {
        enemiesKilled++; }

    public void enemyHeadShot() { /// needs to be implemented
        headShots++; }

    public void attack(float damage) {
        damageDealt += damage; }

    public void attacked(float damage) {
        damageTaken += damage; }

    public void levelUp() {
        maxLevel++; }

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

    public void objectFound() { /// needs to be implemented
        powerObjectsFound++; }

    public void noteFound() { /// needs to be implemented
        notesFound++; }

    public void collectableFound() { /// needs to be implemented
        collectablesFound++; }

    // getters
    public int getEnemiesKilled() { return enemiesKilled; }
    // public void getHeadShots() { return headShots; }
    public float getDamageDealt() { return damageDealt; }
    public float getDamageTaken() { return damageTaken; }
    public int getLevel() { return maxLevel; }
    public int getTotalXP() { return totalXP; }
    public int getTotalMoney() { return totalMoney; }
    public int getDeaths() { return deaths; }
    public int getNearDeaths() { return nearDeaths; }
    public int getAmmoUsed() { return ammoUsed; }
    public int getUpgradesCount() { return upgradesPurchased; }
    public int getShopPurchases() { return shopItemsPurchased; }
    public string getTimeTaken() { return completionTime; }
    public int getPowerObjects() { return powerObjectsFound; }
    public int getNotesFound() { return notesFound; }
    /*public float getCollectablesFound() {
        percentFound3 = collectablesFound / maxCollectables;
        return percentFound3; }*/
    // getters
}