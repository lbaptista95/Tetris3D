using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class takes care of the scene flow.
/// </summary>
public class TetrisSceneManager : MonoBehaviour
{
    public static TetrisSceneManager instance = null;

    public delegate void SceneManagerEvents(string loadedScene);
    public static event SceneManagerEvents OnSceneLoad;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    //Calls scene loading routine
    public void LoadScene(string sceneName)
    {
        StartCoroutine(SceneLoading(sceneName));
    }

    /*The scene loading proccess is async and loads a LoadingScene so there can be a loading animation
      between the scenes*/
    private IEnumerator SceneLoading(string sceneName)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("LoadingScene");
        loading.allowSceneActivation = false;

        while (loading.progress < 0.9f)
            yield return null;

        loading.allowSceneActivation = true;

        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName);

        sceneLoading.allowSceneActivation = false;

        while (sceneLoading.progress < 0.9f)
            yield return null;

        sceneLoading.allowSceneActivation = true;

        OnSceneLoad.Invoke(sceneName);
    }
}
