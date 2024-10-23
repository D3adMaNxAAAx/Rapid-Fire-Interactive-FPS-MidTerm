using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour {

    // stateIndex will have the name of the scene to go to.
    string stateIndex;

    private void Start()
    {
        stateIndex = SceneManager.GetActiveScene().name;
        if (stateIndex == "MainMenu")
            stateIndex = "Level 1";
    }

    public void confirmRestart()
    {
        if (playerStats.Stats != null)
            playerStats.Stats.Reset();

        gameManager.instance.getTimerTracker().SetActive(false);

        if (uiManager.manager != null)
            Destroy(uiManager.manager.gameObject);

        if (playerMovement.player != null)
            Destroy(playerMovement.player.gameObject);

        if (playerStats.Stats != null)
            Destroy(playerStats.Stats.gameObject);

        SceneManager.LoadScene(stateIndex, LoadSceneMode.Single);
        gameManager.instance.stateUnpause();
    }

    public void cancel()
    {
        back();
    }

    public void resume() {
        gameManager.instance.stateUnpause();
    }

    public void restart() {
        gameManager.instance.openConfirmationMenu();
    }

    public void quitMenu()
    {
        gameManager.instance.openQuitMenu();
    }

    public void options() {
        gameManager.instance.openOptionsMenu();
    }

    public void HowTo() {
        gameManager.instance.openHowToMenu();
    }

    public void Tips() {
        gameManager.instance.openTipsMenu();
    }

    public void Stats() {
        gameManager.instance.openStatsMenu();
    }

    public void ControlsMenu() {
        gameManager.instance.openControlsMenu();
    }

    public void settings() {
        gameManager.instance.openSettingsMenu();
    }

    public void onUpgradeMenu() {
        gameManager.instance.openUpgradeMenu();
    }

    public void onTerminalUpgradeMenu()
    {
        gameManager.instance.openTerminalUpgradeMenu();
    }

    public void continueGame() {
        gameManager.instance.nextRoomContinue();
    }

    public void storeMenu() {
        gameManager.instance.openStoreMenu();
    }

    public void onTerminalStoreMenu()
    {
        gameManager.instance.openTerminalStoreMenu();
    }

    public void quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void mainMenu()
    {
        // Hide UI
        gameManager.instance.getMenuActive().SetActive(false);
        gameManager.instance.setMenuActive(null);

        // Wipe Player Data
        Destroy(playerMovement.player.gameObject);
        Destroy(uiManager.manager.gameObject);
        Destroy(playerStats.Stats.gameObject);

        // Unpause
        Time.timeScale = gameManager.instance.getTimeScaleOrig();

        // Load Main Menu
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void invertY() {
        if (CameraMovement.state != null)
        CameraMovement.state.invert();

    }

    bool timerOn = true;
    public void toggleTimer()
    {
        if (gameManager.instance != null)
        {
            timerOn = !timerOn;
            gameManager.instance.getTimer().SetActive(timerOn);
        }
    }

    public void back() {
        if (gameManager.instance != null)
            gameManager.instance.backButton();
    }

    public void toggleSprint()
    {
        if (playerMovement.player != null)
            if (playerMovement.player.getStamina() >= (playerMovement.player.getStaminaOrig() / 2))
            {
                playerMovement.player.toggleSprintOn();

            }
    }

    public void toggleZoom() {
        if (CameraMovement.state != null)
        CameraMovement.state.autoZoom();
    }

    // Upgrade Menu Buttons:
    public void healthUpgrade() {
        upgradeMenu.upgradeUI.onHealthUpgrade();
    }

    public void damageUpgrade() {
        upgradeMenu.upgradeUI.onDamageUpgrade();
    }

    public void speedUpgrade() {
        upgradeMenu.upgradeUI.onSpeedUpgrade();
    }

    public void staminaUpgrade() {
        upgradeMenu.upgradeUI.onStaminaUpgrade();
    }

    public void healthPurchase() {
        storeManager.instance.onHealthPurchase();
    }

    public void ammoPurchase()
    {
        storeManager.instance.onAmmoPurchase();
    }

    public void laserRiflePurchase()
    {
        storeManager.instance.onLaserRiflePurchase();
    }

    public void Respawn()
    {
        playerMovement.player.spawnPlayer();
        gameManager.instance.stateUnpause();
    }

    // When the player's cursor is over a button
    public void playHoverSound()
    {
        audioManager.instance.PlayButtonHover();
    }

    public void playButtonSound()
    {
        audioManager.instance.PlayButtonClick();
    }

    public void openLoseStatsMenu()
    {
        gameManager.instance.openLoseStatsMenu(); 
    }

}
