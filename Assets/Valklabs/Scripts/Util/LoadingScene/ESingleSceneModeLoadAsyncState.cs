namespace Valklabs.Util.LoadingScene
{
    /// <summary>
    /// The current state of a Loading Async call done to a scene loading in Single Scene Mode. This state does not trigger while loading additive
    /// </summary>
    public enum ESingleSceneModeLoadAsyncState
    {
        RequestLoadAsyncFailed = 0,          //Request to load failed. Prevent duplicate load requests
        LoadingAsyncInProgress = 1,          //Loading in progress
        LoadingAsyncFailed = 2,              //Request to load was successful, but failed to load. Possibly tried to load a scene that did not exist or is not in build settings
        LoadingAsyncComplete = 3             //Loading completed.
    }
}