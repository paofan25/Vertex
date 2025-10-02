using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject transitionImage; // 过渡图片
    [SerializeField] private GameObject pausePanel; // 暂停面板
    [SerializeField] private GameObject gameOverUI; // 游戏结束UI
    
    private bool isPaused = false; // 游戏是否暂停
    
    private Vector2 transitionImageOranginalPosition; // 过渡图片的原始位置
    

    void Awake()
    {
        transitionImage = transform.Find("TransitionImage").gameObject; // 获取过渡图片
        pausePanel = transform.Find("Pause").gameObject; // 获取暂停面板
        gameOverUI = transform.Find("GameOver").gameObject; // 获取游戏结束UI
    }

    void Update()
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
        else if (isPaused && Input.GetKeyDown(KeyCode.Escape))
            ResumeGame(new ResumeGameEvent());
    }

    private void OnEnable()
    {
        EventBus.Subscribe<OnPlayerDeathEvent>(OnPlayerDeath); // 订阅玩家死亡事件
        EventBus.Subscribe<ResumeGameEvent>(ResumeGame); // 订阅游戏恢复事件
        EventBus.Subscribe<GameOverEvent>(GameOver); // 订阅游戏结束事件
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<OnPlayerDeathEvent>(OnPlayerDeath); // 退订玩家死亡事件
        EventBus.Unsubscribe<ResumeGameEvent>(ResumeGame); // 退订游戏恢复事件
        EventBus.Unsubscribe<GameOverEvent>(GameOver); // 订阅游戏结束事件
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
        rect.DOAnchorPos(new Vector2(-transitionImageOranginalPosition.x, -transitionImageOranginalPosition.y), 1f); // 过渡图片移动
        
        yield return new WaitForSecondsRealtime(1f); // 等待1秒
        
        rect.anchoredPosition = transitionImageOranginalPosition; // 恢复过渡图片锚点的位置
        transitionImage.SetActive(false); // 激活过渡图片
    }

    // 游戏暂停
    private void PauseGame()
    {
        isPaused = true; // 设置游戏为暂停状态
        Time.timeScale = 0f; // 暂停游戏
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        pausePanel.SetActive(true); // 激活暂停面板
    }
    
    // 游戏恢复
    private void ResumeGame(GameEvent gameEvent)
    {
        isPaused = false; // 设置游戏为非暂停状态
        Time.timeScale = 1f; // 恢复游戏
        EventBus.Publish(new CanInputEvent(true)); // 发布启用输入事件
        pausePanel.SetActive(false); // 隐藏暂停面板
    }
    
    // 游戏结束
    public void GameOver(GameEvent gameEvent)
    {
        gameOverUI.SetActive(true); // 激活游戏结束UI
        Time.timeScale = 0f; // 暂停游戏
    }
}