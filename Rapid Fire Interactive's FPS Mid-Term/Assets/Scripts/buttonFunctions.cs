using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour {
    string stateIndex;
    private void Start()
    {
        stateIndex = SceneManager.GetActiveScene().name;
        if (stateIndex == "MainMenu")
            stateIndex = "Level 1";
    }
    public void confirm()
    {
        Debug.Log("Confirm");
    }

    public void cancel()
    {
        Debug.Log("Cancel");
    }

    public void resume() {
        gameManager.instance.stateUnpause();
    }

    public void restart() {
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

    public void ContorlsMenu() {
        gameManager.instance.openControlsMenu();
    }

    public void settings() {
        gameManager.instance.settingsMenu();
    }

    public void onUpgradeMenu() {
        gameManager.instance.openUpgradeMenu();
    }

    public void continueGame() {
        gameManager.instance.nextRoomContinue();
    }

    public void storeMenu() {
        gameManager.instance.storeMenu();
    }

    public void quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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
        gameManager.instance.getPlayerScript().getAudio().PlayOneShot(audioManager.instance.audButtonHover, audioManager.instance.audButtonHoverVol);
    }

    // When the player selects a button
    public void playButtonSound()
    {
        gameManager.instance.getPlayerScript().getAudio().PlayOneShot(audioManager.instance.audButtonClick, audioManager.instance.audButtonClickVol);
    }

    // TO-DO: IMPLEMENT SPECIAL SOUNDS FOR STORE & UPGRADE

}
