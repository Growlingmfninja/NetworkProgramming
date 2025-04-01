using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_JoinMenu_Buttons : MonoBehaviour
{
    [SerializeField] Button _joinButton;
    [SerializeField] Button _hostButton;
            
    void Start()
    {
        _hostButton.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); DisableButtons(); });
        
        _joinButton.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); DisableButtons(); });
    }

    void DisableButtons()
    {
        _hostButton.gameObject.SetActive(false);
        
        _joinButton.gameObject.SetActive(false);
    }
}
