using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject selectedObject; // 当前选中的对象
    [SerializeField] private GameObject menuCurrentButton; // 菜单面板的第一个按钮
    [SerializeField] private GameObject settingFirstButton; // 设置面板的第一个按钮
    [SerializeField] private GameObject settingPanel; // 设置面板
    private Selectable lastSelected;
    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;
    
    void Start()
    {
        DisableMouse(); // 禁用鼠标
        
        Button firstButton = EventSystem.current.firstSelectedGameObject?.GetComponent<Button>();
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }
    
    void Update()
    {
        selectedObject = EventSystem.current.currentSelectedGameObject;
        Selectable currentSelected = selectedObject?.GetComponent<Selectable>();
        
        if (currentSelected != lastSelected)
        {
            if (lastSelected)
            {
                TMP_Text lastText = lastSelected.GetComponentInChildren<TMP_Text>();
                if (lastText)
                {
                    lastText.color = normalColor; // 设置未选中时的颜色
                    lastText.fontSize -= 20; // 设置未选中时的字体大小
                }
            }
            
            if (currentSelected)
            {
                TMP_Text currentText = currentSelected.GetComponentInChildren<TMP_Text>();
                if (currentText)
                {
                    currentText.color = selectedColor; // 设置选中时的颜色
                    currentText.fontSize += 20; // 设置选中时的字体大小
                    Debug.Log("当前选中的按钮文本: " + currentText.text);
                }
            }
            
            lastSelected = currentSelected;
        }
        
        if (!currentSelected && lastSelected)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected.gameObject);
        }
    }
    
    public void DisableMouse()
    {
        Cursor.visible = false; // 隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked; // 锁定在屏幕中心
    }
    
    public void StartGame()
    {
        #if UNITY_EDITOR
        SceneManager.LoadScene("Level_Test");
        #else
        Debug.Log(1);
        SceneManager.LoadScene("Level_1");
        #endif
    }

    public void EnterSetting()
    {
        settingPanel.SetActive(true); // 激活设置面板
        EventSystem.current.SetSelectedGameObject(settingFirstButton.gameObject);
        RectTransform rect = settingPanel.GetComponent<RectTransform>(); // 获取RectTransform组件
        rect.DOAnchorPos(new Vector2(0, 0), 0.5f); // 移动
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}