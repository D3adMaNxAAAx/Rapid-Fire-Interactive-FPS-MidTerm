using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject mainSceneStuff;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject howToPlayMenu;
    private GameObject currentMenu;

    public void startGame()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    public void optionsMenu() {
        menuOptions.SetActive(true);
    }

    public void optionsBack() {
        menuOptions.SetActive(false);
    }

    public void settingsMenu() {
        menuOptions.SetActive(false);
        menuSettings.SetActive(true);
    }

    public void settingsBack() {
        menuSettings.SetActive(false);
        menuOptions.SetActive(true);
    }

    public void controlMenu()
    {
        menuOptions.SetActive(false);
        controlsMenu.SetActive(true);
        currentMenu = controlsMenu;
    }
    public void howToPlay()
    {
        menuOptions.SetActive(false);
        howToPlayMenu.SetActive(true);
        currentMenu = howToPlayMenu;
    }
    public void backButton()
    {
        currentMenu.SetActive(false);
        menuOptions.SetActive(true);
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
