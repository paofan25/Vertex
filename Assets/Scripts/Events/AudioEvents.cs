/// <summary>
/// 播放移动音效
/// </summary>
public class PlayMoveSEEvent : GameEvent
{
    public bool isPlay; // 是否播放

    public PlayMoveSEEvent(bool isPlay)
    {
        this.isPlay = isPlay;
    }
}

/// <summary>
/// 播放跳跃音效
/// </summary>
public class PlayJumpSEEvent : GameEvent { }

/// <summary>
/// 播放落地音效
/// </summary>
public class PlayFallSEEvent : GameEvent { }

/// <summary>
/// 播放死亡音效
/// </summary>
public class PlayDeadSEEvent : GameEvent { }

/// <summary>
/// 播放重生音效
/// </summary>
public class PlayRebrithSEEvent : GameEvent { }