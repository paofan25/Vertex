using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家移动控制器 - 蔚蓝风格的完整移动系统
/// 整合了原 PlayerController 的所有功能
/// </summary>
public class PlayerMoveControls : MonoBehaviour
{
    [Header("组件引用")]
    [Tooltip("输入收集器")]
    public GatherInput gI;
    [Tooltip("刚体组件")]
    public Rigidbody2D rb;
    [Tooltip("动画控制器")]
    public Animator anim;
    [Tooltip("精灵渲染器")]
    public SpriteRenderer sr;

    [Header("基础移动参数")]
    [Tooltip("奔跑速度")]
    public float runSpeed = 8f;
    [Tooltip("空中控制力")]
    public float airControl = 0.8f;
    [Tooltip("最大下落速度")]
    public float maxFallSpeed = 20f;

    [Header("跳跃参数")]
    [Tooltip("跳跃力度")]
    public float jumpForce = 15f;
    [Tooltip("变速跳跃：最小跳跃高度")]
    public float minJumpForce = 8f;
    [Tooltip("跳跃按键保持时间影响")]
    public float jumpHoldTime = 0.3f;
    [Tooltip("快速下落速度倍数")]
    public float fastFallMultiplier = 2f;
    [Tooltip("土狼时间：离开地面后仍可跳跃的时间")]
    public float coyoteTime = 0.15f;
    [Tooltip("跳跃缓冲：提前按跳跃键的容错时间")]
    public float jumpBufferTime = 0.2f;

    [Header("墙面交互参数")]
    [Tooltip("墙跳水平力")]
    public float wallJumpForceX = 12f;
    [Tooltip("墙跳垂直力")]
    public float wallJumpForceY = 15f;
    [Tooltip("墙滑速度")]
    public float wallSlideSpeed = 3f;
    [Tooltip("贴墙时间")]
    public float wallStickTime = 0.1f;

    [Header("攀爬参数")]
    [Tooltip("攀爬速度")]
    public float climbSpeed = 5f;
    [Tooltip("攀爬耐力")]
    public float climbStamina = 3f;
    [Tooltip("耐力恢复速度")]
    public float staminaRegenRate = 1f;

    [Header("冲刺参数")]
    [Tooltip("冲刺速度")]
    public float dashSpeed = 20f;
    [Tooltip("冲刺持续时间")]
    public float dashDuration = 0.2f;
    [Tooltip("冲刺冷却时间")]
    public float dashCooldown = 1f;

    [Header("地面检测")]
    [Tooltip("地面检测点")]
    public Transform groundCheck;
    [Tooltip("地面检测半径")]
    public float groundCheckRadius = 0.2f;
    [Tooltip("地面图层")]
    public LayerMask groundLayer;

    [Header("墙面检测")]
    [Tooltip("墙面检测点")]
    public Transform wallCheck;
    [Tooltip("墙面检测距离")]
    public float wallCheckDistance = 0.5f;
    [Tooltip("墙面图层")]
    public LayerMask wallLayer;

    // 状态枚举
    public enum PlayerState
    {
        Idle,           // 待机
        Running,        // 奔跑
        Jumping,        // 跳跃
        Falling,        // 下落
        WallSliding,    // 墙滑
        WallJumping,    // 墙跳
        Dashing,        // 冲刺
        Climbing        // 攀爬
    }

    // 状态变量
    [Header("状态显示")]
    [SerializeField] private PlayerState currentState = PlayerState.Idle;
    [SerializeField] private PlayerState previousState = PlayerState.Idle;

    // 检测状态
    private bool isGrounded;
    private bool isTouchingWall;
    private int wallDirection;
    private int facingDirection = 1;

    // 跳跃相关
    private bool canVariableJump;
    private float jumpHoldTimer;
    private float coyoteTimer;
    private float jumpBufferTimer;

    // 墙面交互
    private float wallStickTimer;

    // 攀爬相关
    private bool isClimbing;
    private float currentStamina;

    // 冲刺相关
    private bool isDashing;
    private bool canDash = true;
    private float dashCooldownTimer;
    private Vector2 dashDirection;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        // 获取组件引用
        if (gI == null) gI = GetComponent<GatherInput>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        // 初始化状态
        currentStamina = climbStamina;
        facingDirection = 1;
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    private void Update()
    {
        CheckCollisions();
        UpdateTimers();
        HandleInput();
        UpdateState();
        SetAnimatorValues();
    }

    /// <summary>
    /// 物理更新
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
        HandleClimbing();
        ApplyGravity();
    }

    /// <summary>
    /// 检测碰撞
    /// </summary>
    private void CheckCollisions()
    {
        // 地面检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 墙面检测
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, wallLayer);
        isTouchingWall = wallHit.collider != null;
        
        if (isTouchingWall)
            wallDirection = facingDirection;
        else
            wallDirection = 0;
    }

    /// <summary>
    /// 处理输入
    /// </summary>
    private void HandleInput()
    {
        // 冲刺输入
        if (gI.dashInput && canDash && dashCooldownTimer <= 0)
        {
            StartDash();
            gI.dashInput = false; // 重置输入
        }
    }

    /// <summary>
    /// 更新计时器
    /// </summary>
    private void UpdateTimers()
    {
        // 土狼时间
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // 跳跃缓冲
        if (gI.jumpInput)
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        // 跳跃按键持续时间
        if (gI.jumpHeld && canVariableJump)
            jumpHoldTimer += Time.deltaTime;
        else if (!gI.jumpHeld)
            canVariableJump = false;

        // 冲刺冷却
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // 贴墙时间
        if (wallStickTimer > 0)
            wallStickTimer -= Time.deltaTime;

        // 攀爬耐力恢复
        if (isGrounded && currentStamina < climbStamina)
            currentStamina = Mathf.Min(climbStamina, currentStamina + staminaRegenRate * Time.deltaTime);
    }

    /// <summary>
    /// 状态机更新
    /// </summary>
    private void UpdateState()
    {
        PlayerState newState = currentState;

        switch (currentState)
        {
            case PlayerState.Dashing:
                if (!isDashing)
                {
                    newState = DetermineStateAfterDash();
                }
                break;

            case PlayerState.Climbing:
                if (currentStamina <= 0 || !isTouchingWall || isGrounded)
                {
                    isClimbing = false;
                    newState = isGrounded ? PlayerState.Idle : PlayerState.Falling;
                }
                else if (jumpBufferTimer > 0)
                {
                    WallJump();
                    newState = PlayerState.WallJumping;
                    isClimbing = false;
                }
                break;

            case PlayerState.WallSliding:
                if (!isTouchingWall || isGrounded)
                    newState = isGrounded ? PlayerState.Idle : PlayerState.Falling;
                else if (jumpBufferTimer > 0)
                {
                    WallJump();
                    newState = PlayerState.WallJumping;
                }
                else if (gI.valueY > 0 && currentStamina > 0)
                {
                    newState = PlayerState.Climbing;
                    isClimbing = true;
                }
                break;

            case PlayerState.WallJumping:
                if (rb.velocity.y <= 0)
                    newState = PlayerState.Falling;
                break;

            default:
                newState = DetermineGroundedOrAirborneState();
                break;
        }

        // 状态切换
        if (newState != currentState)
        {
            previousState = currentState;
            currentState = newState;
            OnStateEnter(newState);
        }
    }

    /// <summary>
    /// 确定冲刺后的状态
    /// </summary>
    private PlayerState DetermineStateAfterDash()
    {
        if (rb.velocity.y > 0) return PlayerState.Jumping;
        if (rb.velocity.y < 0) return PlayerState.Falling;
        return isGrounded ? PlayerState.Idle : PlayerState.Falling;
    }

    /// <summary>
    /// 确定地面或空中状态
    /// </summary>
    private PlayerState DetermineGroundedOrAirborneState()
    {
        if (isGrounded)
        {
            if (Mathf.Abs(gI.valueX) > 0.1f)
                return PlayerState.Running;
            else
                return PlayerState.Idle;
        }
        else
        {
            // 检查攀爬条件
            if (isTouchingWall && gI.valueY > 0 && currentStamina > 0 && gI.valueX * wallDirection > 0)
            {
                isClimbing = true;
                return PlayerState.Climbing;
            }
            // 检查墙滑条件
            else if (isTouchingWall && rb.velocity.y < 0 && gI.valueX * wallDirection > 0)
            {
                wallStickTimer = wallStickTime;
                return PlayerState.WallSliding;
            }
            else if (rb.velocity.y > 0)
                return PlayerState.Jumping;
            else
                return PlayerState.Falling;
        }

        // 跳跃逻辑检查
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            Jump();
            return PlayerState.Jumping;
        }

        return currentState;
    }

    /// <summary>
    /// 状态进入事件
    /// </summary>
    private void OnStateEnter(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.WallSliding:
                wallStickTimer = wallStickTime;
                break;
            case PlayerState.Climbing:
                isClimbing = true;
                break;
        }
    }

    /// <summary>
    /// 处理移动
    /// </summary>
    private void HandleMovement()
    {
        if (isDashing || currentState == PlayerState.Climbing) return;

        float targetVelocityX = CalculateTargetVelocityX();
        float acceleration = isGrounded ? 1f : airControl;

        // 平滑应用移动速度
        float newVelocityX = Mathf.MoveTowards(rb.velocity.x, targetVelocityX, 
            runSpeed * acceleration * Time.fixedDeltaTime * 10f);
        
        rb.velocity = new Vector2(newVelocityX, rb.velocity.y);

        // 处理角色翻转
        if (gI.valueX != 0 && currentState != PlayerState.WallJumping && currentState != PlayerState.Climbing)
        {
            Flip(gI.valueX > 0 ? 1 : -1);
        }
    }

    /// <summary>
    /// 计算目标水平速度
    /// </summary>
    private float CalculateTargetVelocityX()
    {
        switch (currentState)
        {
            case PlayerState.Running:
            case PlayerState.Idle:
            case PlayerState.Jumping:
            case PlayerState.Falling:
                return gI.valueX * runSpeed;

            case PlayerState.WallSliding:
                return wallStickTimer > 0 ? 0 : gI.valueX * runSpeed * 0.5f;

            case PlayerState.WallJumping:
                return gI.valueX * runSpeed * 0.6f;

            default:
                return 0f;
        }
    }

    /// <summary>
    /// 处理攀爬
    /// </summary>
    private void HandleClimbing()
    {
        if (currentState != PlayerState.Climbing) return;

        // 消耗耐力
        currentStamina -= Time.fixedDeltaTime;
        
        // 攀爬移动
        float climbVelocityY = gI.valueY > 0 ? gI.valueY * climbSpeed : 0;
        rb.velocity = new Vector2(0, climbVelocityY);
        
        // 面向墙面
        Flip(wallDirection);
    }

    /// <summary>
    /// 应用重力
    /// </summary>
    private void ApplyGravity()
    {
        if (isDashing || currentState == PlayerState.Climbing) return;

        float gravityMultiplier = CalculateGravityMultiplier();

        // 应用重力
        float newVelocityY = rb.velocity.y - Physics2D.gravity.magnitude * gravityMultiplier * Time.fixedDeltaTime;
        newVelocityY = Mathf.Max(newVelocityY, -maxFallSpeed);
        
        rb.velocity = new Vector2(rb.velocity.x, newVelocityY);
    }

    /// <summary>
    /// 计算重力倍数
    /// </summary>
    private float CalculateGravityMultiplier()
    {
        // 变速跳跃：释放跳跃键时增加重力
        if (currentState == PlayerState.Jumping && !gI.jumpHeld && canVariableJump)
            return 3f;
        
        // 快速下落：按下键时增加下落速度
        if (currentState == PlayerState.Falling && gI.valueY < -0.5f)
            return fastFallMultiplier;
        
        // 墙滑时减少重力
        if (currentState == PlayerState.WallSliding)
            return wallSlideSpeed / maxFallSpeed;

        return 1f;
    }

    /// <summary>
    /// 执行跳跃
    /// </summary>
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        
        // 启用变速跳跃
        canVariableJump = true;
        jumpHoldTimer = 0f;
    }

    /// <summary>
    /// 执行墙跳
    /// </summary>
    private void WallJump()
    {
        rb.velocity = new Vector2(-wallDirection * wallJumpForceX, wallJumpForceY);
        Flip(-wallDirection);
        jumpBufferTimer = 0f;
        wallStickTimer = 0f;
    }

    /// <summary>
    /// 开始冲刺
    /// </summary>
    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashCooldownTimer = dashCooldown;
        
        // 确定冲刺方向
        dashDirection = new Vector2(gI.valueX, gI.valueY).normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(facingDirection, 0);
        
        rb.velocity = dashDirection * dashSpeed;
        
        // 启动冲刺协程
        StartCoroutine(DashCoroutine());
    }

    /// <summary>
    /// 冲刺协程
    /// </summary>
    private IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        
        // 冲刺结束后恢复冲刺能力（如果在地面）
        if (isGrounded)
            canDash = true;
    }

    /// <summary>
    /// 翻转角色
    /// </summary>
    private void Flip(int direction)
    {
        if (direction != 0 && direction != facingDirection)
        {
            facingDirection = direction;
            sr.flipX = facingDirection < 0;
        }
    }

    /// <summary>
    /// 设置动画参数
    /// </summary>
    private void SetAnimatorValues()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("vSpeed", rb.velocity.y);
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("IsDashing", isDashing);
        anim.SetBool("IsClimbing", currentState == PlayerState.Climbing);
        anim.SetBool("IsWallSliding", currentState == PlayerState.WallSliding);
        anim.SetBool("IsTouchingWall", isTouchingWall);
        anim.SetFloat("Stamina", currentStamina / climbStamina);
        anim.SetInteger("State", (int)currentState);
    }

    /// <summary>
    /// 绘制调试信息
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = isTouchingWall ? Color.blue : Color.white;
            Gizmos.DrawRay(wallCheck.position, Vector2.right * facingDirection * wallCheckDistance);
        }
    }

    // ========== 公共API ==========
    
    /// <summary>当前状态</summary>
    public PlayerState CurrentState => currentState;
    
    /// <summary>是否在地面</summary>
    public bool IsGrounded => isGrounded;
    
    /// <summary>是否贴墙</summary>
    public bool IsTouchingWall => isTouchingWall;
    
    /// <summary>当前耐力</summary>
    public float CurrentStamina => currentStamina;
    
    /// <summary>是否在攀爬</summary>
    public bool IsClimbing => currentState == PlayerState.Climbing;
    
    /// <summary>是否在冲刺</summary>
    public bool IsDashing => isDashing;
    
    /// <summary>面向方向</summary>
    public int FacingDirection => facingDirection;
}
