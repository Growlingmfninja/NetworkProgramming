using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private int scoreValue;
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.deltaTime);

        if (transform.position.y > 7f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreBoardManager.Singleton.UpdateScoreServerRpc(NetworkManager.Singleton.LocalClientId, scoreValue);
            Destroy(gameObject);
        }
    }
}
