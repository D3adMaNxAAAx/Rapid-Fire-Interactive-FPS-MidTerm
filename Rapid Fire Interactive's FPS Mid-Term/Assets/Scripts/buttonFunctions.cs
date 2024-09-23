using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour {
   
    public void resume() {
        gameManager.instance.stateUnpause();
    }

    public void restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void settings() {
        gameManager.instance.settingsMenu();
    }

    public void onUpgradeMenu() {
        gameManager.instance.upgradeMenu();
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
        CameraMovement.state.invert();

    }

    public void back() {
        gameManager.instance.backButton();
    }

    public void toggleSprint() 
    {
        if (playerMovement.player.getStamina() >= (playerMovement.player.getStaminaOrig() / 2))
        {
            playerMovement.player.toggleSprintOn();

        }
       
    }


    public void toggleZoom() {
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
}
