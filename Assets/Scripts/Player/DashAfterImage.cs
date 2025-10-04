using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float duration;
    private float timer = 0f;
    private Color startColor;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color; // 保存预制体设置的颜色
    }

    /// <summary>
    /// 初始化残影
    /// </summary>
    /// <param name="afterImageDuration">残影持续时间</param>
    public void Initialize(float afterImageDuration)
    {
        duration = afterImageDuration;
        
        // 设置自动销毁
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer <= duration)
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, timer / duration);
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
        }
    }
}
