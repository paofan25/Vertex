using UnityEngine;

/// <summary>
/// 奔跑状态
/// </summary>
public class RunningState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入奔跑状态
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 检查状态转换
        if (!stateMachine.IsGrounded)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
        
        if (Mathf.Abs(stateMachine.inputAdapter.MoveX) <= 0.1f)
        {
            stateMachine.ChangeState<IdleState>();
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
        // 水平移动
        float targetVelocityX = stateMachine.inputAdapter.MoveX * stateMachine.movementData.runSpeed;
        Vector2 velocity = stateMachine.Velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocityX, 
            stateMachine.movementData.runSpeed * Time.fixedDeltaTime * 10f);
        stateMachine.SetVelocity(velocity);
        
        // 翻转角色
        if (stateMachine.inputAdapter.MoveX != 0)
            stateMachine.Flip(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出奔跑状态
    }
}