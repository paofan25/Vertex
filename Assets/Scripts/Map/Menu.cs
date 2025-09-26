using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
    void Update()
    {
        // 获取当前选中的游戏对象
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        
        if (selectedObject != null)
        {
            // 检查是否有Button组件
            Button button = selectedObject.GetComponent<Button>();
            if (button != null)
            {
                // 获取按钮下的Text组件
                TMP_Text buttonText = selectedObject.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    string selectedText = buttonText.text;
                    Debug.Log("当前选中的按钮文本: " + selectedText);
                }
            }
        }
    }
    
    
    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
        
#if UNITY_EDITOR
        SceneManager.LoadScene("Level_Test");
#endif
    }

    public void QuitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
