using UnityEngine;

/// <summary>
/// 跳跃状态
/// </summary>
public class JumpingState : IPlayerState
{
    private bool canVariableJump;
    private float jumpHoldTimer;
    
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 执行跳跃
        Vector2 velocity = stateMachine.Velocity;
        velocity.y = stateMachine.movementData.jumpForce;
        stateMachine.SetVelocity(velocity);
        
        // 重置计时器
        stateMachine.JumpBufferTimer = 0f;
        stateMachine.CoyoteTimer = 0f;
        
        // 启用变速跳跃
        canVariableJump = true;
        jumpHoldTimer = 0f;
        
        // 触发跳跃事件
        AudioManager.Instance?.PlaySFX("Jump");
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 变速跳跃计时
        if (stateMachine.inputAdapter.JumpHeld && canVariableJump)
            jumpHoldTimer += Time.deltaTime;
        else if (!stateMachine.inputAdapter.JumpHeld)
            canVariableJump = false;
        
        // 检查状态转换
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
        
        // 变速跳跃：释放跳跃键时增加重力
        if (!stateMachine.inputAdapter.JumpHeld && canVariableJump)
        {
            velocity.y = Mathf.Max(velocity.y * 0.5f, stateMachine.movementData.minJumpForce);
            canVariableJump = false;
        }
        
        stateMachine.SetVelocity(velocity);
        
        // 翻转角色
        if (stateMachine.inputAdapter.MoveX != 0)
            stateMachine.Flip(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1);
        
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        canVariableJump = false;
    }
}