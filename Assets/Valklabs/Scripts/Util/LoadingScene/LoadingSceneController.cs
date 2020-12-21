using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Valklabs.Util.LoadingScene
{
    /// <summary>
    /// A Unity scene load wrapper that does allows for async calls without using IEnumerators and/or exisiting in the scene. Events for your UI to easily hook into and display back loading percentage/states/etc
    /// </summary>
    public static class LoadingSceneController
    {
        /// <summary>
        /// This only sends the loading percent of loading async scene by Single Scene Mode -- Does not support for additive scenes.
        /// </summary>
        public static event Action<float> OnSingleSceneModeLoadingAsyncPercentEvent = null;                         //Used for your UI to listen to this event, to display the percent currently loaded. Returns a value of 0-1
       
        /// <summary>
        /// This only sends the state of a loading async scene by Single Scene Mode -- Does not support for additive scenes.
        /// </summary>
        public static event Action<ESingleSceneModeLoadAsyncState> OnSingleSceneModeLoadingAsyncStateEvent = null;  //Can be used for triggering your Loading UI to "Show" or "Hide" or anything else that may want to listen to these events.

        private static bool _isLoadingSingleSceneModeAsync = false;

        #region Async Loading Functions
        /// <summary>
        /// Load a scene async by scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="onLoadingComplete">Called when loading is complete.</param>
        public static void LoadAsync(string sceneName, LoadSceneMode loadSceneMode, Action onLoadingComplete = null)
        {
            //Check which mode as we want to send special events for UI to pickup on if loading a single scene.
            if(loadSceneMode == LoadSceneMode.Single)
            {
                LoadSingleSceneSceneAsync(sceneName, onLoadingComplete);
            }
            else
            {
                LoadAdditiveSceneAsync(sceneName, onLoadingComplete);
            }
        }

        private static void LoadSingleSceneSceneAsync(string sceneName, Action onComplete)
        {
            if (_isLoadingSingleSceneModeAsync == true)
            {
                //Makes sure we don't try to load 2 scenes at the same time. This assumes that you never want to load multiple scenes at the same time (additive scenes, etc)
                Debug.LogWarning($"You're making an attempt to load a scene while another scene is trying to load. | Requested: {sceneName}");
                OnSingleSceneModeLoadingAsyncStateEvent?.Invoke(ESingleSceneModeLoadAsyncState.RequestLoadAsyncFailed);
                return;
            }

            _isLoadingSingleSceneModeAsync = true;

            OnSingleSceneModeLoadingAsyncStateEvent?.Invoke(ESingleSceneModeLoadAsyncState.LoadingAsyncInProgress);
            OnSingleSceneModeLoadingAsyncPercentEvent?.Invoke(0);        

            AsyncOperation asyncOperation = null;
            try
            {
                //Begin to load the Scene you specified, make sure the scene name exists and is in build settings
                asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            }
            catch(Exception e)
            {
                //More than likely failed because scene name does not exist or because you did not add it to build settings
                Debug.LogError($"Failed to load scene with exception: {e}");
                OnSingleSceneModeLoadingAsyncStateEvent?.Invoke(ESingleSceneModeLoadAsyncState.LoadingAsyncFailed);
                return;
            }

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

            _isLoadingSingleSceneModeAsync = false;
        }

        private static void LoadAdditiveSceneAsync(string sceneName, Action onComplete)
        {
            AsyncOperation asyncOperation = null;
            try
            {
                //Begin to load the Scene you specified, make sure the scene name exists and is in build settings
                asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            }
            catch (Exception e)
            {
                //More than likely failed because scene name does not exist or because you did not add it to build settings
                Debug.LogError($"Failed to load scene with exception: {e}");
                return;
            }

            while (true)
            {
                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f) //Do not change this value of 0.9f -> recommended by Unity
                {
                    break;
                }
            }

            onComplete?.Invoke();
        }
        #endregion

        /// <summary>
        /// Load a scene by string value.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="LoadSceneMode">Loading mode. Single will unload all loaded scenes and load this scene. Additive will load a scene into the current scene
        #region Non-Async loading functions
        public static void LoadNonAsync(string sceneName, LoadSceneMode loadSceneMode, Action onComplete = null)
        {
            LoadSceneNonAsync(sceneName, loadSceneMode);
        }

        private static void LoadSceneNonAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            SceneManager.LoadScene(sceneName, loadSceneMode);
        }
        #endregion
    }
}

