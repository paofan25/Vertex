using UnityEngine;

/// <summary>
/// 攀爬状态
/// </summary>
public class ClimbingState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 附着在墙上时，速度清零
        stateMachine.SetVelocity(Vector2.zero);
    }

    public void Update(PlayerStateMachine stateMachine)
    {
        // 添加详细调试日志
        Debug.Log($"[Climbing Update] GrabHeld: {stateMachine.inputAdapter.GrabHeld}, IsAgainstWall: {stateMachine.IsAgainstWall}, Stamina: {stateMachine.CurrentStamina}");

        // 如果松开抓墙键，或者离开墙壁，则切换到下落
        if (!stateMachine.inputAdapter.GrabHeld || !stateMachine.IsAgainstWall)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }

        // 如果体力耗尽，则切换到下落
        if (stateMachine.CurrentStamina <= 0)
        {
            stateMachine.ChangeState<FallingState>();
            return;
        }
    }

    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        float moveY = stateMachine.inputAdapter.MoveY;
        
        // 根据是否有垂直输入来决定移动和体力消耗
        if (Mathf.Abs(moveY) > 0.1f)
        {
            // 向上移动消耗更多体力
            float staminaCost = (moveY > 0) ? stateMachine.movementData.climbUpStaminaCost : stateMachine.movementData.climbHoldStaminaCost;
            stateMachine.ConsumeStamina(staminaCost * Time.fixedDeltaTime);
            
            // 垂直移动
            stateMachine.motor.SetVelocityY(moveY * stateMachine.movementData.climbSpeed);
        }
        else
        {
            // 悬停时消耗较少体力
            stateMachine.ConsumeStamina(stateMachine.movementData.climbHoldStaminaCost * Time.fixedDeltaTime);
            stateMachine.motor.SetVelocityY(0);
        }
        
        // 强制将水平速度清零以吸附在墙上
        stateMachine.motor.SetVelocityX(0);
    }

    public void Exit(PlayerStateMachine stateMachine)
    {
    }
    
    
}
