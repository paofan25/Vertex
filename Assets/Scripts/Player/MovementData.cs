using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Player/Movement Data")]
public class MovementData : ScriptableObject
{
    [Header("基础移动")]
    public float runSpeed = 8f;
    public float acceleration = 50f;
    public float deceleration = 60f;
    public float airControl = 0.6f;
    
    [Header("跳跃")]
    public float jumpForce = 15f;
    public float jumpCutMultiplier = 0.5f;
    public float jumpBufferTime = 0.1f;
    public float coyoteTime = 0.15f;
    
    [Header("重力")]
    public float gravity = 25f;
    public float maxFallSpeed = 20f;
    public float fastFallMultiplier = 2f;
    
    [Header("墙壁")]
    public float wallSlideSpeed = 5f;
    public float wallStickTime = 0.25f;
    public float wallJumpForce = 12f;
    public Vector2 wallJumpDirection = new Vector2(1f, 1.2f);
    
    [Header("冲刺")]
    public float dashForce = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    public int maxDashCount = 1;
    public float superDashExtraTime = 0.1f;
    
    [Header("地面检测")]
    public LayerMask groundLayer = 1;
    public LayerMask wallLayer = 1;
    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.1f;
}