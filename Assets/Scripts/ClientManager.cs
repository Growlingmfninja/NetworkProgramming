using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientManager : NetworkBehaviour
{
    public static List<ulong> connectedClients = new List<ulong>();

    #region Singleton
    public static ClientManager Singleton { get; private set; }
    private void OnEnable()
    {
        if (Singleton == null) { SetSingleton(); }
        else if (Singleton != this) { Destroy(gameObject); }
    }
    private void SetSingleton()
    {
        Singleton = this;
    }
    #endregion
    
    private void Start()
    {
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }
    
    
    /*private void OnDisable()
    {
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }*/

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        if (ScoreBoardManager.Singleton == null)
        {
            Debug.LogError("ScoreBoardManager is null in OnClientConnected!");
            return;
        }

        if (ScoreBoardManager.Singleton.PlayerScores == null)
        {
            Debug.LogError("PlayerScores list is null in OnClientConnected! Initializing now...");
            ScoreBoardManager.Singleton.PlayerScores = new List<PlayerInfo>();
        }

        PlayerInfo toAdd = new PlayerInfo(clientId.ToString());
        ScoreBoardManager.Singleton.PlayerScores.Add(toAdd);

        Debug.Log($"Player Added: {toAdd.playerName} - Total Players: {ScoreBoardManager.Singleton.PlayerScores.Count}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        connectedClients.Remove(clientId);
        
        Debug.Log($"Client {clientId} disconnected. Total clients: {connectedClients.Count}");
        
        UpdateClientListClientRpc(connectedClients.ToArray());
    }

    [ClientRpc]
    private void UpdateClientListClientRpc(ulong[] updatedClientList)
    {
        connectedClients = new List<ulong>(updatedClientList);
    }
}