using UnityEngine;

/// <summary>
/// 墙滑状态
/// </summary>
public class WallSlideState : IPlayerState
{
    private float wallStickTimer;
    private int wallDirection;
    
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 确定墙面方向
        wallDirection = stateMachine.inputAdapter.MoveX > 0 ? 1 : -1;
        wallStickTimer = stateMachine.movementData.wallStickTime;
        
        // 面向墙面
        stateMachine.Flip(wallDirection);
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        wallStickTimer -= Time.deltaTime;
        
        // 检查状态转换
        if (stateMachine.IsGrounded)
        {
            if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
                stateMachine.ChangeState<RunningState>();
            else
                stateMachine.ChangeState<IdleState>();
            return;
        }
        
        // 墙跳
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
        
        // 离开墙面
        if (!IsAgainstWall(stateMachine) || 
            (wallStickTimer <= 0 && stateMachine.inputAdapter.MoveX * wallDirection <= 0))
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        Vector2 velocity = stateMachine.Velocity;
        
        // 墙滑速度
        if (wallStickTimer <= 0)
        {
            velocity.y = Mathf.Max(velocity.y, -stateMachine.movementData.wallSlideSpeed);
        }
        else
        {
            velocity.y = 0; // 贴墙时间内不下滑
        }
        
        // 保持贴墙
        velocity.x = 0;
        
        stateMachine.SetVelocity(velocity);
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出墙滑状态
    }
    
    /// <summary>
    /// 检查是否贴着墙面
    /// </summary>
    private bool IsAgainstWall(PlayerStateMachine stateMachine)
    {
        Vector2 rayOrigin = stateMachine.transform.position;
        Vector2 rayDirection = new Vector2(wallDirection, 0);
        
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 
            stateMachine.movementData.wallCheckDistance, stateMachine.movementData.wallLayers);
        
        return hit.collider != null;
    }
}