using UnityEngine;

/// <summary>
/// 下落状态
/// </summary>
public class FallingState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入下落状态
        stateMachine.animator.Play("Fall");
        stateMachine.rb.gravityScale = stateMachine.movementData.fallGravityScale; // 设置下落重力
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 检查是否主动抓墙(不在地面上时才能)
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded && stateMachine.CurrentStamina > 0)
        {
            Debug.Log("[State Switch] Falling -> Climbing | Reason: GrabHeld and Against Wall");
            stateMachine.ChangeState<ClimbingState>();
            return;
        }

        if (stateMachine.IsGrounded)
        {
            // 着地音效和震屏
            AudioManager.Instance?.PlaySFX("Land");
            CameraShaker.Instance?.Shake(0.05f, 0.1f);
            
            // 接触平台，重置冲刺和体力
            stateMachine.ResetDash();
            stateMachine.ResetStamina();
            
            if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
                stateMachine.ChangeState<RunningState>();
            else
                stateMachine.ChangeState<IdleState>();
            return;
        }
        
        
        // 如果按下【冲刺键】且【不在冲刺中】且【有剩余冲刺次数】且【能够冲刺】，切换至冲刺状态
        if (stateMachine.inputAdapter.DashPressed && !stateMachine.IsDashing && stateMachine.DashCount > 0 && stateMachine.CanDash)
        {
            stateMachine.ChangeState<DashState>();
            return;
        }
        
        // 检查墙面交互
        if (CheckWallSlide(stateMachine))
        {
            Debug.Log("[State Switch] Falling -> WallSlide | Reason: Against Wall and Horizontal Input");
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
        // if (stateMachine.inputAdapter.MoveY < -0.5f)
        // {
        //     velocity.y -= stateMachine.movementData.fastFallMultiplier * Time.fixedDeltaTime * 10f;
        // }
        
        // 设置速度
        stateMachine.SetVelocity(velocity);
        
        // 翻转角色
        if (stateMachine.inputAdapter.MoveX != 0)
            stateMachine.Flip(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出下落状态
        stateMachine.rb.gravityScale = stateMachine.movementData.gravityScale; // 恢复重力
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
            stateMachine.movementData.wallCheckDistance, stateMachine.movementData.wallLayer);
        
        return hit.collider != null;
    }
}