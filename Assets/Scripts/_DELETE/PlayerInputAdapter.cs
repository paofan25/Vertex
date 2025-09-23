using UnityEngine;

/// <summary>
/// 玩家输入适配器 - 将Input System转换为抽象指令
/// </summary>
public class PlayerInputAdapter : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private GatherInput gatherInput;
    
    // 抽象指令输出
    public float MoveX => gatherInput.valueX;
    public float MoveY => gatherInput.valueY;
    public bool JumpPressed => gatherInput.jumpInput;
    public bool JumpHeld => gatherInput.jumpHeld;
    public bool DashPressed => gatherInput.dashInput;
    public bool GrabHeld => gatherInput.grabHeld;
    // 输入事件
    public System.Action OnJumpPressed;
    public System.Action OnJumpReleased;
    public System.Action OnDashPressed;
    
    private bool lastJumpState;
    private bool lastDashState;
    
    private void Start()
    {
        if (gatherInput == null)
            gatherInput = GetComponent<GatherInput>();
    }
    
    private void Update()
    {
        // 检测输入变化并触发事件
        if (JumpPressed && !lastJumpState)
            OnJumpPressed?.Invoke();
        else if (!JumpPressed && lastJumpState)
            OnJumpReleased?.Invoke();
            
        if (DashPressed && !lastDashState)
            OnDashPressed?.Invoke();
        
        lastJumpState = JumpPressed;
        lastDashState = DashPressed;
    }
    
    /// <summary>
    /// 重置输入状态
    /// </summary>
    public void ResetInputs()
    {
        if (gatherInput != null)
        {
            gatherInput.jumpInput = false;
            gatherInput.dashInput = false;
        }
    }
}