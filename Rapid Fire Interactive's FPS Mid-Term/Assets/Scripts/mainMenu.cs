using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    [Header("-- Menus --")]
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject mainSceneStuff;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject howToPlayMenu;

    [Header("-- First Selected Buttons --")]
    [SerializeField] GameObject mainMenuFirst;
    [SerializeField] GameObject optionsMenuFirst;
    [SerializeField] GameObject settingsMenuFirst;
    [SerializeField] GameObject howToPlayMenuFirst;
    [SerializeField] GameObject controlsMenuFirst;

    private GameObject currentMenu;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    public void startGame()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    public void optionsMenu() {
        menuOptions.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsMenuFirst);
    }

    public void optionsBack() {
        menuOptions.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    public void settingsMenu() {
        menuOptions.SetActive(false);
        menuSettings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsMenuFirst);
    }

    public void settingsBack() {
        menuSettings.SetActive(false);
        menuOptions.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsMenuFirst);
    }

    public void controlMenu()
    {
        menuOptions.SetActive(false);
        controlsMenu.SetActive(true);
        currentMenu = controlsMenu;
        EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
    }
    public void howToPlay()
    {
        menuOptions.SetActive(false);
        howToPlayMenu.SetActive(true);
        currentMenu = howToPlayMenu;
        EventSystem.current.SetSelectedGameObject(howToPlayMenuFirst);
    }
    public void backButton()
    {
        currentMenu.SetActive(false);
        menuOptions.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsMenuFirst);
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
