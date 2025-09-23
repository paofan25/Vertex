using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public GameObject transitionImage; // 过渡图片
    private Vector2 transitionImageOranginalPosition; // 过渡图片的原始位置

    void Awake()
    {
        transitionImage = transform.Find("TransitionImage").gameObject; // 获取过渡图片
    }

    private void OnEnable()
    {
        EventBus.Subscribe<OnPlayerDeathEvent>(OnPlayerDeath); // 订阅玩家死亡事件
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<OnPlayerDeathEvent>(OnPlayerDeath); // 退订玩家死亡事件
    }

    // 角色死亡过渡
    private void OnPlayerDeath(GameEvent gameEvent)
    {
        StartCoroutine(Transition()); // 开始过渡
    }

    // 过渡图片移动
    private IEnumerator Transition()
    {
        yield return new WaitForSecondsRealtime(0.5f); // 等待0.5秒
        
        transitionImage.SetActive(true); // 激活过渡图片
        
        RectTransform rect = transitionImage.GetComponent<RectTransform>(); // 获取过渡图片的RectTransform组件
        transitionImageOranginalPosition = rect.anchoredPosition; // 获取过渡图片锚点的位置
        rect.DOAnchorPos(new Vector2(-transitionImageOranginalPosition.x, -transitionImageOranginalPosition.y), 1.3f); // 过渡图片移动
        
        yield return new WaitForSecondsRealtime(1.3f); // 等待1秒
        
        rect.anchoredPosition = transitionImageOranginalPosition; // 恢复过渡图片锚点的位置
        transitionImage.SetActive(false); // 激活过渡图片
    }
}