using UnityEngine;

/// <summary>
/// 攀爬状态
/// </summary>
public class ClimbingState : IPlayerState
{
    private float currentStamina;
    private bool isClimbing;
    private PlayerStateMachine cachedStateMachine;
    
    public void Enter(PlayerStateMachine stateMachine)
    {
        cachedStateMachine = stateMachine;
        currentStamina = stateMachine.movementData.climbStamina;
        isClimbing = true;
        
        // 停止垂直速度
        Vector2 velocity = stateMachine.Velocity;
        velocity.y = 0;
        stateMachine.SetVelocity(velocity);
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 检查状态转换
        if (stateMachine.IsGrounded)
        {
            if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
                stateMachine.ChangeState<RunningState>();
            else
                stateMachine.ChangeState<IdleState>();
            return;
        }
        
        // 跳跃离开墙面
        if (stateMachine.JumpBufferTimer > 0)
        {
            stateMachine.ChangeState<WallJumpState>();
            return;
        }
        
        // 冲刺
        if (stateMachine.inputAdapter.DashPressed && stateMachine.CanDash && stateMachine.DashCooldownTimer <= 0)
        {
            stateMachine.ChangeState<DashState>();
            return;
        }
        
        // 离开墙面或耐力耗尽
        if (!IsAgainstWall(stateMachine) || currentStamina <= 0 || 
            stateMachine.inputAdapter.MoveX * stateMachine.FacingDirection <= 0)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        Vector2 velocity = stateMachine.Velocity;
        
        // 攀爬移动
        if (Mathf.Abs(stateMachine.inputAdapter.MoveY) > 0.1f && currentStamina > 0)
        {
            velocity.y = stateMachine.inputAdapter.MoveY * stateMachine.movementData.climbSpeed;
            currentStamina -= Time.fixedDeltaTime;
        }
        else
        {
            velocity.y = 0;
            // 恢复耐力（当不在攀爬时）
            if (Mathf.Abs(stateMachine.inputAdapter.MoveY) <= 0.1f)
            {
                currentStamina = Mathf.Min(stateMachine.movementData.climbStamina, 
                    currentStamina + stateMachine.movementData.staminaRegenRate * Time.fixedDeltaTime);
            }
        }
        
        velocity.x = 0; // 贴墙
        stateMachine.SetVelocity(velocity);
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        isClimbing = false;
        cachedStateMachine = null;
    }
    
    /// <summary>
    /// 检查是否贴着墙面
    /// </summary>
    private bool IsAgainstWall(PlayerStateMachine stateMachine)
    {
        Vector2 rayOrigin = stateMachine.transform.position;
        Vector2 rayDirection = new Vector2(stateMachine.FacingDirection, 0);
        
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 
            stateMachine.movementData.wallCheckDistance, stateMachine.movementData.wallLayers);
        
        return hit.collider != null;
    }
    
    public float CurrentStamina => currentStamina;
    public float MaxStamina => cachedStateMachine?.movementData.climbStamina ?? 0f;
}
