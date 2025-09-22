using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.SetRespawnPosition(transform.position); // 设置出生点为检查点
        GameManager.Instance.SpawnPlayer(); // 在出生点生成玩家
    }
}
