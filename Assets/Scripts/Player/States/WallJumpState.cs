using UnityEngine;

/// <summary>
/// 墙跳状态
/// </summary>
public class WallJumpState : IPlayerState
{
    private float wallJumpTimer;
    private int wallJumpDirection;
    
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 确定墙跳方向（与墙面相反）
        wallJumpDirection = stateMachine.FacingDirection == 1 ? -1 : 1;
        
        // 执行墙跳
        Vector2 wallJumpForce = new Vector2(
            wallJumpDirection * stateMachine.movementData.wallJumpForceX,
            stateMachine.movementData.wallJumpForceY
        );
        
        stateMachine.SetVelocity(wallJumpForce);
        
        // 重置计时器
        stateMachine.JumpBufferTimer = 0f;
        wallJumpTimer = 0.2f; // 短暂的墙跳控制时间
        
        // 翻转角色
        stateMachine.Flip(wallJumpDirection);
        
        // 触发跳跃音效
        AudioManager.Instance?.PlaySFX("Jump");
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        wallJumpTimer -= Time.deltaTime;
        
        // 检查状态转换
        if (stateMachine.IsGrounded)
        {
            stateMachine.CanDash = true; // 着地恢复冲刺
            
            if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
                stateMachine.ChangeState<RunningState>();
            else
                stateMachine.ChangeState<IdleState>();
            return;
        }
        
        if (stateMachine.Velocity.y <= 0)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
        
        if (stateMachine.inputAdapter.DashPressed && stateMachine.CanDash && stateMachine.DashCooldownTimer <= 0)
        {
            stateMachine.ChangeState<DashState>();
            return;
        }
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        Vector2 velocity = stateMachine.Velocity;
        
        // 墙跳初期限制水平控制
        if (wallJumpTimer > 0)
        {
            // 减少水平控制，保持墙跳轨迹
            float inputInfluence = Mathf.Lerp(0.2f, 1f, 1f - (wallJumpTimer / 0.2f));
            float targetVelocityX = stateMachine.inputAdapter.MoveX * stateMachine.movementData.runSpeed * inputInfluence;
            velocity.x = Mathf.Lerp(velocity.x, targetVelocityX, Time.fixedDeltaTime * 5f);
        }
        else
        {
            // 正常空中控制
            float targetVelocityX = stateMachine.inputAdapter.MoveX * stateMachine.movementData.runSpeed;
            velocity.x = Mathf.MoveTowards(velocity.x, targetVelocityX, 
                stateMachine.movementData.runSpeed * stateMachine.movementData.airControl * Time.fixedDeltaTime * 10f);
        }
        
        stateMachine.SetVelocity(velocity);
        
        // 根据输入翻转角色（墙跳后期）
        if (wallJumpTimer <= 0 && stateMachine.inputAdapter.MoveX != 0)
            stateMachine.Flip(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出墙跳状态
    }
}