using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.SetRespawnPosition(transform.position); // 设置玩家的重生位置
            Debug.Log("CheckPoint!");
        }
    }
}