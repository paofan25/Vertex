using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoint : MonoBehaviour
{
    public GameObject playerPrefab; // 玩家预制体
    
    void Awake()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity); // 在出生点生成玩家
        GameManager.Instance.SetRespawnPosition(transform.position); // 设置出生点为检查点
        GameManager.Instance.playerPrefab = playerPrefab; // 设置玩家预制体
    }
}
