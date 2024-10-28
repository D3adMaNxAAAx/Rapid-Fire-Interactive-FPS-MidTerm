using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadingScreen : MonoBehaviour
{
    [SerializeField] GameObject loadScreen;
    [SerializeField] Image loadBar;
    [SerializeField] GameObject hideUIObj;

    float progressF;

    private void Awake()
    {
        if(keepSettings.gameSettings != null && keepSettings.gameSettings.getUIobj() != null)
        {
            keepSettings.gameSettings.getUIobj().SetActive(true);
            
        }
    }

    public void loadScene(int index)
    {
        if (hideUIObj != null)
        {
            hideUIObj.SetActive(false);
        }
        if (keepSettings.gameSettings != null && hideUIObj == null)
        {
            keepSettings.gameSettings.getUIobj().SetActive(false);
        }
        StartCoroutine(loadSceneAsync(index));
       
            //lightFlicker.setFoundPower(false); 
       
    }

    IEnumerator loadSceneAsync(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        loadScreen.SetActive(true);
        if (playerMovement.player != null)
            playerMovement.player.getController().enabled = false;
        while (!operation.isDone)
        {
            progressF = Mathf.Clamp01(operation.progress / 0.9f);
            loadBar.fillAmount = progressF;
            yield return null;
        }
    }
}
