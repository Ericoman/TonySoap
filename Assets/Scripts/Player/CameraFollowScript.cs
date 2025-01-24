using System;
using UnityEngine;
using Cinemachine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform target;
    
    public Vector3 followOffset = new Vector3(0, 5, -10);
    
    
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer transposer;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (target != null)
        {
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
            
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            if (transposer != null)
            {
                transposer.m_FollowOffset = followOffset;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
