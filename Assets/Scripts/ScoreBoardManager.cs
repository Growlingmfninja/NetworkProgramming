using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    #region Singleton
    public static ScoreBoardManager Singleton { get; private set; }
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
    
    public List<PlayerInfo> PlayerScores;

    [SerializeField] private int MaxScore;

    private Dictionary<string, List<int>> _scoreReports = new Dictionary<string, List<int>>();
    private int _expectedResponses;
    
    private void Start()
    {
        if (PlayerScores == null)
        {
            PlayerScores = new List<PlayerInfo>();
            Debug.Log("Initialized PlayerScores list.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScoreServerRpc(ulong clientToVerify, int scoreToAdd)
    {
        Debug.Log("UpdateScoreServerRpc is called!");

        _scoreReports.Clear();
        Debug.Log($"Connected Clients: {NetworkManager.Singleton.ConnectedClientsIds.Count}");
        _expectedResponses = NetworkManager.Singleton.ConnectedClientsIds.Count;
        
        Debug.Log($"PlayerScores count: {PlayerScores.Count}");

        foreach (var player in PlayerScores)
        {
            _scoreReports[player.playerName] = new List<int>();

            Debug.Log($"Requesting score for Player {player.playerName} from Client {NetworkManager.Singleton.LocalClientId}");
            RequestScoreServerRpc(player.playerName, NetworkManager.Singleton.LocalClientId);
        }
    }

    
    [ServerRpc(RequireOwnership = false)]
    private void RequestScoreServerRpc(string playerName, ulong targetClientId)
    {
        RequestScoreClientRpc(playerName, targetClientId);
    }
    
    [ClientRpc]
    private void RequestScoreClientRpc(string playerName, ulong targetClientId)
    {
        Debug.Log($"RequestScoreClientRpc received for {playerName} on Client {NetworkManager.Singleton.LocalClientId}");
        
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            PlayerInfo localPlayer = PlayerScores.Find(p => p.playerName == playerName);
            if (localPlayer != null)
            {
                SendScoreToServerServerRpc(playerName, localPlayer.score);
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SendScoreToServerServerRpc(string playerName, int clientScore, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        _scoreReports[playerName].Add(clientScore);
        
        Debug.Log($"Server received score from Client {clientId}: PlayerName {playerName}: ClientScore {clientScore}");

        if (_scoreReports[playerName].Count >= _expectedResponses)
        {
            VerifyAndSetScore(playerName);
        }
    }

    private void VerifyAndSetScore(string playerName)
    {
        if (!_scoreReports.ContainsKey(playerName)) return;

        List<int> reportedScores = _scoreReports[playerName];
        int verifiedScore = reportedScores.GroupBy(s => s)
            .OrderByDescending(g => g.Count()).First().Key;

        PlayerInfo player = PlayerScores.Find(p => p.playerName == playerName);
        if (player != null && player.score != verifiedScore)
        {
            Debug.Log($"Score discrepancy detected for {playerName}. Setting score to {verifiedScore}");
            player.score = verifiedScore;
            UpdateScoreClientRpc(playerName, verifiedScore);
        }
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(string playerName, int verifiedScore)
    {
        PlayerInfo player = PlayerScores.Find(p => p.playerName == playerName);
        if (player != null)
        {
            player.score = verifiedScore;
            Debug.Log($"Updated {playerName}'s score to {verifiedScore} on all clients.");
        }
    }
}