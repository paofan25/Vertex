using UnityEngine;

/// <summary>
/// 跳跃状态
/// </summary>
public class JumpingState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 消耗跳跃缓冲
        stateMachine.JumpBufferTimer = 0f;
        
        // 执行跳跃
        stateMachine.rb.gravityScale = stateMachine.movementData.gravityScale; // 设置重力
        stateMachine.rb.drag = stateMachine.movementData.upDrag; // 修改阻尼
        stateMachine.rb.velocity = new Vector2(stateMachine.rb.velocity.x, 0); // 重置y轴速度
        stateMachine.rb.AddForce(Vector2.up * stateMachine.movementData.jumpForce, ForceMode2D.Impulse); // 施加跳跃力
        
        // Vector2 velocity = stateMachine.Velocity;
        // velocity.y = stateMachine.movementData.jumpForce;
        // stateMachine.SetVelocity(velocity);
        
        stateMachine.animator.Play("Jump");
        
        // 播放跳跃音效
        AudioManager.Instance?.PlaySFX("Jump");
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 可变跳跃：如果松开跳跃键，则缩短跳跃高度
        if (stateMachine.Velocity.y > 0 && !stateMachine.inputAdapter.JumpHeld)
        {
            stateMachine.rb.AddForce(Vector2.down * stateMachine.movementData.downForce, ForceMode2D.Impulse);
            // stateMachine.SetVelocity(new Vector2(stateMachine.Velocity.x, 0));
        }
        
        // 检查是否主动抓墙(不在地面上时才能)
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded && stateMachine.CurrentStamina > 0) {
            stateMachine.ChangeState<ClimbingState>();
            Debug.Log("Climbing");
            return;
        }
        
        // 检查状态转换
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded && stateMachine.CurrentStamina > 0)
        {
            Debug.Log("[State Switch] Jumping -> Climbing | Reason: GrabHeld and Against Wall");
            stateMachine.ChangeState<ClimbingState>();
            return;
        }

        // 如果速度向下，切换至坠落状态
        if (stateMachine.Velocity.y <= 0)
        {
            stateMachine.ChangeState<FallingState>();
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
        stateMachine.rb.drag = 0; // 重置阻尼
    }
}