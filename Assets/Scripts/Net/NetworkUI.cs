using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class NetworkUI : MonoBehaviour
{
    public void StartHost()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            return;

        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started!");
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            return;

        NetworkManager.Singleton.StartClient();
        Debug.Log("Client started!");
    }

    public void Stop()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Stopped.");
        }
    }
}
