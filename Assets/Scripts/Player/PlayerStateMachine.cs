using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 玩家状态机
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    [Header("组件引用")]
    public Rigidbody2D rb;
    public PlayerInputAdapter inputAdapter;
    public GroundChecker groundChecker;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public LocomotionMotor2D motor;
    
    [Header("参数配置")]
    public MovementData movementData;
    
    // 状态管理
    private IPlayerState currentState;
    private Dictionary<System.Type, IPlayerState> states;
    
    // 公共属性
    public Vector2 Velocity => rb.velocity;
    public bool IsGrounded => groundChecker.IsGrounded;
    public int FacingDirection { get; private set; } = 1;
    public bool IsAgainstWall { get; private set; }
    // 状态数据
    public float DashCooldownTimer { get; set; }
    public bool CanDash { get; set; } = true;
    public float CurrentStamina { get; set; }
    
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
        currentState?.Update(this);
    }
    
    private void FixedUpdate()
    {
        currentState?.FixedUpdate(this);
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
            { typeof(ClimbingState), new ClimbingState() }
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
        // 冲刺冷却
        if (DashCooldownTimer > 0)
        {
            DashCooldownTimer -= Time.deltaTime;
            Debug.Log($"Dash Cooldown: {DashCooldownTimer:F2}");
            if (DashCooldownTimer <= 0 && IsGrounded)
            {
                ResetDash();
            }
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
        
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("vSpeed", rb.velocity.y);
        animator.SetBool("Grounded", IsGrounded);
        animator.SetBool("CanDash", CanDash);
    }
}