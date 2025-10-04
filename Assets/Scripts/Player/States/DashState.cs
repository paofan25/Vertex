using UnityEngine;
using System.Collections;

/// <summary>
/// 冲刺状态
/// </summary>
public class DashState : IPlayerState
{
    private Coroutine dashCoroutine; // 冲刺协程
    private Coroutine afterimageCoroutine; // 残影协程
    private Vector2 dashDirection; // 冲刺方向

    public void Enter(PlayerStateMachine stateMachine)
    {
        // 调用冲刺协程
        // stateMachine.DashBufferTimer = 0f; // 消耗冲刺缓冲
        dashCoroutine = stateMachine.StartCoroutine(Dash(stateMachine)); // 启动冲刺协程
        
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
        stateMachine.animator.Play("Dash");
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
        // stateMachine.motor.enabled = true;
        EventBus.Publish(new CanInputEvent(true)); // 发布启用输入事件
        stateMachine.rb.gravityScale = stateMachine.movementData.gravityScale; // 恢复重力
        stateMachine.IsDashing = false; // 结束冲刺状态
    }

    private IEnumerator Dash(PlayerStateMachine stateMachine)
    {
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        stateMachine.DashCount--; // 减少冲刺次数
        
        float originalGravity = stateMachine.rb.gravityScale; // 保存原始重力
        stateMachine.IsDashing = true; // 开始冲刺状态
        stateMachine.rb.velocity = Vector2.zero; // 重置速度
        stateMachine.rb.gravityScale = 0f; // 设置重力为0，使玩家在空中不受重力影响
        // stateMachine.motor.enabled = false; // 禁用常规移动

        // 确定冲刺方向
        Vector2 inputDirection = new Vector2(stateMachine.inputAdapter.MoveX, stateMachine.inputAdapter.MoveY).normalized;
        // 如果没有方向输入，则使用角色朝向，否则使用输入方向
        if (inputDirection.magnitude < 0.1f)
            dashDirection = new Vector2(stateMachine.FacingDirection, 0);
        else
            dashDirection = inputDirection;

        // 设置冲刺速度
        stateMachine.SetVelocity(dashDirection * stateMachine.movementData.dashForce);
        
        // 生成冲刺残影
        afterimageCoroutine = stateMachine.StartCoroutine(GenerateDashAfterImage(stateMachine));
        
        // 确定冲刺方向
        // Vector2 dashDir = new Vector2(stateMachine.inputAdapter.MoveX, stateMachine.inputAdapter.MoveY).normalized;
        // if (dashDir == Vector2.zero)
        // {
        //     dashDir = new Vector2(stateMachine.FacingDirection, 0);
        // }
        
        // 计算冲刺速度和持续时间
        // float dashSpeed = stateMachine.movementData.dashForce;
        // float dashDuration = stateMachine.movementData.dashDuration;
        //
        // // 向上冲刺特殊处理
        // bool isUpDash = dashDir.y > 0.5f && Mathf.Abs(dashDir.x) < 0.5f;
        // if (isUpDash)
        // {
        //     dashSpeed *= stateMachine.movementData.upDashForceMultiplier;
        // }
        //
        // stateMachine.rb.velocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(stateMachine.movementData.dashDuration); // 等待冲刺持续时间
        
        stateMachine.StopCoroutine(afterimageCoroutine); // 停止生成冲刺残影
        
        stateMachine.rb.AddForce(-dashDirection * stateMachine.movementData.dashBackForce, ForceMode2D.Impulse); // 冲刺后反冲

        // stateMachine.motor.enabled = true; // 恢复常规移动
        stateMachine.rb.gravityScale = originalGravity; // 恢复原始重力
        EventBus.Publish(new CanInputEvent(true)); // 发布启用输入事件
        stateMachine.IsDashing = false; // 结束冲刺状态
        
        

        // 冲刺结束后的速度处理
        // if (isUpDash)
        // {
        //     // 向上冲刺后给予短暂的滞空
        //     stateMachine.rb.velocity = new Vector2(stateMachine.rb.velocity.x, stateMachine.movementData.upDashHangForce);
        //     yield return new WaitForSeconds(stateMachine.movementData.upDashHangTime);
        // }
        // else if (!stateMachine.IsGrounded)
        // {
        //     // 其他方向的空中冲刺，结束后水平速度减半，垂直速度清零
        //     stateMachine.rb.velocity = new Vector2(stateMachine.rb.velocity.x * 0.5f, 0);
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

    /// <summary>
    /// 生成冲刺后影
    /// </summary>
    /// <param name="stateMachine"></param>
    /// <returns></returns>
    private IEnumerator GenerateDashAfterImage(PlayerStateMachine stateMachine)
    {
        while (true)
        {
            stateMachine.SpawnDashAfterImage();
            yield return new WaitForSeconds(stateMachine.movementData.afterImageInterval);
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