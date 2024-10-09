using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    [SerializeField] GameObject menuOptions;

    public void startGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void optionsMenu() {
        menuOptions.SetActive(true);
    }

    public void optionsBack() {
        menuOptions.SetActive(false);
    }

    public void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
