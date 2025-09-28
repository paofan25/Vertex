using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowGameOverDelayed());
        }
    }

    private IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSecondsRealtime(0.5f);
    
        EventBus.Publish(new GameOverEvent()); // 发布游戏结束事件
    }
}
