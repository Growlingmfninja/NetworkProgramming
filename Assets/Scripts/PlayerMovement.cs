using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float speed;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject bulletClient;
    private ulong clientId;
    
    private float cooldownTime = 1f;
    private float lastAttackTime = 0f;

    private Vector3 _input;
    private Vector3 targetPosition;
    private Vector3 _predictedPosition;
    private bool isMoving = false;
    private float lastUpdateTime = 0f;
    private float updateInterval = 0.05f;

    private void Start()
    {
        clientId = NetworkManager.Singleton.LocalClientId;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
        
        if (ChatManager.Singleton.isChatFocused) { return; }

        _input = playerInput.actions["Move"].ReadValue<Vector2>();
        _predictedPosition = transform.position + new Vector3(_input.x, 0, _input.y) * speed * Time.deltaTime;
        
        if (!IsHost) { transform.position = _predictedPosition; }
        
        if (_input != Vector3.zero)
        {
            MoveServerRpc(_input);
        }

        if (playerInput.actions["Attack"].IsPressed() && (Time.time >= lastAttackTime + cooldownTime))
        {
            Instantiate(bulletPrefab, _predictedPosition, Quaternion.identity);
            InstantiateBulletServerRpc(transform.position, transform.rotation, clientId);
            lastAttackTime = Time.time;
        }
    }

    

    [ServerRpc]
    public void MoveServerRpc(Vector2 movementInput)
    {
        float deltaTime = Time.fixedDeltaTime;
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y) * speed * deltaTime;
        
        transform.Translate(movement);
        
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            lastUpdateTime = Time.time;
            UpdatePositionClientRpc(transform.position);
        }
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        if (IsHost)
        {
            transform.position = newPosition;
            return;
        }

        targetPosition = newPosition;

        if (!isMoving)
        {
            StartCoroutine(SmoothMove());
        }
    }

    private IEnumerator SmoothMove()
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
    
    [ServerRpc]
    private void InstantiateBulletServerRpc(Vector3 transform, Quaternion rotation, ulong clientId)
    {
        InstantiateBulletClientRpc(transform, rotation, clientId);
    }

    [ClientRpc]
    private void InstantiateBulletClientRpc(Vector3 transform, Quaternion rotation, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) return;
        Instantiate(bulletClient, transform, rotation);
    }
    
}
