using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private TMP_Text continueText; // 继续游戏文本
    [SerializeField] private TMP_Text restartText; // 重新开始文本
    [SerializeField] private TMP_Text settingText; // 设置文本
    [SerializeField] private TMP_Text mainMenuText; // 返回主页文本
    
    [SerializeField] private GameObject pausePanel; // 暂停面板
    [SerializeField] private GameObject settingPanel; // 设置面板
    
    private int currentSelection = 0; // 当前选中的菜单项索引
    private TMP_Text[] menuTexts; // 菜单文本数组
    
    void Start()
    {
        pausePanel.SetActive(true); // 激活暂停面板
        settingPanel.SetActive(false); // 禁用设置面板
        
        menuTexts = new TMP_Text[] { continueText, restartText, settingText, mainMenuText };
        HighlightCurrentSelection();
    }

    private void Update()
    {
        if (settingPanel.activeInHierarchy) return; // 如果设置面板处于活动状态，则不处理键盘输入
        
        HandleKeyboardInput();
    }
    
    // 处理键盘输入
    void HandleKeyboardInput()
    {
        // 上下导航
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelection--;
            if (currentSelection < 0) currentSelection = menuTexts.Length - 1;
            HighlightCurrentSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection++;
            if (currentSelection >= menuTexts.Length) currentSelection = 0;
            HighlightCurrentSelection();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            switch (currentSelection)
            {
                case 0:
                    Debug.Log("继续游戏");
                    EventBus.Publish(new ResumeGameEvent()); // 发布游戏恢复事件
                    break;
                case 1:
                    SceneTransitionManager.Instance.ReloadSceneWithFade(); // 重新加载场景
                    break;
                case 2:
                    pausePanel.SetActive(false); // 禁用暂停面板
                    settingPanel.SetActive(true); // 激活设置面板
                    break;
                case 3:
                    SceneManager.LoadScene("Menu"); // 返回主菜单
                    break;
            }
        }
    }
    
    // 高亮当前选中的菜单项
    void HighlightCurrentSelection()
    {
        for (int i = 0; i < menuTexts.Length; i++)
        {
            if (menuTexts[i] != null)
            {
                menuTexts[i].color = (i == currentSelection) ? Color.yellow : Color.white;
                menuTexts[i].fontSize = (i == currentSelection) ? 120 : 100 ;
            }
        }
    }
}
