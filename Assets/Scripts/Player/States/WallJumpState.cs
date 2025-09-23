using UnityEngine;

/// <summary>
/// 墙壁跳跃状态
/// </summary>
public class WallJumpState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 确定墙壁方向 (-1 for left, 1 for right)
        int wallDirection = stateMachine.FacingDirection;

        // 获取玩家输入方向
        float moveX = stateMachine.inputAdapter.MoveX;

        // 计算墙跳力度
        float jumpForceX, jumpForceY;

        // 判断是蹬墙跳还是贴墙跳
        if (Mathf.Sign(moveX) == wallDirection)
        {
            // 贴墙跳 (向上跳)
            jumpForceX = 0;
            jumpForceY = stateMachine.movementData.wallJumpForce * 1.2f; // 给予更高的垂直力
        }
        else
        {
            // 蹬墙跳 (向外跳)
            jumpForceX = -wallDirection * stateMachine.movementData.wallJumpForceX;
            jumpForceY = stateMachine.movementData.wallJumpForceY;
        }

        Vector2 jumpForce = new Vector2(jumpForceX, jumpForceY);
        stateMachine.SetVelocity(jumpForce);

        // 翻转角色以背对墙壁
        stateMachine.Flip(-wallDirection);
        
        // 触发跳跃音效
        AudioManager.Instance?.PlaySFX("Jump");
    }

    public void Update(PlayerStateMachine stateMachine)
    {
        // 在墙跳过程中，可以转换到下落状态
        if (stateMachine.Velocity.y <= 0)
        {
            stateMachine.ChangeState<FallingState>();
        }
    }

    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 墙跳时的物理控制（例如空中加速）可以在这里添加
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