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

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            _waitingClients.Clear();
            Debug.Log("NETWORK SPAWN");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        _waitingClients.Add(clientId);
        Debug.Log(_waitingClients.Count);
        if (_waitingClients.Count == 2)
        {
            StartGameClientRpc();
            foreach (ulong id in _waitingClients)
            {
                Vector3 pos = new Vector3(Random.Range(-2, 2), 2, Random.Range(-2, 2));
                GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
            }
        }
    }

    public void StopGame()
    {
        Debug.Log(IsOwner);
        StopGameServerRpc();

        Debug.Log("Pre shutdown");
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
