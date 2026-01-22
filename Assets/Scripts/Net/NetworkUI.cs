using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class NetworkUI : MonoBehaviour
{
    public UnityEvent OnConnected = new();
    public UnityEvent OnStopped = new();

    private Coroutine _searchRoutine;
    public void StartSearch()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            throw new Exception(NetworkManager.Singleton.IsServer + " || " + NetworkManager.Singleton.IsClient);

        _searchRoutine = StartCoroutine(SearchRoutine());
    }
    public void StopSearch()
    {
        if (_searchRoutine != null)
        {
            StopCoroutine(_searchRoutine);
            _searchRoutine = null;
        }
        Stop();
    }

    private IEnumerator SearchRoutine()
    {
        NetworkManager.Singleton.StartClient();

        yield return new WaitForSeconds(5f);

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Сервер не найден, запускаю хост...");
            NetworkManager.Singleton.Shutdown();
            yield return new WaitForSeconds(1f);
            StartHost();
        }
        else
        {
            Debug.Log("Client started!");
            OnConnected.Invoke();
        }
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            return;

        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started!");
        OnConnected.Invoke();
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            return;

        NetworkManager.Singleton.StartClient();
        Debug.Log("Client started!");
        OnConnected.Invoke();
    }

    public void Stop()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Stopped.");
            OnStopped.Invoke();
        }
    }
}
