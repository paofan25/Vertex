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
        // 检查是否主动抓墙
        if (stateMachine.inputAdapter.GrabHeld && stateMachine.IsAgainstWall && !stateMachine.IsGrounded)
        {
            Debug.Log("[State Switch] Running -> Climbing | Reason: GrabHeld and Against Wall");
            stateMachine.ChangeState<ClimbingState>();
            return;
        }

        // 检查状态转换
        if (!stateMachine.IsGrounded)
        {
            // Debug.Log("[RunningState] Not Grounded -> FallingState");
            stateMachine.ChangeState<FallingState>();
            return;
        }
        
        if (Mathf.Abs(stateMachine.inputAdapter.MoveX) <= 0.1f)
        {
            // Debug.Log("[RunningState] No Input -> IdleState");
            stateMachine.ChangeState<IdleState>();
            return;
        }
        
        if (stateMachine.inputAdapter.JumpPressed)
        {
            // Debug.Log("[RunningState] Jump Pressed -> JumpingState");
            stateMachine.ChangeState<JumpingState>();
            return;
        }
        
        if (stateMachine.inputAdapter.DashPressed && stateMachine.CanDash && stateMachine.DashCooldownTimer <= 0)
        {
            // Debug.Log("[RunningState] Dash Pressed -> DashState");
            stateMachine.ChangeState<DashState>();
            return;
        }
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 将物理移动委托给Motor
        // Debug.Log($"[RunningState] MoveX Input: {stateMachine.inputAdapter.MoveX}");
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