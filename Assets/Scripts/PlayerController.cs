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
    public float horizontal; // 水平输入
    [SerializeField] private float knockBack = 2f; // 击退力
    [SerializeField] private float drag = 2f; // 击退力

    private bool canInput = true; // 是否可以输入

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
    }

    private void Update()
    {
        if (!canInput) return; // 禁止输入则返回
        
        GetInput(); // 获取输入
        if (Input.GetButtonDown("Jump"))
        {
            Jump(); // 跳跃
        }
    }

    private void FixedUpdate()
    {
        if (!canInput) return; // 禁止输入则返回
        
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

    // 跳跃
    private void Jump()
    {
        rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse); // 给刚体施加一个向上的力
    }

    // 玩家死亡
    private IEnumerator Die()
    {
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        Vector2 moveDir = rb.velocity.normalized; // 获取移动方向
        horizontal = 0; // 输入设置为0
        rb.velocity = Vector2.zero; // 停止移动
        rb.gravityScale = 0; // 取消重力

        SpriteRenderer sr = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件
        sr.color = Color.gray; // 设置颜色为灰色
        
        Debug.Log("Die");

        rb.drag = 10; // 设置角阻力
        rb.AddForce(-moveDir * knockBack, ForceMode2D.Impulse); // 给刚体施加一个向后的力
        CameraManager.Instance.ShakeCamera(); // 摄像机抖动
        
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
