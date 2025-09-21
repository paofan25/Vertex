using UnityEngine;

/// <summary>
/// 待机状态
/// </summary>
public class IdleState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入待机状态
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 检查状态转换
        if (!stateMachine.IsGrounded)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
        
        if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
        {
            stateMachine.ChangeState<RunningState>();
            return;
        }
        
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
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 应用摩擦力
        Vector2 velocity = stateMachine.Velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, 0, stateMachine.movementData.runSpeed * Time.fixedDeltaTime * 2f);
        stateMachine.SetVelocity(velocity);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出待机状态
    }
}