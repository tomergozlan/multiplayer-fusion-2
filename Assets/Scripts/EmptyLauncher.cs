// From the Fusion 2 Tutorial: https://doc.photonengine.com/fusion/current/tutorials/host-mode-basics/2-setting-up-a-scene#launching-fusion
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// This class demonstrates the most basic procedure for launching Fusion NetworkRunner.
// INetworkRunnerCallbacks is an interface that contains all "On*" methods relevant to connecting to Fusion Network Runner.
public class EmptyLauncher: MonoBehaviour, INetworkRunnerCallbacks {
    public virtual void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public virtual void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public virtual void OnInput(NetworkRunner runner, NetworkInput input) { }
    public virtual void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public virtual void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public virtual void OnConnectedToServer(NetworkRunner runner) { }
    public virtual void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public virtual void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public virtual void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public virtual void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public virtual void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public virtual void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public virtual void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public virtual void OnSceneLoadDone(NetworkRunner runner) { }
    public virtual void OnSceneLoadStart(NetworkRunner runner) { }
    public virtual void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public virtual void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public virtual void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public virtual void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }


    [SerializeField]
    protected NetworkRunner _runner;

    protected async void StartGame(GameMode mode, string sessionName) {
        Debug.Log($"Starting game at mode {mode}, session {sessionName}");
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs() {
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    string SESSION_NAME = "TestRoom";

    // Create and run a simple GUI that allows the player to choose whether to host a new game or join an existing game.
    protected void OnGUI() {
        if (_runner == null) {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host")) {
                StartGame(GameMode.Host, SESSION_NAME);    // This mode requires Internet connection (to the Fusion cloud).
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Client")) {
                StartGame(GameMode.Client, SESSION_NAME);  // This mode requires Internet connection (to the Fusion cloud).
            }
            if (GUI.Button(new Rect(0, 80, 200, 40), "Single player")) {
                StartGame(GameMode.Single, SESSION_NAME);  // This mode does not require Internet connection
            }
        }
    }
}
