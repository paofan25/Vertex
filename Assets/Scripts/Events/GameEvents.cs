using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public abstract class GameEvent { }

/// <summary>
/// 测试事件
/// </summary>
public class TestEvent : GameEvent
{
    public string message;

    public TestEvent(string message)
    {
        this.message = message;
    }
}

/// <summary>
/// 是否可以输入事件
/// </summary>
public class CanInputEvent : GameEvent
{
    public bool canInput;

    public CanInputEvent(bool canInput)
    {
        this.canInput = canInput;
    }
}

/// <summary>
/// 获取角色物体事件
/// </summary>
public class GetPlayerEvent : GameEvent
{
    public GameObject player;

    public GetPlayerEvent(GameObject player)
    {
        this.player = player;
    }
}

/// <summary>
/// 获取相机物体事件
/// </summary>
public class GetCameraEvent : GameEvent
{
    public CinemachineVirtualCamera vcam;

    public GetCameraEvent(CinemachineVirtualCamera vcam)
    {
        this.vcam = vcam;
    }
}

/// <summary>
/// 角色死亡事件
/// </summary>
public class OnPlayerDeathEvent : GameEvent { }
