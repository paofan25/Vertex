using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Menu : MonoBehaviour
{
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
