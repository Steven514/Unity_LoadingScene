# Unity-LoadingScene

Summary:  
Intended to be used with Unity projects. A util to load scenes, specifically async, without needing your scripts to inherit monobehaviours. Done by ditching IEnumerators and wrapped up functions with making sync calls.

Description:    
A static class that loads scenes async or otherwise via a non monobehaviour script. Actions/callback setup for your caller and/or UI to listen to load states. The loading controller does not include any UI, and just performs the action of loading with sending back callbacks, and having events to listen to states. Views/UI is up to you to implement.

Purpose:   
An async/sync scene loading controller free from being a monobehaviour. Can easily hook into it's events for your UI to display loading percentages, or a debug/popup to display states (loading failed, etc)   

How to Use:
The caller would call ```LoadingSceneController.LoadAsync(string sceneName, LoadSceneMode loadSceneMode, Action onLoadingComplete = null)``` if wanting a async load. For non-async ```LoadingSceneController.LoadNonAsync(string sceneName, LoadSceneMode loadSceneMode)```   

Your custom UI/View script can hook into the events to show/hide itself while being able ot grab the percentage loaded to display on screen to the user.
```
 private static void LoadSingleSceneAsync(string sceneName, Action onComplete)
{
    ...
    OnSingleSceneModeLoadingAsyncStateEvent?.Invoke(ESingleSceneModeLoadAsyncState.LoadingAsyncInProgress);
    OnSingleSceneModeLoadingAsyncPercentEvent?.Invoke(0);        

    AsyncOperation asyncOperation = null;
    ...
      asyncOperation = SceneManager.LoadSceneAsync(sceneName);
    ...

    while (true)
    {
        OnSingleSceneModeLoadingAsyncPercentEvent?.Invoke(asyncOperation.progress);

        // Check if the load has finished
        if (asyncOperation.progress >= 0.9f) //Do not change this value of 0.9f -> recommended by Unity
        {
            break;
        }
    }

    OnSingleSceneModeLoadingAsyncPercentEvent?.Invoke(1);
    OnSingleSceneModeLoadingAsyncStateEvent?.Invoke(ESingleSceneModeLoadAsyncState.LoadingAsyncComplete);

    onComplete?.Invoke();
    ...
}
```  
Here the call to async load has events that anything can subscribe to. Your LoadingUI script can listen to event of OnSingleSceneModeLoadingAsyncStateEvent to show/hide itself, and OnSingleSceneModeLoadingAsyncPercentEvent to display a fillbar/text to represent the loading percent

