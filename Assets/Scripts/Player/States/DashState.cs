using UnityEngine;
using System.Collections;

/// <summary>
/// 冲刺状态
/// </summary>
public class DashState : IPlayerState
{
    private Coroutine dashCoroutine;

    public void Enter(PlayerStateMachine stateMachine)
    {
        // 调用冲刺协程
        stateMachine.DashBufferTimer = 0f; // 消耗冲刺缓冲
        dashCoroutine = stateMachine.StartCoroutine(Dash(stateMachine));
        
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
        if (dashCoroutine != null)
        {
            stateMachine.StopCoroutine(dashCoroutine);
        }
        // 确保退出状态时，玩家的控制权和物理状态恢复正常
        stateMachine.motor.enabled = true;
        stateMachine.rb.gravityScale = stateMachine.movementData.gravityScale;
        stateMachine.IsDashing = false;
    }

    private IEnumerator Dash(PlayerStateMachine stateMachine)
    {
        float originalGravity = stateMachine.rb.gravityScale;
        stateMachine.IsDashing = true;
        stateMachine.motor.enabled = false; // 禁用常规移动
        stateMachine.rb.gravityScale = 0f;

        // 确定冲刺方向
        Vector2 dashDir = new Vector2(stateMachine.inputAdapter.MoveX, stateMachine.inputAdapter.MoveY).normalized;
        if (dashDir == Vector2.zero)
        {
            dashDir = new Vector2(stateMachine.FacingDirection, 0);
        }

        // 计算冲刺速度和持续时间
        float dashSpeed = stateMachine.movementData.dashForce;
        float dashDuration = stateMachine.movementData.dashDuration;
        
        // 向上冲刺特殊处理
        bool isUpDash = dashDir.y > 0.5f && Mathf.Abs(dashDir.x) < 0.5f;
        if (isUpDash)
        {
            dashSpeed *= stateMachine.movementData.upDashForceMultiplier;
        }

        stateMachine.rb.velocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        stateMachine.rb.gravityScale = originalGravity;
        stateMachine.motor.enabled = true; // 恢复常规移动
        stateMachine.IsDashing = false;

        // 冲刺结束后的速度处理
        if (isUpDash)
        {
            // 向上冲刺后给予短暂的滞空
            stateMachine.rb.velocity = new Vector2(stateMachine.rb.velocity.x, stateMachine.movementData.upDashHangForce);
            yield return new WaitForSeconds(stateMachine.movementData.upDashHangTime);
        }
        else if (!stateMachine.IsGrounded)
        {
            // 其他方向的空中冲刺，结束后水平速度减半，垂直速度清零
            stateMachine.rb.velocity = new Vector2(stateMachine.rb.velocity.x * 0.5f, 0);
        }
        
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