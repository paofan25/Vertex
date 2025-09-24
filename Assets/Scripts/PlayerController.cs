using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb; // 玩家刚体
    public LayerMask spikeLayer; // 刺的层
    
    public float speed = 5; // 移动速度
    public float jumpForce = 7; // 跳跃力
    public float horizontal; // 水平输入

    private bool canInput = true; // 是否可以输入

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
    }

    private void Update()
    {
        if (!canInput) return; // 禁止输入则返回
        
        // GetInput(); // 获取输入
        
        if (Input.GetButtonDown("Jump"))
        {
            // Jump(); // 跳跃
        }
    }

    private void FixedUpdate()
    {
        if (!canInput) return; // 禁止输入则返回
        
        // Movement(); // 移动
    }
    
    #region -----------------------------------和角色死亡脚本一起用-------------------------------------------
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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果碰到的物体在spikeLayer里
        if (spikeLayer == (spikeLayer | (1 << other.gameObject.layer)))
            EventBus.Publish(new OnPlayerDeathEvent()); // 发布玩家死亡事件
    }
    
    // 是否能够进行输入
    private void CanInput(GameEvent gameEvent)
    {
        CanInputEvent canInputEvent = (CanInputEvent)gameEvent;
        
        canInput = canInputEvent.canInput;
    }
    #endregion-----------------------------------和角色死亡脚本一起用-------------------------------------------

    
    // #region-----------------------------------这部分是调试用的临时移动方法，直接删除替换即可-------------------------------------------
    // 获取输入
    // private void GetInput()
    // {
    //     horizontal = Input.GetAxisRaw("Horizontal"); // 获取水平输入
    // }
    //
    // // 移动
    // private void Movement()
    // {
    //     rb.velocity = new Vector2(horizontal * speed, rb.velocity.y); // 设置刚体的速度
    // }
    //
    // // 跳跃
    // private void Jump()
    // {
    //     rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // 给刚体施加一个向上的力
    // }
    // #endregion-----------------------------------这部分是调试用的临时移动方法，直接删除替换即可-------------------------------------------
}
