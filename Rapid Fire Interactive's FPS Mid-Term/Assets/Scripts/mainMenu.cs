using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] GameObject tipsMenu;

    [Header("-- First Selected Buttons --")]
    [SerializeField] GameObject mainMenuFirst;
    [SerializeField] GameObject optionsMenuFirst;
    [SerializeField] GameObject settingsMenuFirst;
    [SerializeField] GameObject howToPlayMenuFirst;
    [SerializeField] GameObject tipsMenuFirst;
    [SerializeField] GameObject controlsMenuFirst;

    [Header("-- Quit Button --")]
    [SerializeField] Button quitButton;

    private GameObject currentMenu;
    bool isTransitioning;

    private void Start()
    {
        // Set menu vars
        currentMenu = null;
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);

        // Check if on WebGL and disable quit if so
        if (quitButton != null && Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.interactable = false;
        }
    }

    public void startGame()
    {
        if (!isTransitioning)
        {
            SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        }
    }

    public void optionsMenu() {
        if (!isTransitioning)
        {
            // Set currentMenu then start it at 0 alpha and set things up
            currentMenu = menuOptions;
            currentMenu.GetComponent<CanvasRenderer>().SetAlpha(0f);
            currentMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(optionsMenuFirst);

            // Everything set up, fade in the menu
            StartCoroutine(transition(currentMenu, true));
        }
    }

    public void optionsBack() {
        if (!isTransitioning)
        {
            // Fade out the options menu
            StartCoroutine(transition(currentMenu, false));

            // Reset the opacity
            // Turning off the buttons needs to be in the coroutine because of how fast code is going

            // Back to start screen
            EventSystem.current.SetSelectedGameObject(mainMenuFirst);
            currentMenu = null;
        }
    }

    public void settingsMenu() {
        if (!isTransitioning)
        {
            currentMenu.SetActive(false);

            // Set currentMenu then start it at 0 alpha and set things up
            currentMenu = menuSettings;
            currentMenu.GetComponent<CanvasRenderer>().SetAlpha(0f);
            currentMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(settingsMenuFirst);

            // Everything set up, fade in the menu
            StartCoroutine(transition(currentMenu, true));
        }
    }

    public void settingsBack() {
        if (!isTransitioning)
        {
            // Fade out the current menu
            StartCoroutine(transition(currentMenu, false));

            // Reset the opacity
            // Turning off the buttons needs to be in the coroutine because of how fast code is going

            currentMenu = menuOptions;
            EventSystem.current.SetSelectedGameObject(optionsMenuFirst);
            currentMenu.SetActive(true);
        }
    }

    public void controlMenu()
    {
        if (!isTransitioning)
        {
            currentMenu.SetActive(false);

            // Set currentMenu then start it at 0 alpha and set things up
            currentMenu = controlsMenu;
            currentMenu.GetComponent<CanvasRenderer>().SetAlpha(0f);
            currentMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(controlsMenuFirst);

            // Everything set up, fade in the menu
            StartCoroutine(transition(currentMenu, true));
        }
    }
    public void howToPlay()
    {
        if (!isTransitioning)
        {
            currentMenu.SetActive(false);

            // Set currentMenu then start it at 0 alpha and set things up
            currentMenu = howToPlayMenu;
            currentMenu.GetComponent<CanvasRenderer>().SetAlpha(0f);
            currentMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(howToPlayMenuFirst);

            // Everything set up, fade in the menu
            StartCoroutine(transition(currentMenu, true));
        }
    }
    public void tips() {
        if (!isTransitioning) {
            currentMenu.SetActive(false);

            // Set currentMenu then start it at 0 alpha and set things up
            currentMenu = tipsMenu;
            currentMenu.GetComponent<CanvasRenderer>().SetAlpha(0f);
            currentMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(tipsMenuFirst);

            // Everything set up, fade in the menu
            StartCoroutine(transition(currentMenu, true));
        }
    }

    public void tipsBack() {
        if (!isTransitioning) {
            // Fade out the current menu
            StartCoroutine(transition(currentMenu, false));

            // Reset the opacity
            // Turning off the buttons needs to be in the coroutine because of how fast code is going

            currentMenu = howToPlayMenu;
            EventSystem.current.SetSelectedGameObject(howToPlayMenuFirst);
            currentMenu.SetActive(true);
        }
    }

    public void backButton()
    {
        if (!isTransitioning)
        {
            // Fade out the current menu
            StartCoroutine(transition(currentMenu, false));

            // Reset the opacity
            // Turning off the buttons needs to be in the coroutine because of how fast code is going
            currentMenu.GetComponent<CanvasRenderer>().SetAlpha(1f);

            currentMenu = menuOptions;
            EventSystem.current.SetSelectedGameObject(optionsMenuFirst);
            currentMenu.SetActive(true);
        }
    }

    public void quitGame()
    {
        if (!isTransitioning)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    IEnumerator transition(GameObject _menu, bool _state)
    {
        // Fade In/Out Menus
        isTransitioning = true;
        yield return new WaitForSeconds(0.1f);

        float fadeDuration = 0.2f;
        float elapsed = 0f;

        // Fade in
        if (_state)
        {
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                _menu.GetComponent<CanvasRenderer>().SetAlpha(alpha);

                yield return null;
            }
        }
        // Fade out
        else
        {
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                _menu.GetComponent<CanvasRenderer>().SetAlpha(alpha);
                yield return null;
            }

            _menu.SetActive(false);
            _menu.GetComponent<CanvasRenderer>().SetAlpha(1f);
        }

        isTransitioning = false;   
    }
}
