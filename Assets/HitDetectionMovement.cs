using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class HitDetectionMovement : NetworkBehaviour
{

    private Vector3 _leftThreshold = new Vector3(-6, 4, 0);
    public Vector3 _rightThreshold = new Vector3(6, 4, 0);

    private Vector3 _targetPos;
    private Vector3 _endPos;

    private float _speed = 2.0f;

    void Awake()
    {
        _targetPos = _rightThreshold;
    }

    void FixedUpdate()
    {
        Vector3 currentPos = transform.position;

        if (currentPos.x == _leftThreshold.x)
        {
            _targetPos = _rightThreshold;
        }
        else if (currentPos.x == _rightThreshold.x)
        {
            _targetPos = _leftThreshold;
        }
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);
    }
}
