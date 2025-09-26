using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// 冲刺状态
/// </summary>
public class DashState : IPlayerState
{
    private Vector2 dashDirection; // 冲刺方向
    // private float dashTimer; // 冲刺计时器
    // private bool isInvincible; // 无敌状态
    
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 调用冲刺协程
        stateMachine.StartCoroutine(Dash(stateMachine));
        
        // 重置计时器和状态
        // dashTimer = stateMachine.movementData.dashDuration;
        // stateMachine.CanDash = false;
        // stateMachine.DashCooldownTimer = stateMachine.movementData.dashCooldown;
        
        // 启用无敌帧
        // isInvincible = true;
        // stateMachine.StartCoroutine(IFramesCoroutine(stateMachine));
        
        // 触发冲刺事件
        AudioManager.Instance?.PlaySFX("Dash");
        CameraShaker.Instance?.Shake(0.1f, 0.2f);
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        // dashTimer -= Time.deltaTime;
        //
        // if (dashTimer <= 0)
        // {
        //     // 冲刺结束，根据当前状态切换
        //     if (stateMachine.IsGrounded)
        //     {
        //         if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
        //             stateMachine.ChangeState<RunningState>();
        //         else
        //             stateMachine.ChangeState<IdleState>();
        //     }
        //     else
        //     {
        //         stateMachine.ChangeState<FallingState>();
        //     }
        // }
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 冲刺期间忽略重力和摩擦
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // isInvincible = false;
    }

    private IEnumerator Dash(PlayerStateMachine stateMachine)
    {
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        stateMachine.DashCount--; // 减少冲刺次数
        stateMachine.IsDashing = true; // 正在冲刺
        stateMachine.rb.velocity = Vector2.zero; // 重置速度
        stateMachine.rb.gravityScale = 0; // 重力设为0
        
        // 确定冲刺方向
        Vector2 inputDirection = new Vector2(stateMachine.inputAdapter.MoveX, stateMachine.inputAdapter.MoveY).normalized;
        // 如果没有方向输入，则使用角色朝向，否则使用输入方向
        if (inputDirection.magnitude < 0.1f)
            dashDirection = new Vector2(stateMachine.FacingDirection, 0);
        else
            dashDirection = inputDirection;
        
        // 设置冲刺速度
        stateMachine.SetVelocity(dashDirection * stateMachine.movementData.dashForce);
        
        yield return new WaitForSeconds(stateMachine.movementData.dashDuration);
        
        stateMachine.rb.AddForce(-dashDirection * stateMachine.movementData.dashBackForce, ForceMode2D.Impulse); // 冲刺后反冲
        stateMachine.rb.gravityScale = stateMachine.movementData.gravityScale; // 恢复重力
        EventBus.Publish(new CanInputEvent(true)); // 发布启用输入事件
        stateMachine.IsDashing = false; // 冲刺结束

        // 冲刺结束后，将垂直速度清零，防止“微跳”
        // if (!stateMachine.IsGrounded)
        // {
        //     stateMachine.SetVelocity(new Vector2(stateMachine.Velocity.x, 0));
        // }
        
        // 冲刺结束，根据当前状态切换
        if (stateMachine.IsGrounded)
        {
            if (Mathf.Abs(stateMachine.inputAdapter.MoveX) > 0.1f)
                stateMachine.ChangeState<RunningState>();
            else
                stateMachine.ChangeState<IdleState>();
        }
        else
        {
            stateMachine.ChangeState<FallingState>();
        }
    }
    
    // /// <summary>
    // /// 无敌帧协程
    // /// </summary>
    // private IEnumerator IFramesCoroutine(PlayerStateMachine stateMachine)
    // {
    //     yield return new WaitForSeconds(stateMachine.movementData.iFramesDuration);
    //     isInvincible = false;
    // }
    
    // public bool IsInvincible => isInvincible;
}