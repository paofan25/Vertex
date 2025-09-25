using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 玩家状态机
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    [Header("组件引用")]
    public Rigidbody2D rb; // 刚体组件
    public PlayerInputAdapter inputAdapter; // 输入适配器
    public GroundChecker groundChecker; // 地面检测器
    public Animator animator; // 动画控制器
    public SpriteRenderer spriteRenderer; // 精灵渲染器
    public LocomotionMotor2D motor; // 运动控制器
    
    [Header("参数配置")]
    public MovementData movementData; // 运动数据
    public LayerMask spikeLayer; // 刺的层
    
    // 状态管理
    private IPlayerState currentState; // 当前状态
    private Dictionary<System.Type, IPlayerState> states; // 状态字典
    
    // 公共属性
    public Vector2 Velocity => rb.velocity; // 当前速度
    public bool IsGrounded => groundChecker.IsGrounded; // 是否在地面上
    public int FacingDirection { get; private set; } = 1; // 面向方向
    public bool IsAgainstWall { get; private set; } = false; // 是否贴墙
    // 状态数据
    // public float DashCooldownTimer { get; set; } = 0f; // 冲刺冷却计时器
    public int DashCount { get; set; } = 0; // 冲刺次数
    public bool IsDashing { get; set; } = false; // 是否正在冲刺
    public bool CanDash { get; set; } = true; // 是否可以冲刺
    public float CurrentStamina { get; set; } = 0f; // 当前耐力值
    
    public float JumpBufferTimer { get; set; } = 0f; // 跳跃缓冲计时器
    public bool IsJumpBuffered => JumpBufferTimer > 0f; // 是否在跳跃缓冲时间内
    public float CoyoteTimer { get; set; } = 0f; // 郊狼时间计时器
    public bool IsCoyoteTime => CoyoteTimer > 0f; // 是否在郊狼时间内
    
    private void Awake()
    {
        InitializeComponents();
        InitializeStates();
    }
    
    private void Start()
    {
        CurrentStamina = movementData.climbStamina;
        ChangeState<IdleState>();
    }
    
    private void Update()
    {
        UpdateTimers();
        UpdateStamina();
        CheckWall();

        if (inputAdapter.JumpPressed)
        {
            JumpBufferTimer = movementData.jumpBufferTime;
        }
        
        currentState?.Update(this);
    }
    
    private void FixedUpdate()
    {
        currentState?.FixedUpdate(this);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果碰到的物体在spikeLayer里
        if (spikeLayer == (spikeLayer | (1 << other.gameObject.layer)))
        {
            EventBus.Publish(new OnPlayerDeathEvent()); // 发布玩家死亡事件
            ChangeState<DeadState>(); // 切换到死亡状态
        }
    }
    
    public void SetVelocity(Vector2 velocity){
        rb.velocity = velocity;
    }
    
    /// <summary>
    /// 初始化组件
    /// </summary>
    private void InitializeComponents()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (inputAdapter == null) inputAdapter = GetComponent<PlayerInputAdapter>();
        if (groundChecker == null) groundChecker = GetComponent<GroundChecker>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (motor == null) motor = GetComponent<LocomotionMotor2D>();
    }
    
    /// <summary>
    /// 初始化状态
    /// </summary>
    private void InitializeStates()
    {
        states = new Dictionary<System.Type, IPlayerState>
        {
            { typeof(IdleState), new IdleState() },
            { typeof(RunningState), new RunningState() },
            { typeof(JumpingState), new JumpingState() },
            { typeof(FallingState), new FallingState() },
            { typeof(DashState), new DashState() },
            { typeof(WallSlideState), new WallSlideState() },
            { typeof(ClimbingState), new ClimbingState() },
            { typeof(WallJumpState), new WallJumpState() },
            { typeof(DeadState), new DeadState() }
        };
    }
    
    /// <summary>
    /// 切换状态
    /// </summary>
    public void ChangeState<T>() where T : IPlayerState
    {
        if (states.TryGetValue(typeof(T), out IPlayerState newState))
        {
            currentState?.Exit(this);
            currentState = newState;
            Debug.Log(newState);
            currentState.Enter(this);
        }
    }
    
    /// <summary>
    /// 更新计时器
    /// </summary>
    private void UpdateTimers()
    {
        if (JumpBufferTimer > 0)
        {
            JumpBufferTimer -= Time.deltaTime;
        }
        if (CoyoteTimer > 0)
        {
            CoyoteTimer -= Time.deltaTime;
        }
    }
    
    /// <summary>
    /// 更新耐力
    /// </summary>
    private void UpdateStamina()
    {
        // 只有在不攀爬且不紧贴墙壁时才恢复耐力
        if (currentState.GetType() != typeof(ClimbingState) && !IsAgainstWall)
        {
            RestoreStamina(movementData.staminaRegenRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// 消耗耐力
    /// </summary>
    public void ConsumeStamina(float amount)
    {
        CurrentStamina = Mathf.Max(0, CurrentStamina - amount);
        Debug.Log($"[Stamina] Consumed: {amount}. Current: {CurrentStamina}");
    }

    /// <summary>
    /// 恢复耐力
    /// </summary>
    public void RestoreStamina(float amount)
    {
        float oldStamina = CurrentStamina;
        CurrentStamina = Mathf.Min(movementData.climbStamina, CurrentStamina + amount);
        if (CurrentStamina > oldStamina)
        {
            Debug.Log($"[Stamina] Restored: {amount}. Current: {CurrentStamina}");
        }
    }

    /// <summary>
    /// 接触平台时重置冲刺能力
    /// </summary>
    public void ResetDash()
    {
        CanDash = true;
    }

    /// <summary>
    /// 接触平台时重置体力
    /// </summary>
    public void ResetStamina()
    {
        CurrentStamina = movementData.climbStamina;
    }

    private void CheckWall()
    {
        // 我们需要同时检测左右两边，因为玩家可能背对着墙按“抓墙”
        bool wallOnLeft = Physics2D.Raycast(transform.position, Vector2.left,
            movementData.wallCheckDistance, movementData.wallLayer);
            
        bool wallOnRight = Physics2D.Raycast(transform.position, Vector2.right,
            movementData.wallCheckDistance, movementData.wallLayer);

        IsAgainstWall = wallOnLeft || wallOnRight;
    }

    
    /// <summary>
    /// 翻转角色
    /// </summary>
    public void Flip(int direction)
    {
        if (direction != 0 && direction != FacingDirection)
        {
            FacingDirection = direction;
            spriteRenderer.flipX = FacingDirection < 0;
        }
    }
    
    /// <summary>
    /// 设置动画参数
    /// </summary>
    public void UpdateAnimator()
    {
        if (animator == null) return;
        
        // animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        // animator.SetFloat("vSpeed", rb.velocity.y);
        // animator.SetBool("Grounded", IsGrounded);
        // animator.SetBool("CanDash", CanDash);
    }
}