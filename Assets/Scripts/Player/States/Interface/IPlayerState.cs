using UnityEngine;

/// <summary>
/// 玩家状态接口
/// </summary>
public interface IPlayerState
{
    void Enter(PlayerStateMachine stateMachine);
    void Update(PlayerStateMachine stateMachine);
    void FixedUpdate(PlayerStateMachine stateMachine);
    void Exit(PlayerStateMachine stateMachine);
}