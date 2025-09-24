using UnityEngine;

/// <summary>
/// 奔跑状态
/// </summary>
public class RunningState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入奔跑状态
        stateMachine.DashCount = stateMachine.movementData.maxDashCount; // 重置冲刺次数
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // 检查是否主动抓墙
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded)
        {
            Debug.Log("[State Switch] Running -> Climbing | Reason: GrabHeld and Against Wall");
            stateMachine.ChangeState<ClimbingState>();
            return;
        }

        // 如果不在地面上，切换至坠落状态
        if (!stateMachine.IsGrounded)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
        
        // 如果没有水平输入，切换至站立状态
        if (Mathf.Abs(stateMachine.inputAdapter.MoveX) <= 0.01f)
        {
            stateMachine.ChangeState<IdleState>();
            return;
        }
        
        // 如果按下【跳跃键】，切换至跳跃状态
        if (stateMachine.inputAdapter.JumpPressed)
        {
            stateMachine.ChangeState<JumpingState>();
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
        // 将物理移动委托给Motor
        stateMachine.motor.HandleGroundMovement(stateMachine.inputAdapter.MoveX);
        
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