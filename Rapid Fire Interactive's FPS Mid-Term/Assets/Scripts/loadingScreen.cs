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
    public void loadScene(int index)
    {
        if (hideUIObj != null)
        {
            hideUIObj.SetActive(false);
        }
        StartCoroutine(loadSceneAsync(index));
    }

    IEnumerator loadSceneAsync(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        loadScreen.SetActive(true);

        while (!operation.isDone)
        {
            progressF = Mathf.Clamp01(operation.progress / 0.9f);
            loadBar.fillAmount = progressF;
            yield return null;
        }
    }
}
