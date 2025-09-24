using UnityEngine;

/// <summary>
/// 角色物理运动马达
/// 负责所有基于Rigidbody2D的移动计算和执行
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class LocomotionMotor2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStateMachine stateMachine; // 用于获取数据和状态

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = GetComponent<PlayerStateMachine>();
    }

    /// <summary>
    /// 处理地面移动。根据输入平滑地加速或减速。
    /// </summary>
    /// <param name="inputX">水平输入 (-1 到 1)</param>
    public void HandleGroundMovement(float inputX)
    {
        float targetSpeed = inputX * stateMachine.movementData.runSpeed;
        float acceleration = stateMachine.movementData.acceleration;
        float deceleration = stateMachine.movementData.deceleration;
        
        // 根据是否有输入选择加速度或减速度
        float accelRate = (Mathf.Abs(inputX) > 0.01f) ? acceleration : deceleration;
        
        // 使用MoveTowards平滑速度变化
        float newVelocityX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
    }

    /// <summary>
    /// 处理空中移动。提供有限的空中控制能力。
    /// </summary>
    /// <param name="inputX">水平输入 (-1 到 1)</param>
    public void HandleAirMovement(float inputX)
    {
        // float targetSpeed = inputX * stateMachine.movementData.runSpeed;
        // float acceleration = stateMachine.movementData.acceleration * stateMachine.movementData.airControl;
        //
        // // 在空中，我们只允许玩家施加推力，减速主要靠空气阻力（线性阻尼）
        // if (Mathf.Abs(inputX) > 0.01f)
        // {
        //     float newVelocityX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
        //     rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
        // }
    }

    /// <summary>
    /// 立即执行一次向上的跳跃力。
    /// </summary>
    public void PerformJump()
    {
        // 我们需要重置Y轴速度，以确保每次跳跃高度一致
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * stateMachine.movementData.jumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 当玩家提前松开跳跃键时，削减向上的速度。
    /// </summary>
    public void CutJump()
    {
        // 只在角色上升时削减速度
        // if (rb.velocity.y > 0)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * stateMachine.movementData.jumpCutMultiplier);
        // }
    }

    /// <summary>
    /// 施加自定义的重力效果。
    /// </summary>
    public void ApplyGravity()
    {
        // // 快速下落：如果玩家下落且按下了“下”方向键
        // bool isFalling = rb.velocity.y < 0;
        // bool fastFallInput = stateMachine.inputAdapter.MoveY < -0.1f;
        //
        // float gravityMultiplier = (isFalling && fastFallInput) 
        //     ? stateMachine.movementData.fastFallMultiplier 
        //     : 1f;
        //
        // // 施加重力
        // rb.AddForce(Vector2.down * stateMachine.movementData.gravityScale * gravityMultiplier, ForceMode2D.Force);
        //
        // // 限制最大下落速度
        // if (rb.velocity.y < -stateMachine.movementData.maxFallSpeed)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, -stateMachine.movementData.maxFallSpeed);
        // }
    }
    
    /// <summary>
    /// 外部直接设置速度的方法
    /// </summary>
    public void SetVelocity(Vector2 newVelocity)
    {
        rb.velocity = newVelocity;
    }

    /// <summary>
    /// 外部直接设置X轴速度的方法
    /// </summary>
    public void SetVelocityX(float newVelocityX)
    {
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
    }

    /// <summary>
    /// 外部直接设置Y轴速度的方法
    /// </summary>
    public void SetVelocityY(float newVelocityY)
    {
        rb.velocity = new Vector2(rb.velocity.x, newVelocityY);
    }
}