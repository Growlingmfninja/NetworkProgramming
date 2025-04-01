using UnityEngine;

public class BulletBehaviourClient : MonoBehaviour
{
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
        Destroy(gameObject);
    }
}
