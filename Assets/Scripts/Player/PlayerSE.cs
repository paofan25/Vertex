using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSE : MonoBehaviour
{
    public AudioSource SEsound; //音效控制
    public AudioClip move; //角色移动音效
    public AudioClip jump; //角色跳跃音效
    public AudioClip fall; //角色下落音效
    public AudioClip die; //角色死亡音效
    public AudioClip rebirth; //角色重生音效
    
    private Coroutine fadeOutCoroutine; // 淡出协程

    private void Start()
    {
        SEsound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayMoveSEEvent>(Move);
        EventBus.Subscribe<PlayJumpSEEvent>(Jump);
        EventBus.Subscribe<PlayFallSEEvent>(Fall);
        EventBus.Subscribe<PlayDeadSEEvent>(Die);
        EventBus.Subscribe<PlayRebrithSEEvent>(Rebirth);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayMoveSEEvent>(Move);
        EventBus.Unsubscribe<PlayJumpSEEvent>(Jump);
        EventBus.Unsubscribe<PlayFallSEEvent>(Fall);
        EventBus.Unsubscribe<PlayDeadSEEvent>(Die);
        EventBus.Unsubscribe<PlayRebrithSEEvent>(Rebirth);
    }
    
    // 移动音效
    private void Move(GameEvent gameEvent)
    {
        PlayMoveSEEvent moveEvent = (PlayMoveSEEvent)gameEvent;

        if (moveEvent.isPlay)
        {
            // 如果正在淡出，先停止淡出
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = null;
            }
            
            SEsound.volume = 1f; // 重置音量
            
            SEsound.clip = move;
            SEsound.loop = true;
            SEsound.Play();
        }
        else
        {
            fadeOutCoroutine = StartCoroutine(FadeOutSound(0.1f)); // 0.3秒淡出
        }
    }

    // 跳跃音效
    private void Jump(GameEvent gameEvent)
    {
        SEsound.PlayOneShot(jump);
    }

    // 落地音效
    private void Fall(GameEvent gameEvent)
    {
        SEsound.PlayOneShot(fall);
    }

    // 死亡音效
    private void Die(GameEvent gameEvent)
    {
        SEsound.PlayOneShot(die);
    }

    // 重生音效
    private void Rebirth(GameEvent gameEvent)
    {
        Debug.Log(rebirth);
        SEsound.PlayOneShot(rebirth);
    }
    
    // 淡出音效
    private IEnumerator FadeOutSound(float fadeDuration)
    {
        float startVolume = SEsound.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            SEsound.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        SEsound.clip = null;
        SEsound.loop = false;
        SEsound.Stop();
        SEsound.volume = startVolume; // 恢复音量
        fadeOutCoroutine = null;
    }
}
