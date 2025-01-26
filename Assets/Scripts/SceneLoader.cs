using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private class StaticCoroutineRunner : MonoBehaviour
    {
    }

    public enum SceneType
    {
        Title,
        Game
    }

    private static Dictionary<SceneType, string> _sceneDict = new()
    {
        { SceneType.Title, "Pendulumer-Title" },
        { SceneType.Game, "Pendulumer-Prototype" },
    };

    private static GameObject _loadingScreen;

    private static StaticCoroutineRunner _coroutineRunner;

    public static void SetupLoadingScreen()
    {
        if (_coroutineRunner == null)
        {
            _coroutineRunner = new GameObject("StaticCoroutineRunner").AddComponent<StaticCoroutineRunner>();
            Object.DontDestroyOnLoad(_coroutineRunner.gameObject);
        }

        if (_loadingScreen != null) return;

        var go = Resources.Load<GameObject>("Prefabs/LoadingScreen");
        _loadingScreen = Object.Instantiate(go);
        Object.DontDestroyOnLoad(_loadingScreen);
        _loadingScreen.SetActive(false);
    }

    public static void BeginTransitionToScene(SceneType sceneType)
    {
        SetupLoadingScreen();

        _loadingScreen.SetActive(true);
        _coroutineRunner.StartCoroutine(TransitionToScene(_sceneDict[sceneType]));
    }

    private static IEnumerator TransitionToScene(string sceneName)
    {
        var asyncOp = SceneManager.LoadSceneAsync(sceneName);
        if (asyncOp == null)
        {
            _loadingScreen.SetActive(false);
            yield break;
        }

        while (!asyncOp.isDone)
        {
            yield return null;
        }

        _loadingScreen.SetActive(false);
    }
}