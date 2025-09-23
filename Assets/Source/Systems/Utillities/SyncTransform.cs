using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] bool syncRotation = true;
    [SerializeField] bool syncPosition = true;

    void SyncPosition()
    {
        if (syncPosition)
            transform.position = target.position;
    }
    void SyncRotation()
    {
        if (syncRotation)
            transform.rotation = target.rotation;
    }
    void Start()
    {
        SyncPosition();
        SyncRotation();
    }

    void Update()
    {
        SyncPosition();
        SyncRotation();
    }
}
