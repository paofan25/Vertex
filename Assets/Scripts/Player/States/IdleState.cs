using UnityEngine;

/// <summary>
/// 待机状态
/// </summary>
public class IdleState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入待机状态
        stateMachine.rb.velocity = new Vector2(stateMachine.rb.velocity.x, 0);
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
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

        // 如果不在地面上，切换至坠落状态
        if (!stateMachine.IsGrounded)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
        
        // 如果有水平输入，切换至奔跑状态
        if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.01f)
        {
            stateMachine.ChangeState<RunningState>();
            return;
        }
        
        // 如果按下跳跃键，切换至跳跃状态
        if (stateMachine.inputAdapter.JumpPressed)
        {
            stateMachine.ChangeState<JumpingState>();
            return;
        }
        
        // 如果按下冲刺键，切换至冲刺状态
        if (stateMachine.inputAdapter.DashPressed && stateMachine.CanDash && stateMachine.DashCooldownTimer <= 0)
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