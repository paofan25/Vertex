using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Rigidbody2D rb; // 玩家刚体
    
    [SerializeField] private float knockBack = 2f; // 击退力
    [SerializeField] private float drag = 2f; // 阻尼

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
    }

    private void OnEnable()
    {
        EventBus.Subscribe<OnPlayerDeathEvent>(OnDeath); // 订阅玩家死亡事件
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<OnPlayerDeathEvent>(OnDeath); // 取消订阅玩家死亡事件
    }

    // 玩家死亡
    public void OnDeath(GameEvent gameEvent)
    {
        StartCoroutine(Die());
    }
    
    // 死亡协程
    private IEnumerator Die()
    {
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        
        Vector2 moveDir = rb.velocity.normalized; // 获取移动方向
        
        rb.velocity = Vector2.zero; // 停止移动
        rb.gravityScale = 0; // 取消重力

        SpriteRenderer sr = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件
        sr.color = Color.gray; // 设置颜色为灰色
        
        Debug.Log("Die");

        rb.drag = 10; // 设置阻尼
        rb.AddForce(-moveDir * knockBack, ForceMode2D.Impulse); // 给刚体施加一个向后的力
        CameraManager.Instance.ShakeCamera(); // 摄像机抖动
        
        yield return new WaitForSeconds(1f);
        
        GameManager.Instance.OnPlayerDeath(gameObject); // 调用GameManager的OnPlayerDeath方法
    }
}
