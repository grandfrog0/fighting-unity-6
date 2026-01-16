using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] GameObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Vector3 pos = new Vector3(Random.Range(-2, 2), 2, Random.Range(-2, 2));
        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    public override void OnDestroy()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }

        base.OnDestroy();
    }
}
