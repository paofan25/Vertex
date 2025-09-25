using System.Collections;
using UnityEngine;

/// <summary>
/// 奔跑状态
/// </summary>
public class DeadState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入死亡状态
        stateMachine.StopAllCoroutines(); // 停止所有协程
        stateMachine.StartCoroutine(Die(stateMachine)); // 开始死亡协程
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {

    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出死亡状态
    }
    
    // 死亡协程
    private IEnumerator Die(PlayerStateMachine stateMachine)
    {
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        
        Vector2 moveDir = stateMachine.rb.velocity.normalized; // 获取移动方向
        
        stateMachine.rb.velocity = Vector2.zero; // 停止移动
        stateMachine.rb.gravityScale = 0; // 取消重力
        
        stateMachine.spriteRenderer.color = Color.gray; // 设置颜色为灰色
        
        Debug.Log("Die");

        stateMachine.rb.drag = stateMachine.movementData.deadDrag; // 设置阻尼
        stateMachine.rb.AddForce(-moveDir * stateMachine.movementData.knockBackForce, ForceMode2D.Impulse); // 给刚体施加一个向后的力
        CameraManager.Instance.ShakeCamera(); // 摄像机抖动
        
        yield return new WaitForSeconds(1f);
        
        GameManager.Instance.OnPlayerDeath(stateMachine.gameObject); // 调用GameManager的OnPlayerDeath方法
    }
}