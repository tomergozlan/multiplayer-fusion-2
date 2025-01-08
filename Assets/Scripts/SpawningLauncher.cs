// From the Fusion 2 Tutorial: https://doc.photonengine.com/fusion/current/tutorials/host-mode-basics/2-setting-up-a-scene#launching-fusion
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// This class launches Fusion NetworkRunner, and also spanws a new avatar whenever a player joins.
public class SpawningLauncher : EmptyLauncher
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"Player {player} joined");
        if (runner.IsServer) {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, /*input authority:*/ player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }
    
    public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"Player {player} left");
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    [SerializeField] InputAction moveAction = new InputAction(type: InputActionType.Button);
    [SerializeField] InputAction shootAction = new InputAction(type: InputActionType.Button);
    [SerializeField] InputAction colorAction = new InputAction(type: InputActionType.Button);
    private void OnEnable() { moveAction.Enable(); shootAction.Enable(); colorAction.Enable(); }
    private void OnDisable() { moveAction.Disable(); shootAction.Disable(); colorAction.Disable();  }
    void OnValidate() {
        // Provide default bindings for the input actions. Based on answer by DMGregory: https://gamedev.stackexchange.com/a/205345/18261
        if (moveAction.bindings.Count == 0)
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
        if (shootAction.bindings.Count == 0)
            shootAction.AddBinding("<Keyboard>/space");
        if (colorAction.bindings.Count == 0)
            colorAction.AddBinding("<Keyboard>/C");
    }

    NetworkInputData inputData = new NetworkInputData();

    private void Update() {
        if (shootAction.WasPressedThisFrame()) {
            inputData.shootActionValue = true;
        }
        if (colorAction.WasPressedThisFrame()) {
            inputData.colorActionValue = true;
        }
    }

    public override void OnInput(NetworkRunner runner, NetworkInput input) {
        inputData.moveActionValue = moveAction.ReadValue<Vector2>();
        input.Set(inputData);    // pass inputData by value 
        inputData.shootActionValue = false; // clear shoot flag
        inputData.colorActionValue = false; // clear shoot flag
    }
}
