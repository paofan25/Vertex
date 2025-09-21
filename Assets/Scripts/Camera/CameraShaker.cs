using UnityEngine;
using System.Collections;

/// <summary>
/// 相机震动器
/// </summary>
public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }
    
    [Header("震动设置")]
    [SerializeField] private float defaultIntensity = 0.1f;
    [SerializeField] private float defaultDuration = 0.2f;
    
    private Vector3 originalPosition;
    private bool isShaking;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            originalPosition = transform.localPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 震动相机
    /// </summary>
    public void Shake(float intensity = -1f, float duration = -1f)
    {
        if (isShaking) return;
        
        if (intensity < 0) intensity = defaultIntensity;
        if (duration < 0) duration = defaultDuration;
        
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }
    
    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        isShaking = true;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            transform.localPosition = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPosition;
        isShaking = false;
    }
}