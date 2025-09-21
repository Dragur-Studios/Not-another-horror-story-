using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    [SerializeField] Transform target;



    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    void Update()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
