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
    float damageRecieved;
    int maxLevel;
    int totalXP;
    int totalMoney;
    int deaths;
    int nearDeaths;
    int ammoUsed;
    int upgradesPurchased;
    int shopItemsPurchased;
    string completionTime;
    int powerObjectsFound; float percentFound1; int maxPowerObjects;
    int notesFound; float percentFound2; int maxNotes;
    int collectablesFound; float percentFound3; int maxCollectables; // if implemented
    // stats being tracked

    void Start() {
        /// don't destroy on load

        Stats = this;
    }

    public void Reset() {
        /// yo = yoyo = yoyoyo = 0;
    }

    public void collectableFound() {
        collectablesFound++;
    }

    public void currentTime(string time) {
        completionTime = time;
    }

    // getters
    public int getEnemiesKilled() { return enemiesKilled; }
    // public void getHeadShots() { return headShots; }

    public float getCollectablesFound() {
        percentFound3 = collectablesFound / maxCollectables;
        return percentFound3; }

    // getters

}
