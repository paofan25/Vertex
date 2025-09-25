using UnityEngine;

/// <summary>
/// 待机状态
/// </summary>
public class IdleState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 如果有跳跃缓冲，立即跳跃
        if (stateMachine.IsJumpBuffered)
        {
            stateMachine.ChangeState<JumpingState>();
            return;
        }
        
        // 进入待机状态
        stateMachine.rb.velocity = new Vector2(stateMachine.rb.velocity.x, 0); // 重置Y轴速度
        stateMachine.DashCount = stateMachine.movementData.maxDashCount; // 重置冲刺次数
        
        stateMachine.animator.Play("Idle");
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 如果按下【跳跃键】，切换至跳跃状态
        if (stateMachine.inputAdapter.JumpPressed)
        {
            stateMachine.ChangeState<JumpingState>();
            return;
        }
        
        // 检查是否主动抓墙(不在地面上时才能)
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded) {
            stateMachine.ChangeState<ClimbingState>();
            return;
        }

        // 检查状态转换
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded)
        {
            Debug.Log("[State Switch] Idle -> Climbing | Reason: GrabHeld and Against Wall");
            stateMachine.ChangeState<ClimbingState>();
            return;
        }

        // 如果【不在地面】上，切换至坠落状态
        if (!stateMachine.IsGrounded)
        {
            stateMachine.CoyoteTimer = stateMachine.movementData.coyoteTime; // 启动郊狼时间
            stateMachine.ChangeState<FallingState>();
            return;
        }
        
        // 如果有【水平输入】，切换至奔跑状态
        if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.01f)
        {
            stateMachine.ChangeState<RunningState>();
            return;
        }
        
        // 如果按下【冲刺键】且【不在冲刺中】且【有剩余冲刺次数】且【能够冲刺】，切换至冲刺状态
        if (stateMachine.inputAdapter.DashPressed && !stateMachine.IsDashing && stateMachine.DashCount > 0 && stateMachine.CanDash)
        {
            stateMachine.ChangeState<DashState>();
            return;
        }
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 应用摩擦力
        Vector2 velocity = stateMachine.Velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, 0, stateMachine.movementData.runSpeed * Time.fixedDeltaTime * 15f);
        stateMachine.SetVelocity(velocity);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出待机状态
    }
}