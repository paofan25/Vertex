using System.Collections;
using UnityEngine;

/// <summary>
/// 奔跑状态
/// </summary>
public class RebirthState : IPlayerState
{
    public void Enter(PlayerStateMachine stateMachine)
    {
        // 进入重生状态
        // EventBus.Publish(new PlayRebrithSEEvent()); // 播放重生音效
        stateMachine.animator.Play("Rebirth");
        stateMachine.StartCoroutine(Rebirth(stateMachine)); // 开始重生协程
    }
    
    public void Update(PlayerStateMachine stateMachine)
    {

    }
    
    public void FixedUpdate(PlayerStateMachine stateMachine)
    {
        
    }
    
    public void Exit(PlayerStateMachine stateMachine)
    {
        // 退出重生状态
    }
    
    // 重生协程
    private IEnumerator Rebirth(PlayerStateMachine stateMachine)
    {
        EventBus.Publish(new CanInputEvent(false)); // 发布禁用输入事件
        
        stateMachine.rb.velocity = Vector2.zero; // 停止移动
        stateMachine.rb.gravityScale = 0; // 取消重力
        
        yield return new WaitForSeconds(0.6f);
        
        stateMachine.rb.gravityScale = 1; // 恢复重力
        EventBus.Publish(new CanInputEvent(true)); // 发布启用输入事件
        
        stateMachine.ChangeState<IdleState>(); // 切换到空闲状态
    }
}