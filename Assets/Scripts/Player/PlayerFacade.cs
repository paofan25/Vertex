using System;
using UnityEngine;

/// <summary>
/// 玩家对外接口 - 契约API
/// </summary>
public sealed class PlayerFacade : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private PlayerMoveControls moveControls;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    
    // 对外属性
    public Vector2 Velocity => rb.velocity;
    public bool IsGrounded => moveControls.IsGrounded;
    public bool IsDashing => moveControls.IsDashing;
    public bool IsClimbing => moveControls.IsClimbing;
    
    // 对外事件
    public event Action OnDie;
    public event Action<Transform> OnCheckpointReached;
    public event Action<int> OnCollectChanged;
    public event Action OnDashStart;
    public event Action OnLanded;
    
    private bool inputEnabled = true;
    
    private void Start()
    {
        if (moveControls == null)
            moveControls = GetComponent<PlayerMoveControls>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// 强制重生到指定点
    /// </summary>
    public void ForceRespawnAt(Transform point)
    {
        transform.position = point.position;
        rb.velocity = Vector2.zero;
        // 重置状态
        EnableInput(true);
    }
    
    /// <summary>
    /// 启用/禁用输入
    /// </summary>
    public void EnableInput(bool on)
    {
        inputEnabled = on;
        if (moveControls.gI != null)
            moveControls.gI.enabled = on;
    }
    
    /// <summary>
    /// 触发死亡事件
    /// </summary>
    public void TriggerDeath()
    {
        EnableInput(false);
        animator?.SetTrigger("Die");
        OnDie?.Invoke();
    }
    
    /// <summary>
    /// 触发检查点事件
    /// </summary>
    public void TriggerCheckpoint(Transform checkpoint)
    {
        OnCheckpointReached?.Invoke(checkpoint);
    }
    
    /// <summary>
    /// 触发收集品变化事件
    /// </summary>
    public void TriggerCollectChanged(int newCount)
    {
        OnCollectChanged?.Invoke(newCount);
    }
}