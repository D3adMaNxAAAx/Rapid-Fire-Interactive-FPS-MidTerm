using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder.MeshOperations;

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

    [Header("-- Terminal Store Components --")]
    [SerializeField] TMP_Text t_playerSkillPoints;
    [SerializeField] TMP_Text t_healthRankText;
    [SerializeField] TMP_Text t_healthUpgradeText;
    [SerializeField] TMP_Text t_damageRankText;
    [SerializeField] TMP_Text t_damageUpgradeText;
    [SerializeField] TMP_Text t_speedRankText;
    [SerializeField] TMP_Text t_speedUpgradeText;
    [SerializeField] TMP_Text t_staminaRankText;
    [SerializeField] TMP_Text t_staminaUpgradeText;

    [Header("Upgrade Costs")]
    [SerializeField] int healthUpgradeCost;
    [SerializeField] int damageUpgradeCost;
    [SerializeField] int speedUpgradeCost;
    [SerializeField] int staminaUpgradeCost;

    int healthRank = 0;
    int damageRank = 0;
    int speedRank = 0;
    int staminaRank = 0;
    float HPOrig;
    float damageMod = 1;
    float speed;
    int stamina;
    bool terminal = false;

    void Start() 
    {
        upgradeUI = this;
    }

    public void setVars() { // wouldn't work in start
        HPOrig = playerMovement.player.getHPOrig();
        speed = playerMovement.player.getNormOGSpeed();
        stamina = playerMovement.player.getStaminaOrig();

        // Rank Display
        healthRankText.text = healthRank.ToString();
        damageRankText.text = damageRank.ToString();
        speedRankText.text = speedRank.ToString();
        staminaRankText.text = staminaRank.ToString();

        // Stat Display
        healthUpgradeText.text = HPOrig.ToString() + " >> " + (HPOrig + 5).ToString();
        damageUpgradeText.text = damageMod.ToString() + " >> " + (damageMod + 0.1f).ToString();
        speedUpgradeText.text = speed.ToString() + " >> " + (speed + 0.5).ToString();
        staminaUpgradeText.text = stamina.ToString() + " >> " + (stamina + 5).ToString();
        playerSkillPoints.text = playerMovement.player.getSkillPoints().ToString();
        terminal = false;
    }

    public void setTVars()
    {
        HPOrig = playerMovement.player.getHPOrig();
        speed = playerMovement.player.getNormOGSpeed();
        stamina = playerMovement.player.getStaminaOrig();

        // Rank Display
        t_healthRankText.text = healthRank.ToString();
        t_damageRankText.text = damageRank.ToString();
        t_speedRankText.text = speedRank.ToString();
        t_staminaRankText.text = staminaRank.ToString();

        // Stat Display
        t_healthUpgradeText.text = HPOrig.ToString() + " >> " + (HPOrig + 5).ToString();
        t_damageUpgradeText.text = damageMod.ToString("F2") + " >> " + (damageMod + 0.1f).ToString("F2");
        t_speedUpgradeText.text = speed.ToString() + " >> " + (speed + 0.5f).ToString();
        t_staminaUpgradeText.text = stamina.ToString() + " >> " + (stamina + 5).ToString();
        t_playerSkillPoints.text = playerMovement.player.getSkillPoints().ToString();
        terminal = true;
    }

    // Transaction Methods
    bool canAfford(int _cost)
    {
        bool _state;

        // Check if the player has enough to make the purchase
        if (playerMovement.player.getSkillPoints() >= _cost)
            _state = true; // Player can afford the purchase, return true
        else _state = false;

        return _state;
    }

    void makeTransaction(int _cost)
    {
        // Designated function just in case transactions may be more deliberate
        // Method is called if canAfford returns true so player can afford something
        playerMovement.player.setSkillPoints(playerMovement.player.getSkillPoints() - _cost);
        playerStats.Stats.upgraded();

        // Update Skill Points text
        if (!terminal)
            playerSkillPoints.text = playerMovement.player.getSkillPoints().ToString();
        else
            t_playerSkillPoints.text = playerMovement.player.getSkillPoints().ToString();

    }

    // Upgrade Methods -- These methods will be called on button press
    // The methods will check if the player can afford them based on the costs of Skill Points, using the transaction methods.
    public void onHealthUpgrade() {

        if (canAfford(healthUpgradeCost) && healthRank < 5) {
            makeTransaction(healthUpgradeCost);
            healthRank++;

            // Health Rank Text
            if (!terminal)
                healthRankText.text = healthRank.ToString();
            else
                t_healthRankText.text = healthRank.ToString();
            
            HPOrig = HPOrig + 10;
            
            // Health Display
            if (!terminal)
                healthUpgradeText.text = HPOrig.ToString() + " >> " + (HPOrig + 5).ToString();
            else
                t_healthUpgradeText.text = HPOrig.ToString() + " >> " + (HPOrig + 5).ToString();

            playerMovement.player.setHPOrig(HPOrig);

            // Refill players health by 10 too to accomodate the new HP.
            playerMovement.player.setHP(playerMovement.player.getHP() + 5);

            playerMovement.player.updatePlayerUI();
        }
    }

    public void onDamageUpgrade() {
        if (canAfford(damageUpgradeCost) && damageRank < 5) {
            makeTransaction(damageUpgradeCost);
            damageRank++;

            // Damage Rank Text
            if (!terminal)
                damageRankText.text = damageRank.ToString();
            else
                t_damageRankText.text = damageRank.ToString();

            damageMod += 0.1f;

            // Damage Display
            if (!terminal)
                damageUpgradeText.text = damageMod.ToString() + " >> " + (damageMod + 0.1f).ToString();
            else
                t_damageUpgradeText.text = damageMod.ToString() + " >> " + (damageMod + 0.1f).ToString();
            
            playerMovement.player.setDamageMod(damageMod);
            playerMovement.player.updatePlayerUI();
        }
    }

    public void onSpeedUpgrade() {
        if (canAfford(speedUpgradeCost) && speedRank < 5)
        {
            makeTransaction(speedUpgradeCost);
            speedRank++;
            
            // Speed Rank Text
            if (!terminal)
                speedRankText.text = speedRank.ToString();
            else
                t_speedRankText.text = speedRank.ToString();

            speed = speed + 0.5f;

            // Speed Display
            if (!terminal)
                speedUpgradeText.text = speed.ToString() + " >> " + (speed + 0.5f).ToString();
            else
                t_speedUpgradeText.text = speed.ToString() + " >> " + (speed + 0.5f).ToString();

            playerMovement.player.setSpeed(speed);
            playerMovement.player.setNormOGSpeed(speed);
            playerMovement.player.updatePlayerUI();
        }
    }

    public void onStaminaUpgrade() {
        if (canAfford(staminaUpgradeCost) && staminaRank < 5)
        {
            makeTransaction(staminaUpgradeCost);
            staminaRank++;
            
            // Stamina Rank Text
            if (!terminal)
                staminaRankText.text = staminaRank.ToString();
            else
                t_staminaRankText.text = staminaRank.ToString();

            stamina = stamina + 5;

            // Stamina Display
            if (!terminal)
                staminaUpgradeText.text = stamina.ToString() + " >> " + (stamina + 5).ToString();
            else
                t_staminaUpgradeText.text = stamina.ToString() + " >> " + (stamina + 5).ToString();

            playerMovement.player.setOGStamina(stamina);
            playerMovement.player.updatePlayerUI();
        }
    }
}
