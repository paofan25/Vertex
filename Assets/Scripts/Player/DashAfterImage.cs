using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // 残影的SpriteRenderer组件
    private float duration; // 残影持续时间
    private float timer = 0f; // 残影计时器
    private Color startColor; // 预制体设置的颜色
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件
        startColor = spriteRenderer.color; // 保存预制体设置的颜色
    }

    /// <summary>
    /// 初始化残影
    /// </summary>
    /// <param name="afterImageDuration">残影持续时间</param>
    public void Initialize(float afterImageDuration)
    {
        duration = afterImageDuration; // 设置残影持续时间
        
        // 设置自动销毁
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        timer += Time.deltaTime; // 更新计时器
        if (timer <= duration)
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, timer / duration); // 计算透明度
            Color newColor = spriteRenderer.color; // 获取当前颜色
            newColor.a = alpha; // 设置新的透明度
            spriteRenderer.color = newColor; // 应用新的颜色
        }
    }
}
