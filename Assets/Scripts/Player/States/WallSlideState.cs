using UnityEngine;

/// <summary>
/// 墙滑状态
/// </summary>
public class WallSlideState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 面向墙面
        stateMachine.Flip(stateMachine.inputAdapter.MoveX > 0 ? 1 : -1);
    }

    public void Update(PlayerStateMachine stateMachine)
    {
        // 如果玩家按下了抓墙键，则切换到攀爬状态
        if (stateMachine.inputAdapter.GrabHeld)
        {
            Debug.Log("[State Switch] WallSlide -> Climbing | Reason: GrabHeld");
            stateMachine.ChangeState<ClimbingState>();
            return;
        }

        // 如果离开墙壁，或者不再朝墙移动，则切换到下落
        if (!stateMachine.IsAgainstWall || stateMachine.inputAdapter.MoveX * stateMachine.FacingDirection <= 0)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
    }

    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 自动向下滑行
        stateMachine.motor.SetVelocityY(-stateMachine.movementData.wallSlideSpeed);
    }

    public void Exit(PlayerStateMachine stateMachine)
    {
    }
    
    
}