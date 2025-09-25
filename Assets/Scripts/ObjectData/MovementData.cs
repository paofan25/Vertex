using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Player/Movement Data")]
public class MovementData : ScriptableObject
{
    [Header("基础移动")]
    [Tooltip("移动速度")] public float runSpeed = 8f;
    [Tooltip("加速度")] public float acceleration = 50f;
    [Tooltip("减速度")] public float deceleration = 60f;
    [Tooltip("空中系数")] public float airControl = 0.6f;
    
    [Header("跳跃")]
    [Tooltip("跳跃力度")] public float jumpForce = 20f;
    [Tooltip("向上阻尼")] public float upDrag = 5f;
    [Tooltip("松开按键时的向下阻力")] public float downForce = 0.3f;
    // public float minJumpForce = 5f;
    // public float jumpCutMultiplier = 0.5f;
    [Tooltip("跳跃缓冲时间")] public float jumpBufferTime = 0.1f;
    [Tooltip("郊狼时间")] public float coyoteTime = 0.15f;
    
    [Header("重力")]
    [Tooltip("初始重力")] public float gravityScale = 3f;
    [Tooltip("下落重力")] public float fallGravityScale = 4f; 
    [Tooltip("最大下落速度")] public float maxFallSpeed = 20f;
    // public float fastFallMultiplier = 2f;
    
    [Header("墙壁")]
    public float wallSlideSpeed = 5f;
    public float wallStickTime = 0.25f;
    public float wallJumpForce = 12f;
    public float wallJumpForceX = 12f;
    public float wallJumpForceY = 12f;
    public Vector2 wallJumpDirection = new Vector2(1f, 1.2f);

    [Header("冲刺")] 
    [Tooltip("最大冲刺次数")] public int maxDashCount = 2;
    [Tooltip("冲刺速度")] public float dashForce = 20f;
    [Tooltip("冲刺持续时间")] public float dashDuration = 0.15f;
    [Tooltip("冲刺反冲力")] public float dashBackForce = 10f;
    public float dashCooldown = 1f;
    public float superDashExtraTime = 0.1f;
    
    [Header("地面检测")]
    public LayerMask groundLayer = 1;
    public LayerMask wallLayer = 1;
    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 3f;
    public float iFramesDuration = 0.1f;//无敌帧时长
    public float climbStamina=10f;//攀爬耐力大小
    public float climbSpeed = 5f;
    public float staminaRegenRate = 0.3f;//体力回复倍率
    public float climbHoldStaminaCost = 1f; // 悬停体力消耗
    public float climbUpStaminaCost = 2f;   // 上爬体力消耗
}