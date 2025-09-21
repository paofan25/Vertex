using UnityEngine;

/// <summary>
/// 玩家移动参数配置
/// </summary>
[CreateAssetMenu(fileName = "PlayerMovementData", menuName = "Player/Movement Data")]
public class PlayerMovementData : ScriptableObject
{
    [Header("基础移动")]
    [Tooltip("奔跑速度")]
    public float runSpeed = 8f;
    [Tooltip("空中控制力")]
    public float airControl = 0.8f;
    [Tooltip("最大下落速度")]
    public float maxFallSpeed = 20f;
    
    [Header("跳跃参数")]
    [Tooltip("跳跃力度")]
    public float jumpForce = 15f;
    [Tooltip("最小跳跃力度")]
    public float minJumpForce = 8f;
    [Tooltip("跳跃按键保持时间")]
    public float jumpHoldTime = 0.3f;
    [Tooltip("快速下落倍数")]
    public float fastFallMultiplier = 2f;
    [Tooltip("土狼时间")]
    public float coyoteTime = 0.15f;
    [Tooltip("跳跃缓冲时间")]
    public float jumpBufferTime = 0.2f;
    
    [Header("冲刺参数")]
    [Tooltip("冲刺速度")]
    public float dashSpeed = 20f;
    [Tooltip("冲刺持续时间")]
    public float dashDuration = 0.2f;
    [Tooltip("冲刺冷却时间")]
    public float dashCooldown = 1f;
    [Tooltip("无敌帧时间")]
    public float iFramesDuration = 0.1f;
    
    [Header("墙面交互")]
    [Tooltip("墙跳水平力")]
    public float wallJumpForceX = 12f;
    [Tooltip("墙跳垂直力")]
    public float wallJumpForceY = 15f;
    [Tooltip("墙滑速度")]
    public float wallSlideSpeed = 3f;
    [Tooltip("贴墙时间")]
    public float wallStickTime = 0.1f;
    
    [Header("攀爬参数")]
    [Tooltip("攀爬速度")]
    public float climbSpeed = 5f;
    [Tooltip("攀爬耐力")]
    public float climbStamina = 3f;
    [Tooltip("耐力恢复速度")]
    public float staminaRegenRate = 1f;
    
    [Header("检测参数")]
    [Tooltip("地面图层")]
    public LayerMask groundLayers;
    [Tooltip("墙面图层")]
    public LayerMask wallLayers;
    [Tooltip("地面检测距离")]
    public float groundCheckDistance = 0.1f;
    [Tooltip("墙面检测距离")]
    public float wallCheckDistance = 0.5f;
}