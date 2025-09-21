using UnityEngine;

/// <summary>
/// 下落状态
/// </summary>
public class FallingState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入下落状态
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 检查状态转换
        if (stateMachine.IsGrounded)
        {
            // 着地音效和震屏
            AudioManager.Instance?.PlaySFX("Land");
            CameraShaker.Instance?.Shake(0.05f, 0.1f);
            
            stateMachine.CanDash = true; // 着地恢复冲刺
            
            if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
                stateMachine.ChangeState<RunningState>();
            else
                stateMachine.ChangeState<IdleState>();
            return;
        }
        
        // 土狼时间内可以跳跃
        if (stateMachine.JumpBufferTimer > 0 && stateMachine.CoyoteTimer > 0)
        {
            stateMachine.ChangeState<JumpingState>();
            return;
        }
        
        if (stateMachine.inputAdapter.DashPressed && stateMachine.CanDash && stateMachine.DashCooldownTimer <= 0)
        {
            stateMachine.ChangeState<DashState>();
            return;
        }
        
        // 检查墙面交互
        if (CheckWallSlide(stateMachine))
        {
            stateMachine.ChangeState<WallSlideState>();
            return;
        }
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 空中水平控制
        float targetVelocityX = stateMachine.inputAdapter.MoveX * stateMachine.movementData.runSpeed;
        Vector2 velocity = stateMachine.Velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocityX, 
            stateMachine.movementData.runSpeed * stateMachine.movementData.airControl * Time.fixedDeltaTime * 10f);
        
        // 限制最大下落速度
        velocity.y = Mathf.Max(velocity.y, -stateMachine.movementData.maxFallSpeed);
        
        // 快速下落
        if (stateMachine.inputAdapter.MoveY < -0.5f)
        {
            velocity.y -= stateMachine.movementData.fastFallMultiplier * Time.fixedDeltaTime * 10f;
        }
        
        stateMachine.SetVelocity(velocity);
        
        // 翻转角色
        if (stateMachine.inputAdapter.MoveX != 0)
            stateMachine.Flip(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出下落状态
    }
    
    /// <summary>
    /// 检查是否可以墙滑
    /// </summary>
    private bool CheckWallSlide(PlayerStateMachine stateMachine)
    {
        if (Mathf.Abs(stateMachine.inputAdapter.MoveX) < 0.1f) return false;
        
        Vector2 rayOrigin = stateMachine.transform.position;
        Vector2 rayDirection = new Vector2(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1, 0);
        
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 
            stateMachine.movementData.wallCheckDistance, stateMachine.movementData.wallLayers);
        
        return hit.collider != null;
    }
}