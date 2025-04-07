using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum SceneType
    {
        Title,
        Game
    }

    private static readonly Dictionary<SceneType, string> SceneDict = new()
    {
        { SceneType.Title, "Pendulumer-Title" },
        { SceneType.Game, "Pendulumer-Prototype" }
    };

    private static GameObject _loadingScreen;

    private static StaticCoroutineRunner _coroutineRunner;

    public static void SetupLoadingScreen()
    {
        EnsureCoroutineRunnerExists();

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
        StartStaticCoroutine(TransitionToScene(SceneDict[sceneType]));
    }

    private static IEnumerator TransitionToScene(string sceneName)
    {
        var asyncOp = SceneManager.LoadSceneAsync(sceneName);
        if (asyncOp == null)
        {
            _loadingScreen.SetActive(false);
            yield break;
        }

        while (!asyncOp.isDone) yield return null;

        yield return new WaitForNextFrameUnit();
        _loadingScreen.SetActive(false);
    }

    private class StaticCoroutineRunner : MonoBehaviour
    {
    }

    #region StaticCoroutines

    [PublicAPI]
    public static Coroutine StartStaticCoroutine(string methodName, object value)
    {
        EnsureCoroutineRunnerExists();

        return _coroutineRunner.StartCoroutine(methodName, value);
    }

    [PublicAPI]
    public static Coroutine StartStaticCoroutine(IEnumerator routine)
    {
        EnsureCoroutineRunnerExists();

        return _coroutineRunner.StartCoroutine(routine);
    }

    [PublicAPI]
    public static void StopStaticCoroutine(IEnumerator routine)
    {
        EnsureCoroutineRunnerExists();

        _coroutineRunner.StopCoroutine(routine);
    }

    [PublicAPI]
    public static void StopStaticCoroutine(Coroutine routine)
    {
        EnsureCoroutineRunnerExists();

        _coroutineRunner.StopCoroutine(routine);
    }

    private static void EnsureCoroutineRunnerExists()
    {
        if (_coroutineRunner != null) return;

        _coroutineRunner = new GameObject("StaticCoroutineRunner").AddComponent<StaticCoroutineRunner>();
        Object.DontDestroyOnLoad(_coroutineRunner.gameObject);
    }

    #endregion
}