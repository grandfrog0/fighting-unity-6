using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawner : NetworkBehaviour
{
    public UnityEvent OnConnected = new();
    public UnityEvent OnDisconnected = new();
    [SerializeField] GameObject playerPrefab;
    private List<ulong> _waitingClients = new();
    private List<GameObject> _spawnedPlayers = new();
    [SerializeField] List<GameObject> playerModels = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            _waitingClients.Clear();
            _spawnedPlayers.Clear();
            Debugger.Log("NETWORK SPAWN", IsServer);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        _waitingClients.Add(clientId);
        Debugger.Log(_waitingClients.Count, IsServer);
        if (_waitingClients.Count == 2)
        {
            StartGameClientRpc();
        }
    }

    public void SetPlayerConfig(PlayerInfo info, ulong id)
    {
        LogIsServerClientRpc(id);

        Vector3 pos = new Vector3(Random.Range(-2, 2), 2, Random.Range(-2, 2));
        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
        PlayerController controller = player.GetComponent<PlayerController>();

        Instantiate(
            playerModels[info.SelectedPlayerIndex],
            controller.ModelParent
            );

        controller.Awake();

        _spawnedPlayers.Add(player);
    }

    [ClientRpc]
    void LogIsServerClientRpc(ulong id)
    {
        Debugger.Log(id, IsServer);
    }

    public void StopGame()
    {
        Debugger.Log(IsOwner, IsServer);
        StopGameServerRpc();

        Debugger.Log("Pre shutdown", IsServer);
    }
    [ServerRpc(RequireOwnership = false)]
    public void StopGameServerRpc()
    {
        StopGameClientRpc();
    }
    [ClientRpc]
    public void StopGameClientRpc()
    {
        OnDisconnected.Invoke();
    }
    private void OnClientDisconnected(ulong clientId)
    {
        StopGameClientRpc();
    }

    [ClientRpc]
    public void StartGameClientRpc()
    {
        OnConnected.Invoke();
    }

    public override void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }

        base.OnDestroy();
    }
}
