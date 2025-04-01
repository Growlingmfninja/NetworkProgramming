using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ScoreBoardUI : NetworkBehaviour
{
    [SerializeField] private GameObject _scorePanel;
    private ScoreBoardManager _instance;
    
    private void Start()
    {
        StartCoroutine(WaitForManagers());
    }

    private IEnumerator WaitForManagers()
    {
        while (NetworkManager.Singleton == null || ScoreBoardManager.Singleton == null)
        {
            Debug.LogWarning("Waiting for NetworkManager or ScoreBoardManager to initialize...");
            yield return null;
        }
        
        _scorePanel = GameObject.Find("Panel");
        Debug.Log(_scorePanel);
        
        if (IsServer)  
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        _instance = ScoreBoardManager.Singleton;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        if (ScoreBoardManager.Singleton != null)
        {
            PlayerInfo toAdd = new PlayerInfo(clientId.ToString());
            ScoreBoardManager.Singleton.PlayerScores.Add(toAdd);
            Debug.Log($"Player Added: {toAdd.playerName} - Total Players: {ScoreBoardManager.Singleton.PlayerScores.Count}");
        }
        else
        {
            Debug.LogError("ScoreBoardManager is null when trying to add a player.");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        if (_instance == null) return;

        _instance.PlayerScores.RemoveAll(p => p.playerName.Contains(clientId.ToString()));
        Debug.Log($"Player {clientId} removed. Total Players: {_instance.PlayerScores.Count}");
    }
}