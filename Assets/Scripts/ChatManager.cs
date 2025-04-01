using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    #region Singleton

    public static ChatManager Singleton { get; private set; }

    private void Awake()
    {
        #region Singleton
        if (Singleton == null)
        {
            SetSingleton();
            Debug.Log("[ChatManager] Singleton created");
            DontDestroyOnLoad(gameObject);
        }
        else if (Singleton != this)
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void SetSingleton()
    {
        Singleton = this;
    }

    #endregion

    [SerializeField] private GameObject chatBox;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ChatMessage Fab;
    [SerializeField] private int _chatLogMax = 10;

    private Queue<string> _chatlog = new Queue<string>();

    [SerializeField] public bool isChatFocused = false;

    void Update()
    {
        if (!IsClient) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            isChatFocused = true;
            if (!string.IsNullOrWhiteSpace(inputField.text))
            {
                Debug.Log("[ChatManager] Field not null");
                SendMessageServerRpc(inputField.text);
                inputField.text = "";
                inputField.DeactivateInputField();
                
                isChatFocused = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendMessageServerRpc(string msg)
    {
        Debug.Log($"[Server] Received message: {msg}");
        SendMessageClientRpc(msg);
    }

    [ClientRpc]
    private void SendMessageClientRpc(string msg)
    {
        Debug.Log($"[Client] Displaying message: {msg}");
        AddMessageLocally(msg);
    }

    private void AddMessageLocally(string msg)
    {
        Debug.Log($"Adding message locally: {msg}");
        
        string toAdd = msg;

        if (toAdd == null)
        {
            Debug.LogError("Missing TMP_Text component on textObject prefab!");
            return;
        }
        

        if (_chatlog.Count >= _chatLogMax)
        {
            _chatlog.Dequeue();
        }
        else { _chatlog.Enqueue(toAdd); }

        ChatMessage toSpawn = Instantiate(Fab, chatBox.transform);
        toSpawn.SetText(toAdd);
    }
}