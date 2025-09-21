using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Action<GameEvent>>> EventListeners = new(); // 事件监听器字典

    // 订阅事件
    public static void Subscribe<T>(Action<GameEvent> listener) where T : GameEvent
    {
        Type eventType = typeof(T); // 获取事件类型

        // 如果没有对应的监听器列表，则创建一个
        if (!EventListeners.TryGetValue(eventType, out var listeners))
        {
            listeners = new List<Action<GameEvent>>();
            EventListeners[eventType] = listeners;
        }

        listeners.Add(listener); // 添加监听器
    }

    // 取消订阅事件
    public static void Unsubscribe<T>(Action<GameEvent> listener) where T : GameEvent
    {
        Type eventType = typeof(T); // 获取事件类型

        // 如果字典中存在该事件类型的监听器列表，则移除指定的监听器
        if (EventListeners.TryGetValue(eventType, out var listeners))
        {
            listeners.Remove(listener);
            
            // 如果该事件的监听列表为空，从字典中移除以节省空间
            if (listeners.Count == 0)
                EventListeners.Remove(eventType);
        }
    }

    // 发布事件
    public static void Publish(GameEvent gameEvent)
    {
        Type eventType = gameEvent.GetType(); // 获取事件类型

        // 如果字典中存在该事件类型的监听器列表，则调用所有监听器
        if (EventListeners.TryGetValue(eventType, out var listeners))
        {
            // 使用 ToArray() 创建副本以避免修改枚举的异常
            foreach (var listener in listeners.ToArray())
            {
                // 检查是否为 null（如果订阅者被销毁但未取消订阅）
                if (listener != null)
                    listener.Invoke(gameEvent);
            }
        }
    }
    
    // 清空所有订阅，用于场景切换或游戏重启
    public static void Clear()
    {
        EventListeners.Clear();
    }
}
