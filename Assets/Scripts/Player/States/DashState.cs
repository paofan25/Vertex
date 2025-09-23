using UnityEngine;
using System.Collections;

/// <summary>
/// 冲刺状态
/// </summary>
public class DashState : IPlayerState
{
    private Vector2 dashDirection;
    private float dashTimer;
    private bool isInvincible;
    
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 确定冲刺方向
        Vector2 inputDirection = new Vector2(stateMachine.inputAdapter.MoveX, stateMachine.inputAdapter.MoveY);
        if (inputDirection.magnitude < 0.1f)
        {
            // 如果没有方向输入，则使用角色朝向
            dashDirection = new Vector2(stateMachine.FacingDirection, 0);
        }
        else
        {
            // 否则，使用输入方向
            dashDirection = inputDirection.normalized;
        }
        
        // 设置冲刺速度
        stateMachine.SetVelocity(dashDirection * stateMachine.movementData.dashForce);
        
        // 重置计时器和状态
        dashTimer = stateMachine.movementData.dashDuration;
        stateMachine.CanDash = false;
        stateMachine.DashCooldownTimer = stateMachine.movementData.dashCooldown;
        
        // 启用无敌帧
        isInvincible = true;
        stateMachine.StartCoroutine(IFramesCoroutine(stateMachine));
        
        // 触发冲刺事件
        AudioManager.Instance?.PlaySFX("Dash");
        CameraShaker.Instance?.Shake(0.1f, 0.2f);
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {
        dashTimer -= Time.deltaTime;
        
        if (dashTimer <= 0)
        {
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
    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        // 冲刺期间忽略重力和摩擦
        stateMachine.UpdateAnimator();
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        isInvincible = false;
    }
    
    /// <summary>
    /// 无敌帧协程
    /// </summary>
    private IEnumerator IFramesCoroutine(PlayerStateMachine stateMachine)
    {
        yield return new WaitForSeconds(stateMachine.movementData.iFramesDuration);
        isInvincible = false;
    }
    
    public bool IsInvincible => isInvincible;
}