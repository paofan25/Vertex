using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 输入收集器 - 处理玩家输入并转换为游戏指令
/// 支持新输入系统和传统输入系统
/// </summary>
public class GatherInput : MonoBehaviour
{
    // 输入系统
    private PlayerControls myCustomControls;
    
    // 输出值
    [Header("移动输入")]
    [Tooltip("水平移动输入值 (-1到1)")]
    public float valueX;
    [Tooltip("垂直移动输入值 (-1到1，用于冲刺方向)")]
    public float valueY;
    
    [Header("动作输入")]
    [Tooltip("跳跃输入标志")]
    public bool jumpInput;
    [Tooltip("跳跃按键是否持续按住")]
    public bool jumpHeld;
    [Tooltip("冲刺输入标志")]
    public bool dashInput;

    /// <summary>
    /// 初始化输入系统
    /// </summary>
    private void Awake()
    {
        myCustomControls = new PlayerControls();
    }

    /// <summary>
    /// 启用时绑定输入事件
    /// </summary>
    private void OnEnable()
    {
        // 移动输入事件
        myCustomControls.Player.Move.performed += StartMove;  // 开始移动
        myCustomControls.Player.Move.canceled += StopMove;    // 停止移动
        
        // 跳跃输入事件
        myCustomControls.Player.Jump.performed += JumpStart;  // 按下跳跃
        myCustomControls.Player.Jump.canceled += JumpStop;    // 释放跳跃
        
        // 启用输入系统
        myCustomControls.Player.Enable();
    }

    /// <summary>
    /// 禁用时解绑输入事件
    /// </summary>
    private void OnDisable()
    {
        // 解绑移动事件
        myCustomControls.Player.Move.performed -= StartMove; 
        myCustomControls.Player.Move.canceled -= StopMove;
        
        // 解绑跳跃事件
        myCustomControls.Player.Jump.performed -= JumpStart; 
        myCustomControls.Player.Jump.canceled -= JumpStop;
        
        // 禁用输入系统
        myCustomControls.Player.Disable();
    }

    /// <summary>
    /// 每帧更新：处理传统输入系统的输入
    /// </summary>
    private void Update()
    {
        // 冲刺输入检测
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.X))
        {
            dashInput = true;
        }
        
        
        // 垂直输入（用于攀爬和快速下落）
        valueY = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// 开始移动输入回调
    /// </summary>
    /// <param name="context">输入上下文</param>
    private void StartMove(InputAction.CallbackContext context)
    {
        // 读取移动输入值并四舍五入到整数（-1, 0, 1）
        valueX = Mathf.RoundToInt(context.ReadValue<float>());
    }

    /// <summary>
    /// 停止移动输入回调
    /// </summary>
    /// <param name="context">输入上下文</param>
    private void StopMove(InputAction.CallbackContext context)
    {
        valueX = 0; // 重置水平移动值
    }

    /// <summary>
    /// 跳跃开始输入回调
    /// </summary>
    /// <param name="context">输入上下文</param>
    private void JumpStart(InputAction.CallbackContext context)
    {
        jumpInput = true; // 设置跳跃输入标志
        jumpHeld = true;
    }
    
    /// <summary>
    /// 跳跃结束输入回调
    /// </summary>
    /// <param name="context">输入上下文</param>
    private void JumpStop(InputAction.CallbackContext context)
    {
        jumpInput = false;
        jumpHeld = false;
    }
}
