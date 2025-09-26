using UnityEngine;

/// <summary>
/// 输入收集器 - 根据设计文档重构
/// </summary>
public class GatherInput : MonoBehaviour
{
    [Header("移动输入")]
    public float valueX;
    public float valueY;

    [Header("动作输入")]
    public bool jumpInput;    // C键 - 按下瞬间
    public bool jumpHeld;     // C键 - 持续按住
    public bool dashInput;    // X键 - 按下瞬间
    public bool grabHeld;     // Z键 - 持续按住
    
    public bool canInput = true; // 是否能够进行输入

    private void OnEnable()
    {
        // 订阅事件
        EventBus.Subscribe<CanInputEvent>(CanInput); // 订阅是否能够进行输入事件
    }

    private void OnDisable()
    {
        // 退订事件
        EventBus.Unsubscribe<CanInputEvent>(CanInput); // 退订是否能够进行输入事件
    }

    private void Update()
    {
        if (!canInput) return;
        
        // 重置瞬时输入
        jumpInput = false;
        dashInput = false;

        // 收集输入
        GetImput();
    }

    /// <summary>
    /// 收集输入
    /// </summary>
    private void GetImput()
    {
        // 读取移动输入
        valueX = Input.GetAxisRaw("Horizontal");
        valueY = Input.GetAxisRaw("Vertical");

        // 读取动作输入
        if (Input.GetKeyDown(KeyCode.C))
        {
            jumpInput = true;
        }
        jumpHeld = Input.GetKey(KeyCode.C);

        if (Input.GetKeyDown(KeyCode.X))
        {
            dashInput = true;
        }

        grabHeld = Input.GetKey(KeyCode.Z);
    }

    /// <summary>
    /// 是否能够进行输入
    /// </summary>
    private void CanInput(GameEvent gameEvent)
    {
        CanInputEvent canInputEvent = (CanInputEvent)gameEvent;
        
        canInput = canInputEvent.canInput;
    }
}
