using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 单例

    public GameObject playerPrefab; // 玩家预制体
    public Vector2 startPoint; // 起始点
    public Vector2 checkpoint; // 检查点

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 设置起始点
    public void SetStartPoint(Vector2 position)
    {
        startPoint = position;
    }
    
    // 设置检查点
    public void SetRespawnPosition(Vector2 position)
    {
        checkpoint = position;
    }
    
    // 生成玩家
    public void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, startPoint, Quaternion.identity); // 在起始点位置实例化玩家
        EventBus.Publish(new GetPlayerEvent(player)); // 发布获取玩家事件
    }

    // 玩家死亡时处理
    public void OnPlayerDeath(GameObject deadPlayer)
    {
        StartCoroutine(RespawnRoutine(deadPlayer));
    }

    // 玩家重生时处理
    private IEnumerator RespawnRoutine(GameObject deadPlayer)
    {
        Destroy(deadPlayer); // 销毁旧实体
        GameObject newPlayer = Instantiate(playerPrefab, checkpoint, Quaternion.identity); // 在检查点位置实例化新玩家
        EventBus.Publish(new GetPlayerEvent(newPlayer)); // 发布获取玩家事件
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        
        yield return new WaitForSeconds(0.3f);
        
        EventBus.Publish(new CanInputEvent(true)); // 发布启用输入事件
    }
}
