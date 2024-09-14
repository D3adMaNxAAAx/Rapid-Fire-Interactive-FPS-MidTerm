using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
   

    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }


    public void settings()
    {
        gameManager.instance.settingsMenu();
    }

    public void upgradeMenu()
    {
        gameManager.instance.upgradeMenu();
    }

    public void continueMenu()
    {
        gameManager.instance.completeMenu();
    }

    public void storeMenu()
    {
        gameManager.instance.storeMenu();
    }

    public void quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

        #else
            Application.Quit();

        #endif
    }

    public void invertY()
    {
        CameraMovement.state.invert();

    }


    public void back()
    {
        gameManager.instance.backButton();
    }


}
