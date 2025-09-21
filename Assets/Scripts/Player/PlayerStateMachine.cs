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
    
    [Header("参数配置")]
    public PlayerMovementData movementData;
    
    // 状态管理
    private IPlayerState currentState;
    private Dictionary<System.Type, IPlayerState> states;
    
    // 公共属性
    public Vector2 Velocity => rb.velocity;
    public bool IsGrounded => groundChecker.IsGrounded;
    public int FacingDirection { get; private set; } = 1;
    
    // 状态数据
    public float CoyoteTimer { get; set; }
    public float JumpBufferTimer { get; set; }
    public float DashCooldownTimer { get; set; }
    public bool CanDash { get; set; } = true;
    
    private void Awake()
    {
        InitializeComponents();
        InitializeStates();
    }
    
    private void Start()
    {
        ChangeState<IdleState>();
    }
    
    private void Update()
    {
        UpdateTimers();
        currentState?.Update(this);
    }
    
    private void FixedUpdate()
    {
        currentState?.FixedUpdate(this);
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
            { typeof(WallJumpState), new WallJumpState() },
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
            currentState.Enter(this);
        }
    }
    
    /// <summary>
    /// 更新计时器
    /// </summary>
    private void UpdateTimers()
    {
        // 土狼时间
        if (IsGrounded)
            CoyoteTimer = movementData.coyoteTime;
        else
            CoyoteTimer -= Time.deltaTime;
        
        // 跳跃缓冲
        if (inputAdapter.JumpPressed)
            JumpBufferTimer = movementData.jumpBufferTime;
        else
            JumpBufferTimer -= Time.deltaTime;
        
        // 冲刺冷却
        if (DashCooldownTimer > 0)
            DashCooldownTimer -= Time.deltaTime;
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
    /// 设置速度
    /// </summary>
    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
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