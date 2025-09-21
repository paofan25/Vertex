using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb; // 玩家刚体
    public LayerMask spikeLayer; // 刺的层
    public CinemachineVirtualCamera vcam; // 虚拟摄像机
    
    public float speed = 5; // 移动速度
    public float horizontal; // 水平输入

    private bool canInput = true; // 是否可以输入

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
        vcam = FindObjectOfType<CinemachineVirtualCamera>(); // 获取虚拟摄像机
    }

    private void Start()
    {
        vcam.Follow = transform; // 设置虚拟摄像机的跟随对象为玩家
        vcam.LookAt = transform; // 设置虚拟摄像机的观察对象为玩家
    }

    private void Update()
    {
        if (!canInput) return; // 禁止输入则返回
        
        GetInput(); // 获取输入
    }

    private void FixedUpdate()
    {
        Movement(); // 移动
    }
    
    private void OnEnable()
    {
        // 订阅事件
        EventBus.Subscribe<CanInputEvent>(CanInput);
    }

    private void OnDisable()
    {
        // 退订事件
        EventBus.Unsubscribe<CanInputEvent>(CanInput);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果碰到的物体在spikeLayer里
        if (spikeLayer == (spikeLayer | (1 << other.gameObject.layer)))
            StartCoroutine(Die());
    }

    // 获取输入
    private void GetInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); // 获取水平输入
    }

    // 移动
    private void Movement()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y); // 设置刚体的速度
    }

    // 玩家死亡
    private IEnumerator Die()
    {
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        horizontal = 0; // 输入设置为0
        rb.velocity = Vector2.zero; // 停止移动
        rb.gravityScale = 0; // 取消重力

        SpriteRenderer sr = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件
        sr.color = Color.gray; // 设置颜色为灰色
        
        Debug.Log("Die");
        
        yield return new WaitForSeconds(1f);
        
        GameManager.Instance.OnPlayerDeath(gameObject); // 调用GameManager的OnPlayerDeath方法
    }

    // 是否能够进行输入
    private void CanInput(GameEvent gameEvent)
    {
        CanInputEvent canInputEvent = (CanInputEvent)gameEvent;
        
        canInput = canInputEvent.canInput;
    }
}
