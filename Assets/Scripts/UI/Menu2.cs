using System;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu2 : MonoBehaviour
{
    [SerializeField] private TMP_Text startText; // 开始游戏文本
    [SerializeField] private TMP_Text settingText; // 设置文本
    [SerializeField] private TMP_Text exitText; // 退出游戏文本
    
    [SerializeField] private GameObject settingPanel; // 设置面板
    
    private int currentSelection = 0; // 当前选中的菜单项索引
    private TMP_Text[] menuTexts; // 菜单文本数组

    void Start()
    {
        menuTexts = new TMP_Text[] { startText, settingText, exitText };
        HighlightCurrentSelection();
    }

    private void Update()
    {
        if (settingPanel.activeInHierarchy) return; // 如果设置面板处于活动状态，则不处理键盘输入
        
        HandleKeyboardInput();
    }
    
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
                    SceneManager.LoadScene("Level_Test");
                    break;
                case 1:
                    settingPanel.SetActive(true); // 激活设置面板
                    RectTransform rect = settingPanel.GetComponent<RectTransform>(); // 获取RectTransform组件
                    rect.DOAnchorPos(new Vector2(0, 0), 0.5f); // 移动
                    break;
                case 2:
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
            }
        }
    }

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