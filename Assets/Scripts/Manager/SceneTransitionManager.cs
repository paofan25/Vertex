using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    
    [Header("Transition Settings")]
    public Image fadeImage; // 淡入淡出图片
    public float fadeOutDuration = 0.5f; // 淡入持续时间
    public float fadeInDuration = 1f; // 淡入持续时间
    
    private bool isTransitioning = false; // 是否正在过渡
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 初始化遮罩状态
            if (fadeImage != null)
            {
                Color color = fadeImage.color;
                color.a = 0f;
                fadeImage.color = color;
                fadeImage.raycastTarget = false; // 初始不阻挡UI交互
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 重新加载场景
    /// </summary>
    public void ReloadSceneWithFade()
    {
        if (!isTransitioning)
        {
            StartCoroutine(ReloadSceneWithFadeCoroutine());
        }
    }
    
    private IEnumerator ReloadSceneWithFadeCoroutine()
    {
        isTransitioning = true;
        
        // 淡出到黑屏
        yield return StartCoroutine(FadeOut());
        
        // 重新加载场景前重置时间尺度
        Time.timeScale = 1f;
        
        // 重新加载场景
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
        
        // 等待一帧确保场景加载完成
        yield return null;
        
        // 从黑屏淡入
        yield return StartCoroutine(FadeIn());
        
        isTransitioning = false;
    }
    
    // 淡出
    private IEnumerator FadeOut()
    {
        float timer = 0f;
        Color color = fadeImage.color;
        
        // 淡出时阻挡UI交互
        fadeImage.raycastTarget = true;
        
        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(timer / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = 1f;
        fadeImage.color = color;
    }
    
    // 淡入
    private IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;
        color.a = 1f; // 从完全黑屏开始
        fadeImage.color = color;
        
        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = 1f - Mathf.Clamp01(timer / fadeInDuration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = 0f;
        fadeImage.color = color;
        
        // 淡入完成后恢复UI交互
        fadeImage.raycastTarget = false;
    }
}