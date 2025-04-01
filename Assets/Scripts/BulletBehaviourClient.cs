using UnityEngine;

public class BulletBehaviourClient : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
