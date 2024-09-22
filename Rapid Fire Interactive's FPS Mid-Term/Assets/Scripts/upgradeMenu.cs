using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class upgradeMenu : MonoBehaviour {

    public static upgradeMenu upgradeUI; // singleton

    [SerializeField] TMP_Text healthRankText;
    [SerializeField] TMP_Text healthUpgradeText;
    [SerializeField] TMP_Text damageRankText;
    [SerializeField] TMP_Text damageUpgradeText;
    [SerializeField] TMP_Text speedRankText;
    [SerializeField] TMP_Text speedUpgradeText;
    [SerializeField] TMP_Text staminaRankText;
    [SerializeField] TMP_Text staminaUpgradeText;

    int healthRank = 0;
    int damageRank = 0;
    int speedRank = 0;
    int staminaRank = 0;
    int HP;
    float damageMod = 1;
    float speed;
    int stamina;

    // Start is called before the first frame update
    void Start() {
        upgradeUI = this;
        HP = playerMovement.player.getHP();
        speed = playerMovement.player.getSpeed();
        stamina = playerMovement.player.getStamina();
    }

    public void onHealthUpgrade() {
        healthRank++;
        healthRankText.text = healthRank.ToString();
        HP += 10;
        playerMovement.player.setHP(HP);
    }

    public void onDamageUpgrade() {
        damageRank++;
        damageRankText.text = damageRank.ToString();
        damageMod += 0.2f;
        playerMovement.player.setDamageMod(damageMod);
    }

    public void onSpeedUpgrade() {
        speedRank++;
        speedRankText.text = speedRank.ToString();
        speed += 1;
        playerMovement.player.setSpeed(speed);
    }

    public void onStaminaUpgrade() {
        staminaRank++;
        staminaRankText.text = staminaRank.ToString();
        stamina += 5;
        playerMovement.player.setStamina(staminaRank);
    }
}
