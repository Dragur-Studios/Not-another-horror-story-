using System;
using Unity.Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    CinemachineCamera[] cams;
    public Animator Animator;
    public static GameCamera Singleton;

    private void Awake()
    {
        if(Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Animator = GetComponent<Animator>();
        Singleton = this;
    }

    void Start()
    {
        cams = GetComponentsInChildren<CinemachineCamera>();    
    }

    void SetTrackingTarget(Transform target)
    {
        foreach (var cam in cams)
        {
            cam.Target.TrackingTarget = target;
        }
    }

    public static void Track(Transform target)
    {
        Singleton.SetTrackingTarget(target);
    }
}
