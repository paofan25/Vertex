using UnityEngine;

/// <summary>
/// 跳跃状态
/// </summary>
public class JumpingState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 执行跳跃
        Vector2 velocity = stateMachine.Velocity;
        velocity.y = stateMachine.movementData.jumpForce;
        stateMachine.SetVelocity(velocity);
        
        // 触发跳跃事件
        AudioManager.Instance?.PlaySFX("Jump");
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 可变跳跃：如果松开跳跃键，则缩短跳跃高度
        if (stateMachine.Velocity.y > 0 && !stateMachine.inputAdapter.JumpHeld)
        {
            stateMachine.SetVelocity(new Vector2(stateMachine.Velocity.x, 0));
        }
        
        // 检查是否主动抓墙(不在地面上时才能)
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded) {
            stateMachine.ChangeState<ClimbingState>();
            Debug.Log("Climbing");
            return;
        }
        // 检查状态转换
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded)
        {
            Debug.Log("[State Switch] Jumping -> Climbing | Reason: GrabHeld and Against Wall");
            stateMachine.ChangeState<ClimbingState>();
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
        // 空中水平控制
        float targetVelocityX = stateMachine.inputAdapter.MoveX * stateMachine.movementData.runSpeed;
        Vector2 velocity = stateMachine.Velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocityX, 
            stateMachine.movementData.runSpeed * stateMachine.movementData.airControl * Time.fixedDeltaTime * 10f);
        
        stateMachine.SetVelocity(velocity);
        
        // 翻转角色
        if (stateMachine.inputAdapter.MoveX != 0)
            stateMachine.Flip(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
    }
}