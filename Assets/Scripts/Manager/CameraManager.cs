using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    public CinemachineVirtualCamera vcam; // 虚拟相机
    public CinemachineBasicMultiChannelPerlin noiseProfile; // 摇动效果组件
    
    public float shakeAmount = 5f; // 摇动幅度
    public float shakeDuration = 0.3f; // 摇动持续时间

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        EventBus.Subscribe<GetCameraEvent>(SetCamera); // 订阅获取相机事件
    }

    // 设置相机
    private void SetCamera(GameEvent gameEvent)
    {
        GetCameraEvent getCameraEvent = (GetCameraEvent) gameEvent;
        
        vcam = getCameraEvent.vcam; // 设置虚拟相机
    }

    public CinemachineVirtualCamera GetCamera()
    {
        return vcam;
    }

    // 摇动相机
    public void ShakeCamera()
    {
        StartCoroutine(Shake()); // 摇动相机
    }

    // 相机震动效果
    private IEnumerator Shake()
    {
        noiseProfile = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); // 获取相机震动组件
        noiseProfile.m_AmplitudeGain = shakeAmount; // 设置震动幅度
        yield return new WaitForSeconds(shakeDuration); // 持续震动一段时间
        noiseProfile.m_AmplitudeGain = 0f; // 停止震动
    }
}
