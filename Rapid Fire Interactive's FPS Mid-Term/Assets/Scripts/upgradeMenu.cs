using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class upgradeMenu : MonoBehaviour {

    public static upgradeMenu upgradeUI; // singleton

    [Header("-- Store Components --")]
    [SerializeField] TMP_Text playerSkillPoints;
    [SerializeField] TMP_Text healthRankText;
    [SerializeField] TMP_Text healthUpgradeText;
    [SerializeField] TMP_Text damageRankText;
    [SerializeField] TMP_Text damageUpgradeText;
    [SerializeField] TMP_Text speedRankText;
    [SerializeField] TMP_Text speedUpgradeText;
    [SerializeField] TMP_Text staminaRankText;
    [SerializeField] TMP_Text staminaUpgradeText;

    [Header("Upgrade Costs")]
    [SerializeField] int healthUpgradeCost;
    [SerializeField] int damageUpgradeCost;
    [SerializeField] int speedUpgradeCost;
    [SerializeField] int staminaUpgradeCost;

    int healthRank = 0;
    int damageRank = 0;
    int speedRank = 0;
    int staminaRank = 0;
    int HP;
    float damageMod = 1;
    float speed;
    int stamina;

    void Start() {
        upgradeUI = this;
    }

    public void setVars() { // wouldn't work in start
        HP = playerMovement.player.getHP();
        speed = playerMovement.player.getSpeed();
        stamina = playerMovement.player.getStamina();
        healthUpgradeText.text = HP.ToString() + " >> " + (HP + 10).ToString();
        damageUpgradeText.text = damageMod.ToString() + " >> " + (damageMod + 0.2f).ToString();
        speedUpgradeText.text = speed.ToString() + " >> " + (speed + 1).ToString();
        staminaUpgradeText.text = stamina.ToString() + " >> " + (stamina + 5).ToString();
        playerSkillPoints.text = playerMovement.player.getSkillPoints().ToString();
    }

    // Transaction Methods
    bool canAfford(int _cost)
    {
        bool _state;

        // Check if the player has enough to make the purchase
        if (gameManager.instance.getPlayerScript().getSkillPoints() >= _cost)
            _state = true; // Player can afford the purchase, return true
        else _state = false;

        return _state;
    }

    void makeTransaction(int _cost)
    {
        // Designated function just in case transactions may be more deliberate
        // Method is called if canAfford returns true so player can afford something
        gameManager.instance.getPlayerScript().setSkillPoints(gameManager.instance.getPlayerScript().getSkillPoints() - _cost);

        // Update Skill Points text
        playerSkillPoints.text = playerMovement.player.getSkillPoints().ToString();
    }

    // Upgrade Methods -- These methods will be called on button press
    // The methods will check if the player can afford them based on the costs of Skill Points, using the transaction methods.
    public void onHealthUpgrade() {

        if (canAfford(healthUpgradeCost)) {
            makeTransaction(healthUpgradeCost);
            healthRank++;
            healthRankText.text = healthRank.ToString();
            HP += 10;
            healthUpgradeText.text = HP.ToString() + " >> " + (HP + 10).ToString();
            playerMovement.player.setHP(HP);
        }
    }

    public void onDamageUpgrade() {
        if (canAfford(damageUpgradeCost))
        {
            makeTransaction(damageUpgradeCost);
            damageRank++;
            damageRankText.text = damageRank.ToString();
            damageMod += 0.2f;
            damageUpgradeText.text = damageMod.ToString() + " >> " + (damageMod + 0.2f).ToString();
            playerMovement.player.setDamageMod(damageMod);
        }
    }

    public void onSpeedUpgrade() {
        if (canAfford(speedUpgradeCost))
        {
            makeTransaction(speedUpgradeCost);
            speedRank++;
            speedRankText.text = speedRank.ToString();
            speed += 1;
            speedUpgradeText.text = speed.ToString() + " >> " + (speed + 1).ToString();
            playerMovement.player.setSpeed(speed);
        }
    }

    public void onStaminaUpgrade() {
        if (canAfford(staminaUpgradeCost))
        {
            makeTransaction(staminaUpgradeCost);
            staminaRank++;
            staminaRankText.text = staminaRank.ToString();
            stamina += 5;
            staminaUpgradeText.text = stamina.ToString() + " >> " + (stamina + 5).ToString();
            playerMovement.player.setStamina(stamina);
        }
    }
}
