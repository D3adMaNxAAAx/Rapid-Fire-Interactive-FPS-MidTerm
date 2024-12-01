using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class statsMenu : MonoBehaviour {

    public static statsMenu statDisplays; // singleton, will update both lose and win?

    [SerializeField] TMP_Text enemiesKilled;
    [SerializeField] TMP_Text damageDealt;
    [SerializeField] TMP_Text lostDocsHeld;
    [SerializeField] TMP_Text totalMoneyEarnedCount;
    [SerializeField] TMP_Text damageReceived;
    [SerializeField] TMP_Text playerDeathCount;
    [SerializeField] TMP_Text totalXPEarnedCount;
    [SerializeField] TMP_Text secretsFound;
    [SerializeField] TMP_Text ammoUsed;
    [SerializeField] TMP_Text playerNearDeathCount;
    [SerializeField] TMP_Text playerLevelStat;
    [SerializeField] TMP_Text headShots;
    [SerializeField] TMP_Text compTime;
    [SerializeField] TMP_Text upgrades;
    [SerializeField] TMP_Text shopPurchases;

    void Awake() {
        statDisplays = this;
        this.gameObject.SetActive(false);
    }

    public void updateStats() {
        enemiesKilled.text = playerStats.Stats.getEnemiesKilled().ToString("F0");
        damageDealt.text = playerStats.Stats.getDamageDealt().ToString("F0");
        lostDocsHeld.text = playerStats.Stats.getNotesFound().ToString("F0");
        totalMoneyEarnedCount.text = playerStats.Stats.getTotalMoney().ToString("F0");
        damageReceived.text = playerStats.Stats.getDamageTaken().ToString("F0");
        playerDeathCount.text = playerStats.Stats.getDeaths().ToString("F0");
        totalXPEarnedCount.text = playerStats.Stats.getTotalXP().ToString("F0");
        secretsFound.text = playerStats.Stats.getCollectablesFound().ToString("F0") + "%";
        ammoUsed.text = playerStats.Stats.getAmmoUsed().ToString("F0");
        playerNearDeathCount.text = playerStats.Stats.getNearDeaths().ToString("F0");
        playerLevelStat.text = playerStats.Stats.getLevel().ToString("F0");
        headShots.text = playerStats.Stats.getHeadShots().ToString("F0");
        compTime.text = playerStats.Stats.getTimeTaken();
        upgrades.text = playerStats.Stats.getUpgradesCount().ToString("F0");
        shopPurchases.text = playerStats.Stats.getShopPurchases().ToString("F0");
    }

}
