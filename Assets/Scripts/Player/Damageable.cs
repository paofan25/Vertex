using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 可受伤组件
/// </summary>
public class Damageable : MonoBehaviour
{
    [Header("生命值")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    
    [Header("受伤设置")]
    [SerializeField] private float invincibilityTime = 1f;
    [SerializeField] private float flashInterval = 0.1f;
    
    [Header("组件引用")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerFacade playerFacade;
    
    // 状态
    private bool isInvincible;
    private Coroutine flashCoroutine;
    
    // 事件
    public event Action<int> OnHealthChanged;
    public event Action OnDie;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvincible => isInvincible;
    
    private void Start()
    {
        currentHealth = maxHealth;
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (playerFacade == null)
            playerFacade = GetComponent<PlayerFacade>();
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartInvincibility();
        }
    }
    
    /// <summary>
    /// 开始无敌时间
    /// </summary>
    private void StartInvincibility()
    {
        isInvincible = true;
        
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashCoroutine());
        
        Invoke(nameof(EndInvincibility), invincibilityTime);
    }
    
    /// <summary>
    /// 结束无敌时间
    /// </summary>
    private void EndInvincibility()
    {
        isInvincible = false;
        
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }
    
    /// <summary>
    /// 闪烁效果协程
    /// </summary>
    private IEnumerator FlashCoroutine()
    {
        while (isInvincible)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashInterval);
        }
    }
    
    /// <summary>
    /// 死亡处理
    /// </summary>
    private void Die()
    {
        OnDie?.Invoke();
        playerFacade?.TriggerDeath();
        
        // 禁用输入和物理
        playerFacade?.EnableInput(false);
        
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
    }
    
    /// <summary>
    /// 恢复生命值
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// 重置生命值
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        EndInvincibility();
        OnHealthChanged?.Invoke(currentHealth);
    }
}